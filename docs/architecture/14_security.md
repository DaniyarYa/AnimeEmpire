# 14 · Безопасность и приватность

## Угрозовая модель (что защищаем)

| Что | От чего | Меры |
|-----|---------|------|
| Прогресс игрока | Чит-редактирование сейва | AES-256 шифрование, сервер как source of truth, серверная валидация |
| Покупки | Receipt-spoofing | Backend `verifyReceipt` |
| Реклама | Click-fraud / SDK-абуз | Server-side reward для high-value rewarded |
| Лидерборды | Подделка score | Hash + serverside recompute |
| PII | Утечка | Минимизация сбора, шифрование при хранении, GDPR |
| Билды | Реверс-инжиниринг | Обфускация GDScript, отделение секретов |

## Шифрование сейва

См. также `06_save_sync.md`.

- Алгоритм: AES-256-CBC
- Ключ: HKDF(`device_id || obfuscated_app_secret || bundle_id`)
- IV: случайный, сохраняется вместе с шифротекстом
- HMAC-SHA256 для целостности (encrypt-then-MAC)
- Без integrity-check сейв отвергается

## Обфускация секретов

- Не хардкодим API ключи / соли в plain text
- Раскладываем по нескольким файлам, собираем в рантайме
- Простая XOR-обфускация (защита от тривиального strings-grep, не криптография)
- Реальные секреты (server-side) — никогда в клиенте

## Anti-cheat

См. `04_simulation.md`, `06_save_sync.md`, `10_backend.md`.

- Детерминированная симуляция
- Серверная валидация ключевых событий (prestige, IAP, leaderboard)
- Anomaly detection: дельты сейва вне разумных пределов → flag
- Soft-ban перед hard-ban (UX: пользователь не должен сразу терять прогресс)
- Аппеляция через support email

## Сетевая безопасность

- Все запросы — HTTPS 1.2+
- Certificate pinning для критичных endpoints (`/iap/validate`, `/save/push`)
- Запасной канал без pinning на случай сбоя CA (с понижением фичей до read-only)
- JWT-токены с коротким TTL (15 мин), refresh-токены в Keychain/Keystore
- API rate-limit на каждом endpoint

## Приватность и compliance

### GDPR (EU)
- Opt-in диалог при первом запуске для EU-пользователей
- Без согласия — только anonymous-уровень аналитики (без user_id)
- Endpoint `/user/data/export` — выгрузка JSON всех данных
- Endpoint `/user/data/delete` — полное удаление в течение 30 дней
- Privacy Policy URL обязателен в Settings

### COPPA (US, дети < 13)
- Age gate при первом запуске (если опубликовано как rated 4+)
- Для несовершеннолетних:
  - Отключены социальные функции (друзья, чат)
  - Отключены IAP без подтверждения взрослого
  - Реклама — только Family-safe (без поведенческого таргетинга)

### CCPA (California)
- Опция «Do Not Sell My Data» в Settings
- Не продаём данные третьим лицам (декларация)

### LGPD (Brazil) / PIPEDA (Canada)
- Применяем тот же базовый GDPR-уровень (минимизация, opt-in, export, delete)

### Согласие на маркетинг
- Push-нотификации — opt-in после первого дня
- Email-маркетинг — opt-in отдельно при создании аккаунта

## Хранилище секретов на устройстве

- iOS: Keychain Services (через нативный плагин)
- Android: EncryptedSharedPreferences / Keystore
- Что храним: refresh JWT, device key, маленький encryption material
- НЕ храним: full saves (они на диске под AES), пароли (используем платформенный OAuth)

## Логи

- В prod-билдах — никаких PII в логах
- Sentry/Crashlytics — фильтр перед отправкой (scrub email, IDFA, save_blob)
- Локальные логи — циркулярный буфер 200 строк, ротация

## Внешние SDK

Каждый сторонний SDK (AdMob, IAP-плагины, Analytics) проверяется на:
- Что собирает
- Какие permissions требует
- Лицензия
- Известные уязвимости

Список — в `THIRD_PARTY.md` (формируется на Phase 0).

## Permissions запрашиваемые приложением

- iOS: ATT (App Tracking Transparency) prompt
- Android: уведомления (Android 13+), хранилище (по-минимуму)
- Камера/микрофон/гео — НЕ запрашиваем (не требуется)

## Обновления

- Force-update флаг в Remote Config (для критичных багов / security fix)
- При обнаружении компрометации ключа: rotate через RC + server-side check на минимальную версию

## Penetration testing

- Перед глобальным релизом — внутренний pentest (ИЛИ внешний vendor)
- Сценарии: сейв-tampering, receipt-replay, MITM, цит-токен подмена

## Открытые вопросы

- [ ] DPIA (Data Protection Impact Assessment) — нужен ли для EU?
- [ ] Кому подаём отчёты при инцидентах (DPO, security@)?
- [ ] SDK Bug Bounty — на каком этапе?
