# 9. UX/UI Дизайн и Управление

# UX/UI Дизайн и Управление

## 9.1 Философия UX Дизайна

### Основные Принципы UX

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

entity "Ясность" as clarity {
  :Визуальная Иерархия;
  :Четкая Информация;
  :Интуитивные Иконки;
  :Читаемая Типографика;
}

entity "Эффективность" as efficiency {
  :Минимум Кликов;
  :Быстрая Навигация;
  :Умные Значения по Умолчанию;
  :Поддержка Жестов;
}

entity "Доступность" as accessibility {
  :Игра Одной Рукой;
  :Большие Области Касания;
  :Безопасно для Дальтоников;
  :Множественные Языки;
}

entity "Удовольствие" as delight {
  :Плавные Анимации;
  :Удовлетворяющая Обратная Связь;
  :Визуальная Полировка;
  :Личность;
}

clarity --> efficiency : "Четкий интерфейс\nвключает быстрые действия"
efficiency --> accessibility : "Эффективный дизайн\nподдерживает всех пользователей"
accessibility --> delight : "Доступная основа\nпозволяет приятные детали"
delight --> clarity : "Приятный опыт\nусиливает четкий дизайн"

note bottom : "UX служит геймплею в первую очередь\nВизуальная привлекательность поддерживает функцию"
@enduml
```

![image.png](attachment:917fd291-343d-4963-b9b4-1e7d67a6f088:image.png)

### Ограничения Дизайна Mobile-First

```yaml
Mobile_UX_Requirements:
  device_support:
    screen_sizes: "4.7\\" to 6.7\\" phones, 7\\" to 12\\" tablets"
    orientations: "Portrait primary, landscape optional"
    input_methods: "Touch only, no external controllers"

  interaction_design:
    thumb_reach: "All critical actions within thumb zone"
    touch_targets: "Minimum 44px (iOS) / 48dp (Android)"
    gesture_support: "Tap, hold, drag, pinch (optional)"

  performance_constraints:
    ui_responsiveness: "<16ms frame time (60fps)"
    animation_budget: "Smooth on low-end devices"
    memory_usage: "UI textures <50MB total"

  accessibility_requirements:
    font_sizes: "Minimum 14pt for body text"
    contrast_ratios: "4.5:1 minimum for text"
    color_coding: "Never rely solely on color"
    localization: "Support for 8+ languages"

```

## 9.2 Камера и Навигация по Миру

### Дизайн Системы Камеры

```
@startuml
!theme plain

entity "Режимы Камеры" as modes {
  :Режим Следования за Игроком;
  :Режим Фокуса на Здании;
  :Режим Обзора;
  :Кинематографический Режим;
}

entity "Управление Камерой" as controls {
  :Автоматическое Следование;
  :Касание для Фокуса;
  :Щипок для Масштабирования;
  :Панорамирование Краем (Опционально);
}

entity "Визуальная Обратная Связь" as feedback {
  :Плавные Переходы;
  :Индикаторы Фокуса;
  :Ограничения Масштабирования;
  :Визуализация Границ;
}

modes --> controls : "Каждый режим имеет\nсоответствующие элементы управления"
controls --> feedback : "Элементы управления предоставляют\nчеткую визуальную обратную связь"
feedback --> modes : "Обратная связь помогает\nпереходам между режимами"

note bottom : "Камера никогда не борется с игроком\nВсегда поддерживает текущую активность"
@enduml
```

![image.png](attachment:809cc1d5-cbd9-4641-9800-ca77b21ba1f3:image.png)

### Стратегия Навигации по Миру

```yaml
Navigation_System:
  camera_behavior:
    default_mode: "Follow player character"
    auto_focus_triggers:
      - "Building upgrade completion"
      - "New content unlock"
      - "Achievement notification"
      - "Important events"

  zoom_system:
    min_zoom: "Close-up view for detailed work"
    max_zoom: "Overview of entire village"
    default_zoom: "Optimal for character movement"
    zoom_speed: "Smooth, not jarring"

  boundary_management:
    soft_boundaries: "Camera slows near edges"
    visual_indicators: "Show world limits clearly"
    expansion_hints: "Indicate where new areas unlock"

  performance_optimization:
    culling_system: "Don't render off-screen objects"
    lod_system: "Reduce detail at distance"
    ui_scaling: "Maintain readability at all zoom levels"

