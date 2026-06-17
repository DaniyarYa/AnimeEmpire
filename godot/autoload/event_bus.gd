## Pub/sub шина для всех межсистемных событий.
##
## Правило: события — в прошедшем времени (факт случился).
## Команды (запросы) идут прямыми вызовами методов сервисов, не через шину.
extends Node

## Производство ресурса завершилось.
signal resource_produced(building_id: String, resource_id: String, amount: int)
## Продажа партии ресурса.
signal resource_sold(resource_id: String, amount: int, gold: int)
## Здание повышено в уровне.
signal building_upgraded(building_id: String, new_level: int)
## NPC нанят.
signal npc_hired(npc_id: String, instance_id: String)
## NPC завершил задачу.
signal npc_task_completed(instance_id: String, task: String)
## Состояние сохранения изменилось — требуется flush.
signal save_dirty
## Сохранение успешно записано.
signal save_persisted
## Валюта изменилась (kind: "gold" | "gems").
signal currency_changed(kind: String, new_value: int)
## IAP-покупка завершена.
signal iap_completed(sku: String, success: bool)
## Игрок инициировал prestige.
signal prestige_triggered(points_earned: int)
## Загружена новая версия удалённого конфига.
signal config_loaded(version: int)
## Произошёл переход между экранами.
signal screen_changed(from: String, to: String)


func _ready() -> void:
	print("[EventBus] ready")
