## Камера в follow-mode с pinch zoom и free-mode при двух пальцах.
##
## Phase 1 refactor (P1-033): убрали drag-pan, добавили follow_target.
## Подробности — docs/architecture/09_ui_architecture.md §«Camera».
class_name CameraRig
extends Camera3D

const PINCH_SENSITIVITY := 0.01
const SMOOTHING_BASE := 30.0  ## Множитель для frame-rate independent lerp.

## Узел (обычно Player), за которым следует камера.
@export var follow_target: Node3D = null

## Сглаживание 0..1: 0 = instant, чем выше — тем плавнее (и медленнее).
@export_range(0.0, 0.99) var follow_smoothing: float = 0.15

## Базовый offset от target. Длина = расстояние, нормализованный = угол.
@export var follow_offset: Vector3 = Vector3(0, 8, 12)

## Минимум / максимум расстояния от target при zoom.
@export var min_zoom_distance: float = 6.0
@export var max_zoom_distance: float = 30.0

## После двух-пальцевого drag — сколько секунд оставаться в free-mode перед
## возвратом offset к базе.
@export var free_mode_timeout: float = 3.0

var _free_mode_timer: float = 0.0
var _active_touches: Dictionary = {}  ## index -> position
var _last_pinch_distance: float = 0.0


func _process(delta: float) -> void:
	if follow_target == null:
		return
	if _free_mode_timer > 0.0:
		_free_mode_timer -= delta
	var goal: Vector3 = follow_target.global_position + follow_offset
	var t: float = 1.0 - pow(follow_smoothing, delta * SMOOTHING_BASE)
	global_position = global_position.lerp(goal, t)
	look_at(follow_target.global_position, Vector3.UP)


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		_handle_touch(event)
	elif event is InputEventScreenDrag:
		_handle_drag(event)
	elif event is InputEventMouseButton:
		_handle_mouse_button(event)


func _handle_touch(event: InputEventScreenTouch) -> void:
	if event.pressed:
		_active_touches[event.index] = event.position
		if _active_touches.size() == 2:
			_last_pinch_distance = _calc_pinch_distance()
	else:
		_active_touches.erase(event.index)
		if _active_touches.size() < 2:
			_last_pinch_distance = 0.0


func _handle_drag(event: InputEventScreenDrag) -> void:
	_active_touches[event.index] = event.position
	if _active_touches.size() == 2:
		_apply_pinch()


func _handle_mouse_button(event: InputEventMouseButton) -> void:
	if event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed:
		_zoom_by(-1.0)
	elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed:
		_zoom_by(1.0)


func _apply_pinch() -> void:
	var d: float = _calc_pinch_distance()
	if _last_pinch_distance > 0.0:
		var delta: float = _last_pinch_distance - d
		_zoom_by(delta * PINCH_SENSITIVITY)
	_last_pinch_distance = d
	_free_mode_timer = free_mode_timeout


func _zoom_by(amount: float) -> void:
	var direction: Vector3 = follow_offset.normalized()
	var current_dist: float = follow_offset.length()
	var new_dist: float = clampf(current_dist + amount, min_zoom_distance, max_zoom_distance)
	follow_offset = direction * new_dist


func _calc_pinch_distance() -> float:
	var points: Array = _active_touches.values()
	if points.size() < 2:
		return 0.0
	return (points[0] as Vector2).distance_to(points[1] as Vector2)
