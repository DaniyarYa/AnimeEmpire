## Тесты ProductionLine.get_upgrade_cost() и get_income_rate().
extends RefCounted


func _make_line(cycle: float, base_cost: int, growth: float, level: int) -> ProductionLine:
	var wheat := ResourceDef.new()
	wheat.id = "wheat"
	wheat.base_sell_price = 1

	var farm := BuildingDef.new()
	farm.base_cycle_seconds = cycle
	farm.base_cost_gold = base_cost
	farm.cost_growth = growth
	farm.output_resource = wheat
	farm.output_amount = 1

	var line := ProductionLine.new()
	line.building_def = farm
	line.current_level = level
	return line


## Cost growth по формуле base × growth^(level-1).
## base=100, growth=1.12, level=1 → 100
func test_cost_at_level_1() -> Variant:
	var line := _make_line(1.0, 100, 1.12, 1)
	var cost := line.get_upgrade_cost()
	if cost != 100:
		return "expected 100, got %d" % cost
	return true


## level=5 → 100 × 1.12^4 ≈ 157
func test_cost_at_level_5() -> Variant:
	var line := _make_line(1.0, 100, 1.12, 5)
	var cost := line.get_upgrade_cost()
	if cost != 157:
		return "expected 157, got %d" % cost
	return true


## level=10 → 100 × 1.12^9 ≈ 277
func test_cost_at_level_10() -> Variant:
	var line := _make_line(1.0, 100, 1.12, 10)
	var cost := line.get_upgrade_cost()
	if cost != 277:
		return "expected 277, got %d" % cost
	return true


## income rate = output_amount × sell_price / cycle = 1 × 1 / 1.0 = 1.0 gold/sec на level 1.
func test_income_rate_level_1() -> Variant:
	var line := _make_line(1.0, 100, 1.12, 1)
	var rate := line.get_income_rate()
	if abs(rate - 1.0) > 0.001:
		return "expected rate ~1.0, got %f" % rate
	return true


## Cycle decay: level 5 → cycle = 1.0 × 0.97^4 = 0.885, rate ≈ 1.13.
func test_income_rate_grows_with_level() -> Variant:
	var line_1 := _make_line(1.0, 100, 1.12, 1)
	var line_5 := _make_line(1.0, 100, 1.12, 5)
	if line_5.get_income_rate() <= line_1.get_income_rate():
		return "rate at level 5 should be higher than at level 1"
	return true
