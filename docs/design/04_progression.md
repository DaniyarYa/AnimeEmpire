# 04 · Прогрессия

## Источник

`GDD/4_game_progression.md`, `GDD/7_meta_progression.md`. Здесь — конкретные тиры, разблокировки, цепочки.

## Здания (MVP — 8)

### Generators (4)
| ID | Open at | Output | Max Lv |
|----|---------|--------|--------|
| `wheat_farm` | start | wheat | 25 |
| `forest` | Player Lv 3 | wood | 25 |
| `quarry` | Player Lv 8 | stone | 25 |
| `iron_mine` | Player Lv 15 | iron | 25 |

### Processors (4)
| ID | Open at | Input → Output | Max Lv |
|----|---------|----------------|--------|
| `mill` | Player Lv 3 | wheat → flour | 25 |
| `bakery` | Player Lv 8 | flour → bread | 25 |
| `lumberyard` | Player Lv 5 | wood → plank | 25 |
| `workshop` | Player Lv 12 | plank → furniture | 25 |

### Services (Phase 2+, опционально для MVP)
- `market` — sell point (всегда есть на старте)
- `warehouse` — увеличивает inventory cap

## Цепочки производства (MVP)

```
Wheat Farm → Mill → Bakery → Market
   (wheat)   (flour)  (bread)  (gold ×15)

Forest → Lumberyard → Workshop → Market
 (wood)   (plank)    (furniture) (gold ×50)
```

Post-MVP:
- Quarry → ? (камень для построек?)
- Iron Mine → Blacksmith → Weapons / Tools

## Tool tiers (Phase 2)

Игрок прокачивает свои инструменты:

| Tool | Tier 1 | Tier 2 | Tier 3 | Tier 4 |
|------|--------|--------|--------|--------|
| Sickle / Scythe | base | +25% harvest | +50% harvest | +100% harvest |
| Bag / Cart / Wagon | cap 10 | cap 20 | cap 35 | cap 60 |

## Character stats

| Stat | Базовое | Макс. через апгрейды | Способ |
|------|---------|----------------------|--------|
| Speed | 100% | +160% (260%) | gold-апгрейды |
| Capacity | 10 | +10 (20) | gold-апгрейды |
| Harvest | 100% | +100% (200%) | gold-апгрейды |
| Price | 100% | +22.5% (122.5%) | gold-апгрейды |
| NPC slots | 4 | +8 (12) | gold + gems |

## Уровень игрока

```
xp_to_next_level(n) = 100 × n^1.5
```

XP за действия:
- Апгрейд здания: 10 × building_level
- Продажа партии: 1 за каждые 10 gold
- Найм NPC: 50
- Открытие здания: 200
- Первая prestige: 5000

## Meta-progression (Prestige)

См. `09_meta_progression.md`.

Кратко:
- Доступен с Day 3 (после ~5 часов игры)
- Точка триггера: достижение income threshold
- Сброс: gold, уровни зданий, NPC
- Сохранение: prestige points, prestige upgrades, achievements, cosmetics

### Prestige earned (формула)
```
points = floor(sqrt(income_per_hour / 100_000) + buildings_total_levels / 100 + chains_completed × 0.5 + time_bonus)
```

### Prestige tracks (упрощённое дерево)
- **Income** — +5% per node, costs Fibonacci (1,2,3,5,8,13,21,...) points
- **Speed** — +3% per node
- **Starting gold** — +1K per node (cap 50K)
- **NPC slots** — +1 per 3 nodes (cap 12)

## Региональная экспансия (Phase 5)

| Регион | Prestige req | Новое содержимое |
|--------|--------------|------------------|
| Village (base) | 0 | стартовый сетап |
| Mountain | 3 | mining, stone-цепочка, ice-вариация |
| Coast | 5 | fishing, salt, ship cargo |
| Desert | 10 | exotic, jewelry, гл. ресурс — gold ore |
| Magical Realm | 15 | magic items, enchant система |
| Tech Era | 20 | factories, robots, automation |

Каждый регион — отдельный SubViewport / сцена. Переключение через `RegionPicker`.

## Достижения

Из `GDD/12`. Группы:
- **Production:** «Собери 1M пшеницы», «Произведи 10K хлеба»
- **Wealth:** «Заработай 1M gold», «Открой prestige 5»
- **Mastery:** «Прокачай здание до 25»
- **Collection:** «Найми всех Operators», «Завершите цепочку»
- **Time:** «Играй 7 дней подряд», «Зарегистрировался год назад»

Награды — gems + cosmetics.

## Daily quests (Phase 3)

3-5 заданий в день, обновляются в 00:00 UTC. Примеры:
- Произведи N бутербродов
- Апгрейдни 5 раз
- Выполни prestige
- Потрать 1000 gold

Награды: gold, gems, временные бустеры.

## Открытые вопросы

- [ ] Talent tree (RPG-уровень игрока) — добавлять или не нужно?
- [ ] Pet-система (бонусы) — отдельная механика или часть NPC?
