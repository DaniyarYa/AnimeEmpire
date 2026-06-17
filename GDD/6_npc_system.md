# Система Персонажей и NPC

## 6.1 Обзор Системы Персонажей

### Архитектура Экосистемы Персонажей

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

actor "Персонаж Игрока" as player #lightblue
rectangle "NPC Работники" as workers #lightgreen
rectangle "NPC Специалисты" as specialists #lightyellow
rectangle "NPC Менеджеры" as managers #lightcoral

player --> workers : "Нанимает и назначает\\nзадачи"
workers --> specialists : "Разблокировка через\\nпрогрессию зданий"
specialists --> managers : "Разблокировка через\\nсистему престижа"
managers --> player : "Предоставляют глобальные\\nбонусы"

note bottom of player : "Всегда активен\\nНикогда не устаревает\\nЛичная прогрессия"
note bottom of workers : "Базовая автоматизация\\nСпецифичные задачи\\nФокус средней игры"
note bottom of specialists : "Продвинутые бонусы\\nСпецифичные для зданий\\nРазблокировка поздней игры"
note bottom of managers : "Системные эффекты\\nОграниченное количество\\nКонтент эндгейма"
@enduml

```

![image.png](attachment:09012454-f91e-4ae1-90e7-23b4b391e67e:image.png)

### Философия Дизайна Персонажей

* **Визуальная Идентичность** : Аниме/Chibi стиль для геймплея, Hi-res для событий
* **Функциональное Назначение** : Каждый персонаж решает специфичное игровое трение
* **Интеграция Прогрессии** : Персонажи улучшают, никогда не заменяют основные механики
* **Эмоциональная Связь** : Визуальная привлекательность создает привязанность, не нарратив

## 6.2 Система Персонажа Игрока

### Роли Персонажа Игрока и Эволюция

```
@startuml
!theme plain

entity "Роль Ранней Игры" as early {
  Основной Работник
  Сборщик Ресурсов
  Ручной Перевозчик
  Прямой Контроллер
}

entity "Роль Средней Игры" as mid {
  Менеджер NPC
  Оптимизатор Эффективности
  Стратегический Планировщик
  Активный Ускоритель
}

entity "Роль Поздней Игры" as late {
  Надзиратель Системы
  Драйвер Мета-Прогрессии
  Принимающий Решения о Престиже
  Эксперт по Оптимизации
}

early --> mid : "Разблокировка NPC\nВведение автоматизации"
mid --> late : "Полная автоматизация\nМета-системы"

note bottom of early : "Практический геймплей\nИзучение механик\nНемедленная обратная связь"
note bottom of mid : "Навыки делегирования\nСтратегическое мышление\nФокус на эффективности"
note bottom of late : "Решения высокого уровня\nДолгосрочное планирование\nМастерство системы"
@enduml
```

![image.png](attachment:521ca82d-a5dc-4315-b65b-511358b71528:image.png)

### Характеристики Персонажа Игрока и Прогрессия

```yaml
Player_Character:
  base_stats:
    movement_speed: 100 # Base units per second
    carry_capacity: 1 # Number of items
    harvest_efficiency: 100 # Percentage of base rate
    sale_bonus: 0 # Percentage bonus to sale prices
    npc_slots: 0 # Number of manageable NPCs

  progression_formula:
    movement_speed: "base × (1 + 0.08 × level)" # Max +160% at level 20
    carry_capacity: "base + floor(level / 3)" # Max +10 at level 30
    harvest_efficiency: "base × (1 + 0.04 × level)" # Max +100% at level 25
    sale_bonus: "level × 1.5" # Max +22.5% at level 15
    npc_slots: "floor(level / 5)" # Max 6 slots at level 30

  upgrade_costs:
    base_cost: [500, 300, 400, 1000, 2000] # Per stat type
    growth_rate: [1.2, 1.15, 1.18, 1.25, 1.3] # Per stat type
    max_levels: [20, 30, 25, 15, 6] # Per stat type

