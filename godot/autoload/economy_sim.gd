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


func _ready() -> void:
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


## Прогон сжатого тика для оффлайн-расчёта (см. P2-003).
func simulate_offline(elapsed_seconds: int) -> Dictionary:
	var capped: int = mini(elapsed_seconds, OFFLINE_CAP_SECONDS)
	var effective: float = float(capped) * OFFLINE_EFFICIENCY
	# TODO(P2-003): полный оффлайн-расчёт с чанками по 60 c
	return {"gold": 0, "resources": {}, "elapsed_effective": effective}
