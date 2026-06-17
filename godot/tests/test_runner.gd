## Минимальный test runner для Phase 0.
##
## Полноценный gdUnit4 — задача P2-121 (когда test suite вырастет).
##
## Использование:
##   godot --headless --path godot --script res://tests/test_runner.gd
##
## Соглашения:
##   - Файлы: tests/test_<unit>.gd, наследник RefCounted
##   - Методы: test_<имя>() -> bool (true = pass, false = fail)
##   - Опционально: alternatively вернуть String с описанием ошибки = fail
extends SceneTree

const TESTS_DIR := "res://tests/"

var _passed: int = 0
var _failed: int = 0
var _failures: Array[String] = []


func _init() -> void:
	print("=== test runner ===")
	var files := _collect_tests()
	if files.is_empty():
		print("WARNING: no tests found in ", TESTS_DIR)
	for path in files:
		_run_file(path)
	_summary()


func _collect_tests() -> Array[String]:
	var files: Array[String] = []
	var dir := DirAccess.open(TESTS_DIR)
	if dir == null:
		push_error("Cannot open tests directory: " + TESTS_DIR)
		return files
	dir.list_dir_begin()
	var name := dir.get_next()
	while name != "":
		if (
			not dir.current_is_dir()
			and name.begins_with("test_")
			and name.ends_with(".gd")
			and name != "test_runner.gd"
		):
			files.append(TESTS_DIR + name)
		name = dir.get_next()
	dir.list_dir_end()
	files.sort()
	return files


func _run_file(path: String) -> void:
	var script: GDScript = load(path)
	if script == null:
		_record_failure(path, "<load>", "не удалось загрузить скрипт")
		return
	var instance: RefCounted = script.new()
	if instance == null:
		_record_failure(path, "<instantiate>", "не удалось создать инстанс")
		return
	var methods: Array = script.get_script_method_list()
	for m in methods:
		var method_name: String = m["name"]
		if not method_name.begins_with("test_"):
			continue
		_run_method(instance, path, method_name)


func _run_method(instance: RefCounted, path: String, method_name: String) -> void:
	var file_name := path.get_file()
	var label := "%s::%s" % [file_name, method_name]
	var result: Variant = instance.call(method_name)
	if result == true:
		print("  ✓ ", label)
		_passed += 1
	elif result is String and result != "":
		_record_failure(path, method_name, result)
	else:
		_record_failure(path, method_name, "assertion failed (returned %s)" % str(result))


func _record_failure(path: String, method_name: String, reason: String) -> void:
	var label := "%s::%s" % [path.get_file(), method_name]
	print("  ✗ ", label, " — ", reason)
	_failures.append("%s — %s" % [label, reason])
	_failed += 1


func _summary() -> void:
	print("===")
	print("Passed: ", _passed, "  Failed: ", _failed)
	if _failed > 0:
		print("Failures:")
		for f in _failures:
			print("  ", f)
		quit(1)
	else:
		quit(0)
