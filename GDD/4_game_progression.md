# **4. Система Прогрессии**

# Система Прогрессии

## 4.1 Обзор Многослойной Прогрессии

### Архитектура Прогрессии

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

package "Слои Прогрессии" {
  rectangle "Здания" as buildings #lightblue
  rectangle "Инструменты" as tools #lightgreen
  rectangle "Персонаж Игрока" as player #lightyellow
  rectangle "NPC Работники" as npc #lightcoral
  rectangle "Мета-Прогрессия" as meta #lightgray
}

buildings --> tools : "Разблокирует лучшие\\nинструменты для эффективности"
tools --> player : "Игрок использует инструменты\\nдля ручной работы"
player --> npc : "Игрок обучает/управляет\\nNPC работниками"
npc --> buildings : "NPC управляют\\nпродвинутыми зданиями"
buildings --> meta : "Вехи зданий\\nзапускают престиж"
meta --> buildings : "Мета-бонусы усиливают\\nэффективность всех зданий"

note bottom of buildings : "Основной сток валюты\\nДрайвер основной прогрессии"
note bottom of tools : "Улучшение активного геймплея\\nФокус ранней-средней игры"
note bottom of player : "Всегда актуально\\nЛичная прогрессия"
note bottom npc : "Слой автоматизации\\nФокус средней-поздней игры"
note bottom of meta : "Долгосрочное удержание\\nКонтент эндгейма"
@enduml

```

![image.png](attachment:ddf2d728-9f65-4091-af66-dba7eae6e6c7:image.png)

### Фреймворк Временных Рамок Прогрессии

```yaml
Progression_Timeline:
  early_game: # Days 1-3
    focus: [buildings, tools, player]
    upgrade_frequency: "30-60 seconds"
    cost_scaling: "1.12x per level"

  mid_game: # Days 4-14
    focus: [buildings, npc, player]
    upgrade_frequency: "5-10 minutes"
    cost_scaling: "1.15x per level"

  late_game: # Days 15+
    focus: [meta, npc, buildings]
    upgrade_frequency: "30+ minutes"
    cost_scaling: "1.18x per level"

```

## 4.2 Система Прогрессии Зданий

### Категории Зданий и Последовательность Разблокировки

```
@startuml
!theme plain

rectangle "Генераторы Ресурсов" as generators {
  :Пшеничная Ферма (Уровень 1);
  :Лес (Уровень 5);
  :Каменоломня (Уровень 12);
  :Железный Рудник (Уровень 25);
}

rectangle "Здания Переработки" as processors {
  :Мельница (Уровень 3);
  :Пекарня (Уровень 8);
  :Лесопилка (Уровень 10);
  :Кузница (Уровень 20);
}

rectangle "Сервисные Здания" as services {
  :Рынок (Уровень 1);
  :Склад (Уровень 6);
  :Мастерская (Уровень 15);
  :Гильдия (Уровень 30);
}

generators --> processors : "Сырье\\nпоступает на переработку"
processors --> services : "Готовые товары\\nидут на рынок"
services --> generators : "Прибыль финансирует\\nновые генераторы"

note bottom of generators : "Всегда производят\\nНе требуют ввода"
note bottom of processors : "Преобразуют ресурсы\\nУвеличивают ценность в 3-5 раз"
note bottom of services : "Вспомогательные системы\\nКачество жизни"
@enduml

```

![image.png](attachment:40c7f4fd-01ea-4e6c-8058-c9249e798aa6:image.png)

### Параметры Апгрейда Зданий

```yaml
Building_Stats:
  upgrade_parameters:
    - production_speed: "Base rate × (1 + 0.3 × level)"
    - input_capacity: "Base capacity × (1 + 0.2 × level)"
    - output_multiplier: "1.0 + (0.1 × level)"
    - processing_time: "Base time × (0.95 ^ level)"

  cost_formula: "base_cost × (1.15 ^ level)"

  visual_upgrades:
    level_5: "Building size increase"
    level_10: "New architectural elements"
    level_15: "Particle effects"
    level_20: "Unique building skin"

