## Gatherer NPC: автоматически ходит к assigned_building, работает,
## эмитит resource_produced через EventBus.
##
## Phase 1 FSM (упрощённая):
##   IDLE → MOVE_TO_SOURCE → WORK (loop)
##
## NPC не носит ресурс к маркету — это делает игрок. Carrier NPC будет
## в Phase 2 (P2-011).
class_name NPC
extends CharacterBody3D

const STATE_IDLE := "idle"
const STATE_MOVE := "move"
const STATE_WORK_SIT := "work_sit"
const STATE_WORK_GATHER := "work_gather"
const STATE_WORK_STAND := "work_stand"

## Если true — после dismiss() при следующем animation_finished
## переключаемся на STAND. Иначе GATHER replay (loop).
var _stand_requested: bool = false

const ARRIVAL_THRESHOLD := 0.5
const ACCEL_LERP := 10.0
const ROTATE_LERP := 10.0

## Определение NPC (.tres). Скорость, efficiency, и т.д.
@export var npc_def: Resource = null

## Путь к зданию, к которому привязан NPC. Резолвится в _ready.
@export var assigned_building_path: NodePath = NodePath()

## Смещение точки работы относительно центра здания.
@export var work_offset: Vector3 = Vector3(2.5, 0, 0)

var assigned_building: Node3D = null

@onready var _anim: Node = $AnimationController

var _state: String = STATE_IDLE
var _target_position: Vector3 = Vector3.ZERO
var _work_timer: float = 0.0
var _work_duration: float = 1.0
var _speed: float = 2.0


func _ready() -> void:
	if npc_def != null and "base_speed" in npc_def:
		_speed = float(npc_def.base_speed)
	if not assigned_building_path.is_empty():
		assigned_building = get_node_or_null(assigned_building_path)
	# Дожидаемся завершения _ready всех детей (особенно AnimationController).
	await get_tree().process_frame
	if _anim != null and _anim.has_signal("state_finished"):
		_anim.state_finished.connect(_on_anim_state_finished)
	if assigned_building != null:
		_target_position = assigned_building.global_position + work_offset
		if assigned_building.has_method("set_worker"):
			assigned_building.set_worker(self)
		_enter_state(STATE_MOVE)
	else:
		_enter_state(STATE_IDLE)


func _physics_process(delta: float) -> void:
	match _state:
		STATE_IDLE:
			_process_idle(delta)
		STATE_MOVE:
			_process_move(delta)
		STATE_WORK_GATHER:
			_process_work(delta)
		# SIT / STAND — pure animation states, прогресс через state_finished.


func _enter_state(state: String) -> void:
	_state = state
	match state:
		STATE_IDLE:
			velocity = Vector3.ZERO
			_stand_requested = false
			if _anim != null and _anim.has_method("update"):
				_anim.update(0.0)
		STATE_MOVE:
			pass
		STATE_WORK_SIT:
			velocity = Vector3.ZERO
			if _anim != null and _anim.has_method("override"):
				_anim.override(STATE_WORK_SIT)
		STATE_WORK_GATHER:
			velocity = Vector3.ZERO
			if assigned_building != null and assigned_building.building_def != null:
				_work_duration = float(assigned_building.building_def.base_cycle_seconds)
			if _anim != null and _anim.has_method("override"):
				_anim.override(STATE_WORK_GATHER)
		STATE_WORK_STAND:
			velocity = Vector3.ZERO
			if _anim != null and _anim.has_method("override"):
				_anim.override(STATE_WORK_STAND)


func _process_idle(_delta: float) -> void:
	pass


func _process_move(delta: float) -> void:
	var to_target: Vector3 = _target_position - global_position
	to_target.y = 0.0
	if to_target.length() < ARRIVAL_THRESHOLD:
		_enter_state(STATE_WORK_SIT)
		return
	var dir: Vector3 = to_target.normalized()
	var target_velocity: Vector3 = dir * _speed
	velocity = velocity.lerp(target_velocity, clampf(ACCEL_LERP * delta, 0.0, 1.0))
	move_and_slide()
	var look_target_basis: Basis = Basis.looking_at(dir, Vector3.UP)
	var t: float = clampf(ROTATE_LERP * delta, 0.0, 1.0)
	transform.basis = transform.basis.slerp(look_target_basis, t).orthonormalized()
	if _anim != null and _anim.has_method("update"):
		_anim.update(velocity.length() / _speed)


func _process_work(delta: float) -> void:
	# Production timer независим от анимации.
	_work_timer += delta
	if _work_timer >= _work_duration:
		_work_timer -= _work_duration
		_emit_production()


## Снять NPC с работы. После завершения текущей gather-итерации
## NPC проиграет STAND → IDLE.
func dismiss() -> void:
	if _state == STATE_WORK_GATHER or _state == STATE_WORK_SIT:
		_stand_requested = true


func _on_anim_state_finished(state: String) -> void:
	# SIT → GATHER (первый запуск).
	# GATHER → replay GATHER (бесконечно), либо STAND если dismiss.
	# STAND → IDLE.
	match state:
		STATE_WORK_SIT:
			if _state != STATE_WORK_SIT:
				return
			if _stand_requested:
				_enter_state(STATE_WORK_STAND)
			else:
				_enter_state(STATE_WORK_GATHER)
		STATE_WORK_GATHER:
			if _state != STATE_WORK_GATHER:
				return
			if _stand_requested:
				_enter_state(STATE_WORK_STAND)
			else:
				_enter_state(STATE_WORK_GATHER)
		STATE_WORK_STAND:
			if _state == STATE_WORK_STAND:
				_enter_state(STATE_IDLE)


func _emit_production() -> void:
	if assigned_building == null or assigned_building.building_def == null:
		return
	var b: Resource = assigned_building.building_def
	if b.output_resource == null:
		return
	EventBus.resource_produced.emit(b.id, b.output_resource.id, b.output_amount)
