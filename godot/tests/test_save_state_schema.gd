## Тесты структуры сейв-схемы (без зависимости от autoload).
##
## Проверяют, что _new_state() из SaveService выдаёт правильную начальную
## структуру. Используем загрузку скрипта вручную, без autoload.
extends RefCounted

const SAVE_SCRIPT := "res://autoload/save_service.gd"


func _new_state_via_script() -> Dictionary:
	# Загружаем скрипт SaveService и вызываем приватный _new_state.
	# Это не способ unit-тестить production-код, но для smoke-тестов
	# Phase 0 хватает.
	var script: GDScript = load(SAVE_SCRIPT)
	if script == null:
		return {}
	var instance: Node = script.new()
	if not instance.has_method("_new_state"):
		instance.free()
		return {}
	var state: Dictionary = instance.call("_new_state")
	instance.free()
	return state


func test_new_state_has_save_version() -> Variant:
	var state := _new_state_via_script()
	if not state.has("save_version"):
		return "missing save_version"
	if state["save_version"] != 1:
		return "expected save_version=1, got %s" % str(state["save_version"])
	return true


func test_new_state_has_currencies() -> Variant:
	var state := _new_state_via_script()
	if not state.has("currencies"):
		return "missing currencies"
	var c: Dictionary = state["currencies"]
	if c.get("gold") != 0:
		return "gold should start at 0"
	if c.get("gems") != 0:
		return "gems should start at 0"
	return true


func test_new_state_has_prestige() -> Variant:
	var state := _new_state_via_script()
	if not state.has("prestige"):
		return "missing prestige"
	var p: Dictionary = state["prestige"]
	if p.get("level") != 0:
		return "prestige level should start at 0"
	return true


func test_new_state_has_tutorial() -> Variant:
	var state := _new_state_via_script()
	if not state.has("tutorial"):
		return "missing tutorial"
	var t: Dictionary = state["tutorial"]
	if t.get("step") != 0:
		return "tutorial step should start at 0"
	if t.get("completed") != false:
		return "tutorial should not be completed initially"
	return true