```

### Пример: Прогрессия Мельницы

| Level | Cost    | Speed       | Capacity | Output  | Time |
| ----- | ------- | ----------- | -------- | ------- | ---- |
| 1     | 500g    | 1 flour/10s | 5 wheat  | 1 flour | 10s  |
| 5     | 1,200g  | 2 flour/7s  | 8 wheat  | 1 flour | 7s   |
| 10    | 3,800g  | 3 flour/5s  | 12 wheat | 2 flour | 5s   |
| 15    | 12,000g | 4 flour/3s  | 18 wheat | 2 flour | 3s   |
| 20    | 38,000g | 6 flour/2s  | 25 wheat | 3 flour | 2s   |

## 4.3 Система Прогрессии Инструментов

### Категории Инструментов и Эффекты

```
@startuml
!theme plain

package "Инструменты Сбора" {
  [Серп] --> [Коса] --> [Механический Комбайн]
  note bottom: "Скорость сбора пшеницы\\n+25% → +75% → +150%"
}

package "Транспортные Инструменты" {
  [Руки] --> [Сумка] --> [Телега] --> [Повозка]
  note bottom: "Вместимость\\n1 → 3 → 8 → 15 предметов"
}

package "Инструменты Переработки" {
  [Базовые Инструменты] --> [Качественные Инструменты] --> [Мастерские Инструменты]
  note bottom: "Бонус скорости переработки\\n+0% → +20% → +50%"
}

package "Инструменты Движения" {
  [Ходьба] --> [Сапоги] --> [Лошадь] --> [Телепортер]
  note bottom: "Скорость движения\\n100% → 125% → 175% → 300%"
}

note bottom: "Инструменты улучшают активный геймплей\\nНаиболее актуальны в ранней-средней игре"
@enduml

```

![image.png](attachment:5ba230f0-08ab-4d16-bea5-a7b0c9214708:image.png)

### Экономика Апгрейдов Инструментов

```yaml
Tool_Progression:
  sickle_line:
    sickle: {cost: 100, effect: "+25% harvest speed"}
    scythe: {cost: 800, effect: "+75% harvest speed"}
    harvester: {cost: 5000, effect: "+150% harvest speed"}

  bag_line:
    bag: {cost: 200, effect: "3 item capacity"}
    cart: {cost: 1500, effect: "8 item capacity"}
    wagon: {cost: 8000, effect: "15 item capacity"}

  upgrade_timing:
    first_tool: "Day 1, within 10 minutes"
    second_tier: "Day 2-3"
    final_tier: "Day 7-14"

```

## 4.4 Прогрессия Персонажа Игрока

### Характеристики Персонажа и Масштабирование

```
@startuml
!theme plain

rectangle "Характеристики Игрока" as stats {
  :Скорость Движения;
  :Базовая Вместимость;
  :Эффективность Сбора;
  :Бонус к Цене Продажи;
  :Слоты Управления NPC;
}

rectangle "Источники Апгрейдов" as sources {
  :Прямые Инвестиции Золота;
  :Награды за Достижения;
  :Бонусы Престижа;
  :Специальные События;
}

rectangle "Влияние на Геймплей" as impact {
  :Быстрее Основные Циклы;
  :Выше Активная Эффективность;
  :Лучший Контроль Автоматизации;
  :Увеличенная Прибыль;
}

stats <-- sources
stats --> impact

note bottom of stats : "Всегда актуально\\nМасштабируется на протяжении игры"
note bottom of sources : "Множественные пути прогрессии\\nВыбор игрока в фокусе"
note bottom of impact : "Измеримые преимущества\\nНемедленная обратная связь"
@enduml

```

![image.png](attachment:872cdd72-6955-475d-ad8d-0feef9f72551:image.png)

### Формула Прогрессии Персонажа

```yaml
Character_Stats:
  movement_speed:
    base: 100
    formula: "base × (1 + 0.1 × level)"
    max_level: 20
    cost_per_level: "500 × (1.2 ^ level)"

  carry_capacity:
    base: 1
    formula: "base + floor(level / 3)"
    max_level: 30
    cost_per_level: "300 × (1.15 ^ level)"

  harvest_efficiency:
    base: 100
    formula: "base × (1 + 0.05 × level)"
    max_level: 25
    cost_per_level: "400 × (1.18 ^ level)"

  price_bonus:
    base: 0
    formula: "level × 2" # +2% per level
    max_level: 15
    cost_per_level: "1000 × (1.25 ^ level)"

```

## 4.5 Система NPC Работников

### Типы NPC и Специализации

```
@startuml
!theme plain

rectangle "NPC Сборщики" as gatherers {
  :Фермер (Пшеница);
  :Лесоруб (Дерево);
  :Шахтер (Камень/Железо);
  :Эффективность: 70-95%;
  :Стоимость: 1000-5000г;
}

