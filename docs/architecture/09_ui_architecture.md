# 09 · UI-архитектура

## Принципы

- Mobile-first, портретная ориентация
- Тач-таргеты ≥ 44×44 pt (GDD/9)
- Однорукое управление в области большого пальца
- Все экраны строятся из переиспользуемых виджетов и темы

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

## Layout HUD

```
┌─────────────────────────────────────────────┐  ← safe area top
│ ⬆ GOLD: 1,234  💎 50  ⏱ 12:34            │  ← currency bar (top)
├─────────────────────────────────────────────┤
│                                             │
│                                             │
│            3D Game World                    │
│         (камера, здания, NPC)               │
│                                             │
│                                             │
├─────────────────────────────────────────────┤
│ [Shop] [NPCs] [Upgrades] [Events] [Menu]   │  ← bottom action bar
└─────────────────────────────────────────────┘  ← safe area bottom
```

Bottom bar — контекстный: меняется в зависимости от того, что игрок тапнул в мире (здание → апгрейд-панель, NPC → инфо-панель).

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

- **Tap:** `gui_input` на Control / `input_event` на Area3D в мире
- **Long press:** через таймер на `_pressed` (default 500 мс)
- **Pinch zoom (камера):** через `InputEventScreenPinch` (Godot 4 поддерживает) или fallback на два `InputEventScreenTouch`
- **Pan мир:** drag над пустой областью

Чувствительность настраивается в `Settings`.

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

- [ ] Landscape support для tablets — версия 1.x или post-launch?
- [ ] Глобальный UI sound layer (tap-сэмплы) — через AudioBus отдельный
