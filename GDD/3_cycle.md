# **3. Основной Игровой Цикл**

# Основной Игровой Цикл

## 3.1 Высокоуровневый Основной Цикл

### Структура Основного Цикла

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

start
:Сбор ресурсов;
note right: Ручной или через NPC
:Транспорт к зданию;
note right: Физическое перемещение
:Переработка ресурса;
note right: Время + возможное ускорение
:Продажа продукта;
note right: Получение золота
:Инвестиции в апгрейды;
note right: Здания, инструменты, NPC
if (Достаточно ресурсов?) then (да)
  :Открытие нового этапа;
  note right: Расширение цепочки
else (нет)
endif
stop

note bottom: Цикл повторяется с\\nувеличивающейся эффективностью
@enduml

```

![image.png](attachment:a175f480-3c07-464d-89ab-99e620625872:image.png)

### Loop Timing & Pacing

* **Micro Loop** : 10-30 секунд (сбор → переработка → продажа)
* **Meso Loop** : 2-5 минут (накопление → апгрейд → новая эффективность)
* **Macro Loop** : 15-60 минут (новая цепочка → освоение → оптимизация)

## 3.2 Detailed Production Flow

### Example: Wheat → Bread Chain

```
@startuml
!theme plain

rectangle "Wheat Farm" as farm {
  :Generate Wheat;
  :Base: 1 wheat/5sec;
  :Upgradeable to 5 wheat/2sec;
}

rectangle "Mill" as mill {
  :Process Wheat → Flour;
  :Input: 2 wheat;
  :Output: 1 flour;
  :Time: 8 seconds;
}

rectangle "Bakery" as bakery {
  :Process Flour → Bread;
  :Input: 1 flour;
  :Output: 1 bread;
  :Time: 12 seconds;
}

rectangle "Market" as market {
  :Sell Products;
  :Wheat: 1 gold;
  :Flour: 4 gold;
  :Bread: 15 gold;
}

farm --> mill : "Player/NPC\\nTransport"
mill --> bakery : "Player/NPC\\nTransport"
bakery --> market : "Player/NPC\\nTransport"
farm --> market : "Direct Sale\\n(Lower Profit)"
mill --> market : "Skip Bakery\\n(Medium Profit)"

note bottom of farm : "Always Available\\nNo Input Required"
note bottom of mill : "Unlocked at Level 3\\nRequires 500 gold"
note bottom of bakery : "Unlocked at Level 8\\nRequires 2000 gold"
@enduml

```

![image.png](attachment:49ea0099-b12e-4e2d-9bf5-fd1126f88000:image.png)

### Value Progression Formula

```
Base Resource Value = 1
Processed Value = Base × Processing Multiplier × Chain Depth Bonus

Examples:
- Wheat = 1 gold
- Flour = 1 × 3.5 × 1.0 = 3.5 gold
- Bread = 3.5 × 4.0 × 1.2 = 16.8 gold

```

## 3.3 Player Action Categories

### Активные Действия (Управление Игроком)

```
@startuml
!theme plain
skinparam actorStyle awesome

actor Player
usecase "Move Character" as move
usecase "Collect Resources" as collect
usecase "Transport Items" as transport
usecase "Trigger Processing" as process
usecase "Purchase Upgrades" as upgrade
usecase "Manage NPC" as manage

Player --> move : "Joystick\\nNavigation"
Player --> collect : "Proximity\\nAuto-collect"
Player --> transport : "Automatic when\\nmoving between buildings"
Player --> process : "Tap to start/\\nspeed up"
Player --> upgrade : "Tap upgrade\\nbuttons"
Player --> manage : "Assign tasks\\nto workers"

note right of move : "Always available\\nCore interaction"
note right of collect : "No tap required\\nSmooth experience"
note right of transport : "Visual feedback\\nInventory limits"
note right of process : "Optional speedup\\nTime vs engagement"
note right of upgrade : "Clear progression\\nImmediate feedback"
note right of manage : "Mid-game unlock\\nAutomation layer"
@enduml

