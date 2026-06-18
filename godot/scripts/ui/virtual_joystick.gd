## Динамический виртуальный джойстик.
##
## Появляется в точке касания вне UI-элементов и следит за пальцем.
## Параметры из GDD/9_uiux.md §«Movement_Controls»:
##   - dead_zone: 10% of joystick radius
##   - max_range: 80px from center
##   - dynamic positioning at touch point
##
## Использование:
##   - Помести `virtual_joystick.tscn` как ребёнка CanvasLayer (HUD)
##   - Подключи сигнал `direction_changed(Vector2)` на контроллер игрока
##   - Vector2 нормализован в [-1, 1] (длина учитывает dead zone)
class_name VirtualJoystick
extends Control

signal direction_changed(direction: Vector2)
signal touch_started
signal touch_ended

@export var max_radius: float = 80.0
@export var dead_zone_pct: float = 0.1  ## 0..1, доля от max_radius

## Дефолтная позиция, когда пользователь не держит палец.
## Левый-нижний угол с отступом.
@export var resting_position: Vector2 = Vector2(160.0, -200.0)

## Прозрачность джойстика в неактивном (resting) состоянии.
@export var resting_alpha: float = 0.35

## Прозрачность во время активного use.
@export var active_alpha: float = 0.85

## Доля ширины экрана слева, где активируется джойстик.
## Остальная часть экрана пропускает события 3D-сцене (тапы зданий).
@export_range(0.0, 1.0) var active_zone_width_pct: float = 0.5

@onready var _background: Control = $Background
@onready var _knob: Control = $Background/Knob

var _active: bool = false
var _touch_index: int = -1
var _origin: Vector2 = Vector2.ZERO
var _current_direction: Vector2 = Vector2.ZERO


func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	_show_at_rest()


func _show_at_rest() -> void:
	# Позиция: bottom-left относительно viewport.
	var viewport_size: Vector2 = get_viewport_rect().size
	_origin = Vector2(resting_position.x, viewport_size.y + resting_position.y)
	_background.position = _origin - _background.size * 0.5
	_knob.position = _background.size * 0.5 - _knob.size * 0.5
	_background.visible = true
	_background.modulate.a = resting_alpha


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if event.pressed and not _in_active_zone(event.position):
			return
		_handle_touch(event)
	elif event is InputEventScreenDrag:
		if _active and event.index == _touch_index:
			_handle_drag(event)
	elif event is InputEventMouseButton:
		if event.pressed and not _in_active_zone(event.position):
			return
		_handle_mouse_button(event)
	elif event is InputEventMouseMotion and _active and _touch_index == -1:
		_update_knob(event.position)


func _in_active_zone(pos: Vector2) -> bool:
	var viewport_size: Vector2 = get_viewport_rect().size
	return pos.x < viewport_size.x * active_zone_width_pct


func _handle_touch(event: InputEventScreenTouch) -> void:
	if event.pressed and not _active:
		_start(event.position, event.index)
	elif not event.pressed and event.index == _touch_index:
		_stop()


func _handle_drag(event: InputEventScreenDrag) -> void:
	if _active and event.index == _touch_index:
		_update_knob(event.position)


func _handle_mouse_button(event: InputEventMouseButton) -> void:
	if event.button_index != MOUSE_BUTTON_LEFT:
		return
	if event.pressed and not _active:
		_start(event.position, -1)
	elif not event.pressed and _touch_index == -1:
		_stop()


func _start(at: Vector2, touch_index: int) -> void:
	_active = true
	_touch_index = touch_index
	_origin = at
	_background.position = _origin - _background.size * 0.5
	_knob.position = _background.size * 0.5 - _knob.size * 0.5
	_animate_to_alpha(active_alpha)
	touch_started.emit()


func _stop() -> void:
	_active = false
	_touch_index = -1
	_current_direction = Vector2.ZERO
	direction_changed.emit(_current_direction)
	_show_at_rest()
	_animate_to_alpha(resting_alpha)
	touch_ended.emit()


func _update_knob(at: Vector2) -> void:
	var offset := at - _origin
	if offset.length() > max_radius:
		offset = offset.normalized() * max_radius
	_knob.position = _background.size * 0.5 - _knob.size * 0.5 + offset
	var normalized := offset / max_radius
	if normalized.length() < dead_zone_pct:
		normalized = Vector2.ZERO
	else:
		var scale := (normalized.length() - dead_zone_pct) / (1.0 - dead_zone_pct)
		normalized = normalized.normalized() * scale
	if normalized != _current_direction:
		_current_direction = normalized
		direction_changed.emit(_current_direction)


func _animate_to_alpha(target_alpha: float) -> void:
	create_tween().tween_property(_background, "modulate:a", target_alpha, 0.15)


func get_direction() -> Vector2:
	return _current_direction


func is_active() -> bool:
	return _active
