## Модалка взаимодействия со зданием.
##
## Phase 1 минимум:
##   - Заголовок (display_name_key через tr())
##   - Информация: cycle, rate, current inventory
##   - Кнопка "Start production" (для generator/processor)
##   - Кнопка "Sell all <resource>" (для service - market)
##   - Кнопка "Close"
class_name BuildingModal
extends CanvasItem

@onready var _title_label: Label = $Panel/VBox/Title
@onready var _info_label: Label = $Panel/VBox/Info
@onready var _action_button: Button = $Panel/VBox/ActionButton
@onready var _close_button: Button = $Panel/VBox/CloseButton

var _current_building = null


func _ready() -> void:
	hide()
	_close_button.pressed.connect(_on_close_pressed)
	_action_button.pressed.connect(_on_action_pressed)
	EventBus.resource_produced.connect(_on_inventory_changed)
	EventBus.resource_sold.connect(_on_resource_sold)


func show_for(building) -> void:
	_current_building = building
	_refresh()
	show()


func _refresh() -> void:
	if _current_building == null or _current_building.building_def == null:
		_title_label.text = "?"
		_info_label.text = ""
		_action_button.disabled = true
		return
	var b: Resource = _current_building.building_def
	var name_translated: String = tr(b.display_name_key)
	if name_translated == b.display_name_key:
		# Нет перевода — показываем id.
		name_translated = b.id.capitalize()
	_title_label.text = name_translated

	match b.category:
		"generator":
			_info_label.text = _generator_info(b)
			_action_button.text = (
				"Producing..." if _current_building.is_started() else "Start production"
			)
			_action_button.disabled = _current_building.is_started()
		"processor":
			_info_label.text = _processor_info(b)
			_action_button.text = (
				"Producing..." if _current_building.is_started() else "Start production"
			)
			_action_button.disabled = _current_building.is_started()
		"service":
			_info_label.text = _service_info(b)
			_action_button.text = "Sell all wheat"
			_action_button.disabled = EconomySim.get_inventory("wheat") == 0


func _generator_info(b: Resource) -> String:
	var rate: float = float(b.output_amount) / b.base_cycle_seconds
	var out_id: String = b.output_resource.id if b.output_resource else "?"
	return "Output: %s / sec %s\nInventory: %d" % [
		_format_rate(rate), out_id, EconomySim.get_inventory(out_id)
	]


func _processor_info(b: Resource) -> String:
	var rate: float = float(b.output_amount) / b.base_cycle_seconds
	var in_id: String = b.input_resource.id if b.input_resource else "?"
	var out_id: String = b.output_resource.id if b.output_resource else "?"
	return "%d %s → %d %s every %.0fs\nInventory: %s=%d, %s=%d" % [
		b.input_amount,
		in_id,
		b.output_amount,
		out_id,
		b.base_cycle_seconds,
		in_id,
		EconomySim.get_inventory(in_id),
		out_id,
		EconomySim.get_inventory(out_id),
	]


func _service_info(_b: Resource) -> String:
	return "Inventory wheat: %d × 1g\nInventory flour: %d × 3g\nInventory bread: %d × 15g" % [
		EconomySim.get_inventory("wheat"),
		EconomySim.get_inventory("flour"),
		EconomySim.get_inventory("bread"),
	]


func _format_rate(rate: float) -> String:
	if rate >= 1.0:
		return "%.1f" % rate
	return "%.2f" % rate


func _on_action_pressed() -> void:
	if _current_building == null or _current_building.building_def == null:
		return
	var b: Resource = _current_building.building_def
	match b.category:
		"generator", "processor":
			_current_building.start_production()
			_refresh()
		"service":
			_sell_all()


func _sell_all() -> void:
	var wheat: Resource = load("res://resources/resources_chain/wheat.tres")
	var flour: Resource = load("res://resources/resources_chain/flour.tres")
	var bread: Resource = load("res://resources/resources_chain/bread.tres")
	EconomySim.sell_inventory(wheat)
	EconomySim.sell_inventory(flour)
	EconomySim.sell_inventory(bread)
	_refresh()


func _on_close_pressed() -> void:
	_current_building = null
	hide()


func _on_inventory_changed(_b: String, _r: String, _a: int) -> void:
	if visible:
		_refresh()


func _on_resource_sold(_r: String, _a: int, _g: int) -> void:
	if visible:
		_refresh()
