## Всплывающий 3D-текст (Label3D billboard).
##
## Использование:
##   FloatingText.spawn(get_tree().current_scene, position, "+10", Color.YELLOW)
##
## Phase 0: создаём instance per call с auto-free. Пулинг — Phase 2 (P2-112).
class_name FloatingText
extends Label3D

const RISE_DISTANCE := 1.2
const DURATION := 1.0


static func spawn(
	parent: Node,
	world_pos: Vector3,
	text: String,
	color: Color = Color(1, 1, 1, 1)
) -> FloatingText:
	var ft := FloatingText.new()
	ft.text = text
	ft.modulate = color
	ft.billboard = BaseMaterial3D.BILLBOARD_ENABLED
	ft.no_depth_test = true
	ft.fixed_size = true
	ft.pixel_size = 0.005
	ft.outline_size = 8
	ft.outline_modulate = Color(0, 0, 0, 0.8)
	ft.position = world_pos
	parent.add_child(ft)
	ft._animate()
	return ft


func _animate() -> void:
	var start := position
	var end := start + Vector3.UP * RISE_DISTANCE
	var tween := create_tween().set_parallel(true)
	tween.tween_property(self, "position", end, DURATION).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)
	tween.tween_property(self, "modulate:a", 0.0, DURATION).set_delay(DURATION * 0.4)
	tween.chain().tween_callback(queue_free)
