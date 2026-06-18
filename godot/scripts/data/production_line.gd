## Производственная линия: тикает прогресс и эмитит resource_produced.
##
## Регистрируется в EconomySim через `EconomySim.register_line()` или
## `register_line_from_def()`. Подробности — docs/architecture/04_simulation.md.
class_name ProductionLine
extends Resource

const CYCLE_DECAY_PER_LEVEL := 0.97  ## Множитель cycle_time за уровень: 3% быстрее.

## Описание здания, к которому привязана линия.
## Тип Resource (не BuildingDef) — избегаем циклической class_name-зависимости.
@export var building_def: Resource = null

## Текущий уровень здания.
@export var current_level: int = 1

## Запущена ли линия (если false — tick не делает ничего).
@export var owned: bool = false

var _progress: float = 0.0  ## Прогресс текущего цикла [0..1].


## Вызывается EconomySim.tick() с 10 Hz.
func tick(dt: float, modifiers: Dictionary) -> void:
	if not owned or building_def == null or building_def.output_resource == null:
		return
	var speed_mult: float = float(modifiers.get("speed", 1.0))
	var cycle: float = building_def.base_cycle_seconds * pow(CYCLE_DECAY_PER_LEVEL, current_level - 1)
	cycle = cycle / maxf(speed_mult, 0.0001)
	_progress += dt / cycle
	while _progress >= 1.0:
		_progress -= 1.0
		EventBus.resource_produced.emit(
			building_def.id, building_def.output_resource.id, building_def.output_amount
		)


## Стоимость апгрейда здания на следующий уровень.
func get_upgrade_cost() -> int:
	if building_def == null:
		return 0
	return int(building_def.base_cost_gold * pow(building_def.cost_growth, current_level - 1))


## Доход в gold/sec при текущем level (без учёта sell-цепочки).
func get_income_rate() -> float:
	if building_def == null or building_def.output_resource == null:
		return 0.0
	var cycle: float = building_def.base_cycle_seconds * pow(CYCLE_DECAY_PER_LEVEL, current_level - 1)
	return (
		float(building_def.output_amount * building_def.output_resource.base_sell_price)
		/ maxf(cycle, 0.0001)
	)


## Прогресс текущего цикла в [0..1].
func get_progress() -> float:
	return _progress
