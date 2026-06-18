# 10 · Бэкенд

## Цели

- Облачные сейвы и кросс-устройственная синхронизация
- Серверная валидация (IAP, anti-cheat)
- Доставка Remote Config
- Сбор аналитики и LiveOps-события
- Доставка пуш-нотификаций

## Архитектура (целевая для prod)

```
            ┌──────────────────────────────────────┐
            │ AWS Cognito (auth, social, guest)     │
            └──────────────┬───────────────────────┘
                           │
            ┌──────────────▼───────────────────────┐
            │  API Gateway  ←──── HTTPS ────────┐   │
            └──────────────┬───────────────────┐│   │
                           │                   ││   │
            ┌──────────────▼─────────┐         ││   │
            │  AWS Lambda            │         ││   │
            │  • save-sync           │         ││   │
            │  • iap-validate        │         ││   │
            │  • config              │         ││   │
            │  • events              │         ││   │
            │  • analytics-batch     │         ││   │
            └──────┬─────────────┬───┘         ││   │
                   │             │              ││   │
        ┌──────────▼──┐    ┌─────▼───┐  ┌──────▼┴─┐
        │ PostgreSQL  │    │ Redis   │  │  S3+CF  │
        │ (RDS)       │    │ (cache) │  │ config+ │
        │ player data │    │ rate-lim│  │ analytics│
        └─────────────┘    └─────────┘  └─────────┘
```

## MVP: Firebase fast-path

> **ADR-002: Для MVP стартуем на Firebase, мигрируем на AWS перед глобальным релизом.**
> **Дата:** 2026-06-17.
> **Решение подтверждено в Phase 0 (P0-040).**
> **Причина:** Время до первого билда критично. Firebase Auth + Firestore + Remote Config + Crashlytics + Analytics дают 80% покрытия за неделю. AWS-стек ставим, когда выручка оправдает SRE-нагрузку.
> **Phase 0 минимум (только то, что реально нужно):**
>   - **Firebase Hosting** — статический CDN для `config.json` (наш дешёвый Remote Config). Не используем Firebase Remote Config SDK — он требует Google SDK на клиенте, не подходит для Godot без плагина.
>   - Auth, Firestore, Functions — отложены до Phase 2.
> **Последствия:**
>   - Слой `services/backend_facade.gd` (Phase 2) абстрагирует провайдера, чтобы миграция не требовала переписывания клиента
>   - В Phase 0 клиент общается с backend только через прямой HTTPRequest к Hosting URL
>   - Конфиг публичный, никаких секретов в `config.json`

### Mapping Firebase ↔ AWS
| Функция | Firebase (MVP) | AWS (prod) |
|---------|----------------|------------|
| Auth | Firebase Auth (Apple/Google/anon) | Cognito |
| Save sync | Firestore document per user | API Gateway + Lambda + PostgreSQL |
| Remote Config | Firebase RC | S3 + CloudFront + custom service |
| Analytics | Firebase Analytics + BigQuery | Custom event collector + S3 + Athena |
| Crash | Crashlytics | Sentry / Bugsnag |
| Push | FCM | FCM (через SNS) |

## Endpoints (целевые, REST)

| Метод | Путь | Назначение |
|-------|------|------------|
| POST | `/auth/login` | exchange platform token → JWT |
| POST | `/auth/refresh` | refresh JWT |
| POST | `/save/push` | загрузить локальный сейв |
| GET  | `/save/pull` | получить серверный сейв |
| POST | `/save/heartbeat` | keepalive + детекция сессий |
| POST | `/iap/validate` | проверка чека Apple/Google |
| GET  | `/config` | удалённый конфиг |
| POST | `/analytics/batch` | пачка событий |
| GET  | `/events` | активные LiveOps-ивенты |
| POST | `/leaderboard/submit` | отправить score |
| GET  | `/leaderboard/:id` | топ N + ранг игрока |
| POST | `/prestige/commit` | подтвердить prestige (anti-cheat) |

Все защищённые — Bearer JWT. Rate-limit на `analytics/batch` и `save/push` (Redis token bucket).

## Хранилища

### PostgreSQL (RDS)
- `users` (id, platform_id, created_at, last_seen_at, country, settings)
- `saves` (user_id, save_blob, save_version, schema_version, last_modified_at, checksum)
- `save_history` (user_id, snapshot_at, snapshot_blob, reason) — последние 10 чекпоинтов
- `iap_purchases` (user_id, sku, store, receipt_hash, validated_at, status)
- `leaderboards` (id, user_id, score, period, snapshot_at)

### Redis
- Кеш конфигов
- Rate limiting
- Сессионные replay-токены
- Кеш топ-100 лидербордов

### S3
- Remote Config JSON (через CloudFront)
- Аналитика raw events (через Firehose / Lambda)
- Backup-снэпшоты сейвов

## Auth flow

```
Client
  │
  │ 1. Platform login (Sign in with Apple / Google)
  ▼
Platform OAuth → ID token
  │
  │ 2. POST /auth/login { id_token, platform }
  ▼
Backend
  │ — verify ID token (Apple JWKS / Google verifier)
  │ — create or upsert user
  │ — issue JWT (15 min) + refresh token (30 d)
  ▼
Client сохраняет токены в Keychain / Keystore
```

Гостевой режим: device-id → анонимный аккаунт, можно «связать» с Apple/Google позже.

## Anti-cheat

См. также `04_simulation.md`, `06_save_sync.md`.

- **IAP:** Все консьюмеры валидируются на сервере через `verifyReceipt`. Без OK от сервера — награда не выдаётся.
- **Prestige:** Клиент отправляет `prestige/commit { current_state_hash, expected_points }`. Сервер пересчитывает по формуле и сверяет.
- **Save delta:** При каждом `save/push` сервер сравнивает с предыдущим. Невозможные дельты (gold +∞, level skip) → flag.
- **Leaderboard:** Score подтверждается hash от save_state.
- **Soft-ban:** Flag → отключаем рекламу + покупки, но не банарим явно; повторные флаги → ручной review.

## Push-нотификации

Через FCM (iOS через APNs внутри FCM). Сценарии:
- D+1: «Урожай готов!»
- D+3: пропущенный игрок — пакет приветствия
- Событие началось: за 1 час до старта
- Подписка — рекомендации

Локальные пуши (без сервера) — через `OS.set_notifications()` / нативные плагины: для ежедневного login-bonus.

## Прод-окружения

| Среда | Назначение | URL |
|-------|------------|-----|
| dev | разработка | `dev-api.animeempire.example` |
| qa | внутренний QA | `qa-api.animeempire.example` |
| prod | релиз | `api.animeempire.example` |

CI/CD: GitHub Actions → деплой Terraform / Serverless Framework.

## SRE-минимумы

- Monitoring: CloudWatch / Grafana (latency, error rate)
- Alerts: PagerDuty (5xx > 1%, latency p95 > 500 мс)
- SLO: 99.5% availability MVP, 99.9% после launch
- RPO/RTO для сейвов: 24ч / 1ч

## Открытые вопросы

- [ ] Регионы AWS на старте: us-east-1 + eu-west-1 — достаточно?
- [ ] Cold start Lambda для config-endpoint — рассмотреть provisioned concurrency
- [ ] GDPR data export endpoint — отдельный сервис или ad-hoc?
