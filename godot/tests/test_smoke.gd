## Базовые smoke-тесты — проверяют, что test runner работает.
##
## Не зависят от autoload-сервисов (тестам без main scene autoload недоступны).
## Тесты с зависимостями на сервисы — Phase 2 (P2-120).
extends RefCounted


func test_arithmetic() -> bool:
	return 1 + 1 == 2


func test_string_concat() -> bool:
	return "anime " + "empire" == "anime empire"


func test_array_operations() -> Variant:
	var arr: Array[int] = [1, 2, 3]
	arr.append(4)
	if arr.size() != 4:
		return "expected size 4, got %d" % arr.size()
	if arr[-1] != 4:
		return "expected last element 4, got %d" % arr[-1]
	return true


func test_dict_merge() -> Variant:
	var a := {"x": 1, "y": 2}
	var b := {"y": 99, "z": 3}
	a.merge(b, true)
	if a.get("x") != 1:
		return "x should remain 1"
	if a.get("y") != 99:
		return "y should be overwritten to 99"
	if a.get("z") != 3:
		return "z should be added as 3"
	return true
