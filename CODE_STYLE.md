# Code Style — Anime Empire (GDScript 2.0)

Конвенции для всего GDScript-кода в `godot/`. Дополнение к `docs/architecture/02_project_layout.md` (структура и naming) и `docs/architecture/03_core_systems.md` (паттерны).

Линтер: `gdlint` (gdtoolkit 4.x). Форматтер: `gdformat`. Оба запускаются в CI.

## 1 · Файлы и папки

- `snake_case.gd` для скриптов; `snake_case.tscn` для сцен; `snake_case.tres` для ресурсов.
- Один `class_name` на файл; имя файла совпадает с именем класса в snake_case.
- Размещение — по слою: `scripts/systems/`, `scripts/entities/`, `scripts/data/`, `scripts/services/`, `scripts/platform/`, `scripts/ui/`, `scripts/utils/`. Подробнее — `docs/architecture/02_project_layout.md`.

## 2 · Слои и зависимости

Один направление зависимостей:

```
Presentation (ui/)  →  Game Logic (services/, systems/)  →  Simulation  →  Data
```

- `scripts/systems/*` не импортирует ничего из `scripts/ui/*`.
- UI получает обновления **только** через сигналы `EventBus`. Команды наружу — прямые вызовы сервисов.
- `addons/*` изолированы фасадом в `scripts/platform/*`. Прямые вызовы стороннего SDK из gameplay-кода запрещены.

## 3 · Именование

| Сущность | Конвенция | Пример |
|---------|-----------|--------|
| Класс (`class_name`) | `PascalCase` | `EconomySim`, `BuildingDef` |
| Файл скрипта | `snake_case.gd` | `economy_sim.gd` |
| Переменная / параметр | `snake_case` | `tick_accumulator` |
| Функция | `snake_case` | `apply_modifiers()` |
| Приватная (внутренняя) | `_leading_underscore` | `_tick_accumulator`, `_tick()` |
| Константа | `SCREAMING_SNAKE_CASE` | `MAX_NPC_PER_BUILDING` |
| Enum | `PascalCase` имя, `SCREAMING_SNAKE` значения | `enum State { IDLE, MOVING }` |
| Сигнал | `snake_case`, **прошедшее время** | `resource_produced`, `npc_arrived` |
| Autoload-узел | `PascalCase` | `EventBus`, `GameState` |

Что не использовать: венгерскую нотацию, `m_` префиксы, camelCase для переменных, `__` префикс.

## 4 · Типизация

GDScript 2.0 поддерживает статические типы — используй их везде, где это не мешает.

```gdscript
# Хорошо
func upgrade_building(id: String, level: int) -> bool:
    var cost: int = _calc_cost(id, level)
    return _spend_gold(cost)

# Плохо
func upgrade_building(id, level):
    var cost = _calc_cost(id, level)
    return _spend_gold(cost)
```

- Возвращаемый тип обязателен.
- Параметры — типизированы.
- Локальные переменные — тип где не выводится из контекста.
- `:= ` для type inference, когда тип очевиден.

## 5 · Сигналы (Signals)

- Имена в прошедшем времени — это **уведомление о факте**, не команда.
- Объявление с типизированными параметрами:
  ```gdscript
  signal resource_produced(building_id: String, resource_id: String, amount: int)
  ```
- Команды (действия) — методы сервисов, не сигналы.
- Подключение в `_ready()` через `signal.connect(handler)`, не `connect("signal_name", ...)` (старый API).
- Отписка не нужна, если объект удаляется через `queue_free()` — Godot чистит автоматически. Если нужна по логике — явный `disconnect()`.

## 6 · Magic numbers и константы

- Литералы в коде запрещены, кроме `0`, `1`, `-1`, `2` в очевидных контекстах.
- Всё остальное — `const` на уровне класса или в отдельном `Constants` ресурсе.

```gdscript
# Плохо
if _accumulator >= 0.1: ...

# Хорошо
const TICK_DT := 0.1
if _accumulator >= TICK_DT: ...
```