```

![image.png](attachment:73573f4b-b59e-4705-895d-68c7a67ea68d:image.png)

### Пассивные Действия (Автоматизированные)

* **Работа NPC** : Назначенные задачи продолжаются автоматически
* **Офлайн Доходы** : Производство продолжается 2-4 часа
* **Автоматическая Переработка** : Здания работают независимо при наличии ресурсов

## 3.4 Баланс Idle и Активного Геймплея

### Game Phase Progression

```
@startuml
!theme plain

rectangle "Ранняя Игра (Дни 1-3)" as early #lightblue
rectangle "Средняя Игра (Дни 4-14)" as mid #lightgreen
rectangle "Поздняя Игра (Дни 15+)" as late #lightyellow

early : Ручной Фокус (80% Активно)
early : • Игрок делает всё
early : • Быстрая кривая обучения
early : • Немедленная обратная связь
early : • Быстрые апгрейды (30-60 сек)

mid : Гибрид (50% Активно, 50% Idle)
mid : • Первая автоматизация NPC
mid : • Оптимизация цепочек
mid : • Стратегические решения
mid : • Средние апгрейды (5-10 мин)

late : Фокус на Автоматизации (20% Активно)
late : • Большинство процессов автоматизировано
late : • Фокус на мета-прогрессии
late : • Долгосрочное планирование
late : • Крупные апгрейды (часы/дни)

early -right-> mid : "NPC Unlock\\nAutomation Introduction"
mid -right-> late : "Full Automation\\nMeta Systems"

note bottom of early : "Learning & Engagement"
note bottom of mid : "Transition & Choice"
note bottom of late : "Optimization & Scale"
@enduml

```

![image.png](attachment:3cf29149-83fa-449b-887a-ea574f4c799b:image.png)

### Принципы Баланса

1. **Активность Никогда Не Бесполезна** : Действия игрока всегда приносят пользу
2. **Idle Никогда Не Обязателен** : Игра прогрессирует без постоянного внимания
3. **Плавный Переход** : Постепенный переход от активного к idle
4. **Выбор Игрока** : Возможность оставаться активным для более быстрого прогресса

## 3.5 Система Влияния Апгрейдов

### Категории Апгрейдов и Эффекты

```
@startuml
!theme plain

package "Типы Апгрейдов" {
  [Скорость Производства] as speed
  [Вместимость Инвентаря] as capacity
  [Эффективность Переработки] as efficiency
  [Скорость Движения] as movement
  [Бонус к Цене Продажи] as price
}

package "Визуальные Эффекты" {
  [Изменения Чисел] as numbers
  [Визуальные Апгрейды] as visual
  [Скорость Анимации] as animation
  [Обратная Связь UI] as ui
}

speed --> numbers : "+50% скорость производства"
speed --> animation : "Быстрее анимации работы"

capacity --> visual : "Большие сумки/телеги"
capacity --> ui : "Расширение UI инвентаря"

efficiency --> numbers : "Более высокие коэффициенты выхода"
efficiency --> visual : "Улучшения зданий"

movement --> animation : "Быстрее движение персонажа"
movement --> ui : "Индикатор скорости"

price --> numbers : "Отображение множителя золота"
price --> visual : "Счастье торговца"

note bottom : "Каждый апгрейд ДОЛЖЕН иметь\\nвизуальное + числовое влияние"
@enduml

```

![image.png](attachment:e62ce2bc-4150-41f7-8de5-a3ea9a959b6b:image.png)

### Чеклист Валидации Апгрейда

* ✅  **Числовое Влияние** : Четкое сравнение до/после
* ✅  **Визуальное Изменение** : Что-то в мире меняется
* ✅  **Немедленный Эффект** : Преимущество видно в течение 10 секунд
* ✅  **Масштабируемая Ценность** : Преимущество растет с прогрессом игры

## 3.6 Системы Обратной Связи и Наград

### Архитектура Цикла Обратной Связи

```
@startuml
!theme plain

