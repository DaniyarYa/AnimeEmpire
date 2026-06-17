# Phase 5 · Post-Launch (6+ месяцев)

## Цель

Continuous development: LiveOps события, престиж + регионы, подписка, социалка. Спринты 2-недельные.

## Phase exit criteria

Continuous. Целевые KPI:
- DAU > 50K через 3 мес после launch
- D30 retention 8%
- ARPDAU $0.12+ к концу 6 мес
- Минимум 1 событие / неделя

## E1 · Prestige система

- [ ] **P5-001** • Prestige UI screen + flow
  - Files: `scenes/ui/prestige_screen.tscn`
  - Estimate: L
- [ ] **P5-002** • Формула очков (см. `design/09_meta_progression.md`)
  - Estimate: M
- [ ] **P5-003** • Reset state + сохранение permanent данных
  - Estimate: M
- [ ] **P5-004** • Prestige upgrades tree (4 трека)
  - Estimate: L
- [ ] **P5-005** • Backend validation prestige
  - Estimate: M
- [ ] **P5-006** • UI первого prestige (расширенная анимация и tutorial)
  - Estimate: M

## E2 · LiveOps event engine

- [ ] **P5-010** • `EventDef` ресурс
  - Estimate: S
- [ ] **P5-011** • `EventManager` autoload
  - Estimate: L
- [ ] **P5-012** • Event UI (HUD-индикатор, events screen, прогресс)
  - Estimate: L
- [ ] **P5-013** • Event currency (отдельные валюты + shop)
  - Estimate: L
- [ ] **P5-014** • Reward tiers + claim
  - Estimate: M
- [ ] **P5-015** • Backend leaderboard endpoint
  - Estimate: L
- [ ] **P5-016** • Leaderboard UI
  - Estimate: M
- [ ] **P5-017** • Push notification: за 1 ч до начала / конца event
  - Estimate: M

## E3 · Первые ивенты (первые 2 месяца)

- [ ] **P5-020** • Farm Festival (production, 48 ч, +100% wheat yield)
  - Estimate: M
- [ ] **P5-021** • Build Challenge (progression, 7 д)
  - Estimate: M
- [ ] **P5-022** • NPC Hunt (collection, 14 д)
  - Estimate: L
- [ ] **P5-023** • Winter Season (mega, 30 д)
  - Estimate: XL

## E4 · Регион Mountain (первая экспансия)

- [ ] **P5-030** • Region unlock UI (prestige 3 req)
  - Estimate: M
- [ ] **P5-031** • Mountain scene + камера переход
  - Estimate: L
- [ ] **P5-032** • Новые ресурсы (stone, iron)
  - Estimate: M
- [ ] **P5-033** • Новые здания (quarry, smelter)
  - Estimate: L
- [ ] **P5-034** • Mountain NPCs (3DAI генерация)
  - Estimate: L
- [ ] **P5-035** • Mountain BGM + ambient
  - Estimate: M

## E5 · Подписка

- [ ] **P5-040** • Subscription IAP setup
  - Estimate: M
- [ ] **P5-041** • Subscription benefits implementation (2x offline, 50% speed, ad-free)
  - Estimate: M
- [ ] **P5-042** • Subscription UI screen
  - Estimate: M
- [ ] **P5-043** • Server-side subscription validation + renewal
  - Estimate: L

## E6 · Daily quests

- [ ] **P5-050** • Quest pool + rotation
  - Estimate: M
- [ ] **P5-051** • Quest UI
  - Estimate: M
- [ ] **P5-052** • Quest reward grant
  - Estimate: S

## E7 · Социалка

- [ ] **P5-060** • Friends list (Phase 5.2)
  - Estimate: XL
- [ ] **P5-061** • Friends leaderboard
  - Estimate: M
- [ ] **P5-062** • Share achievement
  - Estimate: M

## E8 · Specialists и Managers

- [ ] **P5-070** • Specialist NPC (3-5 шт, отыгрывают мини-эффекты)
  - Estimate: L
- [ ] **P5-071** • Manager NPC (4 шт, глобальные мультипликаторы)
  - Estimate: L
- [ ] **P5-072** • Manager UI

## E9 · Battle pass (Phase 5.3, опционально)

- [ ] **P5-080** • Season pass design + tiers
  - Estimate: L
- [ ] **P5-081** • Free + premium tracks
  - Estimate: L
- [ ] **P5-082** • Реализация в Godot
  - Estimate: XL

## E10 · Регионы 2-4 (Phase 5.4-5.6)

Каждый регион — 2-3 нед работ:

- [ ] **P5-090** • Coast (prestige 5)
  - Estimate: XL
- [ ] **P5-091** • Desert (prestige 10)
  - Estimate: XL
- [ ] **P5-092** • Magical Realm (prestige 15)
  - Estimate: XL
- [ ] **P5-093** • Tech Era (prestige 20)
  - Estimate: XL

## E11 · Локализация tier-2

- [ ] **P5-100** • Portuguese (pt-BR)
  - Estimate: M
- [ ] **P5-101** • Russian (ru)
  - Estimate: M
- [ ] **P5-102** • Korean (ko)
  - Estimate: M
- [ ] **P5-103** • Chinese (zh-CN) — требует доп. публикации
  - Estimate: L

## E12 · Continuous

- [ ] **P5-110** • Еженедельный балансовый ревью
- [ ] **P5-111** • Еженедельный bug-bash
- [ ] **P5-112** • Ежемесячный roadmap-review
- [ ] **P5-113** • Quarterly retrospective

## Спринт-кейденс (рекомендация)

```
Спринт 1-2: Prestige + первый event
Спринт 3-4: Mountain region
Спринт 5-6: Подписка + daily quests
Спринт 7-8: Coast region + specialists
Спринт 9-10: Battle pass + Korean loc
Спринт 11-12: Desert region + managers
...
```

## Метрики успеха

- DAU growth ≥ 5% / месяц первые 6 мес
- Retention D30 ≥ 8%
- ARPDAU $0.12+
- Event participation > 60%
- App Store rating > 4.5
