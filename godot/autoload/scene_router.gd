## Переходы между экранами с анимацией fade.
##
## Стек экранов для back-навигации.
extends Node

const SCENE_BOOT := "res://scenes/boot/boot.tscn"
const SCENE_WORLD := "res://scenes/world/world.tscn"

const FADE_DURATION := 0.2

var _stack: Array[String] = []
var _fade_rect: ColorRect


func _ready() -> void:
	_setup_fade_overlay()
	print("[SceneRouter] ready")


func _setup_fade_overlay() -> void:
	_fade_rect = ColorRect.new()
	_fade_rect.color = Color.BLACK
	_fade_rect.modulate.a = 0.0
	_fade_rect.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_fade_rect.set_anchors_preset(Control.PRESET_FULL_RECT)
	_fade_rect.z_index = 4096
	# Добавляем в дерево после готовности root.
	get_tree().root.call_deferred("add_child", _fade_rect)


func push(scene_path: String) -> void:
	_stack.append(scene_path)
	_goto(scene_path)


func replace(scene_path: String) -> void:
	if not _stack.is_empty():
		_stack[-1] = scene_path
	else:
		_stack.append(scene_path)
	_goto(scene_path)


func pop() -> void:
	if _stack.size() <= 1:
		return
	_stack.pop_back()
	_goto(_stack[-1])


func _goto(scene_path: String) -> void:
	var prev := GameState.current_screen
	await _fade_out()
	get_tree().change_scene_to_file(scene_path)
	GameState.current_screen = scene_path
	EventBus.screen_changed.emit(prev, scene_path)
	await _fade_in()


func _fade_out() -> void:
	if _fade_rect == null:
		return
	var tween := create_tween()
	tween.tween_property(_fade_rect, "modulate:a", 1.0, FADE_DURATION)
	await tween.finished


func _fade_in() -> void:
	if _fade_rect == null:
		return
	var tween := create_tween()
	tween.tween_property(_fade_rect, "modulate:a", 0.0, FADE_DURATION)
	await tween.finished
