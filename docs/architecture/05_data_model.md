# 05 · Модель данных

## Подход

Контент описывается через Godot `Resource` (`.tres`). Это даёт:
- Редактирование в инспекторе
- Сериализация без кода
- Подмена через Remote Config (загруженный JSON → новый Resource в памяти)
- Лёгкие диффы в git (текстовый формат `.tres`)

## Иерархия классов данных

```
Resource (Godot built-in)
└── GameResource (наш базовый)
    ├── BuildingDef
    ├── ResourceDef        (товар: пшеница, мука, хлеб)
    ├── NPCDef
    ├── ToolDef            (серп, коса, комбайн)
    ├── UpgradeDef
    ├── PrestigeUpgradeDef
    ├── EventDef
    └── RegionDef
```

## BuildingDef

```gdscript
class_name BuildingDef extends Resource

@export var id: String                       # "wheat_farm"
@export var display_name_key: String         # ключ локализации
@export var category: String                 # "generator" | "processor" | "service"
@export var input_resource: ResourceDef      # null для генераторов
@export var input_amount: int = 0
@export var output_resource: ResourceDef
@export var output_amount: int = 1
@export var base_cycle_seconds: float = 1.0
@export var base_cost_gold: int = 100
@export var cost_growth: float = 1.15        # 1.12 / 1.15 / 1.18 по тиру
@export var max_level: int = 25
@export var unlock_level: int = 1            # уровень игрока для разблокировки
@export var visual_tiers: Array[PackedScene] # 4-5 визуальных тиров
@export var npc_slots: int = 1
```

## ResourceDef

```gdscript
class_name ResourceDef extends Resource

@export var id: String                       # "wheat"
@export var display_name_key: String
@export var base_sell_price: int = 1
@export var icon: Texture2D
@export var stage: int = 0                   # 0=raw, 1=processed, 2=final
@export var tier: String = "common"          # common | rare | epic | legendary
```

## NPCDef

```gdscript
class_name NPCDef extends Resource

@export var id: String                       # "gatherer_farmer"
@export var display_name_key: String
@export var category: String                 # "gatherer" | "carrier" | "operator" | "specialist" | "manager"
@export var rarity: String = "common"        # common | rare | epic | legendary
@export var base_speed: float = 2.0
@export var base_capacity: int = 5
@export var base_efficiency: float = 0.75
@export var hire_cost_gold: int = 1000
@export var attached_building_category: String = ""  # пусто = универсал
@export var model_scene: PackedScene
@export var portrait: Texture2D              # 1024×1024
@export var bonus_modifiers: Array[ModifierDef]
```

## Рантайм-стейт игрока

`PlayerSave` — не Resource, а сериализуемый Dictionary (стабильнее для миграций). Структура:

```json
{
  "save_version": 3,
  "player_id": "uuid-...",
  "created_at": 1718600000,
  "last_seen_at": 1718700000,
  "currencies": {
    "gold": 12345,
    "gems": 50
  },
  "buildings": {
    "wheat_farm": {"level": 5, "progress": 0.42, "npc_ids": ["npc_1"]},
    "mill": {"level": 2, "progress": 0.10, "npc_ids": []}
  },
  "npcs": [
    {"instance_id": "npc_1", "def_id": "gatherer_farmer", "stats": {...}}
  ],
  "tools": {"sickle": 1},
  "stats": {"speed": 1, "capacity": 1, "harvest": 1, "price": 1, "slots": 1},
  "prestige": {"level": 0, "points": 0, "upgrades": {}},
  "tutorial": {"step": 5, "completed": false},
  "session": {"streak_days": 1, "last_login": 1718700000},
  "inventory": {"wheat": 100, "flour": 25, "bread": 0},
  "active_events": [],
  "iap": {"non_consumables": [], "subscription_until": 0},
  "settings": {"sfx": 1.0, "music": 0.7, "locale": "ru"}
}
```

## Схема и миграции

```gdscript
# scripts/services/save_migrations.gd
const CURRENT_VERSION := 3

func migrate(data: Dictionary) -> Dictionary:
    var v = data.get("save_version", 0)
    while v < CURRENT_VERSION:
        match v:
            0: data = _migrate_0_to_1(data)
            1: data = _migrate_1_to_2(data)
            2: data = _migrate_2_to_3(data)
        v += 1
    data["save_version"] = CURRENT_VERSION
    return data

func _migrate_0_to_1(d: Dictionary) -> Dictionary:
    # пример: добавили поле `tools`
    if not d.has("tools"):
        d["tools"] = {}
    return d
```

Каждая миграция — отдельная функция, юнит-тест обязателен.

## Validation

- При загрузке: проверка типов, наличия обязательных полей, разумных диапазонов (gold ≥ 0, levels ≤ max_level)
- Невалидный сейв → попытка восстановить из бэкап-слота → fallback на новую игру с уведомлением

## Бэкап-сейв

- 3 слота локально: `save_0.bin` (текущий), `save_1.bin`, `save_2.bin` (предыдущие)
- Ротация при каждом сохранении

## Открытые вопросы

- [ ] JSON vs MessagePack для размера сейва? Замер на Phase 2.
- [ ] Сжимать сейв (gzip перед AES)?
