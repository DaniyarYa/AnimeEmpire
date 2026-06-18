## Определение здания (ферма, мельница, пекарня, рынок).
##
## Подробности — docs/architecture/05_data_model.md §«BuildingDef».
class_name BuildingDef
extends GameResource

## Категория: generator | processor | service.
@export var category: String = "generator"

## Входной ресурс для переработки (null для генераторов). Тип Resource
## для избегания class_name-циклов при parse.
@export var input_resource: Resource = null

## Сколько единиц входного ресурса нужно за цикл.
@export var input_amount: int = 0

## Что производим. Тип Resource для избегания class_name-циклов.
@export var output_resource: Resource = null

## Сколько единиц output за один цикл.
@export var output_amount: int = 1

## Длительность одного цикла производства на уровне 1.
@export var base_cycle_seconds: float = 1.0

## Стоимость покупки на уровне 1 (gold).
@export var base_cost_gold: int = 100

## Коэффициент роста стоимости апгрейда: cost(N) = base × growth^(N-1).
## По GDD: 1.12 (ранние), 1.15 (средние), 1.18 (поздние).
@export var cost_growth: float = 1.15

## Потолок уровня.
@export var max_level: int = 25

## Уровень игрока, на котором здание разблокируется.
@export var unlock_level: int = 1

## Сколько NPC-слотов даёт здание (можно нанять рабочих).
@export var npc_slots: int = 1
