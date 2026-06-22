## Универсальное здание. Тапается → emit `clicked`.
## Если category != "service" — оборачивает ProductionLine при старте.
##
## Дочерние узлы (через сцену):
##   - ClickArea (Area3D)
##   - Mesh (MeshInstance3D)  — визуал
##   - CollisionShape3D       — для click pickup
class_name Building
extends Node3D

signal clicked(building)

## Описание здания (.tres). Можно задать в инспекторе сцены.
@export var building_def: Resource = null  # BuildingDef

@onready var _click_area: Area3D = $ClickArea

var _line: Resource = null  # ProductionLine
var _started: bool = false
var _worker: Node = null  # NPC, если назначен


func _ready() -> void:
	add_to_group("buildings")
	if _click_area != null:
		_click_area.input_event.connect(_on_click_area_input)


## Запустить производство (для generator / processor). Идемпотентно.
## Если уже работает NPC — ProductionLine не создаётся (избегаем дубля).
func start_production() -> void:
	if _started or building_def == null:
		return
	if building_def.category == "service":
		_started = true
		return
	if _worker != null:
		_started = true
		return
	_line = EconomySim.register_line_from_def(building_def, 1)
	_started = true
	print("[Building] started production: ", building_def.id)


func is_started() -> bool:
	return _started


func has_worker() -> bool:
	return _worker != null


func set_worker(npc: Node) -> void:
	_worker = npc
	_started = true  ## С NPC производство идёт автоматически.


## Назначить уже существующего IDLE NPC на это здание.
## Возвращает true если назначение прошло.
func assign_worker(npc: Node) -> bool:
	if _worker != null or npc == null:
		return false
	if not npc.has_method("assign"):
		return false
	# Фоновую production line гасим — будет производить NPC.
	if _line != null:
		EconomySim.unregister_line(_line)
		_line = null
	_worker = npc
	_started = true
	npc.assign(self)
	return true


func dismiss_worker() -> void:
	if _worker == null:
		return
	if _worker.has_method("dismiss"):
		_worker.dismiss()
	_worker = null
	_started = false
	# Чистим возможную фоновую production line (если стартовали без NPC ранее).
	if _line != null:
		EconomySim.unregister_line(_line)
		_line = null


func get_line() -> Resource:
	return _line


func _on_click_area_input(
	_camera: Node, event: InputEvent, _click_pos: Vector3, _normal: Vector3, _idx: int
) -> void:
	var is_tap_press: bool = event is InputEventScreenTouch and event.pressed
	var is_left_click: bool = (
		event is InputEventMouseButton
		and event.pressed
		and event.button_index == MOUSE_BUTTON_LEFT
	)
	if is_tap_press or is_left_click:
		clicked.emit(self)
		get_viewport().set_input_as_handled()
