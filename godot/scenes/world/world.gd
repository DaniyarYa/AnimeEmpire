## Прототип Phase 0: tap → +gold → upgrade.
##
## Не использует EconomySim — это намеренный минимум, чтобы проверить
## responsiveness Godot mobile renderer без зависимости от симуляции.
## Полноценная мир-сцена строится в Phase 1 (P1-030).
extends Node3D

const BASE_GOLD_PER_TAP := 1
const UPGRADE_BASE_COST := 10
const UPGRADE_COST_GROWTH := 1.5

@onready var _building: Area3D = $Building
@onready var _building_mesh: MeshInstance3D = $Building/Mesh
@onready var _avatar: Node3D = $Avatar
@onready var _gold_label: Label = $HUD/TopPanel/VBox/GoldLabel
@onready var _mult_label: Label = $HUD/TopPanel/VBox/MultLabel
@onready var _upgrade_button: Button = $HUD/BottomPanel/UpgradeButton

var _gold: int = 0
var _level: int = 1


func _ready() -> void:
	print("[World] ready (prototype)")
	_building.input_event.connect(_on_building_input)
	_upgrade_button.pressed.connect(_on_upgrade_pressed)
	_refresh_hud()
	_start_avatar_idle()


const AVATAR_ANIM_SPEED := 2.0


func _start_avatar_idle() -> void:
	if _avatar == null:
		return
	var anim_player := _find_anim_player(_avatar)
	if anim_player == null:
		push_warning("[World] AnimationPlayer не найден в Avatar")
		return
	var anims := anim_player.get_animation_list()
	if anims.is_empty():
		push_warning("[World] нет анимаций в Avatar")
		return
	print("[World] avatar animations: ", anims)
	anim_player.speed_scale = AVATAR_ANIM_SPEED
	anim_player.play(anims[0])


func _find_anim_player(node: Node) -> AnimationPlayer:
	if node is AnimationPlayer:
		return node
	for child in node.get_children():
		var found := _find_anim_player(child)
		if found != null:
			return found
	return null


func _on_building_input(_camera: Node, event: InputEvent, click_pos: Vector3, _normal: Vector3, _idx: int) -> void:
	if event is InputEventScreenTouch and event.pressed:
		_collect(click_pos)
	elif event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		_collect(click_pos)


func _collect(at: Vector3) -> void:
	var amount := BASE_GOLD_PER_TAP * _level
	_gold += amount
	_pulse_building()
	FloatingText.spawn(self, at + Vector3.UP * 0.5, "+%d" % amount, Color(1, 0.85, 0.3))
	_refresh_hud()


func _on_upgrade_pressed() -> void:
	var cost := _upgrade_cost()
	if _gold < cost:
		_pulse_button_error()
		return
	_gold -= cost
	_level += 1
	_refresh_hud()
	FloatingText.spawn(self, _building.global_position + Vector3.UP * 2.5, "Lv %d!" % _level, Color(0.5, 0.83, 0.43))


func _upgrade_cost() -> int:
	return int(UPGRADE_BASE_COST * pow(UPGRADE_COST_GROWTH, _level - 1))


func _refresh_hud() -> void:
	_gold_label.text = "Gold: %d" % _gold
	_mult_label.text = "Tap = +%dg · Lv %d" % [BASE_GOLD_PER_TAP * _level, _level]
	var cost := _upgrade_cost()
	_upgrade_button.text = "Upgrade Lv %d → %d (cost %d)" % [_level, _level + 1, cost]
	_upgrade_button.disabled = _gold < cost


func _pulse_building() -> void:
	var tween := create_tween()
	_building_mesh.scale = Vector3.ONE
	tween.tween_property(_building_mesh, "scale", Vector3.ONE * 1.1, 0.05)
	tween.tween_property(_building_mesh, "scale", Vector3.ONE, 0.1)


func _pulse_button_error() -> void:
	var tween := create_tween()
	_upgrade_button.modulate = Color(1, 1, 1, 1)
	tween.tween_property(_upgrade_button, "modulate", Color(1, 0.4, 0.4), 0.1)
	tween.tween_property(_upgrade_button, "modulate", Color(1, 1, 1), 0.2)
