## Контроллер анимаций игрока: переключает AnimationPlayer.play() по
## состоянию.
##
## В Phase 1 — простое прямое переключение без AnimationTree.
## Полноценный AnimationTree с blend — задача BL-810 (см. backlog).
##
## Имена анимаций в FBX содержат префикс типа `Armature|Armature|<Name>|baselayer`.
## Контроллер автоматически находит правильное имя по подстроке.
class_name PlayerAnimationController
extends Node

const STATE_IDLE := "idle"
const STATE_WALK := "walk"
const STATE_RUN := "run"
const STATE_WORK := "work"
const STATE_CARRY_WALK := "carry_walk"
const STATE_CELEBRATE := "celebrate"

const SPEED_THRESHOLD_WALK := 0.05
const SPEED_THRESHOLD_RUN := 0.85

## Подстроки для поиска нужной анимации в `AnimationPlayer.get_animation_list()`.
const NAME_HINTS := {
	STATE_IDLE: ["Idle"],
	STATE_WALK: ["Walking", "Walk"],
	STATE_RUN: ["Running", "Run"],
	STATE_WORK: ["Collect_Object", "Collect", "Harvest"],
	STATE_CARRY_WALK: ["Carry_Heavy_Object_Walk", "Carry"],
	STATE_CELEBRATE: ["Victory_Cheer", "Cheer", "Celebrate"],
}

const ANIM_PLAYBACK_SPEED := 2.0

## Анимации, которые должны зацикливаться. Подстроки имён.
const LOOP_HINTS := ["Idle", "Walking", "Walk", "Running", "Run", "Carry"]

## Дополнительные FBX-файлы, из которых вытаскиваем анимации.
## Базовый player_avatar.fbx содержит только idle.
const EXTRA_ANIM_FILES := [
	"res://assets/characters/player_avatar/v0/animations/walk.fbx",
	"res://assets/characters/player_avatar/v0/animations/run.fbx",
	"res://assets/characters/player_avatar/v0/animations/walk_inplace.fbx",
	"res://assets/characters/player_avatar/v0/animations/carry_walk.fbx",
	"res://assets/characters/player_avatar/v0/animations/work_harvest.fbx",
	"res://assets/characters/player_avatar/v0/animations/celebrate.fbx",
]

var _player: AnimationPlayer = null
var _resolved_names: Dictionary = {}  ## STATE_* -> actual animation name
var _current_state: String = ""
var _override_state: String = ""


func _ready() -> void:
	_player = _find_anim_player(get_parent())
	if _player == null:
		push_warning("[PlayerAnimController] AnimationPlayer not found")
		return
	_player.speed_scale = ANIM_PLAYBACK_SPEED
	_load_extra_animations()
	_resolve_names()
	_play_state(STATE_IDLE)


func _load_extra_animations() -> void:
	if _player == null:
		return
	var lib: AnimationLibrary = _player.get_animation_library("")
	if lib == null:
		lib = AnimationLibrary.new()
		_player.add_animation_library("", lib)
	for path in EXTRA_ANIM_FILES:
		var scene: PackedScene = load(path)
		if scene == null:
			continue
		var instance: Node = scene.instantiate()
		var src_player := _find_anim_player(instance)
		if src_player != null:
			var src_lib: AnimationLibrary = src_player.get_animation_library("")
			if src_lib != null:
				for anim_name in src_lib.get_animation_list():
					if not lib.has_animation(anim_name):
						lib.add_animation(anim_name, src_lib.get_animation(anim_name))
		instance.queue_free()
	_apply_loop_flags(lib)
	print("[PlayerAnimController] all loaded anims: ", _player.get_animation_list())


func _apply_loop_flags(lib: AnimationLibrary) -> void:
	for anim_name in lib.get_animation_list():
		var should_loop: bool = false
		for hint in LOOP_HINTS:
			if anim_name.findn(hint) != -1:
				should_loop = true
				break
		if should_loop:
			var anim: Animation = lib.get_animation(anim_name)
			anim.loop_mode = Animation.LOOP_LINEAR


func _find_anim_player(node: Node) -> AnimationPlayer:
	if node is AnimationPlayer:
		return node
	for child in node.get_children():
		var found := _find_anim_player(child)
		if found != null:
			return found
	return null


func _resolve_names() -> void:
	if _player == null:
		return
	var available: PackedStringArray = _player.get_animation_list()
	for state in NAME_HINTS.keys():
		var hints: Array = NAME_HINTS[state]
		for hint in hints:
			for name in available:
				if name.findn(hint) != -1:
					_resolved_names[state] = name
					break
			if _resolved_names.has(state):
				break
	print("[PlayerAnimController] resolved: ", _resolved_names)


## Обновляет состояние по нормализованной скорости [0..1].
func update(speed_normalized: float) -> void:
	if _override_state != "":
		return
	var target: String = STATE_IDLE
	if speed_normalized >= SPEED_THRESHOLD_RUN:
		target = STATE_RUN
	elif speed_normalized >= SPEED_THRESHOLD_WALK:
		target = STATE_WALK
	_play_state(target)


## Принудительно проиграть состояние. duration <= 0 — без авто-снятия.
func override(state: String, duration: float = -1.0) -> void:
	_override_state = state
	_play_state(state)
	if duration > 0.0:
		await get_tree().create_timer(duration).timeout
		_override_state = ""


func _play_state(state: String) -> void:
	if state == _current_state:
		return
	if not _resolved_names.has(state):
		# Fallback на idle если не нашли.
		if state != STATE_IDLE and _resolved_names.has(STATE_IDLE):
			_play_state(STATE_IDLE)
		return
	if _player == null:
		return
	_current_state = state
	_player.play(_resolved_names[state])
