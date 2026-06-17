# 02 · Балансировка экономики

## Источник

`GDD/5_resource_model.md`, `GDD/13_game_config.md`. Здесь — детализированные формулы и стартовые значения.

## Общая структура экономики

```
Resource (raw) ──→ Resource (processed) ──→ Resource (final) ──→ Gold
   1 g/шт                3.5 g/шт                15 g/шт
```

Множитель ценности по стадиям ≈ 4-5x.

## Формулы

### Стоимость апгрейда здания
```
cost(level) = base_cost × growth^(level - 1)
```
- `base_cost` — из `BuildingDef`
- `growth` — 1.12 (ранняя), 1.15 (средняя), 1.18 (поздняя)

### Доход здания
```
output_per_cycle(level) = base_output × output_growth^(level - 1)
cycle_seconds(level) = base_cycle × (cycle_decay^(level - 1))
income_rate(level) = output_per_cycle / cycle_seconds × sell_price
```

### Прогрессия NPC
```
hire_cost(n) = base_hire × hire_growth^n
efficiency = clamp(0.50 + 0.05 × tier, 0.50, 1.00)
```

### Prestige points
Из `GDD/7`:
```
points = sqrt(income_per_hour / 100_000) + buildings_total / 100 + chains_completed × 0.5 + time_bonus
time_bonus = log10(hours_played + 1) × 0.2
```

### Offline earnings
```
offline_gold = min(elapsed_seconds, 4h) × current_rate × 0.5
```

## Стартовые конфиги (MVP)

### Здания (ранний тир, growth 1.12)

| Building | base_cost | output | cycle (с) | sell_price | growth |
|----------|-----------|--------|-----------|------------|--------|
| Wheat Farm | 100 | 1 wheat | 1.0 | — | 1.12 |
| Mill | 500 | 1 flour (из 3 wheat) | 10 | — | 1.12 |
| Bakery | 2500 | 1 bread (из 2 flour) | 30 | — | 1.12 |
| Market | 200 | sell | — | wheat:1, flour:3.5, bread:15 | 1.12 |

### Цепочка лес → мебель (вторая для MVP)

| Building | base_cost | output | cycle | sell_price |
|----------|-----------|--------|-------|------------|
| Forest | 200 | 1 wood | 2.0 | — |
| Lumberyard | 1500 | 1 plank (из 2 wood) | 15 | — |
| Workshop | 7500 | 1 furniture (из 3 planks) | 60 | — |
| Market sell | — | — | — | wood:2, plank:8, furniture:50 |

### Сравнение rate-of-income (Lv 1)

| Цепочка | Gold/min при идеальной работе |
|---------|-------------------------------|
| Wheat → Bread | ~10 |
| Wood → Furniture | ~18 |

## Кривая прогрессии (моделирование)

| Время игры | Уровень | Income/min | Кумулятивный gold |
|------------|---------|-----------|-------------------|
| 5 мин | Wheat Lv 3, Mill Lv 1 | 50 | 250 |
| 30 мин | Wheat Lv 8, Mill Lv 5, Bakery Lv 2 | 250 | 5K |
| 1 ч | Wheat Lv 15, Bakery Lv 8 | 600 | 25K |
| 2 ч | + 2-я цепочка | 2K | 150K |
| 5 ч | Готов к первому prestige | 8K | 1M |

Целевой темп `first_prestige`: 4-6 часов в первой сессии (suite of 6-8 сессий).

## Sinks (куда уходит gold)

- Апгрейды зданий (главное)
- Найм NPC
- Апгрейды tools
- Апгрейды character (speed, capacity, harvest, price)
- Sklad / warehouse (вторичное)

Источники только: продажа ресурсов. Это ключевое — gold не печатается «из воздуха».

## Gems (premium currency)

| Action | Cost |
|--------|------|
| Skip cycle | 2 gems |
| Auto-collect 1 ч | 10 gems |
| Permanent NPC slot | 100 gems |
| Skin / cosmetic | 200 gems |
| Daily speed boost +50% | 50 gems |

Gems источники:
- IAP (основное)
- Daily login (5/day) → 150/мес
- Achievements (~200 за все)
- Quests (~10/day)
- Rewarded ads (1/ad)

## Anti-inflation

При длительной игре gold растёт экспоненциально. Чтобы не было «бесконечности»:
- Cost growth (1.12 → 1.18) обгоняет income growth
- Display очень больших чисел через суффиксы (K/M/B/T/aa/ab/...) — стандарт idle-жанра
- Prestige reset обнуляет gold, но даёт permanent multiplier

## Tuning loop

Балансировка — итеративная:
1. Симулятор в Python/Sheets гоняет прогрессию
2. Корректируем числа в `.tres`
3. Playtest сессии (3-5 ч)
4. Аналитика после soft-launch (% stuck at level X)

## Прогноз ARPU

Из `GDD/8`: $0.08 ARPDAU старт. Раскладка:
- F2P: 95% (0$)
- Light spenders ($1-5/мес): 4%
- Mid ($5-50/мес): 0.8%
- Whales ($50+/мес): 0.2%

Балансим так, чтобы whales не «ломали» игру (нет P2W).

## Открытые вопросы

- [ ] Soft cap или hard cap на скорость apgrades-per-second?
- [ ] Sink через warehouse capacity — ввести как обязательный?
