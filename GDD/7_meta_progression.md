# 7. Система Мета-Прогрессии

# Система Мета-Прогрессии

## 7.1 Философия Мета-Прогрессии

### Цель и Цели Дизайна

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

entity "Цели Мета-Прогрессии" as goals {
  :Долгосрочное Удержание;
  :Контент Эндгейма;
  :Ценность Сброса Прогрессии;
  :Выражение Навыков;
}

entity "Преимущества для Игрока" as benefits {
  :Свежий Старт с Преимуществами;
  :Более Глубокие Стратегические Выборы;
  :Постоянная Прогрессия;
  :Признание Мастерства;
}

entity "Преимущества для Игры" as game {
  :Расширенный LTV;
  :Удержание За D30;
  :Возможности Монетизации;
  :Масштабируемость Контента;
}

goals --> benefits : "Улучшение опыта\\nигрока"
benefits --> game : "Создание бизнес\\nценности"
game --> goals : "Устойчивый\\nдизайн цикл"

note bottom : "Мета-прогрессия никогда не заменяет основной геймплей\\nОна улучшает и ускоряет знакомые системы"
@enduml

```

![image.png](attachment:6110ffcf-17d5-48d6-83c9-6d231435236a:image.png)

### Основные Принципы

* **Добровольный Сброс** : Престиж всегда выбор игрока, никогда не принуждается
* **Значимое Преимущество** : Каждый сброс предоставляет четкие, постоянные преимущества
* **Ускоренное Повторение** : Последующие прохождения быстрее, не просто легче
* **Ворота Нового Контента** : Мета-прогрессия разблокирует действительно новый опыт
* **Признание Навыков** : Система награждает знания и оптимизацию игрока

## 7.2 Архитектура Системы Престижа

### Условия Запуска Престижа

```
@startuml
!theme plain

start
:Reach Income Threshold;
note right: 100K gold/hour baseline
if (All Core Buildings Level 15+?) then (yes)
  if (At Least 5 Production Chains?) then (yes)
    if (3+ Manager NPCs Hired?) then (yes)
      :Prestige Available;
      :Calculate Prestige Points;
      note right: Based on total progress
      :Player Choice: Prestige Now?;
      if (Player Confirms?) then (yes)
        :Execute Prestige Reset;
        :Grant Prestige Points;
        :Apply Meta Bonuses;
        :Start Enhanced Run;
      else (no)
        :Continue Current Run;
        :Accumulate More Progress;
      endif
    else (no)
      :Continue Building NPCs;
    endif
  else (no)
    :Expand Production Chains;
  endif
else (no)
  :Upgrade Buildings;
endif
stop
@enduml
```

![image.png](attachment:b80e29af-331d-459d-898e-01df9d36a136:image.png)

### Расчет Очков Престижа

```yaml
Prestige_Points_Formula:
  base_calculation:
    income_component: "sqrt(max_income_per_hour / 100000)" # Square root scaling
    building_component: "sum(building_levels) / 100"
    chain_component: "completed_chains × 0.5"
    time_component: "max(0, (14 - days_played) × 0.1)" # Bonus for faster prestige

  total_formula: |
    prestige_points = floor(
      income_component +
      building_component +
      chain_component +
      time_component
    )

  example_calculations:
    first_prestige: # Day 3, 150K income, 8 buildings avg level 12, 3 chains
      income: "sqrt(150000/100000) = 1.22"
      buildings: "(8 × 12) / 100 = 0.96"
      chains: "3 × 0.5 = 1.5"
      time: "(14 - 3) × 0.1 = 1.1"
      total: "floor(4.78) = 4 prestige points"

    third_prestige: # Day 10, 500K income, 12 buildings avg level 18, 6 chains
      income: "sqrt(500000/100000) = 2.24"
      buildings: "(12 × 18) / 100 = 2.16"
      chains: "6 × 0.5 = 3.0"
      time: "(14 - 10) × 0.1 = 0.4"
      total: "floor(7.8) = 7 prestige points"

```

### Механики Сброса и Удержания

```
@startuml
!theme plain

package "Что Сбрасывается" as resets #lightcoral {
  [Уровни Зданий] --> [Сброс до Уровня 1]
  [NPC Работники] --> [Оставить по 1 каждого типа]
  [Текущие Ресурсы] --> [Сброс до 0]
  [Апгрейды Инструментов] --> [Оставить высший уровень]
  [Производственные Цепочки] --> [Сброс прогресса, сохранить разблокировки]
}

