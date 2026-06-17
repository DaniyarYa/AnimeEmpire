## Boot-сцена. Загружает базовые сервисы и переключается на основной мир.
extends Control

@onready var _status_label: Label = $CenterContainer/VBoxContainer/StatusLabel


func _ready() -> void:
	_status_label.text = "Загрузка..."
	# Даём autoload-сервисам отработать _ready().
	await get_tree().process_frame
	await get_tree().create_timer(0.5).timeout
	_status_label.text = "Запуск мира..."
	await get_tree().create_timer(0.3).timeout
	SceneRouter.replace(SceneRouter.SCENE_WORLD)
