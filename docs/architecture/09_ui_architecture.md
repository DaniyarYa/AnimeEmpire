# 09 · UI-архитектура

## Принципы

- Mobile-first, **landscape ориентация** (см. ADR-004)
- Тач-таргеты ≥ 44×44 pt (GDD/9)
- **Двуручный grip:** левый палец — virtual joystick для движения, правый — UI и тап-взаимодействия
- Все экраны строятся из переиспользуемых виджетов и темы

> **ADR-004: Landscape вместо portrait + Virtual Joystick.**
> **Дата:** 2026-06-17.
> **Причина:** GDD/9 explicitly описывает виртуальный джойстик как primary
> movement control (§9.3 «Иерархия Тактильных Взаимодействий» + §«Movement
> Controls»). GDD/3 показывает `Joystick Navigation`. Это RPG-action
> paradigm, не tap-to-move idle-tycoon. Landscape даёт больше горизонтальной
> площади для джойстика слева + UI справа + 3D-сцены в центре.
> **Следствия:**
> - `project.godot`: viewport 1920×1080, orientation = LANDSCAPE
> - HUD перерисован под landscape (gold/level — top-left, action panel — bottom-right)
> - Player avatar управляется через `VirtualJoystick` widget, не через tap-to-move
> - Тап здания = открыть menu (не переместиться). Игрок сам подходит джойстиком
> - Camera-follow (не drag-pan) когда есть movable player

## Структура UI

```
scenes/ui/
├── theme/                         # глобальная тема
│   ├── main.theme.tres
│   ├── fonts/
│   └── styleboxes/
├── widgets/                       # переиспользуемые компоненты
│   ├── resource_chip.tscn         # иконка + кол-во
│   ├── currency_bar.tscn          # gold + gems + ads
│   ├── upgrade_button.tscn
│   ├── modal.tscn                 # базовый модал
│   ├── confirm_dialog.tscn
│   ├── floating_text.tscn         # +5 gold вылетает над зданием
│   └── timer_bar.tscn
└── screens/
    ├── boot.tscn
    ├── world_hud.tscn             # HUD поверх 3D мира
    ├── shop.tscn
    ├── npcs.tscn
    ├── upgrades.tscn
    ├── settings.tscn
    ├── prestige.tscn
    └── events.tscn
```

## Theme

Один `.theme.tres` для всего UI. Содержит:
- Шрифты (3-4 размера: caption/body/title/display)
- StyleBox для кнопок (default / hover / pressed / disabled)
- Цветовая палитра (см. `design/06_art_direction.md`)
- Иконки (atlas)

Альтернативные темы (например, тёмная) — отдельные `.theme.tres`, переключаются на лету.

## Layout HUD (landscape 1920×1080)

```
┌───────────────────────────────────────────────────────────────────────┐  ← safe area
│ ⬆ GOLD: 1,234  💎 50  ⏱ 12:34                          [Menu] [⚙] │  ← top status bar
├───────────────────────────────────────────────────────────────────────┤
│                                                                       │
│                                                                       │
│                                                       ┌─[Shop]──┐    │
│                                                       │[NPCs]    │    │
│              3D Game World                            │[Upgrades]│    │  ← right action panel
│         (камера, здания, NPC)                         │[Events]  │    │
│                                                       └──────────┘    │
│                                                                       │
│      ◯  ← virtual joystick (динамический)                            │  ← left thumb zone
│     ◯◯    появляется в точке касания                                 │
│      ◯                                                                │
└───────────────────────────────────────────────────────────────────────┘
```

- **Left thumb zone (1/3 экрана слева):** virtual joystick активируется при тапе вне UI
- **Right action panel:** Shop / NPCs / Upgrades / Events — постоянно видимы
- **Top bar:** валюты + меню + settings
- **Context overlays:** building upgrade modal появляется по центру при тапе здания

Right panel — контекстный: меняется в зависимости от тапнутого в мире (здание → апгрейд + NPC slots).

## Wireframe ключевых экранов

См. `design/05_ui_ux_spec.md`. Здесь — только архитектурные принципы.

## Адаптивность