```

### UX Поток Навигации

```
@startuml
!theme plain

start
:Игрок Появляется;
:Камера Следует за Игроком;
note right: Состояние по умолчанию
if (Игрок Касается Здания?) then (да)
  :Камера Фокусируется на Здании;
  :Показать UI Здания;
  if (Игрок Закрывает UI?) then (да)
    :Вернуться к Режиму Следования;
  else (нет)
    :Поддерживать Фокус на Здании;
  endif
else (нет)
  if (Важное Событие?) then (да)
    :Авто-Фокус Камеры на Событии;
    :Показать UI События;
    :Вернуться к Следованию Через 3с;
  else (нет)
    :Продолжить Следование за Игроком;
  endif
endif
stop

@enduml

```

note bottom: "Камера всегда служит намерению игрока\\nНикогда не прерывает без веской причины"

![image.png](attachment:5737b083-359c-41ce-a28f-0b5d9e594a7d:image.png)

## 9.3 Основная Модель Взаимодействия

### Иерархия Тактильных Взаимодействий

```
@startuml
!theme plain

package "Основные Взаимодействия" as primary {
  entity "Движение" as movement {
    :Виртуальный Джойстик;
    :Везде на Экране;
    :Динамическое Позиционирование;
  }
  
  entity "Взаимодействие с Зданием" as building {
    :Касание для Открытия Меню;
    :Удержание для Быстрых Действий;
    :Четкая Визуальная Обратная Связь;
  }
  
  entity "Навигация UI" as ui_nav {
    :Касание Кнопок;
    :Свайп Панелей;
    :Щипок Масштабирования (Опционально);
  }
}

package "Вторичные Взаимодействия" as secondary {
  entity "Жестовые Сокращения" as gestures {
    :Двойное Касание для Быстрых Действий;
    :Долгое Нажатие для Контекстного Меню;
    :Свайп для Навигации Панелей;
  }
  
  entity "Продвинутые Элементы Управления" as advanced {
    :Мультитач для Точности;
    :Жесты Краев;
    :Сокращения Доступности;
  }
}

primary --> secondary : "Основные взаимодействия\nработают идеально сначала"
secondary --> primary : "Продвинутые функции\nулучшают основной опыт"

note bottom : "Модель взаимодействия масштабируется от\nказуального до продвинутого пользователя"
@enduml
```

![image.png](attachment:92ed0077-9f17-4459-9ac1-7bfe7ae011a6:image.png)

### Система Управления Движением

```yaml
Movement_Controls:
  virtual_joystick:
    activation: "Touch anywhere outside UI elements"
    visual_feedback: "Dynamic joystick appears at touch point"
    dead_zone: "10% of joystick radius"
    max_range: "80px from center"

  movement_feel:
    acceleration: "Smooth start/stop, not instant"
    max_speed: "Balanced for screen size and zoom"
    precision_mode: "Slower movement near interactive objects"

  accessibility_features:
    large_touch_area: "Entire screen except UI is movement area"
    visual_indicators: "Clear movement direction feedback"
    customizable_sensitivity: "Player can adjust in settings"

  performance_optimization:
    input_sampling: "60Hz input sampling for smooth movement"
    prediction: "Slight input prediction for responsiveness"
    battery_efficiency: "Optimize for minimal power consumption"

```

### Автоматизация Сбора Ресурсов

```
@startuml
!theme plain

