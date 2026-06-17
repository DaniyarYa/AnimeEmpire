## Реестр активных NPC и распределение задач.
##
## Подробности — docs/architecture/04_simulation.md, docs/design/03_npc_design.md.
extends Node

var _npcs: Dictionary = {}  # instance_id: String -> NPC node


func _ready() -> void:
	print("[NPCSystem] ready")


func register(instance_id: String, npc: Node) -> void:
	_npcs[instance_id] = npc


func unregister(instance_id: String) -> void:
	_npcs.erase(instance_id)


func get_npc(instance_id: String) -> Node:
	return _npcs.get(instance_id, null)


func count() -> int:
	return _npcs.size()


## Вызывается из EconomySim в одном тике с симуляцией.
func tick(_dt: float) -> void:
	pass  # TODO(P1-024): диспетчеризация задач
