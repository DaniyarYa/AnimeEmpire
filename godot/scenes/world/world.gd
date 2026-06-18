## Phase 1 vertical slice: оркестрация Player + Buildings + UI.
##
## Подключает joystick → player, camera → follow player, buildings → modal.
extends Node3D

@onready var _player: Node3D = $Player
@onready var _camera: Camera3D = $Camera3D
@onready var _joystick: Control = $HUD/VirtualJoystick
@onready var _gold_label: Label = $HUD/TopBar/Margin/HBox/GoldLabel
@onready var _inventory_label: Label = $HUD/TopBar/Margin/HBox/InventoryLabel
@onready var _modal: CanvasItem = $HUD/BuildingModal


func _ready() -> void:
	print("[World] ready (Phase 1)")
	_camera.follow_target = _player
	_joystick.direction_changed.connect(_player.set_movement_direction)
	EventBus.currency_changed.connect(_on_currency_changed)
	EventBus.resource_produced.connect(_on_resource_produced)
	EventBus.resource_sold.connect(_on_resource_sold)
	for building in get_tree().get_nodes_in_group("buildings"):
		if building.has_signal("clicked"):
			building.clicked.connect(_on_building_clicked)
	_refresh_hud()


const RESOURCE_ICON := {"wheat": "🌾", "flour": "🌾→", "bread": "🍞"}
const FLOATING_TEXT_COOLDOWN := 0.6

var _last_float_per_building: Dictionary = {}  ## building_id -> int amount accumulated
var _last_float_time_per_building: Dictionary = {}  ## building_id -> unix_time


func _on_building_clicked(building) -> void:
	_modal.show_for(building)


func _on_currency_changed(_kind: String, _value: int) -> void:
	_refresh_hud()


func _on_resource_produced(building_id: String, resource_id: String, amount: int) -> void:
	_refresh_hud()
	_accumulate_floating(building_id, resource_id, amount)


func _accumulate_floating(building_id: String, resource_id: String, amount: int) -> void:
	var prev_amount: int = int(_last_float_per_building.get(building_id, 0))
	_last_float_per_building[building_id] = prev_amount + amount

	var now: float = Time.get_ticks_msec() / 1000.0
	var last_time: float = float(_last_float_time_per_building.get(building_id, 0.0))
	if now - last_time < FLOATING_TEXT_COOLDOWN:
		return

	var building := _find_building_node(building_id)
	if building == null:
		return
	var accumulated: int = int(_last_float_per_building[building_id])
	var icon: String = RESOURCE_ICON.get(resource_id, "+")
	FloatingText.spawn(
		self,
		building.global_position + Vector3(0, 2.5, 0),
		"+%d %s" % [accumulated, icon],
		Color(1, 0.85, 0.3)
	)
	_last_float_per_building[building_id] = 0
	_last_float_time_per_building[building_id] = now


func _on_resource_sold(resource_id: String, amount: int, gold: int) -> void:
	_refresh_hud()
	var market := _find_building_node("market")
	if market != null:
		FloatingText.spawn(
			self,
			market.global_position + Vector3(0, 2.5, 0),
			"+%d 💰 (−%d %s)" % [gold, amount, resource_id],
			Color(1, 0.722, 0.302)
		)


func _find_building_node(building_id: String) -> Node3D:
	for b in get_tree().get_nodes_in_group("buildings"):
		if b.building_def != null and b.building_def.id == building_id:
			return b
	return null


func _refresh_hud() -> void:
	_gold_label.text = "💰 %d" % EconomySim.get_gold()
	_inventory_label.text = "🌾 %d   🌾→ %d   🍞 %d" % [
		EconomySim.get_inventory("wheat"),
		EconomySim.get_inventory("flour"),
		EconomySim.get_inventory("bread"),
	]