start
:Игрок Приближается к Ресурсу;
note right: В пределах радиуса сбора
:Автоматический Триггер Сбора;
:Визуальная Анимация Сбора;
:Ресурс Добавлен в Инвентарь;
:Аудио Обратная Связь;
if (Инвентарь Полон?) then (да)
  :Показать Индикатор Полного Инвентаря;
  :Предложить Апгрейд или Доставку;
else (нет)
  :Продолжить Обычный Геймплей;
endif
stop

@enduml

```

note bottom: "Касание не требуется для базовых действий\\nПлавная, удовлетворяющая автоматизация"

![image.png](attachment:36785b66-6509-4e3c-a7f9-87a5e8df9df5:image.png)

## 9.4 Макет UI и Информационная Архитектура

### Стратегия Макета Экрана

```
@startuml
!theme plain

entity "Верхняя Панель (Статус)" as top {
  :Счетчик Золота;
  :Счетчик Камней;
  :Активные Бустеры;
  :Доступ к Настройкам;
}

entity "Игровой Мир (Центр)" as center {
  :3D Вид Деревни;
  :Персонаж и NPC;
  :Здания и Ресурсы;
  :Интерактивные Элементы;
}

entity "Нижняя Панель (Действия)" as bottom {
  :Апгрейды Зданий;
  :Быстрые Действия;
  :Навигация Меню;
  :Контекстно-Зависимые Инструменты;
}

entity "Боковые Панели (Детали)" as sides {
  :Детали Зданий;
  :Характеристики Персонажа;
  :Прогресс Достижений;
  :Информация о Событиях;
}

top --> center : "Статус информирует\nигровые решения"
center --> bottom : "Состояние мира управляет\nдоступными действиями"
bottom --> sides : "Действия открывают\nдетальные интерфейсы"
sides --> center : "Детали улучшают\nпонимание мира"

note bottom : "Информационная иерархия поддерживает\nестественный поток геймплея"
@enduml
```

![image.png](attachment:b69f474f-263b-4d0b-9f03-78bd866757b7:image.png)

### Спецификации Компонентов UI

```yaml
UI_Components:
  top_status_bar:
    height: "60px (safe area + content)"
    elements:
      gold_counter: "Left side, large font, coin icon"
      gem_counter: "Left-center, medium font, gem icon"
      boosters: "Right-center, icon badges with timers"
      settings: "Right side, gear icon"

  bottom_action_panel:
    height: "120px (expandable to 200px)"
    behavior: "Slides up from bottom, context-sensitive"
    elements:
      primary_actions: "Large buttons, 80px height"
      secondary_actions: "Smaller buttons, 40px height"
      upgrade_queue: "Horizontal scroll, progress indicators"

  side_detail_panels:
    width: "300px (80% screen width max)"
    animation: "Slide in from left/right"
    content: "Scrollable, rich information display"

  floating_elements:
    notifications: "Top-center, auto-dismiss"
    tooltips: "Context-sensitive, near relevant elements"
    progress_indicators: "Overlay on relevant world objects"

```

### Система Адаптивного Дизайна

```
@startuml
!theme plain

entity "Маленькие Телефоны (4.7)" as small {
  :Компактные Элементы UI;
  :Одноколоночные Макеты;
  :Большие Области Касания;
  :Упрощенная Информация;
}

entity "Стандартные Телефоны (5.5-6.1\")" as standard {
  :Сбалансированная Плотность UI;
  :Двухколоночные Макеты;
  :Стандартные Области Касания;
  :Полное Отображение Информации;
}

entity "Большие Телефоны (6.2-6.7)" as large {
  :Богатые Элементы UI;
  :Многоколоночные Макеты;
  :Точные Области Касания;
  :Расширенная Информация;
}

entity "Планшеты (7-12)" as tablets {
  :Макеты как на Десктопе;
  :Боковая Навигация;
  :Множественные Панели;
  :Максимальная Информация;
}

small --> standard : "Масштабировать\nплотность UI"
standard --> large : "Добавить больше\nинформации"
large --> tablets : "Реструктурировать\nдля больших экранов"

