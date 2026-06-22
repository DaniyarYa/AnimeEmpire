## Gatherer NPC: автоматически ходит к assigned_building, работает,
## набирает carry-load до base_capacity, несёт к Market, выгружает,
## возвращается на work-точку и повторяет цикл.
##
## Phase 1 FSM:
##   IDLE → MOVE → WORK_SIT → WORK_GATHER (loop, до capacity)
##        → WORK_STAND → CARRY → DELIVER → RETURN → WORK_SIT (или IDLE)
##
## Carrier-как-отдельный-тип — Phase 2 (P2-011). Здесь carry «зашит»
## в Gatherer для vertical slice.
class_name NPC
extends CharacterBody3D

const STATE_IDLE := "idle"
const STATE_MOVE := "move"
const STATE_WORK_SIT := "work_sit"
const STATE_WORK_GATHER := "work_gather"
const STATE_WORK_STAND := "work_stand"
const STATE_CARRY := "carry"
const STATE_DELIVER := "deliver"
const STATE_RETURN := "return"

## Если true — после dismiss() при следующем animation_finished
## переключаемся на STAND. Иначе GATHER replay (loop).
var _stand_requested: bool = false

const ARRIVAL_THRESHOLD := 0.5
const ACCEL_LERP := 10.0
const ROTATE_LERP := 10.0
const DELIVER_DURATION := 0.4
const STORAGE_ARRIVAL_OFFSET := Vector3(0, 0, 1.5)
## Длительности animation states. FSM сам продвигается по таймеру,
## не зависим от animation_finished signal (нестабилен с FBX-импортом).
const SIT_DURATION := 1.0
const STAND_DURATION := 1.0

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

## Resolved at _ready. Service-категория здание (Market) для доставки.
var _storage_target: Node3D = null
## Кэшированная точка работы (assigned_building.global_position + work_offset).
var _work_position: Vector3 = Vector3.ZERO
## Накопленный груз за текущий рейс: {resource_id: amount}.
var _carried: Dictionary = {}
## После delivery идём в IDLE, не запускаем новый SIT.
var _dismiss_pending: bool = false
## Countdown для визуальной паузы в DELIVER.
var _deliver_pause: float = 0.0
## Время в текущем animation-state (для defensive timeout).
var _state_timer: float = 0.0


func _ready() -> void:
	if npc_def != null and "base_speed" in npc_def:
		_speed = float(npc_def.base_speed)
	if not assigned_building_path.is_empty():
		assigned_building = get_node_or_null(assigned_building_path)
	# Дожидаемся завершения _ready всех детей (особенно AnimationController).
	await get_tree().process_frame
	if _anim != null and _anim.has_signal("state_finished"):
		_anim.state_finished.connect(_on_anim_state_finished)
	add_to_group("npcs")
	if assigned_building != null:
		_bind_to_building(assigned_building)
		if assigned_building.has_method("set_worker"):
			assigned_building.set_worker(self)
		_enter_state(STATE_MOVE)
	else:
		_enter_state(STATE_IDLE)


## Привязать NPC к зданию (общая логика для авто-старта и re-assign).
func _bind_to_building(building: Node3D) -> void:
	assigned_building = building
	_work_position = building.global_position + work_offset
	_target_position = _work_position
	_storage_target = _find_storage_target()


## Назначить NPC на здание из IDLE. Используется UI re-assign.
func assign(building: Node3D) -> void:
	if building == null:
		return
	_dismiss_pending = false
	_stand_requested = false
	_carried.clear()
	_bind_to_building(building)
	if building.has_method("set_worker"):
		building.set_worker(self)
	_enter_state(STATE_MOVE)


func is_available() -> bool:
	return _state == STATE_IDLE and not _dismiss_pending


func _physics_process(delta: float) -> void:
	match _state:
		STATE_IDLE:
			_process_idle(delta)
		STATE_MOVE:
			_process_move(delta)
		STATE_WORK_SIT:
			_process_sit(delta)
		STATE_WORK_GATHER:
			_process_work(delta)
		STATE_WORK_STAND:
			_process_stand(delta)
		STATE_CARRY:
			_process_carry(delta)
		STATE_DELIVER:
			_process_deliver(delta)
		STATE_RETURN:
			_process_return(delta)