package "Что Сохраняется" as persists #lightgreen {
  [Очки Престижа] --> [Накопление Навсегда]
  [Мета-Апгрейды] --> [Постоянные Бонусы]
  [Коллекция Персонажей] --> [Все NPC Разблокированы]
  [Прогресс Достижений] --> [Никогда Не Теряется]
  [Визуальные Разблокировки] --> [Косметика Сохраняется]
}

package "Улучшенный Старт" as enhanced #lightyellow {
  [Стартовые Бонусы] --> [На Основе Мета-Апгрейдов]
  [Быстрее Прогрессия] --> [Сниженные Стоимости/Времена]
  [Доступ к Новому Контенту] --> [Ранее Заблокированные Области]
  [Продвинутые Стратегии] --> [Знания Игрока]
}

resets --> enhanced : "Свежий старт с\\nизученными оптимизациями"
persists --> enhanced : "Постоянные преимущества\\nиз предыдущих прохождений"

note bottom : "Баланс: Значимый сброс против Постоянного прогресса"
@enduml

```

![image.png](attachment:73267a65-0a0b-4b8a-ad62-d5737bbc9ee7:image.png)

## 7.3 Мета-Валюта и Апгрейды

### Экономика Очков Престижа

```yaml
Prestige_Economy:
  earning_sources:
    prestige_reset: 85% # Primary source
    special_achievements: 10% # Milestone bonuses
    limited_events: 5% # Seasonal content

  spending_categories:
    global_bonuses: 60% # Income, speed, efficiency multipliers
    starting_advantages: 25% # Better initial state
    new_content_unlocks: 10% # Areas, chains, NPCs
    quality_of_life: 5% # Convenience features

  point_scarcity:
    first_10_points: "Easy to earn and spend"
    points_11_50: "Moderate progression"
    points_51_100: "Long-term goals"
    points_100+: "Hardcore player territory"

```

### Категории Мета-Апгрейдов

```
@startuml
!theme plain

package "Глобальные Множители" as global {
  entity "Усиление Дохода" as income {
    :+10% за уровень;
    :Макс 10 уровней;
    :Стоимость: 1,2,3,5,8,13,21,34,55,89 очков;
  }
  
  entity "Усиление Скорости" as speed {
    :+8% скорости переработки за уровень;
    :Макс 8 уровней;
    :Стоимость: 2,3,5,8,13,21,34,55 очков;
  }
  
  entity "Усиление Эффективности" as efficiency {
    :+5% эффективности NPC за уровень;
    :Макс 12 уровней;
    :Стоимость: 1,1,2,3,5,8,13,21,34,55,89,144 очков;
  }
}

package "Стартовые Преимущества" as starting {
  entity "Начальное Золото" as gold {
    :Начать с 1К,5К,25К,100К золота;
    :4 уровня доступно;
    :Стоимость: 3,8,20,50 очков;
  }
  
  entity "Бесплатные Здания" as buildings {
    :Начать с построенными базовыми зданиями;
    :Пропустить раннее строительство;
    :Стоимость: 5,15,35 очков;
  }
  
  entity "Слоты NPC" as slots {
    :+1 слот NPC за уровень;
    :Макс 3 дополнительных слота;
    :Стоимость: 10,25,50 очков;
  }
}

package "Разблокировки Контента" as content {
  entity "Новые Области" as areas {
    :Горный Регион;
    :Прибрежный Регион;
    :Пустынный Регион;
    :Стоимость: 15,30,45 очков;
  }
  
  entity "Продвинутые Цепочки" as chains {
    :Роскошные Товары;
    :Магические Предметы;
    :Технологии;
    :Стоимость: 20,35,50 очков;
  }
}

global --> starting : "Множители усиливают\nстартовые преимущества"
starting --> content : "Сильная основа\nвключает новый контент"
content --> global : "Новый контент предоставляет\nбольше ценности множителей"

note bottom : "Взаимосвязанные пути апгрейдов\nМножественные жизнеспособные стратегии"
@enduml
```

![image.png](attachment:231d8e02-4778-4d8b-953c-fe80ec663962:image.png)

### Примеры Прогрессии Мета-Апгрейдов

```yaml
Meta_Upgrade_Examples:
  income_multiplier:
    level_1: {cost: 1, effect: "+10% all income", total_bonus: "10%"}
    level_5: {cost: 8, effect: "+10% all income", total_bonus: "50%"}
    level_10: {cost: 89, effect: "+10% all income", total_bonus: "100%"}

  starting_gold:
    tier_1: {cost: 3, effect: "Start with 1,000 gold"}
    tier_2: {cost: 8, effect: "Start with 5,000 gold"}
    tier_3: {cost: 20, effect: "Start with 25,000 gold"}
    tier_4: {cost: 50, effect: "Start with 100,000 gold"}

  new_area_mountain:
    unlock: {cost: 15, effect: "Access to Mountain Region"}
    benefits:
      - "New resource: Gems"
      - "Advanced building: Crystal Mine"
      - "Unique NPC: Mountain Dwarf"
      - "Higher value production chains"

