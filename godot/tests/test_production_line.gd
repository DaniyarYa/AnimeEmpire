## Тесты ProductionLine.tick() и формул.
##
## Используем class_name типизацию, чтобы поля были доступны статически.
extends RefCounted


func _make_resource(id: String, price: int) -> ResourceDef:
	var r := ResourceDef.new()
	r.id = id
	r.base_sell_price = price
	return r


func _make_building(id: String, cycle: float, output: Resource) -> BuildingDef:
	var b := BuildingDef.new()
	b.id = id
	b.base_cycle_seconds = cycle
	b.output_resource = output
	b.output_amount = 1
	return b


func _make_line(building: Resource, level: int, owned: bool) -> ProductionLine:
	var line := ProductionLine.new()
	line.building_def = building
	line.current_level = level
	line.owned = owned
	return line


## Один тик длительностью равной cycle → ровно одна эмиссия.
func test_one_full_cycle_emits_once() -> Variant:
	var wheat := _make_resource("wheat", 1)
	var farm := _make_building("wheat_farm", 1.0, wheat)
	var line := _make_line(farm, 1, true)

	var emissions: Array = []
	var callback := func(_b: String, r: String, _a: int) -> void:
		emissions.append(r)
	EventBus.resource_produced.connect(callback)
	line.tick(1.0, {})
	EventBus.resource_produced.disconnect(callback)

	if emissions.size() != 1:
		return "expected 1 emission, got %d" % emissions.size()
	if emissions[0] != "wheat":
		return "expected wheat, got %s" % str(emissions[0])
	return true


## Линия с owned=false не эмитит ничего.
func test_unowned_line_no_emit() -> Variant:
	var wheat := _make_resource("wheat", 1)
	var farm := _make_building("wheat_farm", 1.0, wheat)
	var line := _make_line(farm, 1, false)

	var emissions: int = 0
	var callback := func(_b: String, _r: String, _a: int) -> void:
		emissions += 1
	EventBus.resource_produced.connect(callback)
	line.tick(5.0, {})
	EventBus.resource_produced.disconnect(callback)

	if emissions != 0:
		return "expected 0 emissions, got %d" % emissions
	return true


## Speed multiplier x2 → дважды эмитит за полный цикл.
func test_speed_multiplier_doubles_output() -> Variant:
	var wheat := _make_resource("wheat", 1)
	var farm := _make_building("wheat_farm", 1.0, wheat)
	var line := _make_line(farm, 1, true)

	var emissions: int = 0
	var callback := func(_b: String, _r: String, _a: int) -> void:
		emissions += 1
	EventBus.resource_produced.connect(callback)
	line.tick(1.0, {"speed": 2.0})
	EventBus.resource_produced.disconnect(callback)

	if emissions != 2:
		return "expected 2 emissions with speed=2, got %d" % emissions
	return true


## Прогресс не пропадает между тиками: 0.5 + 0.5 = 1 полный цикл.
func test_progress_accumulates_across_ticks() -> Variant:
	var wheat := _make_resource("wheat", 1)
	var farm := _make_building("wheat_farm", 1.0, wheat)
	var line := _make_line(farm, 1, true)

	var emissions: int = 0
	var callback := func(_b: String, _r: String, _a: int) -> void:
		emissions += 1
	EventBus.resource_produced.connect(callback)
	line.tick(0.5, {})
	if emissions != 0:
		EventBus.resource_produced.disconnect(callback)
		return "premature emission after 0.5s"
	line.tick(0.5, {})
	EventBus.resource_produced.disconnect(callback)

	if emissions != 1:
		return "expected 1 emission after 0.5+0.5, got %d" % emissions
	return true


## Линия без output_resource не падает и ничего не эмитит.
func test_line_without_output_safe() -> Variant:
	var farm := _make_building("market", 0.0, null)
	var line := _make_line(farm, 1, true)

	var emissions: int = 0
	var callback := func(_b: String, _r: String, _a: int) -> void:
		emissions += 1
	EventBus.resource_produced.connect(callback)
	line.tick(1.0, {})
	EventBus.resource_produced.disconnect(callback)

	if emissions != 0:
		return "expected 0 emissions for null output"
	return true
