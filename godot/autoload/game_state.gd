## Мета-состояние сессии (не баланс ресурсов — это в EconomySim).
extends Node

const SCREEN_BOOT := "boot"
const SCREEN_WORLD := "world"

var current_screen: String = SCREEN_BOOT
var session_start_time: int = 0
var is_tutorial_active: bool = false
var current_prestige_level: int = 0
var current_region: String = "village"
var session_id: String = ""
var user_id: String = ""
var country_code: String = ""


func _ready() -> void:
	session_start_time = Time.get_unix_time_from_system()
	print("[GameState] ready, session_start=", session_start_time)