func _process_sit(delta: float) -> void:
	_state_timer += delta
	if _state_timer >= SIT_DURATION:
		if _stand_requested:
			_enter_state(STATE_WORK_STAND)
		else:
			_enter_state(STATE_WORK_GATHER)


func _process_stand(delta: float) -> void:
	_state_timer += delta
	if _state_timer >= STAND_DURATION:
		if _carried_total() > 0:
			_enter_state(STATE_CARRY)
		elif _dismiss_pending:
			_enter_state(STATE_IDLE)
		else:
			_enter_state(STATE_WORK_SIT)


func _enter_state(state: String) -> void:
	print("[NPC ", name, "] state -> ", state, " carried=", _carried_total(), " dismiss=", _dismiss_pending)
	_state = state
	match state:
		STATE_IDLE:
			velocity = Vector3.ZERO
			_stand_requested = false
			_carried.clear()
			_dismiss_pending = false
			if _anim != null and _anim.has_method("clear_override"):
				_anim.clear_override()
			if _anim != null and _anim.has_method("update"):
				_anim.update(0.0)
		STATE_MOVE:
			pass
		STATE_WORK_SIT:
			velocity = Vector3.ZERO
			_stand_requested = false
			_state_timer = 0.0
			if _anim != null and _anim.has_method("override"):
				_anim.override(STATE_WORK_SIT)
		STATE_WORK_GATHER:
			velocity = Vector3.ZERO
			# _work_timer НЕ сбрасываем — между re-entry (anim replay) хотим
			# непрерывный счёт production. Иначе timer всегда меньше anim cycle.
			_state_timer = 0.0
			if assigned_building != null and assigned_building.building_def != null:
				var b: Resource = assigned_building.building_def
				var eff: float = 0.75
				if npc_def != null and "base_efficiency" in npc_def:
					eff = max(0.01, float(npc_def.base_efficiency))
				_work_duration = float(b.base_cycle_seconds) / eff
			if _anim != null and _anim.has_method("override"):
				_anim.override(STATE_WORK_GATHER)
		STATE_WORK_STAND:
			velocity = Vector3.ZERO
			_state_timer = 0.0
			if _anim != null and _anim.has_method("override"):
				_anim.override(STATE_WORK_STAND)
		STATE_CARRY:
			velocity = Vector3.ZERO
			if _storage_target != null:
				_target_position = _storage_target.global_position + STORAGE_ARRIVAL_OFFSET
			else:
				# Fallback: коллапсируем CARRY в DELIVER на месте.
				_target_position = global_position
			if _anim != null and _anim.has_method("override"):
				_anim.override(PlayerAnimationController.STATE_CARRY_WALK)
		STATE_DELIVER:
			velocity = Vector3.ZERO
			_deliver_pause = DELIVER_DURATION
			_deposit_carried()
			if _anim != null and _anim.has_method("clear_override"):
				_anim.clear_override()
			if _anim != null and _anim.has_method("update"):
				_anim.update(0.0)
		STATE_RETURN:
			_target_position = _work_position
			if _anim != null and _anim.has_method("clear_override"):
				_anim.clear_override()


func _process_idle(_delta: float) -> void:
	pass


func _process_move(delta: float) -> void:
	if _walk_toward(delta):
		_enter_state(STATE_WORK_SIT)


func _process_carry(delta: float) -> void:
	if _walk_toward(delta):
		_enter_state(STATE_DELIVER)


func _process_return(delta: float) -> void:
	# Если зданье освобождено пока шли назад — нечего делать, IDLE.
	if assigned_building == null:
		_enter_state(STATE_IDLE)
		return
	if _walk_toward(delta):
		if _dismiss_pending:
			_enter_state(STATE_IDLE)
		else:
			_enter_state(STATE_WORK_SIT)


