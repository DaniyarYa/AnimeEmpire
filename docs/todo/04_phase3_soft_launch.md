# Phase 3 · Soft-Launch (4 нед)

## Цель

Запустить ограниченный релиз в небольших рынках (Канада, Скандинавия, Австралия, Новая Зеландия), собрать метрики, отбалансировать, провести A/B-тесты, исправить критические баги.

## Phase exit criteria

См. `00_milestones.md` — D1 ≥ 35%, crash-free ≥ 99%, ARPDAU ≥ $0.05.

## E1 · Soft-launch подготовка

- [ ] **P3-001** • App Store Connect / Google Play Console — создать релиз
  - Acceptance: TestFlight + Closed Track готовы
  - Estimate: M
- [ ] **P3-002** • Privacy Policy + Terms of Service
  - Estimate: S
- [ ] **P3-003** • Маркетинг-ассеты: screenshots, video, описание (en)
  - Estimate: M
- [ ] **P3-004** • Compliance: GDPR opt-in, COPPA age gate
  - Estimate: M
- [ ] **P3-005** • Soft-launch UA budget plan
  - Estimate: S

## E2 · UA для soft-launch

- [ ] **P3-010** • Facebook / Google Ads кампания на small markets
  - Estimate: M
- [ ] **P3-011** • TikTok тесты (опционально)
  - Estimate: S
- [ ] **P3-012** • Daily UA spend monitoring
  - Estimate: S
  - Notes: continuous, не одноразово

## E3 · Аналитика и BI

- [ ] **P3-020** • Дашборды retention (D1, D7, D30)
  - Acceptance: дашборд в Looker / Metabase / Firebase BI
  - Estimate: M
- [ ] **P3-021** • Дашборды monetization (ARPDAU, conversion, LTV)
  - Estimate: M
- [ ] **P3-022** • Дашборд touring funnel
  - Estimate: M
- [ ] **P3-023** • Alerting: внезапное падение D1, conversion drops
  - Estimate: M

## E4 · A/B-тесты

- [ ] **P3-030** • Туториал A (быстрый, 3 мин) vs B (стандартный, 5 мин)
  - Acceptance: ассигн через RemoteConfig, событие включает variant
  - Estimate: M
- [ ] **P3-031** • Starter pack цена $1.99 vs $2.99 vs $4.99
  - Estimate: M
- [ ] **P3-032** • Building cost growth 1.12 vs 1.15
  - Estimate: M
- [ ] **P3-033** • Report шаблон A/B-результатов
  - Estimate: S

## E5 · Баланс-итерации

- [ ] **P3-040** • Identify points игроки застревают (% stuck at level X)
  - Estimate: M
  - Depends on: P3-020
- [ ] **P3-041** • Корректировка балансов через RemoteConfig (без билда)
  - Estimate: M
- [ ] **P3-042** • Оффлайн-доход: реальный темп vs ожидаемый
  - Estimate: M

## E6 · Bug fixing

- [ ] **P3-050** • Triage Crashlytics ошибок (топ-10 по affected users)
  - Acceptance: 0 critical, ≤ 5 high
  - Estimate: L (continuous)
- [ ] **P3-051** • Crash-free sessions ≥ 99%
  - Estimate: M
- [ ] **P3-052** • Quick fixes через server-side toggle (фича-флаги)
  - Estimate: M

## E7 · Подготовка к глобальному релизу

- [ ] **P3-060** • Локализация tier-1 финальная проверка
  - Estimate: M
- [ ] **P3-061** • Push-нотификации: настройка (D+1, D+3, offline-ready)
  - Estimate: L
- [ ] **P3-062** • Migration / Scale тест бэкенда (10x users)
  - Estimate: L
- [ ] **P3-063** • SRE: monitoring + alerts + on-call rotation
  - Estimate: M

## E8 · UX-улучшения от данных

- [ ] **P3-070** • Iteration туториала на основе drop-off
  - Estimate: M
- [ ] **P3-071** • Offer placement (когда показывать starter pack)
  - Estimate: M
- [ ] **P3-072** • UI fixes (полировка анимаций, перехватов, error states)
  - Estimate: M

## E9 · Документация

- [ ] **P3-080** • Update `design/02_economy_tuning.md` финальными числами
  - Estimate: S
- [ ] **P3-081** • Post-mortem soft-launch отчёт
  - Acceptance: документ с метриками, выводами, готовностью к launch
  - Estimate: M

## Итого Phase 3

~25 задач, 4 нед. По завершении — данные подтверждают viability, готовы к глобальному релизу.
