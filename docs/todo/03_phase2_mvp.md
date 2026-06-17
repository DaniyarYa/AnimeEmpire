# Phase 2 · MVP (8 нед)

## Цель

Закрытый, полный, играбельный продукт с туториалом, оффлайном, сейвом и базовой монетизацией. Готовность к soft-launch.

## Phase exit criteria

См. `00_milestones.md`.

## E1 · Симуляция (расширение)

- [ ] **P2-001** • Все 8 зданий (4 generator + 4 processor) реализованы
  - Acceptance: forest, lumberyard, workshop, mill, bakery, wheat_farm, quarry (заглушка для post-MVP), iron_mine (post-MVP)
  - Estimate: L
- [ ] **P2-002** • Вторая цепочка wood → plank → furniture работает
  - Estimate: M
- [ ] **P2-003** • Оффлайн-расчёт (≤ 4 ч × 0.5 эффективность)
  - Acceptance: при запуске после паузы — модалка с накопленным доходом
  - Estimate: L
  - Files: `autoload/economy_sim.gd`, `scenes/ui/offline_modal.tscn`
- [ ] **P2-004** • Inventory cap + warehouse уровень (опционально для MVP)
  - Estimate: M

## E2 · NPC

- [ ] **P2-010** • Все 4 Gatherer типа (Farmer, Lumberjack, Miner, Fisher) — данные + 3DAI ассеты
  - Estimate: L
  - Depends on: P0-031, art pipeline
- [ ] **P2-011** • Carrier тип (Porter, Merchant)
  - Estimate: L
- [ ] **P2-012** • NPC найм UI + slots logic
  - Files: `scenes/ui/npc_screen.tscn`
  - Estimate: L
- [ ] **P2-013** • Многочисленные NPC одновременно (10+) без перфоманс-падения
  - Acceptance: 60 FPS при 10 NPC на iPhone 8
  - Estimate: M
- [ ] **P2-014** • NPC привязка к зданию (drag-to-assign в UI)
  - Estimate: M

## E3 · Здания и апгрейды

- [ ] **P2-020** • Визуальные тиры зданий (4-5 на тип)
  - Acceptance: при апгрейде на ключевых уровнях — меняется меш
  - Estimate: XL
  - Depends on: 3D ассеты от художника
- [ ] **P2-021** • Player stats апгрейды (speed, capacity, harvest, price, slots)
  - Files: `scenes/ui/upgrades_screen.tscn`
  - Estimate: L
- [ ] **P2-022** • Tool tiers (sickle → scythe → harvester; bag → cart → wagon)
  - Estimate: L

## E4 · UI/UX (полный)

- [ ] **P2-030** • Все screens из `design/05_ui_ux_spec.md`
  - Estimate: XL (это самая большая задача MVP)
- [ ] **P2-031** • Settings (volume, language, account, privacy)
  - Estimate: M
- [ ] **P2-032** • Restore Purchases UX (Apple требование)
  - Estimate: S
- [ ] **P2-033** • Onboarding modal flow
  - Estimate: M

## E5 · Туториал

- [ ] **P2-040** • TutorialController autoload
  - Acceptance: state machine со шагами, фиксирует прогресс в сейве
  - Estimate: L
- [ ] **P2-041** • Подсветка UI элементов через mask / cutout
  - Estimate: M
- [ ] **P2-042** • Все 15-20 шагов первой сессии скриптованы
  - Acceptance: новый игрок проходит туториал за 3-5 мин, достигает первого апгрейда
  - Estimate: L
- [ ] **P2-043** • Skip-tutorial option (для тест/QA билдов)
  - Estimate: S

## E6 · Сейв (полный)

- [ ] **P2-050** • AES-256 шифрование сейва
  - Files: `services/save_service.gd`, `services/crypto.gd`
  - Estimate: L
- [ ] **P2-051** • Бэкап-слоты (3)
  - Estimate: M
- [ ] **P2-052** • Облачный sync (push/pull)
  - Acceptance: смена устройства → восстанавливается прогресс
  - Estimate: XL
- [ ] **P2-053** • Миграции схемы (v1 → v2 → v3)
  - Acceptance: юнит-тесты на миграции
  - Estimate: M

## E7 · Монетизация

- [ ] **P2-060** • IAP плагин Godot интегрирован (iOS + Android)
  - Estimate: L
  - Notes: использовать community плагин или GDExtension
- [ ] **P2-061** • SKU подключены (gem packs, starter pack, remove ads)
  - Estimate: M
- [ ] **P2-062** • Серверная валидация receipt
  - Files: backend `iap-validate` endpoint
  - Estimate: L
- [ ] **P2-063** • AdMob (или альтернатива) интегрирована
  - Estimate: L
- [ ] **P2-064** • Rewarded ad placements (offline 2x, instant finish, booster)
  - Estimate: M
- [ ] **P2-065** • Shop screen
  - Estimate: L
- [ ] **P2-066** • Starter pack UX (показ при условии, окно 7 дней)
  - Estimate: M

## E8 · Backend

- [ ] **P2-070** • Auth (Apple / Google / guest)
  - Files: backend `/auth/login`, `/auth/refresh`
  - Estimate: L
- [ ] **P2-071** • Save sync endpoint
  - Estimate: L
- [ ] **P2-072** • Remote config endpoint
  - Estimate: M
- [ ] **P2-073** • Analytics batch endpoint
  - Estimate: M

## E9 · Аналитика

- [ ] **P2-080** • `AnalyticsBus` + батчинг
  - Estimate: M
- [ ] **P2-081** • Все события из `architecture/11_analytics.md`
  - Estimate: M
- [ ] **P2-082** • Crashlytics / Sentry интегрирован
  - Estimate: M
- [ ] **P2-083** • Воронка туториала видна в дашборде
  - Estimate: M
  - Depends on: P2-040, P2-080

## E10 · Локализация

- [ ] **P2-090** • Tier-1 локали (en, es, fr, de, ja)
  - Acceptance: все UI strings переведены, native review для en, ja
  - Estimate: XL
- [ ] **P2-091** • Auto-detect локаль на старте
  - Estimate: S
- [ ] **P2-092** • Number formatting per-locale
  - Estimate: S

## E11 · Аудио

- [ ] **P2-100** • BGM village
  - Estimate: M
  - Notes: либо фриланс composer, либо asset-pack
- [ ] **P2-101** • Все SFX из `design/07_audio_design.md`
  - Estimate: M
- [ ] **P2-102** • Audio buses + volume settings
  - Estimate: S

## E12 · Performance

- [ ] **P2-110** • Profiler-проход на min spec device
  - Acceptance: 60 FPS, < 200 MB, < 3 c cold start
  - Estimate: L
- [ ] **P2-111** • Asset compression (ETC2 / ASTC)
  - Estimate: M
- [ ] **P2-112** • Pool для NPC, particles, floating-text
  - Estimate: M

## E13 · QA

- [ ] **P2-120** • Test plan документ
  - Estimate: M
- [ ] **P2-121** • Internal alpha test (5-10 человек × 1 нед)
  - Estimate: M
- [ ] **P2-122** • Bug bash 2 раза в неделю
  - Estimate: M
- [ ] **P2-123** • Compatibility test на 5-10 устройствах
  - Estimate: M

## Итого Phase 2

~60 задач, 8 нед для команды 3-4 человека. По завершении — soft-launch ready.
