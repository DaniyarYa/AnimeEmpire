## Удалённый конфиг с фолбэком на локальные значения.
##
## Архитектура: HTTP GET → JSON parse → in-memory cache + disk cache.
## При offline / failure используется локальный кеш или встроенные defaults.
##
## URL конфигурируется через ProjectSettings:
##   anime_empire/backend/config_url   (string, default: "")
##
## Пустой URL = HTTP fetch отключён, используются только defaults.
##
## Подробности — docs/architecture/07_remote_config.md.
extends Node

const CACHE_FILE := "user://config_cache.json"
const CONFIG_URL_KEY := "anime_empire/backend/config_url"
const SCHEMA_VERSION := 1
const FETCH_TIMEOUT_SECONDS := 5.0

var version: int = 0
var schema_version: int = SCHEMA_VERSION
var _values: Dictionary = {}
var _ab_variants: Dictionary = {}
var _http: HTTPRequest
var _config_url: String = ""


func _ready() -> void:
	_config_url = ProjectSettings.get_setting(CONFIG_URL_KEY, "")
	_setup_http_client()
	_load_cache_or_defaults()
	if _config_url != "":
		_fetch_remote()
	else:
		print("[RemoteConfig] no URL configured (set ", CONFIG_URL_KEY, " in ProjectSettings)")
	print("[RemoteConfig] ready, version=", version)


func _setup_http_client() -> void:
	_http = HTTPRequest.new()
	_http.timeout = FETCH_TIMEOUT_SECONDS
	add_child(_http)
	_http.request_completed.connect(_on_request_completed)


func _load_cache_or_defaults() -> void:
	if FileAccess.file_exists(CACHE_FILE):
		var file := FileAccess.open(CACHE_FILE, FileAccess.READ)
		if file != null:
			var raw := file.get_as_text()
			file.close()
			var parsed: Variant = JSON.parse_string(raw)
			if parsed is Dictionary and _apply(parsed):
				print("[RemoteConfig] loaded from disk cache, version=", version)
				return
	_apply_defaults()
	print("[RemoteConfig] using built-in defaults")


func _apply(data: Dictionary) -> bool:
	var incoming_schema: int = int(data.get("schema_version", 0))
	if incoming_schema > SCHEMA_VERSION:
		push_warning(
			"[RemoteConfig] config schema_version=%d > supported=%d, ignoring"
			% [incoming_schema, SCHEMA_VERSION]
		)
		return false
	version = int(data.get("version", 0))
	schema_version = incoming_schema
	_values = data.get("values", {})
	_ab_variants = data.get("ab_variants", {})
	EventBus.config_loaded.emit(version)
	return true


func _apply_defaults() -> void:
	_values = {
		"economy.cost_growth_early": 1.12,
		"economy.cost_growth_mid": 1.15,
		"economy.cost_growth_late": 1.18,
		"flags.enable_friends": false,
		"flags.enable_battle_pass": false,
	}
	_ab_variants = {}
	version = 0
	schema_version = SCHEMA_VERSION
	EventBus.config_loaded.emit(0)


func _fetch_remote() -> void:
	print("[RemoteConfig] fetching ", _config_url)
	var err := _http.request(_config_url)
	if err != OK:
		push_warning("[RemoteConfig] HTTPRequest.request failed with code %d" % err)


func _on_request_completed(
	result: int,
	response_code: int,
	_headers: PackedStringArray,
	body: PackedByteArray
) -> void:
	if result != HTTPRequest.RESULT_SUCCESS:
		push_warning(
			"[RemoteConfig] fetch failed (result=%d, http=%d), using cache/defaults"
			% [result, response_code]
		)
		return
	if response_code < 200 or response_code >= 300:
		push_warning("[RemoteConfig] HTTP %d, using cache/defaults" % response_code)
		return
	var text := body.get_string_from_utf8()
	var parsed: Variant = JSON.parse_string(text)
	if not parsed is Dictionary:
		push_warning("[RemoteConfig] invalid JSON, ignoring")
		return
	if not _apply(parsed):
		return
	_save_cache(text)
	print("[RemoteConfig] fetched ok, version=", version)


func _save_cache(raw: String) -> void:
	var file := FileAccess.open(CACHE_FILE, FileAccess.WRITE)
	if file == null:
		push_warning("[RemoteConfig] cannot write cache file")
		return
	file.store_string(raw)
	file.close()


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


func force_refetch() -> void:
	if _config_url == "":
		return
	_fetch_remote()
