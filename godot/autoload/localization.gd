## Тонкая обёртка над TranslationServer.
##
## Подробности — docs/design/10_localization.md.
extends Node

const FALLBACK_LOCALE := "en"
const SUPPORTED_LOCALES: Array[String] = ["en", "es", "fr", "de", "ja"]

var current: String = FALLBACK_LOCALE


func _ready() -> void:
	_detect_initial_locale()
	print("[Localization] ready, locale=", current)


func _detect_initial_locale() -> void:
	var os_locale := OS.get_locale_language()
	if SUPPORTED_LOCALES.has(os_locale):
		set_locale(os_locale)
	else:
		set_locale(FALLBACK_LOCALE)


func set_locale(locale: String) -> void:
	if not SUPPORTED_LOCALES.has(locale):
		locale = FALLBACK_LOCALE
	current = locale
	TranslationServer.set_locale(locale)


func tr_format(key: String, args: Array) -> String:
	return tr(key).format(args)
