## Симулятор экономики. Тикает с фиксированной частотой 10 Hz.
##
## Подробности — docs/architecture/04_simulation.md.
extends Node

const TICK_HZ := 10.0
const TICK_DT := 1.0 / TICK_HZ

const OFFLINE_CAP_SECONDS := 4 * 3600
const OFFLINE_EFFICIENCY := 0.5

var _accumulator: float = 0.0
var _production_lines: Array = []
var _inventory: Dictionary = {}  ## resource_id (String) -> amount (int)
var _gold: int = 0


func _ready() -> void:
	EventBus.resource_produced.connect(_on_resource_produced)
	print("[EconomySim] ready, tick_hz=", TICK_HZ)


func _process(delta: float) -> void:
	_accumulator += delta
	while _accumulator >= TICK_DT:
		_accumulator -= TICK_DT
		_tick(TICK_DT)


func _tick(dt: float) -> void:
	for line in _production_lines:
		if line.has_method("tick"):
			line.tick(dt, {})


func register_line(line: Resource) -> void:
	if not _production_lines.has(line):
		_production_lines.append(line)


func unregister_line(line: Resource) -> void:
	_production_lines.erase(line)


## Создать ProductionLine из BuildingDef и зарегистрировать её.
func register_line_from_def(building_def: Resource, level: int = 1) -> Resource:
	var line_script: GDScript = load("res://scripts/data/production_line.gd")
	var line: Resource = line_script.new()
	line.building_def = building_def
	line.current_level = level
	line.owned = true
	register_line(line)
	return line


## Сколько единиц ресурса лежит в инвентаре.
func get_inventory(resource_id: String) -> int:
	return int(_inventory.get(resource_id, 0))


## Получить копию словаря инвентаря.
func get_inventory_snapshot() -> Dictionary:
	return _inventory.duplicate()


## Текущий gold баланс.
func get_gold() -> int:
	return _gold


## Продать всё, что есть данного ресурса, по `base_sell_price`. Возвращает gold.
func sell_inventory(resource: Resource) -> int:
	if resource == null:
		return 0
	var amount: int = int(_inventory.get(resource.id, 0))
	if amount == 0:
		return 0
	var gold: int = amount * resource.base_sell_price
	_inventory[resource.id] = 0
	_gold += gold
	EventBus.resource_sold.emit(resource.id, amount, gold)
	EventBus.currency_changed.emit("gold", _gold)
	EventBus.save_dirty.emit()
	return gold


## Списать gold (например, на апгрейд). Возвращает true если хватило.
func spend_gold(amount: int) -> bool:
	if amount < 0 or _gold < amount:
		return false
	_gold -= amount
	EventBus.currency_changed.emit("gold", _gold)
	EventBus.save_dirty.emit()
	return true


## Прибавить gold (тестовый / промо). Не путать с продажей.
func grant_gold(amount: int) -> void:
	if amount <= 0:
		return
	_gold += amount
	EventBus.currency_changed.emit("gold", _gold)
	EventBus.save_dirty.emit()


func _on_resource_produced(_building_id: String, resource_id: String, amount: int) -> void:
	_inventory[resource_id] = int(_inventory.get(resource_id, 0)) + amount


## Прогон сжатого тика для оффлайн-расчёта (см. P2-003).
func simulate_offline(elapsed_seconds: int) -> Dictionary:
	var capped: int = mini(elapsed_seconds, OFFLINE_CAP_SECONDS)
	var effective: float = float(capped) * OFFLINE_EFFICIENCY
	# TODO(P2-003): полный оффлайн-расчёт с чанками по 60 c
	return {"gold": 0, "resources": {}, "elapsed_effective": effective}
