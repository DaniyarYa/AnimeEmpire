## Очередь аналитических событий + батчевая отправка.
##
## Stub Phase 0: пишет в локальный лог, без сетевой отправки.
## Полная реализация — Phase 2 (P2-080..P2-083).
## Подробности — docs/architecture/11_analytics.md.
extends Node

const FLUSH_EVERY_SECONDS := 30.0
const FLUSH_AT_QUEUE_SIZE := 50

var _queue: Array[Dictionary] = []
var _session_id: String = ""
var _time_since_flush: float = 0.0


func _ready() -> void:
	_session_id = _generate_session_id()
	log_event("lifecycle.session.started", {})
	print("[AnalyticsBus] ready, session=", _session_id)


func _process(delta: float) -> void:
	_time_since_flush += delta
	if _time_since_flush >= FLUSH_EVERY_SECONDS:
		_flush()


func log_event(name: String, params: Dictionary) -> void:
	var event := _enrich(name, params)
	_queue.append(event)
	if _queue.size() >= FLUSH_AT_QUEUE_SIZE:
		_flush()


func _enrich(name: String, p: Dictionary) -> Dictionary:
	var enriched := p.duplicate()
	enriched["event"] = name
	enriched["ts"] = Time.get_unix_time_from_system()
	enriched["session_id"] = _session_id
	enriched["app_version"] = ProjectSettings.get_setting("application/config/version", "0.0.1")
	enriched["platform"] = OS.get_name()
	enriched["locale"] = Localization.current
	return enriched


func _flush() -> void:
	if _queue.is_empty():
		_time_since_flush = 0.0
		return
	# TODO(P2-080): отправка батчем на backend
	for ev in _queue:
		print("[Analytics] ", ev)
	_queue.clear()
	_time_since_flush = 0.0


func _generate_session_id() -> String:
	var rng := RandomNumberGenerator.new()
	rng.randomize()
	return str(Time.get_unix_time_from_system(), "_", rng.randi())


func _notification(what: int) -> void:
	if what == NOTIFICATION_APPLICATION_PAUSED or what == NOTIFICATION_WM_CLOSE_REQUEST:
		log_event("lifecycle.session.ended", {})
		_flush()