```

## 7.4 Темп Престижа и Прогрессия

### Дизайн Временной Шкалы Престижа

```
@startuml
!theme plain

rectangle "Первый Престиж" as first {
  :Цель: День 3-5;
  :Доход: 100-200К/час;
  :Очков Заработано: 3-6;
  :Длительность: 2-4 дня;
  :Фокус: Изучение системы;
}

rectangle "Второй Престиж" as second {
  :Цель: День 7-10;
  :Доход: 300-600К/час;
  :Очков Заработано: 5-9;
  :Длительность: 3-5 дней;
  :Фокус: Оптимизация;
}

rectangle "Третий Престиж" as third {
  :Цель: День 12-18;
  :Доход: 800К-1.5М/час;
  :Очков Заработано: 7-12;
  :Длительность: 4-7 дней;
  :Фокус: Новый контент;
}

rectangle "Продолжающиеся Престижи" as ongoing {
  :Цель: Каждые 1-2 недели;
  :Доход: Экспоненциальный рост;
  :Очков Заработано: 10-20+;
  :Длительность: 7-14 дней;
  :Фокус: Мастерство и коллекция;
}

first --> second : "Понимание системы\\nБазовые мета-апгрейды"
second --> third : "Стратегические выборы\\nРазблокировки контента"
third --> ongoing : "Долгосрочная прогрессия\\nКонтент эндгейма"

note bottom : "Каждый цикл престижа должен ощущаться\\nбыстрее и более награждающим, чем предыдущий"
@enduml

```

![image.png](attachment:aeeba0fa-da68-432c-a331-c6595644150d:image.png)

### Механики Ускорения

```yaml
Prestige_Acceleration:
  run_speed_factors:
    meta_bonuses: "10-100% faster progression"
    player_knowledge: "Optimal build orders and strategies"
    starting_advantages: "Skip early game bottlenecks"
    unlocked_content: "More efficient production chains"

  progression_comparison:
    first_run: "3-5 days to prestige readiness"
    second_run: "2-3 days with 20-30% meta bonuses"
    third_run: "1.5-2.5 days with 50-70% meta bonuses"
    fifth_run: "1-2 days with 100%+ meta bonuses"

  engagement_maintenance:
    new_challenges: "Higher tier content unlocks"
    optimization_puzzles: "Efficient point spending strategies"
    collection_goals: "Rare NPCs and achievements"
    competitive_elements: "Leaderboards for prestige speed"

```

## 7.5 Ограничение Доступа к Новому Контенту

### Прогрессия Разблокировки Контента

```
@startuml
!theme plain

start
:Base Game Content;
note right: Available from start
:First Prestige;
:Unlock: Advanced Building Tiers;
note right: Buildings can reach level 30
:Third Prestige;
:Unlock: Mountain Region;
note right: New resources and chains
:Fifth Prestige;
:Unlock: Coastal Region;
note right: Maritime trade mechanics
:Tenth Prestige;
:Unlock: Desert Region;
note right: Rare resource extraction
:Fifteenth Prestige;
:Unlock: Magical Content;
note right: Fantasy production chains
:Twentieth Prestige;
:Unlock: Technology Era;
note right: Industrial revolution content
stop

@enduml