note bottom : "UI адаптируется к размеру экрана\nПоддерживает удобство использования на всех устройствах"
@enduml
```

![image.png](attachment:ef8f032a-f1ca-4a0e-8be2-9e42e1a52631:image.png)

## 9.5 Система Визуальной Обратной Связи и Анимации

### Иерархия Обратной Связи

```yaml
Feedback_System:
  immediate_feedback: # <100ms
    touch_response: "Button press states, ripple effects"
    collection_feedback: "Resource pickup animations"
    ui_interactions: "Hover states, selection indicators"

  short_term_feedback: # 100ms-1s
    action_results: "Number changes, progress bars"
    state_changes: "Building upgrades, NPC assignments"
    achievement_notifications: "Popup celebrations"

  medium_term_feedback: # 1s-10s
    process_completion: "Building construction, resource processing"
    milestone_celebrations: "Level ups, unlocks"
    system_notifications: "Offline earnings, event starts"

  long_term_feedback: # 10s+
    progression_visualization: "Village growth, character development"
    meta_progression: "Prestige celebrations, new content unlocks"
    seasonal_changes: "Event themes, holiday decorations"

```

### Принципы Анимации

```
@startuml
!theme plain

entity "Целевая Анимация" as purposeful {
  :Коммуникация Изменений Состояния;
  :Направление Внимания Игрока;
  :Предоставление Удовлетворяющей Обратной Связи;
  :Улучшение Личности;
}

entity "Осознание Производительности" as performance {
  :60fps на Целевых Устройствах;
  :Эффективное Использование GPU;
  :Минимальное Влияние на CPU;
  :Дружелюбность к Батарее;
}

entity "Осознание Доступности" as accessibility {
  :Уважение Предпочтений Движения;
  :Четкость Без Анимации;
  :Нет Триггеров Приступов;
  :Настраиваемая Скорость;
}

purposeful --> performance : "Красивые анимации\nдолжны работать хорошо"
performance --> accessibility : "Плавная производительность\nвключает доступность"
accessibility --> purposeful : "Доступный дизайн\nулучшает цель"

note bottom : "Каждая анимация служит геймплею\nНи одна не является чисто декоративной"
@enduml
```

![image.png](attachment:b4291846-6e7d-4a2d-8329-ae9caff6faa4:image.png)

### Ключевые Последовательности Анимации

```yaml
Core_Animations:
  resource_collection:
    duration: "0.3-0.5 seconds"
    easing: "Ease-out for natural feel"
    visual_elements: "Resource flies to inventory, counter updates"
    audio: "Satisfying pickup sound"

  building_upgrade:
    duration: "1.0-2.0 seconds"
    stages: "Preparation → Construction → Completion"
    visual_elements: "Sparkle effects, model changes, celebration"
    audio: "Construction sounds, completion fanfare"

  npc_assignment:
    duration: "0.5-1.0 seconds"
    visual_elements: "NPC walks to position, starts work animation"
    feedback: "Assignment confirmation, efficiency indicator"

  prestige_activation:
    duration: "3.0-5.0 seconds"
    stages: "Confirmation → Reset → Bonus Application → New Start"
    visual_elements: "Screen effects, progress visualization, celebration"
    audio: "Epic music, achievement sounds"

```

## 9.6 Онбординг и Дизайн Туториала

### Философия Туториала

```
@startuml
!theme plain

entity "Обучение Через Делание" as doing {
  :Нет Отдельного Режима Туториала;
  :Интегрировано в Геймплей;
  :Реальный Прогресс с Начала;
  :Немедленные Награды;
}

entity "Постепенное Раскрытие" as disclosure {
  :Одна Концепция за Раз;
  :Строится на Предыдущем Обучении;
  :Контекстно-Зависимая Помощь;
  :Опциональные Глубокие Погружения;
}