```

### Визуальная Прогрессия Персонажа Игрока

```yaml
Visual_Upgrades:
  movement_upgrades:
    level_5: "Better boots animation"
    level_10: "Speed trail effects"
    level_15: "Upgraded character model"
    level_20: "Teleportation effects"

  capacity_upgrades:
    level_3: "Small bag equipped"
    level_9: "Large backpack"
    level_18: "Magical storage effects"
    level_30: "Dimensional storage aura"

  efficiency_upgrades:
    level_5: "Tool improvements"
    level_15: "Skill effect particles"
    level_25: "Master craftsman aura"

```

## 6.3 Система NPC Работников

### Категории NPC и Специализации

```
@startuml
!theme plain

package "NPC Сборщики" {
  entity "Фермер" as farmer {
    Специализация: Пшеница;
    Эффективность: 70-90%;
    Стоимость: 1000-3000г;
    Разблокировка: Ферма Уровень 5;
  }
  
  entity "Лесоруб" as lumber {
    Специализация: Дерево;
    Эффективность: 75-95%;
    Стоимость: 1200-3500г;
    Разблокировка: Лес Уровень 3;
  }
  
  entity "Шахтер" as miner {
    Специализация: Камень/Железо;
    Эффективность: 65-85%;
    Стоимость: 1500-4000г;
    Разблокировка: Шахта Уровень 4;
  }
}

package "NPC Перевозчики" {
  entity "Носильщик" as porter {
    Вместимость: 3-8 предметов;
    Скорость: 80-110% от игрока;
    Стоимость: 800-2500г;
    Разблокировка: Склад Уровень 2;
  }
  
  entity "Торговец" as merchant {
    Вместимость: 5-12 предметов;
    Скорость: 100-140% от игрока;
    Бонус: +10% к ценам продажи;
    Стоимость: 2000-6000г;
    Разблокировка: Рынок Уровень 5;
  }
}

farmer --> porter : Автоматизация\nпотока ресурсов
lumber --> porter
miner --> merchant : Премиум перевозка\nдля ценных товаров
@enduml
```

![image.png](attachment:7698d247-455d-412b-833f-c3d76a2c6ee1:image.png)

### Система Назначения и Управления NPC

```yaml
NPC_Management:
  assignment_rules:
    one_task_per_npc: true
    reassignment_cost: "25% of hire cost"
    task_specificity: "Farmer #1 → Wheat Farm #2"

  work_patterns:
    active_hours: "When player online + 2 hours offline"
    efficiency_variance: "±5% random variation per task"
    breakdown_chance: "1% per hour, requires 50g repair"

  upgrade_system:
    stat_improvements:
      work_speed: "base × (1 + 0.12 × level)"
      capacity: "base + (level × 0.4)"
      efficiency: "min(98%, base + (level × 1.5%))"
      reliability: "max(99%, base + (level × 2%))"

    cost_scaling: "hire_cost × (1.3 ^ level)"
    max_level: 10

  visual_progression:
    level_3: "Better tools/equipment"
    level_6: "Improved character model"
    level_10: "Master-tier appearance + effects"

```

### Прогрессия Разблокировки NPC

```
@startuml
!theme plain

start
:Game Start;
:Manual Labor Only;
note right: Days 1-2
:First Building Level 5;
:Unlock First Gatherer NPC;
note right: Day 2-3
:Warehouse Level 2;
:Unlock Porter NPC;
note right: Day 3-4
:Multiple Buildings Level 8;
:Unlock Specialist NPCs;
note right: Day 5-7
:Market Level 10;
:Unlock Merchant NPCs;
note right: Day 7-10
:First Prestige;
:Unlock Manager NPCs;
note right: Day 14+
stop

note bottom: "Gradual automation introduction\\nPlayer always remains relevant"
@enduml