```

![image.png](attachment:468b2f1c-e010-47f4-a0c9-626a92ca8f6b:image.png)

### Дизайн Регионального Контента

```yaml
Regional_Content:
  mountain_region: # Prestige 3 unlock
    new_resources: ["Gems", "Rare Metals", "Crystal"]
    new_buildings: ["Crystal Mine", "Gem Cutter", "Enchanter"]
    new_npcs: ["Mountain Dwarf", "Crystal Sage"]
    production_bonus: "25% higher value chains"
    unlock_cost: 15 prestige points

  coastal_region: # Prestige 5 unlock
    new_resources: ["Fish", "Pearls", "Salt"]
    new_buildings: ["Harbor", "Shipyard", "Trading Post"]
    new_npcs: ["Sea Captain", "Pearl Diver"]
    special_mechanic: "Maritime trade routes"
    unlock_cost: 25 prestige points

  desert_region: # Prestige 10 unlock
    new_resources: ["Spices", "Oil", "Ancient Artifacts"]
    new_buildings: ["Oasis", "Spice Market", "Archaeological Site"]
    new_npcs: ["Desert Nomad", "Archaeologist"]
    special_mechanic: "Caravan trading system"
    unlock_cost: 40 prestige points

  magical_realm: # Prestige 15 unlock
    new_resources: ["Mana", "Magical Herbs", "Enchanted Items"]
    new_buildings: ["Wizard Tower", "Alchemy Lab", "Portal"]
    new_npcs: ["Wizard", "Alchemist", "Magical Familiar"]
    special_mechanic: "Spell crafting and enchantments"
    unlock_cost: 60 prestige points

```

## 7.6 Баланс Мета-Прогрессии

### Управление Кривой Прогрессии

```
@startuml
!theme plain

entity "Ранняя Мета-Игра" as early {
  :Престижи 1-3;
  :Фокус: Изучение Системы;
  :Заработок Очков: 3-8 за престиж;
  :Фокус Апгрейдов: Глобальные множители;
  :Контент: Оптимизация базовой игры;
}

entity "Средняя Мета-Игра" as mid {
  :Престижи 4-10;
  :Фокус: Разблокировки Контента;
  :Заработок Очков: 8-15 за престиж;
  :Фокус Апгрейдов: Новые регионы;
  :Контент: Исследование регионов;
}

entity "Поздняя Мета-Игра" as late {
  :Престижи 11-20;
  :Фокус: Мастерство и Коллекция;
  :Заработок Очков: 15-25 за престиж;
  :Фокус Апгрейдов: Специализация;
  :Контент: Продвинутые механики;
}

entity "Мета-Эндгейм" as endgame {
  :Престижи 21+;
  :Фокус: Соревнование и Совершенство;
  :Заработок Очков: 25+ за престиж;
  :Фокус Апгрейдов: Мин-максинг;
  :Контент: Таблицы лидеров и события;
}

early --> mid : "Мастерство системы\nГолод по контенту"
mid --> late : "Завершение регионов\nФокус на оптимизации"
late --> endgame : "Завершение коллекции\nСоревновательный драйв"

note bottom : "Каждая фаза предлагает разные\nмотивации и награды"
@enduml
```

![image.png](attachment:de98244b-bbb7-451f-844a-de18952ddbd5:image.png)

### Механизмы Безопасности Баланса

```yaml
Meta_Balance_Safety:
  power_scaling_limits:
    max_global_multiplier: "200% (3x total effectiveness)"
    diminishing_returns: "Each upgrade less impactful than previous"
    soft_caps: "Exponential cost increases after certain thresholds"

  progression_gates:
    content_unlock_requirements: "Multiple prestiges + point investment"
    achievement_prerequisites: "Skill demonstration required"
    time_gates: "Some content requires multiple prestige cycles"

  player_choice_preservation:
    multiple_viable_paths: "Income vs. Speed vs. Content focus"
    respec_options: "Limited ability to reallocate points"
    experimentation_encouragement: "Low-cost early upgrades"

  retention_protection:
    never_punish_prestige: "Each reset always beneficial"
    clear_progress_indicators: "Visible advancement toward goals"
    meaningful_milestones: "Regular achievement recognition"

```

## 7.7 Метрики Мета-Прогрессии

### Индикаторы Успеха


```yaml
Meta_Progression_KPIs:
  engagement_metrics:
    first_prestige_rate: ">60% of D7 players"
    multiple_prestige_rate: ">35% of D14 players"
    long_term_prestige_rate: ">15% of D30 players"

  progression_satisfaction:
    prestige_decision_confidence: ">4.0/5 rating"
    meta_upgrade_value_perception: ">4.2/5 rating"
    content_unlock_excitement: ">4.5/5 rating"

  retention_impact:
    d30_retention_boost: "+25% vs. non-prestige players"
    d60_retention_maintenance: ">8% absolute retention"
    long_term_engagement: "Strong correlation with LTV"

  monetization_integration:
    prestige_related_purchases: "20-30% of total IAP"
    meta_progression_acceleration: "Popular monetization vector"
    content_unlock_motivation: "Drives engagement and spending"

  balance_validation:
    prestige_timing_distribution: "80% within target windows"
    upgrade_path_diversity: "No single dominant strategy"
    content_unlock_progression: "Steady advancement through regions"

```
