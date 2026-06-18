## Определение игрового товара / ресурса (пшеница, мука, хлеб, ...).
##
## Подробности — docs/architecture/05_data_model.md §«ResourceDef».
class_name ResourceDef
extends GameResource

## Базовая цена продажи в gold за единицу.
@export var base_sell_price: int = 1

## Иконка для UI (инвентарь, магазин, ачивки).
@export var icon: Texture2D = null

## Стадия переработки: 0 = сырьё, 1 = промежуточный продукт, 2 = финальный.
@export var stage: int = 0

## Редкость: common | rare | epic | legendary.
@export var tier: String = "common"
