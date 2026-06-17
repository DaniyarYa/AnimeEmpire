## Удалённый конфиг с фолбэком на локальные значения.
##
## Подробности — docs/architecture/07_remote_config.md.
extends Node

const CACHE_FILE := "user://config_cache.json"
const SCHEMA_VERSION := 1

var version: int = 0
var _values: Dictionary = {}
var _ab_variants: Dictionary = {}


func _ready() -> void:
	_load_cache_or_defaults()
	print("[RemoteConfig] ready, version=", version)


func _load_cache_or_defaults() -> void:
	if FileAccess.file_exists(CACHE_FILE):
		var file := FileAccess.open(CACHE_FILE, FileAccess.READ)
		if file != null:
			var raw := file.get_as_text()
			file.close()
			var parsed: Variant = JSON.parse_string(raw)
			if parsed is Dictionary:
				_apply(parsed)
				return
	_apply_defaults()


func _apply(data: Dictionary) -> void:
	version = int(data.get("version", 0))
	_values = data.get("values", {})
	_ab_variants = data.get("ab_variants", {})
	EventBus.config_loaded.emit(version)


func _apply_defaults() -> void:
	_values = {
		"economy.cost_growth_early": 1.12,
		"economy.cost_growth_mid": 1.15,
		"economy.cost_growth_late": 1.18,
	}
	EventBus.config_loaded.emit(0)


func get_float(key: String, default: float) -> float:
	return float(_values.get(key, default))


func get_int(key: String, default: int) -> int:
	return int(_values.get(key, default))


func get_bool(key: String, default: bool) -> bool:
	return bool(_values.get(key, default))


func get_string(key: String, default: String) -> String:
	return str(_values.get(key, default))


func active_variants() -> Dictionary:
	return _ab_variants.duplicate()
