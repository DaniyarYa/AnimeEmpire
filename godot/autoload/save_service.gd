## Загрузка / сохранение прогресса игрока.
##
## Stub-версия Phase 0: JSON без шифрования, без облачного sync.
## Полноценная реализация — Phase 2 (P2-050..P2-053).
## Подробности — docs/architecture/06_save_sync.md.
extends Node

const SAVE_FILE := "user://save_0.bin"
const SAVE_VERSION := 1
const SAVE_DEBOUNCE_SECONDS := 2.0

var _state: Dictionary = {}
var _save_pending: bool = false
var _debounce_timer: Timer


func _ready() -> void:
	_setup_debounce_timer()
	EventBus.save_dirty.connect(_on_save_dirty)
	load_state()
	print("[SaveService] ready, save_version=", _state.get("save_version", "new"))


func _setup_debounce_timer() -> void:
	_debounce_timer = Timer.new()
	_debounce_timer.one_shot = true
	_debounce_timer.wait_time = SAVE_DEBOUNCE_SECONDS
	_debounce_timer.timeout.connect(_flush)
	add_child(_debounce_timer)


func _on_save_dirty() -> void:
	_save_pending = true
	_debounce_timer.start()


func _flush() -> void:
	if not _save_pending:
		return
	save_state()
	_save_pending = false


func save_state() -> void:
	_state["save_version"] = SAVE_VERSION
	_state["last_seen_at"] = Time.get_unix_time_from_system()
	var json := JSON.stringify(_state)
	var file := FileAccess.open(SAVE_FILE, FileAccess.WRITE)
	if file == null:
		push_warning("[SaveService] cannot open save file for write")
		return
	file.store_string(json)
	file.close()
	EventBus.save_persisted.emit()


func load_state() -> void:
	if not FileAccess.file_exists(SAVE_FILE):
		_state = _new_state()
		return
	var file := FileAccess.open(SAVE_FILE, FileAccess.READ)
	if file == null:
		_state = _new_state()
		return
	var raw := file.get_as_text()
	file.close()
	var parsed: Variant = JSON.parse_string(raw)
	if parsed is Dictionary:
		_state = parsed
	else:
		push_warning("[SaveService] corrupt save, starting fresh")
		_state = _new_state()


func get_state() -> Dictionary:
	return _state


func patch_state(patch: Dictionary) -> void:
	_state.merge(patch, true)
	EventBus.save_dirty.emit()


func _new_state() -> Dictionary:
	return {
		"save_version": SAVE_VERSION,
		"created_at": Time.get_unix_time_from_system(),
		"last_seen_at": Time.get_unix_time_from_system(),
		"currencies": {"gold": 0, "gems": 0},
		"buildings": {},
		"npcs": [],
		"prestige": {"level": 0, "points": 0, "upgrades": {}},
		"tutorial": {"step": 0, "completed": false},
		"settings": {"sfx": 1.0, "music": 0.7, "locale": "en"},
	}


func _notification(what: int) -> void:
	if what == NOTIFICATION_APPLICATION_PAUSED or what == NOTIFICATION_WM_CLOSE_REQUEST:
		_flush()