- **Safe area:** учитываем notch / home indicator через `DisplayServer.screen_get_usable_rect()`. Margin по краям.
- **Aspect ratios:** поддерживаем 9:16, 9:18, 9:19.5 (iPhone X+, современные Android). На широких экранах — pillarbox или растяжение фона.
- **Текст:** Auto-wrap, локализованные строки могут быть на 30% длиннее английских (особенно немецкий)
- **Min font size:** 14pt
- **Accessibility scale:** мультипликатор 0.8-1.5 в настройках

## Сцена и роутинг

`SceneRouter` (autoload) управляет переходами:
```gdscript
SceneRouter.push("shop")
SceneRouter.pop()
SceneRouter.replace("world")
```

Анимация перехода: fade 200 мс (default), `slide_from_right` для модальных стэков.

## Сигналы UI

UI не подписывается на симуляцию напрямую — только через `EventBus`:
```gdscript
# world_hud.gd
func _ready() -> void:
    EventBus.currency_changed.connect(_on_currency_changed)
    EventBus.resource_produced.connect(_on_resource_produced)
```

Команды наружу — прямые вызовы сервисов:
```gdscript
# upgrade_button.gd
func _on_pressed() -> void:
    EconomySim.try_upgrade_building(building_id)
```

## Локализация

- `TranslationServer` со CSV/`.po` файлами
- Ключи: `tr("ui.shop.title")` (по dot-нотации)
- Плюрализация: `tr_n("inventory.wheat.count", count)` (через `TranslationServer.translate_plural()`)
- Дата/числа — через `Locale.format_number()`, `Locale.format_currency()` (наш wrapper)
- Tier-1 на запуск: en, es, fr, de, ja
- Tier-2 пост-лонч: pt, ru, ko, zh
- Подробно: `10_localization.md`

## Input

### Primary controls (landscape, двуручный grip)

- **Левый палец — Virtual Joystick:** тап вне UI → появляется динамический джойстик в точке касания → drag для движения аватара. Параметры (GDD/9):
  - max_radius: 80 px
  - dead_zone: 10% от max_radius
  - fade in/out 150-200 мс
  - Реализация: `scripts/ui/virtual_joystick.gd` + `scenes/ui/widgets/virtual_joystick.tscn`
- **Правый палец — UI и взаимодействия:**
  - Тап здания (Area3D) → открыть upgrade-modal (не перемещает аватара!)
  - Тап NPC → инфо-панель
  - Тап action panel (right) → переход на экран
  - Тап top bar → settings / меню

### Жесты

- **Long press (500 мс):** info tooltip
- **Pinch zoom (камера):** `InputEventScreenPinch` или fallback на два `InputEventScreenTouch`
- **Pan камеры:** drag двумя пальцами одновременно (одиночный drag = joystick)
- **Double tap по миру:** центрирование камеры на аватаре

Чувствительность джойстика и камеры — в `Settings`.

## Анимации UI

- Tween-анимации (`create_tween()`) для появления/исчезновения, NumberCounter
- AnimationPlayer для сложных сцен (например, модалка при апгрейде)
- 60 fps на UI обязательно (даже если симуляция в 10 Hz)

## Floating-text

Пул из 20 `FloatingText` нодов. При `EventBus.resource_produced` — берём свободный, ставим над зданием, твином 1 с вверх + fade.

## Туториал

Интегрирован в UI:
- Подсветка элементов через Mask / cutout
- Стрелки и хинты — отдельный layer
- `TutorialController` (autoload) подписан на `EventBus.event_*` и продвигает шаги

## Тёмная тема / акцессибилити

- Большой текст (`accessibility_text_scale`)
- Color-blind safe палитра (опц.) — альтернативный `.theme.tres` с дублированием иконок shape-кодами
- Button alternatives для жестов (например, кнопка вместо pinch zoom)

## Open вопросы

- [ ] Portrait fallback для tablet / большой экран — нужен ли вариант?
- [ ] Глобальный UI sound layer (tap-сэмплы) — через AudioBus отдельный
- [ ] Joystick: фиксированный режим (всегда видимый) как fallback для acessibility
