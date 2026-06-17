# 09 · Мета-прогрессия (Prestige)

## Источник

`GDD/7_meta_progression.md`. Здесь — конкретные формулы, ветки апгрейдов, таргеты.

## Концепция

Prestige = воспалительный (опциональный) сброс прогресса в обмен на permanent множители. Игрок выбирает момент сам, когда апгрейды стали «дорогими».

## Триггер

Кнопка `Prestige` доступна, когда:
- `current_income_per_hour >= threshold(prestige_level)`
- `total_building_levels >= min_buildings(prestige_level)`
- `chains_completed >= 1`

```gdscript
threshold(0) = 5_000      # первая prestige
threshold(1) = 25_000
threshold(2) = 100_000
threshold(n) = threshold(n-1) × 3
```

## Формула очков

```
points = floor(
    sqrt(income_per_hour / 100_000) +
    total_building_levels / 100 +
    chains_completed × 0.5 +
    log10(hours_played + 1) × 0.2
)
```

### Прогнозный темп
| Prestige # | Earned points (range) |
|------------|-----------------------|
| P1 | 3-6 |
| P2 | 5-9 |
| P3 | 7-12 |
| P4-P10 | 10-20 |
| P10+ | 20+ |

## Что сбрасывается

- Gold
- Уровни зданий → 1
- Нанятые NPC → 0 (но мета-апгрейд может вернуть стартовых)
- Tool tiers → 0
- Character stats → базовые

## Что остаётся

- Prestige points (общий счёт)
- Купленные prestige upgrades
- Cosmetics
- Achievements
- Discovered NPCs (для коллекции)
- Subscription state, IAP non-consumables

## Ветки апгрейдов

### Track 1 — Income
| Node | Effect | Cost (pts) | Cumulative cost |
|------|--------|------------|-----------------|
| 1 | +5% income | 1 | 1 |
| 2 | +5% income | 2 | 3 |
| 3 | +5% income | 3 | 6 |
| 4 | +5% income | 5 | 11 |
| 5 | +5% income | 8 | 19 |
| ... | Fibonacci продолжается | ... | ... |

Линейно нарастающий +5% / nodes, но Fibonacci cost — на N-м уровне игрок думает «куда дальше».

### Track 2 — Speed
+3% production speed per node. Аналогичный Fibonacci.

### Track 3 — Starting bonus
- +1 NPC slot per 3 pts
- +500 gold start per 1 pt
- +5% offline efficiency per 2 pts

### Track 4 — Region access
- Mountain unlock: 3 cumulative prestige points
- Coast: 5
- Desert: 10
- Magical: 15
- Tech: 20

### Track 5 — Cosmetic (необязательно)
- Player skin variants
- Building visual themes

## Pacing цели

Из `GDD/7`:
- 60% игроков достигают P1 к D7
- 35% достигают многократного prestige к D14
- 20% доходят до P5 к D30

## UX престижа

См. `05_ui_ux_spec.md` (модалка). Ключевые принципы:
- Показать «что будет потом» — превью бонусов
- Не давить (это опционально, можно копить)
- При первом prestige — расширенная анимация и tutorial
- После prestige — `Welcome back!` модалка с пояснением, что осталось

## Меры предосторожности

- Anti-cheat: backend подтверждает prestige (см. `architecture/04_simulation.md`)
- Откат при flag: возвращается к pre-prestige state (server checkpoint)
- Confirmation dialog с double-tap (избегаем случайных prestige)

## Менеджеры и prestige

Из `GDD/6`:
- Managers — глобальные мультипликаторы, доступны post-prestige
- 4 базовых: Production, Efficiency, Sales, Automation
- Каждый разблокируется на определённом prestige level

## Открытые вопросы

- [ ] Limited-time prestige bonuses (event-based) — добавлять?
- [ ] Soft cap на prestige points — нужен ли для anti-inflation?
- [ ] Refund prestige upgrades (с штрафом 20%) — UX-удобство или ловушка?