entity "Дружелюбность к Ошибкам" as friendly {
  :Нет Постоянных Неудач;
  :Легкая Отмена/Повтор;
  :Мягкая Коррекция;
  :Ободряющий Тон;
}

doing --> disclosure : "Активное обучение\nраскрывает сложность постепенно"
disclosure --> friendly : "Постепенная сложность\nуменьшает ошибки"
friendly --> doing : "Безопасная среда\nпоощряет экспериментирование"

note bottom : "Туториал невидим\nИгроки учатся во время игры"
@enduml
```

![image.png](attachment:82c23891-818a-4a0e-879b-b02f63ff5d2d:image.png)

### Дизайн Потока Онбординга

```yaml
Tutorial_Sequence:
  step_1_movement: # 0-30 seconds
    goal: "Learn character movement"
    method: "Highlight movement area, show joystick"
    success_criteria: "Player moves character 5 meters"
    reward: "Positive feedback, next step unlock"

  step_2_collection: # 30-60 seconds
    goal: "Understand resource collection"
    method: "Guide to wheat farm, automatic collection"
    success_criteria: "Collect 3 wheat"
    reward: "Inventory update, collection satisfaction"

  step_3_processing: # 1-2 minutes
    goal: "Learn resource processing"
    method: "Guide to mill, show processing UI"
    success_criteria: "Convert wheat to flour"
    reward: "Value increase demonstration"

  step_4_selling: # 2-3 minutes
    goal: "Complete first economic cycle"
    method: "Guide to market, show sale process"
    success_criteria: "Sell flour for gold"
    reward: "Gold earned, upgrade possibility"

  step_5_upgrading: # 3-5 minutes
    goal: "Understand progression system"
    method: "Show upgrade options, guide first purchase"
    success_criteria: "Upgrade building or tool"
    reward: "Visible improvement, efficiency gain"

```

### Архитектура Системы Помощи

```
@startuml
!theme plain

rectangle "Контекстная Помощь" as contextual {
  :Подсказки на Элементах UI;
  :Советы для Текущей Ситуации;
  :Умные Предложения;
  :Обнаружение Проблем;
}

rectangle "Справочная Система" as reference {
  :Поисковая База Данных Помощи;
  :Видео Туториалы;
  :Раздел FAQ;
  :Ссылки на Сообщество;
}

rectangle "Постепенная Помощь" as progressive {
  :Мягкие Подталкивания Сначала;
  :Более Сильные Подсказки Если Застрял;
  :Прямая Помощь Если Нужно;
  :Опции Пропуска Доступны;
}

contextual --> progressive : "Осознание контекста\\nуправляет уровнем помощи"
progressive --> reference : "Сложные проблемы\\nнуждаются в детальной помощи"
reference --> contextual : "Обучение по справочнику\\nулучшает контекстную помощь"

note bottom : "Система помощи адаптируется к потребностям игрока\\nНикогда не перегружает, всегда доступна"
@enduml

```

![image.png](attachment:41908e8e-d102-4a06-9444-a76bdb4d52df:image.png)

## 9.7 Доступность и Локализация

### Стандарты Доступности

```yaml
Accessibility_Requirements:
  visual_accessibility:
    color_blind_support: "Never rely solely on color for information"
    contrast_ratios: "WCAG AA compliance (4.5:1 minimum)"
    font_sizes: "Scalable text, minimum 14pt"
    visual_indicators: "Icons + text for all important information"

  motor_accessibility:
    large_touch_targets: "Minimum 44px/48dp touch areas"
    gesture_alternatives: "Tap alternatives for all gestures"
    timing_flexibility: "No time-critical actions required"
    one_handed_play: "All functions accessible with thumb"

  cognitive_accessibility:
    clear_language: "Simple, direct communication"
    consistent_patterns: "Predictable UI behavior"
    error_prevention: "Confirmation for destructive actions"
    progress_indicators: "Clear feedback on all actions"

  hearing_accessibility:
    visual_audio_cues: "Visual indicators for all audio feedback"
    subtitle_support: "Text for all spoken content"
    vibration_feedback: "Haptic alternatives to audio cues"

