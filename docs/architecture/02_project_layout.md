# 02 · Структура проекта Godot

## Корневая структура

```
AnimeEmpire/
├── GDD/                          # геймдизайн (только для чтения от кода)
├── docs/                         # архитектура / дизайн / TODO
├── godot/                        # сам Godot-проект
│   ├── project.godot
│   ├── default_env.tres
│   ├── export_presets.cfg
│   ├── addons/                   # плагины (Beehave, AdMob, и т.д.)
│   ├── autoload/                 # синглтоны (GameState.gd, EventBus.gd, ...)
│   ├── scenes/
│   │   ├── boot/                 # стартовая загрузка, splash
│   │   ├── world/                # игровая сцена (3D мир)
│   │   ├── ui/                   # экраны и виджеты Control
│   │   └── shared/               # переиспользуемые сцены
│   ├── scripts/
│   │   ├── systems/              # EconomySim, NPCSystem, ProductionLine, ...
│   │   ├── entities/             # NPC, Building, Player, ResourceItem
│   │   ├── data/                 # классы Resource (BuildingDef, NPCDef, ...)
│   │   ├── services/             # SaveService, RemoteConfig, MonetizationService
│   │   ├── platform/             # IAP, Ads, Analytics обёртки
│   │   ├── ui/                   # контроллеры экранов
│   │   └── utils/                # хелперы, math, easing
│   ├── resources/                # .tres-конфиги (data-driven содержимое)
│   │   ├── buildings/
│   │   ├── resources_chain/
│   │   ├── npcs/
│   │   ├── upgrades/
│   │   └── events/
│   ├── assets/
│   │   ├── characters/           # .glb + анимации (LFS)
│   │   ├── buildings/
│   │   ├── environments/
│   │   ├── portraits/            # 1024×1024 анимэ-портреты
│   │   ├── ui/                   # иконки, фоны (LFS)
│   │   ├── audio/                # SFX / музыка (LFS)
│   │   └── fx/                   # эффекты
│   ├── themes/                   # Theme .tres + шрифты
│   ├── locales/                  # CSV/PO локализации
│   └── tests/                    # unit / integration (gdUnit4)
├── tools/                        # скрипты сборки, миграций, балансировки
├── .github/workflows/            # CI
├── .gitattributes                # LFS-пути
├── .gitignore
└── README.md
```

## Конвенции именования

| Сущность | Конвенция | Пример |
|---------|-----------|--------|
| Файлы скриптов | `snake_case.gd` | `economy_sim.gd` |
| Файлы сцен | `snake_case.tscn` | `npc_gatherer.tscn` |
| Ресурсы (data) | `snake_case.tres` | `wheat_farm_lv1.tres` |
| Классы | `PascalCase` (через `class_name`) | `EconomySim`, `BuildingDef` |
| Autoload-узлы | `PascalCase` | `EventBus`, `GameState` |
| Сигналы | `snake_case`, прошедшее время | `resource_produced`, `npc_arrived` |
| Константы | `SCREAMING_SNAKE_CASE` | `MAX_NPC_PER_BUILDING` |
| Приватное | `_leading_underscore` | `_tick_accumulator` |

## Правила импортов и зависимостей

- **Никаких циклов:** `Presentation → Logic → Simulation → Data`. Нижний слой не знает о верхнем.
- Скрипты в `scripts/systems/` не должны импортировать `scripts/ui/`. Связь только через `EventBus`.
- `addons/*` — изолирован, проектный код не зависит от конкретного аддона напрямую, оборачиваем фасадом.

## Resource-driven контент

Контент описывается через `.tres`, чтобы:
- Гейм-дизайнер мог править баланс без касания кода
- Remote Config мог подменять значения (см. `07_remote_config.md`)
- Миграции схемы версионировались (см. `05_data_model.md`)

Пример: `resources/buildings/wheat_farm.tres` ссылается на `scripts/data/building_def.gd` (тип `BuildingDef`).

## Сцены — состав узлов

Принципы:
- Корень сцены — наследник базового типа (`Node3D`, `Control`, `CharacterBody3D`)
- Логика — отдельный `.gd` скрипт, прикреплённый к корню
- Дочерние узлы — только визуал/коллизии. Подсцены ссылаются на отдельные `.tscn`

## Git LFS

`*.glb`, `*.fbx`, `*.png`, `*.psd`, `*.wav`, `*.ogg`, `*.mp3` — в LFS. См. `.gitattributes`.

## Открытые вопросы

- [ ] Outsource `assets/` в отдельный sub-репозиторий? (если LFS станет тяжёлым)
