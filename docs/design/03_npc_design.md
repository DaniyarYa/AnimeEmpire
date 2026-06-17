# 03 · Дизайн NPC

## Источник

`GDD/6_npc_system.md`. Здесь — стат-блоки, AI-состояния, прогрессия.

## Категории

| Категория | Когда появляется | Функция | Активность |
|-----------|------------------|---------|------------|
| Gatherer | Day 2-3 | Собирает ресурсы у генераторов | Active 3D |
| Carrier | Day 3-4 | Носит ресурсы между зданиями | Active 3D |
| Operator | Day 5-7 | Усиливает конкретное здание | Passive (modifier) |
| Specialist | Mid-game | Building-specific bonus (+25% mill speed) | Passive (modifier) |
| Manager | Endgame (post-prestige) | Глобальный мультипликатор | UI-only |

## Стат-блоки (MVP)

### Gatherer Farmer (стартовый)
```
id: gatherer_farmer
category: gatherer
rarity: common
base_speed: 2.0 (units/sec)
base_capacity: 5
base_efficiency: 0.75
hire_cost_gold: 1000
attached_building_category: "generator"
bonus_modifiers: []
```

### Gatherer Lumberjack
```
id: gatherer_lumberjack
base_speed: 1.8
base_capacity: 4
base_efficiency: 0.80
hire_cost_gold: 1500
attached_building_category: "generator"
```

### Gatherer Miner
```
id: gatherer_miner
base_speed: 1.5
base_capacity: 6
base_efficiency: 0.70
hire_cost_gold: 2500
```

### Gatherer Fisher
```
id: gatherer_fisher
base_speed: 2.5
base_capacity: 3
base_efficiency: 0.85
hire_cost_gold: 2000
```

### Carrier Porter
```
id: carrier_porter
category: carrier
base_speed: 3.0
base_capacity: 10
base_efficiency: 0.80
hire_cost_gold: 3500
attached_building_category: ""
```

### Carrier Merchant
```
id: carrier_merchant
base_speed: 2.5
base_capacity: 15
base_efficiency: 0.85
hire_cost_gold: 5000
```

## Operators (Phase 2)

### Mill Operator
```
id: operator_mill
category: operator
attached_building_category: "processor:mill"
bonus_modifiers:
  - {type: "cycle_speed", value: 0.25}  # +25% speed
hire_cost_gold: 8000
```

### Bakery Operator
```
id: operator_bakery
bonus_modifiers:
  - {type: "cycle_speed", value: 0.20}
  - {type: "output_amount", value: 0.10}
hire_cost_gold: 12000
```

## Specialists (Phase 5)

Редкие, ограниченное число. Каждый — уникальный бонус.

| ID | Bonus | Rarity |
|----|-------|--------|
| `specialist_master_baker` | +50% bakery output | epic |
| `specialist_quartermaster` | +25% storage all | epic |
| `specialist_inventor` | -10% upgrade cost all | legendary |

## Managers (Phase 5, после первого prestige)

Глобальные эффекты. Не появляются в 3D.

| ID | Effect |
|----|--------|
| `manager_production` | +10% all production |
| `manager_efficiency` | +15% all NPC efficiency |
| `manager_sales` | +12% sell price |
| `manager_automation` | +4h offline cap |

## AI: FSM

```
       ┌──────┐
       │ IDLE │ ← старт / после DELIVER
       └──┬───┘
          │ assign_task
          ▼
   ┌──────────────┐
   │ MOVING_TO_SRC│
   └──┬───────────┘
      │ target_reached
      ▼
   ┌──────────┐
   │ WORKING  │ ← таймер cycle_seconds
   └──┬───────┘
      │ work_done
      ▼
   ┌──────────────┐
   │ MOVING_TO_TGT│
   └──┬───────────┘
      │ target_reached
      ▼
   ┌──────────┐
   │ DELIVER  │
   └──┬───────┘
      │ delivered
      ▼
   ┌────────┐
   │ RETURN │
   └──┬─────┘
      │ at_home
      ▼
    IDLE
```

### Переходы — параметры
- Все таймеры — детерминированные (для anti-cheat)
- `navigation_finished` сигнал от `NavigationAgent3D`
- На IDLE NPCSystem может перепривязать к другому зданию

### Edge cases
- Здание разрушено / перенесено: NPC возвращается в IDLE
- Здание заполнено инвентарём: NPC ждёт (UI hint)
- Path не найден: NPC телепортируется (fallback)

## Найм и слоты

- Каждое здание имеет `npc_slots` (1-4)
- Игрок открывает слот апгрейдом
- Нанятый NPC привязан к конкретному зданию (можно перепривязать в UI)

## Прогрессия NPC

NPC можно повышать в уровне (Phase 3+):
- Уровень даёт +efficiency и +capacity
- Стоимость в gold или специальном ресурсе (training tokens)

## Раритеты

- Common: drop / гарантированно нанимаются
- Rare: специальные ивенты, низкий шанс
- Epic: эвент-награды, IAP
- Legendary: подписка / эксклюзивные ивенты

## Визуальные требования

См. `06_art_direction.md` и `architecture/08_asset_pipeline.md`.

- Chibi 500-1000 трисов
- Анимации: idle, walk, work (зависит от категории), carry, celebrate
- Портреты 1024×1024 для UI (3-4 эмоции)

## Открытые вопросы

- [ ] Рандомизация имён NPC (тематика по аниме) — отдельный pool
- [ ] Permadeath не предусмотрен — подтверждаем