rectangle "Действие Игрока" as action
rectangle "Немедленная Обратная Связь" as immediate
rectangle "Обратная Связь Прогресса" as progress
rectangle "Выдача Награды" as reward
rectangle "Подсветка Следующей Цели" as next

action --> immediate : "<1 секунда"
immediate --> progress : "1-5 секунд"
progress --> reward : "5-30 секунд"
reward --> next : "Немедленно"
next --> action : "Игрок мотивирован\\nна следующее действие"

note bottom of immediate : "Визуальные эффекты\\nЗвуковая обратная связь\\nИзменения чисел"
note bottom of progress : "Полосы прогресса\\nАнимации завершения\\nИзменения зданий"
note bottom of reward : "Получение валюты\\nРазблокировки\\nНовый контент"
note bottom of next : "Четкий следующий шаг\\nДоступный апгрейд\\nИндикатор прогресса"
@enduml

```

![image.png](attachment:1168cf3e-bd1b-4049-a59a-f5ef8d7d9dad:image.png)

### Типы Наград и Время

* **Микро Награды** (каждые 10-30 сек): Золото, ресурсы, небольшие апгрейды
* **Мезо Награды** (каждые 2-5 мин): Разблокировки зданий, NPC, инструменты
* **Макро Награды** (каждые 15-60 мин): Новые цепочки, крупные апгрейды, престиж

## 3.7 Намеренные Точки Трения

### Спроектированные Узкие Места

```
@startuml
!theme plain

rectangle "Источники Трения" as friction {
  :Ограниченная Скорость Движения;
  :Вместимость Инвентаря;
  :Время Переработки;
  :Дефицит Ресурсов;
  :Стоимость Апгрейдов;
}

rectangle "Решения через Апгрейды" as solutions {
  :Апгрейды Скорости;
  :Апгрейды Сумок/Телег;
  :Апгрейды Эффективности;
  :Апгрейды Производства;
  :Автоматизация NPC;
}

rectangle "Монетизация" as money {
  :Бустеры Скорости;
  :Мгновенное Завершение;
  :Наборы Ресурсов;
  :Премиум Апгрейды;
}

friction --> solutions : "Игровая\\nПрогрессия"
friction --> money : "Опциональное\\nУскорение"

note bottom of friction : "Создает мотивацию к апгрейдам\\nПредотвращает мгновенное удовлетворение"
note bottom of solutions : "Заработано через игру\\nЧувство прогрессии"
note bottom of money : "Опциональные сокращения\\nНикогда не обязательны"
@enduml

```

![image.png](attachment:0ebd871a-e73e-45ad-bd75-eb8beb5c00d2:image.png)

### Правила Балансировки Трения

1. **Всегда Решаемо** : У каждого трения есть игровое решение
2. **Постепенное Облегчение** : Трение уменьшается, но никогда не исчезает
3. **Множественные Решения** : Апгрейды + автоматизация + монетизация
4. **Справедливая Прогрессия** : F2P игроки могут преодолеть всё трение

## 3.8 Метрики Основного Цикла

### Индикаторы Успеха

* **Время Завершения Цикла** : 15-45 секунд в среднем
* **Действий на Цикл** : 3-7 вводов игрока
* **Частота Апгрейдов** : Каждые 1-3 цикла в ранней игре
* **Рейтинг Удовлетворенности** : ≥4.0/5 для основных механик

### Цели Оптимизации

* **Корреляция с Удержанием** : Сильная корреляция между вовлеченностью в цикл и D7
* **Интеграция Монетизации** : 15-25% циклов должны представлять возможность монетизации
* **Темп Прогрессии** : 80% игроков должны завершить 10+ циклов в первой сессии
