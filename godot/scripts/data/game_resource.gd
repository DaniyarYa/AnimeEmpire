## Базовый класс для всех data-Resource (.tres) в проекте.
##
## Гарантирует наличие id и i18n-ключа для отображаемого имени.
## Все игровые сущности (BuildingDef, ResourceDef, NPCDef, ...) наследуют этот класс.
class_name GameResource
extends Resource

## Уникальный ID сущности (snake_case). Используется как ключ в Dictionary,
## в сейве, в RemoteConfig override-путях.
@export var id: String = ""

## Ключ локализации для отображаемого имени (см. `Localization.tr_format()`).
@export var display_name_key: String = ""
