## Player — управляемый аватар на CharacterBody3D.
##
## Получает направление от VirtualJoystick через set_movement_direction().
## Управляет скоростью + поворотом + анимацией (через AnimationController).
class_name Player
extends CharacterBody3D

const SPEED := 4.0  ## units/sec. В Phase 2 будет читаться из NPCDef-like.
const ACCEL := 18.0  ## Сглаживание velocity.
const ROTATION_SPEED := 12.0  ## rad/sec при slerp.

@onready var _anim_controller: PlayerAnimationController = $AnimationController

var _input_direction: Vector2 = Vector2.ZERO


## Установить направление движения. dir в [-1..1] (нормализованный joystick).
## Вызывается извне (joystick.direction_changed).
func set_movement_direction(dir: Vector2) -> void:
	_input_direction = dir


func _physics_process(delta: float) -> void:
	# Joystick: x = right, y = forward в плоскости XZ.
	# В Godot Control-системе y джойстика растёт вниз → тянем вверх = -y.
	# В 3D forward = -Z, поэтому y_joystick = -1 (вверх) должен дать velocity.z = -1 (вперёд).
	# Финально: velocity.z = +input_direction.y.
	var target_velocity: Vector3 = Vector3(_input_direction.x, 0.0, _input_direction.y) * SPEED
	velocity = velocity.lerp(target_velocity, clampf(ACCEL * delta, 0.0, 1.0))
	move_and_slide()

	if _input_direction.length() > 0.01:
		var horizontal_velocity: Vector3 = Vector3(velocity.x, 0.0, velocity.z)
		if horizontal_velocity.length() > 0.01:
			var look_dir: Vector3 = horizontal_velocity.normalized()
			var target_basis: Basis = Basis.looking_at(look_dir, Vector3.UP)
			var t: float = clampf(ROTATION_SPEED * delta, 0.0, 1.0)
			transform.basis = transform.basis.slerp(target_basis, t).orthonormalized()

	if _anim_controller != null:
		var horizontal_speed: float = Vector3(velocity.x, 0.0, velocity.z).length()
		_anim_controller.update(horizontal_speed / SPEED)