func _process_deliver(delta: float) -> void:
	_deliver_pause -= delta
	if _deliver_pause <= 0.0:
		_enter_state(STATE_RETURN)


## Общая логика «иди к _target_position». Возвращает true если прибыли.
func _walk_toward(delta: float) -> bool:
	var to_target: Vector3 = _target_position - global_position
	to_target.y = 0.0
	if to_target.length() < ARRIVAL_THRESHOLD:
		velocity = Vector3.ZERO
		return true
	var dir: Vector3 = to_target.normalized()
	var target_velocity: Vector3 = dir * _speed
	velocity = velocity.lerp(target_velocity, clampf(ACCEL_LERP * delta, 0.0, 1.0))
	move_and_slide()
	var look_target_basis: Basis = Basis.looking_at(dir, Vector3.UP)
	var t: float = clampf(ROTATE_LERP * delta, 0.0, 1.0)
	transform.basis = transform.basis.slerp(look_target_basis, t).orthonormalized()
	if _anim != null and _anim.has_method("update"):
		_anim.update(velocity.length() / _speed)
	return false


func _process_work(delta: float) -> void:
	# Production timer независим от анимации.
	_work_timer += delta
	if _work_timer >= _work_duration:
		_work_timer -= _work_duration
		_accumulate_carried()
	# Достигли capacity — переходим в STAND.
	if npc_def != null and "base_capacity" in npc_def:
		if _carried_total() >= int(npc_def.base_capacity):
			_stand_requested = true
	if _stand_requested:
		_enter_state(STATE_WORK_STAND)


## Снять NPC с работы. Поведение зависит от текущего состояния:
##   SIT/GATHER — выйдем через STAND, затем доедем CARRY→DELIVER→RETURN→IDLE.
##   MOVE — разворачиваемся в RETURN сразу.
##   CARRY/DELIVER/RETURN — даём текущему рейсу доехать, по прибытию IDLE.
##   IDLE — no-op.
func dismiss() -> void:
	_dismiss_pending = true
	match _state:
		STATE_WORK_SIT, STATE_WORK_GATHER:
			_stand_requested = true
		STATE_MOVE:
			_enter_state(STATE_RETURN)


func _on_anim_state_finished(state: String) -> void:
	# SIT → GATHER (первый запуск).
	# GATHER → replay GATHER либо STAND если capacity hit / dismiss.
	# STAND → CARRY (если carried) / IDLE (если dismiss + пусто) / SIT (fallback).
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
				# Visual replay anim только — БЕЗ re-enter state,
				# иначе _work_timer сбрасывается и production никогда не фитит.
				if _anim != null and _anim.has_method("override"):
					_anim.override(STATE_WORK_GATHER)
		STATE_WORK_STAND:
			if _state != STATE_WORK_STAND:
				return
			if _carried_total() > 0:
				_enter_state(STATE_CARRY)
			elif _dismiss_pending:
				_enter_state(STATE_IDLE)
			else:
				_enter_state(STATE_WORK_SIT)


func _find_storage_target() -> Node3D:
	for node in get_tree().get_nodes_in_group("buildings"):
		if node == assigned_building:
			continue
		var def: Resource = null
		if "building_def" in node:
			def = node.building_def
		if def != null and "category" in def and String(def.category) == "service":
			return node
	return null


func _accumulate_carried() -> void:
	if assigned_building == null or assigned_building.building_def == null:
		return
	var b: Resource = assigned_building.building_def
	if b.output_resource == null:
		return
	var rid: String = String(b.output_resource.id)
	_carried[rid] = int(_carried.get(rid, 0)) + int(b.output_amount)


func _carried_total() -> int:
	var sum: int = 0
	for v in _carried.values():
		sum += int(v)
	return sum


func _deposit_carried() -> void:
	if _carried.is_empty():
		return
	var bid: String = "unknown"
	if assigned_building != null and assigned_building.building_def != null:
		bid = String(assigned_building.building_def.id)
	for rid in _carried.keys():
		EventBus.resource_produced.emit(bid, rid, int(_carried[rid]))
	_carried.clear()
