## Камера-риг с тач-управлением: drag pan по XZ + pinch zoom.
##
## Прикрепляется к Camera3D в мире. На мобиле работают touch, в Editor —
## эмуляция мыши (включена в project.godot).
class_name CameraRig
extends Camera3D

@export var pan_speed: float = 0.02
@export var min_zoom_distance: float = 6.0
@export var max_zoom_distance: float = 30.0
@export var zoom_step: float = 1.0

## Лимиты по XZ — простая «коробка» вокруг центра мира.
@export var pan_limit_xz: Vector2 = Vector2(20.0, 20.0)

## Pitch камеры сохраняется при движении (top-down угол из сцены).
var _initial_basis: Basis
var _initial_distance: float
var _drag_anchor: Vector2 = Vector2.ZERO
var _is_dragging: bool = false

# Pinch state.
var _active_touches: Dictionary = {}  # index -> position
var _last_pinch_distance: float = 0.0


func _ready() -> void:
	_initial_basis = transform.basis
	_initial_distance = global_position.length()


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
		if _active_touches.size() == 1:
			_drag_anchor = event.position
			_is_dragging = true
		elif _active_touches.size() == 2:
			_is_dragging = false
			_last_pinch_distance = _calc_pinch_distance()
	else:
		_active_touches.erase(event.index)
		if _active_touches.size() < 2:
			_last_pinch_distance = 0.0
		if _active_touches.is_empty():
			_is_dragging = false


func _handle_drag(event: InputEventScreenDrag) -> void:
	_active_touches[event.index] = event.position
	if _active_touches.size() == 2:
		_apply_pinch()
	elif _is_dragging:
		_apply_pan(event.relative)


func _handle_mouse_button(event: InputEventMouseButton) -> void:
	# Колесо мыши — зум в редакторе.
	if event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed:
		_zoom_by(-zoom_step)
	elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed:
		_zoom_by(zoom_step)


func _apply_pan(delta: Vector2) -> void:
	# Преобразуем экранный delta в смещение по XZ.
	# Right vector камеры — для горизонтального drag; forward проецируем на XZ.
	var right := global_transform.basis.x
	var forward := -global_transform.basis.z
	forward.y = 0.0
	forward = forward.normalized()
	var move := -right * delta.x * pan_speed + forward * delta.y * pan_speed
	global_position += move
	global_position.x = clampf(global_position.x, -pan_limit_xz.x, pan_limit_xz.x)
	global_position.z = clampf(global_position.z, -pan_limit_xz.y, pan_limit_xz.y)


func _apply_pinch() -> void:
	var d := _calc_pinch_distance()
	if _last_pinch_distance == 0.0:
		_last_pinch_distance = d
		return
	var delta := _last_pinch_distance - d
	_zoom_by(delta * 0.01)
	_last_pinch_distance = d


func _calc_pinch_distance() -> float:
	var points: Array = _active_touches.values()
	if points.size() < 2:
		return 0.0
	return (points[0] as Vector2).distance_to(points[1] as Vector2)


func _zoom_by(amount: float) -> void:
	# Двигаем камеру вдоль её forward-оси, сохраняя угол наклона.
	var forward := -global_transform.basis.z
	var new_pos := global_position - forward * amount
	var distance := new_pos.length()
	if distance < min_zoom_distance or distance > max_zoom_distance:
		return
	global_position = new_pos
