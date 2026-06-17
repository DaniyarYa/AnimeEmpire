# 04 · Симуляция: производство, NPC, оффлайн

## Модель симуляции

Двухуровневая:
1. **Онлайн-тик** — 10 Hz, обрабатывает все активные линии и NPC в реальном времени.
2. **Оффлайн-расчёт** — детерминированный «свертанный» прогон при возврате игрока, учитывает капы и эффективность.

## Производственная линия

```gdscript
class_name ProductionLine extends Resource

@export var building_def: BuildingDef
@export var input_resource: ResourceDef   # null для генераторов
@export var input_amount: int
@export var output_resource: ResourceDef
@export var output_amount: int
@export var cycle_seconds: float          # базовое время цикла
@export var current_level: int = 1

var _progress: float = 0.0  # 0..1, накапливается с каждым тиком

func tick(dt: float, modifiers: Dictionary) -> Dictionary:
    var speed_mult = modifiers.get("speed", 1.0)
    _progress += dt / (cycle_seconds * speed_mult)
    if _progress >= 1.0:
        _progress -= 1.0
        return {"produced": output_resource.id, "amount": output_amount}
    return {}
```

Конфигурация скейлинга — в `BuildingDef`, формула применяется в `apply_level()`.

Параметры из `GDD/13_game_config.md`:
- Wheat Farm: base 1/сек, scaling 1.15x за уровень
- Mill: 10 с цикл, 0.95x за уровень (быстрее)
- Множители стоимости: 1.12 (ранняя), 1.15 (средняя), 1.18 (поздняя)

## NPC AI

Иерархическая FSM (если `Beehave` не зайдёт — простой `match`-FSM на GDScript).

Состояния общие:
- `IDLE` — ждёт назначения
- `MOVING_TO_SOURCE` — идёт к ресурсу/зданию
- `WORKING` — выполняет действие (тик `cycle_seconds`)
- `MOVING_TO_TARGET` — несёт ресурс/идёт к дропу
- `DELIVERING` — выгружает в инвентарь/склад
- `RETURNING` — возврат к домашнему зданию

Каждый NPC-тип переопределяет переходы:
- **Gatherer:** IDLE → MOVE_SRC → WORK → MOVE_TGT → DELIVER → RETURNING
- **Carrier:** ждёт сигнала `resource_ready`, забирает, везёт в магазин/склад
- **Operator:** усиливает производство здания, к которому привязан (passive multiplier)
- **Manager:** глобальный множитель, не имеет физического NPC в мире (только UI)

### Поведенческие параметры (из `NPCDef`)
- `speed: float` — единицы/сек
- `capacity: int` — макс груз
- `efficiency: float` — 0..1, влияет на cycle_time и доход

## Оффлайн-расчёт

```gdscript
# economy_sim.gd
const OFFLINE_CAP_SECONDS := 4 * 3600  # 4 часа (GDD/5)
const OFFLINE_EFFICIENCY := 0.5         # 50% производства

func simulate_offline(elapsed_seconds: int) -> Dictionary:
    var capped = min(elapsed_seconds, OFFLINE_CAP_SECONDS)
    var effective = capped * OFFLINE_EFFICIENCY

    var summary = {"gold": 0, "resources": {}}

    # Большой dt — но не один шаг: дробим на «куски» по 60 с,
    # чтобы корректно сработали циклы и кап инвентаря.
    var step := 60.0
    var remaining := float(effective)
    while remaining > 0.0:
        var dt = min(step, remaining)
        for line in _production_lines:
            var modifiers = _get_offline_modifiers(line)
            var result = line.tick(dt, modifiers)
            if result.has("produced"):
                summary.resources[result.produced] = summary.resources.get(result.produced, 0) + result.amount
        remaining -= dt

    # NPC оффлайн упрощённо: вклад через множитель на производство,
    # без полноценного pathfinding.
    return summary
```

После расчёта показывается модалка с результатом, плюс кнопка `2x rewarded ad`, см. `GDD/8`.

## Модификаторы

Финальное `cycle_seconds` и `output_amount` определяются цепочкой модификаторов:
- Уровень здания
- Tool tier игрока
- Привязанный specialist NPC (например, `+25% mill speed`)
- Manager bonus (глобальный множитель)
- Активные ивенты (`+100% yield` во время Festival)
- Prestige multiplier

Реализация — единый pipeline `apply_modifiers(base_value, stack: Array)`.

## Детерминизм

Симуляция должна быть детерминированной для anti-cheat и для серверной валидации престижа.
- Использовать `RandomNumberGenerator` со сидом, привязанным к session_id
- Избегать `randf()` (глобальный) в логике; только в визуальных эффектах

## Производительность

- Все формулы — без аллокаций в горячем пути
- Линии и NPC — пулы объектов, не создаём/удаляем по ходу
- При >50 одновременных NPC — рассмотреть batched-обновление (тик каждого через раз)

## Anti-cheat

Серверная валидация ключевых событий (см. `10_backend.md`):
- Prestige trigger — проверка income threshold
- IAP — проверка чека
- Подозрительные дельты (gold +1M за тик) — флаг + rollback

## Открытые вопросы

- [ ] Behavior Tree через Beehave vs ручной FSM — выбираем на Phase 1 после прототипа
- [ ] Пул объектов для частиц/floating-text — кастомный или встроенный?