```

![image.png](attachment:46623045-e7dd-4d5a-b1fb-51cb73b6a03a:image.png)

## 6.4 Система NPC Специалистов

### Специалисты для Конкретных Зданий

```yaml
Specialist_NPCs:
  mill_specialist:
    name: "Master Miller"
    bonus: "+25% mill processing speed"
    cost: 5000 gold
    unlock: "Mill Level 10"
    rarity: "Rare"

  bakery_specialist:
    name: "Expert Baker"
    bonus: "+30% bread quality (higher sale price)"
    cost: 8000 gold
    unlock: "Bakery Level 8"
    rarity: "Epic"

  farm_specialist:
    name: "Agricultural Expert"
    bonus: "+20% all farm production"
    cost: 6000 gold
    unlock: "3 Farms at Level 5+"
    rarity: "Rare"

  market_specialist:
    name: "Trade Master"
    bonus: "+15% all sale prices"
    cost: 12000 gold
    unlock: "Market Level 15"
    rarity: "Legendary"

```

### Методы Получения Специалистов

```
@startuml
!theme plain

entity "Прямая Покупка" as purchase {
  Стоимость Золотом
  Требования Зданий
  Всегда Доступно
  Гарантированное Получение
}

entity "Награды за Достижения" as achievement {
  Завершение Вех
  Бесплатное Получение
  Только Один Раз
  Редкие Специалисты
}

entity "Награды Событий" as events {
  Ограниченное Время
  Специальные Вызовы
  Эксклюзивные Специалисты
  Уникальные Бонусы
}

entity "Разблокировки Престижа" as prestige {
  Мета-Прогрессия
  Постоянные Разблокировки
  Мощные Бонусы
  Контент Эндгейма
}

purchase --> achievement : Прогресс вех
achievement --> events : Усиление вовлеченности
events --> prestige : Долгосрочные цели
prestige --> purchase : Улучшенные опции

note bottom : Множественные пути получения\nНет pay-to-win специалистов\nВсе доступны через геймплей
@enduml
```

![image.png](attachment:4ccea1a4-6179-4bae-bb61-635dfc12d23c:image.png)

## 6.5 Система NPC Менеджеров (Эндгейм)

### Менеджеры с Глобальными Эффектами

```yaml
Manager_NPCs:
  production_manager:
    name: "Production Overseer"
    effect: "+10% global production speed"
    cost: 25000 gold + 100 prestige points
    limit: 1 total
    unlock: "First Prestige"

  efficiency_manager:
    name: "Efficiency Expert"
    effect: "+15% all NPC efficiency"
    cost: 35000 gold + 150 prestige points
    limit: 1 total
    unlock: "Second Prestige"

  sales_manager:
    name: "Commerce Director"
    effect: "+12% all sale prices"
    cost: 50000 gold + 200 prestige points
    limit: 1 total
    unlock: "Third Prestige"

  automation_manager:
    name: "System Administrator"
    effect: "NPCs work 4 hours offline (instead of 2)"
    cost: 75000 gold + 300 prestige points
    limit: 1 total
    unlock: "Fifth Prestige"

```

### Система Прогрессии Менеджеров

```
@startuml
!theme plain

entity "Найм Менеджера" as hire {
  Требование Престижа
  Золото + Очки Престижа
  Ограниченные Слоты [3 всего]
  Постоянная Инвестиция
}

entity "Обучение Менеджера" as training {
  Дополнительные Инвестиции Золота
  Распределение Очков Навыков
  Пути Специализации
  Инкрементальные Бонусы
}

entity "Синергия Менеджеров" as synergy {
  Бонусы Множественных Менеджеров
  Совокупные Эффекты
  Стратегические Комбинации
  Оптимизация Эндгейма
}

hire --> training : Развитие после\nнайма
training --> synergy : Взаимодействия\nмножественных менеджеров
synergy --> hire : Мотивация для\nследующего менеджера

note bottom : Глубокая система эндгейма\nСтратегическое принятие решений\nДолгосрочная инвестиция
@enduml
```

![image.png](attachment:6bd999b7-8329-459f-83d0-7affcef38db0:image.png)

## 6.6 Стратегия Визуального Представления

### Система Двойного Художественного Стиля

```
@startuml
!theme plain

entity "Представление в Игре" as ingame {
  Chibi/Low-Poly Стиль
  Оптимизированная Производительность
  Четкая Идентификация Роли
  Плавные Анимации
  Читаемость в Малом Размере
}