rectangle "NPC Перевозчики" as carriers {
  :Носильщик (Базовая Перевозка);
  :Торговец (Быстрая Перевозка);
  :Вместимость: 3-10 предметов;
  :Скорость: 80-120% от игрока;
  :Стоимость: 800-3000г;
}

rectangle "NPC Операторы" as operators {
  :Мельник (Работа Мельницы);
  :Пекарь (Работа Пекарни);
  :Бонус: +10-30% эффективности;
  :Надежность: 85-98%;
  :Стоимость: 2000-8000г;
}

rectangle "NPC Менеджеры" as managers {
  :Менеджер Производства;
  :Менеджер Продаж;
  :Глобальный Бонус: +5-15%;
  :Ограниченные Слоты: 1-3 всего;
  :Стоимость: 10000-50000г;
}

gatherers --> carriers : "Автоматизированный\\nпоток ресурсов"
carriers --> operators : "Оптимизация\\nцепочки поставок"
operators --> managers : "Системная\\nэффективность"

note bottom : "Последовательность разблокировки NPC:\\nСборщики → Перевозчики → Операторы → Менеджеры"
@enduml

```

![image.png](attachment:97be1fce-d90e-496c-a10a-c61df07e96bc:image.png)

### Прогрессия и Управление NPC

```yaml
NPC_System:
  unlock_sequence:
    first_npc: "Building level 5"
    carrier_npc: "Building level 8"
    operator_npc: "Building level 12"
    manager_npc: "Prestige 1"

  upgrade_stats:
    work_speed: "base × (1 + 0.15 × level)"
    capacity: "base + (level × 0.5)"
    efficiency: "min(98%, base + (level × 2%))"

  assignment_rules:
    - one_npc_per_task: true
    - reassignment_cost: "50% of hire cost"
    - offline_work_time: "2 hours maximum"
    - reliability_factor: "efficiency × uptime"

```

## 4.6 Дизайн Фаз Прогрессии

### Прогрессия Ранней Игры (Дни 1-3)

```
@startuml
!theme plain

start
:Tutorial Completion;
:First Building Upgrade;
:Tool Purchase;
:Second Production Chain;
:First NPC Hire;
:Automation Understanding;
stop

note right: "Fast progression\\nConstant rewards\\nLearning focus"
@enduml

```

![image.png](attachment:cf3b7e75-4580-4d4f-b498-27237bb40656:image.png)

**Characteristics:**

* Upgrade every 30-60 seconds
* Clear progression path
* High dopamine feedback
* Minimal complexity

### Прогрессия Средней Игры (Дни 4-14)

**Области Фокуса:**

* Множественные производственные цепочки
* Настройка автоматизации NPC
* Оптимизация эффективности
* Стратегический выбор между апгрейдами

**Темп Прогрессии:**

* Крупные апгрейды каждые 5-10 минут
* Множественные жизнеспособные пути
* Управление ресурсами становится важным
* Подготовка к первому престижу

### Прогрессия Поздней Игры (Дни 15+)

**Области Фокуса:**

* Системы мета-прогрессии
* Глубокая оптимизация
* Циклы престижа
* Долгосрочные цели

**Темп Прогрессии:**

* Крупные апгрейды каждые 30+ минут
* Сложное принятие решений
* Множественные циклы престижа
* Разблокировки контента эндгейма

## 4.7 Мягкие Потолки и Баланс Масштабирования

### Фреймворк Экономического Масштабирования

```yaml
Scaling_Rules:
  cost_growth:
    early_game: "1.12x per level"
    mid_game: "1.15x per level"
    late_game: "1.18x per level"

  benefit_growth:
    linear_stats: "additive bonuses"
    multiplicative_stats: "diminishing returns after level 10"

  soft_caps:
    building_levels: 25 (before prestige)
    npc_count: 8 (before manager unlock)
    tool_tiers: 4 (before meta tools)

  balance_targets:
    upgrade_frequency: "maintain 2-5 minute gaps"
    progress_feeling: "always something affordable"
    long_term_goals: "visible but distant"

```

### Метрики Валидации Прогрессии

* **Доступность Апгрейдов** : 80% игроков должны иметь доступный апгрейд
* **Удовлетворенность Прогрессом** : ≥4.0/5 рейтинг за ощущение прогрессии
* **Корреляция с Удержанием** : Сильная корреляция между вовлеченностью в прогрессию и удержанием
* **Интеграция Монетизации** : 20-30% точек прогрессии должны предлагать платное ускорение