## 7 · Глобальные синглтоны (autoload)

- Регистрируются в `project.godot` под именем `PascalCase`.
- Прямой доступ: `EventBus.resource_produced.emit(...)`.
- Не делай circular dependencies между autoload — выноси общую логику в `scripts/utils/`.

## 8 · Сцены и узлы

- Корень сцены — один script-файл; он публичный API сцены.
- Дочерние узлы — `@onready var name := $Path` для кеша ссылок (избегай `find_node` в горячих циклах).
- `_ready()` — инициализация; `_process()` — только если нужно, иначе `set_process(false)`.
- Сигналы между дочерними узлами и корнем — через шину, а не напрямую через `get_parent()`.

## 9 · Ресурсы (.tres)

- Один `class_name` для каждого типа конфига (`BuildingDef`, `NPCDef`, ...).
- Все поля помечены `@export` для editor-визуализации.
- Дефолты на уровне класса, не во встроенных вызовах.

## 10 · Производительность

- Не аллоцируй массивы / словари в `_process` без необходимости.
- Кешируй ссылки на узлы в `_ready()` или через `@onready`.
- `print()` — только в dev-билдах. В горячих местах — никаких принтов.
- Циклы по большим коллекциям — измерить профайлером перед оптимизацией.

## 11 · Комментарии

- Пиши **почему**, не **что**. Имена и сигнатуры объясняют что.
- Не комментируй очевидное.
- Допустимые комментарии: пояснение неочевидной механики, ссылка на GDD-документ, предупреждение об edge case.
- TODO/FIXME с автором и датой: `# TODO(dy 2026-06-17): cap inventory`.

## 12 · Тесты

- `tests/` — gdUnit4 (или gut, выбирается в P0-050).
- Файл теста: `test_<unit>.gd`.
- Тестируем data-классы, чистые функции, миграции, формулы.
- UI-тесты — позже, через `SceneTree` interactions.

## 13 · Форматирование

Используй `gdformat`. Запуск локально:

```bash
gdformat godot/
```

Проверка без изменений (CI):

```bash
gdformat --check godot/
```

Lint:

```bash
gdlint godot/
```

CI отвергает PR с `gdlint` ошибками или несоответствием `gdformat`.

## 14 · Импорты и зависимости от плагинов

- Любая зависимость от `addons/<name>/` объявляется в `docs/architecture/01_engine_stack.md`.
- Версия плагина фиксируется (не подтягиваем main).
- Минимизация плагинов — приоритет.

## 15 · Git и коммиты

- Conventional Commits: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`.
- Subject ≤ 72 символов, в повелительном наклонении.
- Тело коммита объясняет почему (что — видно в diff).
- Один логический commit на одну задачу TODO (ID в commit message в скобках).

```
feat: add EconomySim tick at 10 Hz (P1-010)

Implements deterministic simulation tick at 10 Hz as defined in
architecture/04_simulation.md. Required for offline calculation.
```

## 16 · Документация в коде

- Doc-комментарий перед публичной функцией только если поведение неочевидно.
- Doc-комментарий перед `class_name` — одна строка о роли класса.
- Подробная документация — в `docs/`, не в коде.

## 17 · Что запрещено

- ❌ Глобальные переменные не через autoload
- ❌ Прямые `get_node("/root/...")` пути — только через autoload или `@onready`
- ❌ `Engine.get_main_loop().get_root()` — антипаттерн
- ❌ Логика в `_input()` без явного `accept_event()`
- ❌ Mutation `Resource` (`.tres`) в рантайме без явного `duplicate()`
- ❌ Циклический импорт `class_name` (используй forward references)

## Перекрёстные ссылки

- Структура проекта: `docs/architecture/02_project_layout.md`
- Autoload-сервисы: `docs/architecture/03_core_systems.md`
- Naming-конвенции расширенно: `docs/architecture/02_project_layout.md` § «Конвенции именования»
- Сценарий тестирования: будет в `docs/todo/01_phase0_preprod.md` (P0-050)
