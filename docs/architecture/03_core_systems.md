# 03 · Базовые системы и autoload-сервисы

## Принцип

Каждый кросс-сценный сервис — autoload (синглтон). Связь между ними — через `EventBus` (pub/sub сигналов). Прямые ссылки из UI в Simulation запрещены.

## EventBus

Центральный pub/sub. Все межсистемные события идут через него.

```gdscript
# autoload/event_bus.gd
extends Node

signal resource_produced(building_id: String, resource_id: String, amount: int)
signal resource_sold(resource_id: String, amount: int, gold: int)
signal building_upgraded(building_id: String, new_level: int)
signal npc_hired(npc_id: String, instance_id: String)
signal npc_task_completed(instance_id: String, task: String)
signal save_dirty()
signal save_persisted()
signal currency_changed(kind: String, new_value: int)  # "gold" | "gems"
signal iap_completed(sku: String, success: bool)
signal prestige_triggered(points_earned: int)
signal config_loaded(version: int)
signal screen_changed(from: String, to: String)
```

Правило: события — в **прошедшем времени** (факт случился). Запросы (команды) идут прямыми вызовами методов сервисов, а не через шину.

## Список autoload-сервисов

### GameState

Текущее состояние сессии. Не хранит баланс ресурсов (это в `EconomySim`), а отражает мета-состояние.

```gdscript
extends Node
var current_screen: String = "boot"
var session_start_time: int = 0
var is_tutorial_active: bool = false
var current_prestige_level: int = 0
var current_region: String = "village"
```

### EconomySim

Главный симулятор. Тикает с фиксированной частотой 10 Hz через свой внутренний таймер. Держит производственные линии, доходы, формулы.

См. `04_simulation.md`.

### NPCSystem

Реестр активных NPC, очередь свободных задач, привязка NPC к зданиям. Тикается одновременно с `EconomySim` (или внутри его тика).

### SaveService

Сериализация состояния. Дебаунс сохранения 2 с после `save_dirty`. Полный сейв при `app_pause`. Локально AES-256, фоновый sync в backend.

См. `06_save_sync.md`.

### RemoteConfig

Загрузка конфигов с backend, кеш в `user://config_cache.json`. Фолбэк на встроенные `.tres`. Поддержка hot-reload без перезапуска.

См. `07_remote_config.md`.

### AnalyticsBus

Очередь событий. Батчинг (флаш каждые 30 с или 50 событий). Локальная очередь при оффлайне.

См. `11_analytics.md`.

### MonetizationService

Фасад над платформенным IAP + Ads + подписка. Серверная валидация.

См. `12_monetization.md`.

### Localization

Тонкая обёртка над `TranslationServer`. Удобные методы `tr_plural()`, `tr_format()`.

### SceneRouter

Переходы между сценами с анимацией (fade). Отслеживает стек экранов для back-навигации.

## Паттерн узлов

### Здания
```
Building (Node3D, building.gd)
├── Mesh (MeshInstance3D)
├── ClickArea (Area3D)
├── ProductionAnchor (Node3D)
└── UpgradeFX (CPUParticles3D, скрытый)
```
Логика — в `building.gd`, держит ссылку на `BuildingDef` (resource) и `BuildingState` (рантайм).

### NPC
```
NPC (CharacterBody3D, npc.gd)
├── Visual (Node3D — модель + Skeleton3D)
├── Nav (NavigationAgent3D)
├── StateMachine (Node, npc_state_machine.gd)
└── CarryAnchor (Node3D, для ресурсов в руках)
```

### Игрок
То же, что NPC, но с тач-управлением через `PlayerInput` (Control-узел).

## Тик симуляции

```gdscript
# economy_sim.gd
const TICK_HZ := 10.0
const TICK_DT := 1.0 / TICK_HZ
var _accumulator := 0.0

func _process(delta: float) -> void:
    _accumulator += delta
    while _accumulator >= TICK_DT:
        _accumulator -= TICK_DT
        _advance_simulation(TICK_DT)

func _advance_simulation(dt: float) -> void:
    for line in _production_lines:
        line.tick(dt)
    NPCSystem.tick(dt)
```

10 Hz — компромисс между точностью и нагрузкой. На длительных оффлайн-расчётах используется «сжатый тик» с большим dt (см. `04_simulation.md`).

## Lifecycle

```
Boot scene loads
 ↓
Autoload _ready() (в порядке: EventBus → Localization → RemoteConfig → SaveService → EconomySim → NPCSystem → AnalyticsBus → MonetizationService → GameState)
 ↓
RemoteConfig.fetch() → config_loaded
 ↓
SaveService.load() → state_restored
 ↓
EconomySim.simulate_offline(now - last_seen)
 ↓
SceneRouter.goto("world")
```

## Открытые вопросы

- [ ] Логирование сигналов EventBus для отладки — отдельный dev-аддон?
- [ ] NPC AI — `Beehave` BT или ручной FSM? Тестим оба в Phase 1.