```

### Стратегия Локализации

```
@startuml
!theme plain

entity "Языки Уровня 1" as tier1 {
  :Английский (Основной);
  :Испанский;
  :Французский;
  :Немецкий;
  :Японский;
}

entity "Языки Уровня 2" as tier2 {
  :Португальский;
  :Русский;
  :Корейский;
  :Китайский Упрощенный;
}

entity "Языки Уровня 3" as tier3 {
  :Итальянский;
  :Голландский;
  :Китайский Традиционный;
  :Арабский;
}

tier1 --> tier2 : "Запуск с Уровнем 1\nДобавить Уровень 2 после запуска"
tier2 --> tier3 : "Расширение на основе\nпроизводительности рынка"

note bottom : "Приоритет локализации на основе\nразмера рынка и потенциала монетизации"
@enduml
```

![image.png](attachment:c6ab8f18-d21e-48f2-9763-a301bb815373:image.png)

### Технические Требования Локализации

```yaml
Localization_Tech:
  text_expansion:
    ui_space_buffer: "40% extra space for text expansion"
    dynamic_layouts: "UI adapts to text length"
    font_support: "Unicode support for all target languages"

  cultural_adaptation:
    number_formats: "Locale-appropriate number formatting"
    currency_display: "Local currency symbols and formats"
    date_time: "Regional date/time formats"
    cultural_colors: "Avoid culturally inappropriate color choices"

  technical_implementation:
    string_externalization: "All text in external files"
    pseudo_localization: "Testing with expanded text"
    rtl_support: "Right-to-left language support (Arabic)"
    font_fallbacks: "Graceful degradation for missing characters"

```

## 9.8 Производительность UI и Оптимизация

### Цели Производительности

```yaml
UI_Performance:
  frame_rate_targets:
    target_devices: "60fps on iPhone 8, Galaxy S8 equivalent"
    minimum_acceptable: "30fps on low-end devices"
    ui_budget: "4ms per frame for UI rendering"

  memory_constraints:
    ui_texture_budget: "50MB maximum for all UI textures"
    font_memory: "10MB maximum for all font assets"
    animation_memory: "Dynamic allocation, 20MB peak"

  loading_performance:
    ui_initialization: "<2 seconds from app launch"
    screen_transitions: "<300ms between screens"
    popup_appearance: "<100ms for modal dialogs"

  battery_optimization:
    animation_efficiency: "Use GPU acceleration where possible"
    update_frequency: "Minimize unnecessary UI updates"
    background_behavior: "Reduce UI activity when backgrounded"

```

### Стратегии Оптимизации



```
@startuml
!theme plain

entity "Оптимизация Ассетов" as assets {
  :Атласирование Текстур;
  :Сжатие;
  :Масштабирование Разрешения;
  :Выбор Формата;
}

entity "Оптимизация Рендеринга" as rendering {
  :Батчинг;
  :Отсечение;
  :Система LOD;
  :Оптимизация Шейдеров;
}

entity "Оптимизация Анимации" as animation {
  :Пул Твинов;
  :Ускорение GPU;
  :Выборочные Обновления;
  :Масштабирование Производительности;
}

entity "Управление Памятью" as memory {
  :Стриминг Ассетов;
  :Сборка Мусора;
  :Управление Пулами;
  :Оптимизация Кэша;
}

assets --> rendering : "Оптимизированные ассеты\nвключают эффективный рендеринг"
rendering --> animation : "Эффективный рендеринг\nподдерживает плавную анимацию"
animation --> memory : "Умная анимация\nуменьшает давление памяти"
memory --> assets : "Хорошее управление памятью\nпозволяет более богатые ассеты"

note bottom : "Холистический подход к оптимизации\nБалансирует качество и производительность"
@enduml
```