entity "Представление UI/Событий" as ui {
  Высококачественное Аниме Искусство
  Детальные Портреты Персонажей
  Эмоциональное Выражение
  Премиум Визуальное Качество
  Маркетинговая Привлекательность
}

entity "Технические Преимущества" as tech {
  Эффективность Ресурсов
  Масштабируемый Контент
  Оптимизация Производительности
  Повторное Использование Арт-Ассетов
}

entity "Преимущества для Игрока" as player {
  Визуальное Разнообразие
  Эмоциональная Связь
  Премиум Ощущение
  Привлекательность Коллекции
}

ingame --> tech : "Оптимизация\nгеймплея"
ui --> player : "Эмоциональное\nвовлечение"
tech --> player : "Устойчивая\nразработка"

note bottom : "Лучшее из обоих миров\nПроизводительность + Премиум ощущение"
@enduml
```

![image.png](attachment:8d7f4342-4f48-449e-9ea7-746d40af9e0d:image.png)

### Визуальная Прогрессия Персонажей

```yaml
Visual_Design_Rules:
  ingame_models:
    polygon_budget: "500-1000 triangles per character"
    texture_resolution: "256x256 maximum"
    animation_frames: "8-12 per action"
    color_coding: "Role identification through colors"

  ui_portraits:
    resolution: "1024x1024 minimum"
    art_style: "High-quality anime illustration"
    emotional_range: "Happy, focused, excited states"
    seasonal_variants: "Special event versions"

  progression_indicators:
    level_badges: "Visible rank indicators"
    equipment_changes: "Visual tool/clothing upgrades"
    effect_particles: "Skill level indicators"
    rarity_borders: "Common/Rare/Epic/Legendary frames"

```

## 6.7 Получение и Коллекция Персонажей

### Дизайн Потока Получения

```
@startuml
!theme plain

start
:Building Progression;
if (Building Level Threshold?) then (yes)
  :NPC Unlock Available;
  :Gold Cost Payment;
  :Character Acquired;
  if (Rare Character?) then (yes)
    :Special Unlock Animation;
    :Collection Achievement;
  else (no)
    :Standard Acquisition;
  endif
else (no)
  :Continue Building Progression;
endif
:Character Assignment;
:Immediate Gameplay Benefit;
stop

@enduml
```

![image.png](attachment:f0105240-d2ea-498c-9913-70fdba9434d6:image.png)

### Механики Коллекции (Опциональный Слой)

```yaml
Collection_System:
  rarity_distribution:
    common: 60% # Basic workers
    rare: 25% # Specialists
    epic: 12% # Advanced specialists
    legendary: 3% # Managers

  collection_benefits:
    completion_bonuses: "+5% per rarity tier completed"
    showcase_features: "Character gallery with stats"
    achievement_integration: "Collection milestones"

  acquisition_methods:
    guaranteed_progression: 80% # Building unlocks
    achievement_rewards: 15% # Milestone completion
    event_exclusives: 5% # Limited time content

  design_constraints:
    no_gacha_mechanics: true
    no_duplicate_characters: true
    all_characters_obtainable: true
    no_pay_exclusive_characters: true

```

## 6.8 Метрики Системы Персонажей

### Индикаторы Успеха

```yaml
Character_KPIs:
  engagement_metrics:
    npc_utilization_rate: ">85% of hired NPCs actively used"
    character_progression_satisfaction: ">4.2/5 rating"
    visual_appeal_rating: ">4.5/5 for character design"

  progression_metrics:
    first_npc_hire_time: "Day 2-3 average"
    specialist_unlock_rate: ">70% reach first specialist"
    manager_acquisition_rate: ">40% reach first manager"

  monetization_integration:
    character_related_purchases: "15-25% of total IAP"
    visual_upgrade_conversion: ">8% purchase rate"
    collection_completion_drive: "Strong correlation with retention"

  retention_correlation:
    character_attachment: "Strong correlation with D30 retention"
    progression_satisfaction: "Character progress drives session length"
    automation_appreciation: "NPC systems reduce churn"

```
