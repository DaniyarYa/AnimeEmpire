## Определение NPC (Gatherer, Carrier, Operator, Specialist, Manager).
##
## Подробности — docs/architecture/05_data_model.md §«NPCDef»,
## docs/design/03_npc_design.md.
class_name NPCDef
extends GameResource

## Категория: gatherer | carrier | operator | specialist | manager.
@export var category: String = "gatherer"

## Редкость: common | rare | epic | legendary.
@export var rarity: String = "common"

## Скорость движения в units/sec.
@export var base_speed: float = 2.0

## Сколько единиц груза может нести.
@export var base_capacity: int = 5

## КПД (0..1) — влияет на cycle_time / output amount.
@export var base_efficiency: float = 0.75

## Стоимость найма (gold).
@export var hire_cost_gold: int = 1000

## Категория здания, к которому привязан NPC (пусто = универсал).
@export var attached_building_category: String = ""

## Сцена 3D-модели (PackedScene .tscn или .glb / .fbx).
@export var model_scene: PackedScene = null

## Портрет 1024×1024 для UI (магазин персонажей, события).
@export var portrait: Texture2D = null
