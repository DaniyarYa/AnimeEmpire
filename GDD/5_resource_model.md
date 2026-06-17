# 5. **Модель Ресурсов и Экономики**

# Модель Ресурсов и Экономики

## 5.1 Экономическая Основа

### Основные Экономические Принципы

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

rectangle flow [
  == Поток Ресурсов ==
  --
  Первичные Ресурсы
  ↓ Переработка
  Промежуточные Продукты
  ↓ Продвинутая Переработка
  Финальные Продукты
  ↓ Продажа на Рынке
  Валюта (Золото)
]

rectangle investment [
  == Цикл Инвестиций ==
  --
  Накопление Валюты
  ↓ Стратегический Выбор
  Апгрейды Зданий
  Улучшения Инструментов
  Найм NPC
  ↓ Прирост Эффективности
  Более Высокое Производство
]

flow --> investment : Генерация Дохода
investment --> flow : Улучшенное Производство

note bottom of flow
    Ценность увеличивается в 3-5 раз\\nна каждом шаге переработки
end note
note bottom of investment
    70-80% дохода\\nреинвестируется
end note

@enduml

```

![image.png](attachment:2b2f135c-2f0c-4c15-93ed-40d83af3be39:image.png)

### Цели Экономического Баланса

```yaml
Economy_KPIs:
  reinvestment_rate: 75% # Percentage of income spent on upgrades
  progression_pacing:
    early_game: "upgrade every 30-60 seconds"
    mid_game: "upgrade every 5-10 minutes"
    late_game: "upgrade every 30+ minutes"

  value_scaling:
    processing_multiplier: 3.0-5.0
    chain_depth_bonus: 1.2-1.5

  currency_distribution:
    buildings: 60% # Primary sink
    tools: 15% # Early game focus
    npc: 20% # Mid-late game
    other: 5% # Misc upgrades

```

## 5.2 Resource Classification System

### Resource Hierarchy & Properties

```
@startuml
!theme plain

package "Первичные Ресурсы" as primary #lightblue {
  [Пшеница]
  [Дерево]
  [Камень]
  [Железная Руда]
}

package "Переработанные Ресурсы" as processed #lightgreen {
  [Мука]
  [Доски]
  [Кирпичи]
  [Железные Слитки]
}

package "Финальные Продукты" as final #lightyellow {
  [Хлеб]
  [Мебель]
  [Здания]
  [Инструменты]
}

primary --> processed : "Переработка\\n3-5x ценность"
processed --> final : "Создание\\n3-4x ценность"

note bottom of primary : "Бесконечная генерация\\nНизкая ценность\\nВысокий объем"
note bottom of processed : "Ограничено временем\\nСредняя ценность\\nСредний объем"
note bottom of final : "Сложные цепочки\\nВысокая ценность\\nНизкий объем"
@enduml

```

![image.png](attachment:ab805057-ed26-4593-a224-8b9fb491a175:image.png)

### Прогрессия Ценности Ресурсов

```yaml
Resource_Values:
  wheat_chain:
    wheat: 1 gold
    flour: 3 gold (3x multiplier)
    bread: 12 gold (4x multiplier)

  wood_chain:
    wood: 1 gold
    planks: 4 gold (4x multiplier)
    furniture: 18 gold (4.5x multiplier)

  stone_chain:
    stone: 2 gold
    bricks: 8 gold (4x multiplier)
    buildings: 35 gold (4.4x multiplier)

  iron_chain:
    iron_ore: 3 gold
    iron_bars: 15 gold (5x multiplier)
    tools: 75 gold (5x multiplier)

```

## 5.3 Архитектура Валютной Системы

### Модель Двойной Валюты

```
@startuml
!theme plain

entity "Мягкая Валюта (Золото)" as soft {
  :Основная игровая валюта;
  :Зарабатывается через геймплей;
  :Используется для основной прогрессии;
  :Неограниченный потенциал заработка;
}

entity "Твердая Валюта (Камни)" as hard {
  :Премиум валюта;
  :Ограниченные бесплатные источники;
  :Ускорение и удобство;
  :IAP основной источник;
}

entity "Игровые Системы" as systems {
  :Апгрейды Зданий;
  :Покупки Инструментов;
  :Найм NPC;
  :Апгрейды Престижа;
}

entity "Системы Монетизации" as monetization {
  :Пропуски Времени;
  :Мгновенные Завершения;
  :Премиум Бустеры;
  :Эксклюзивный Контент;
}

soft --> systems : "Основная прогрессия\\nВсегда доступна"
hard --> monetization : "Опциональное ускорение\\nФункции удобства"
hard --> systems : "Альтернативный путь\\nБыстрее прогрессия"

note bottom of soft : "Источники: Продажи, офлайн доходы,\\nдостижения, события"
note bottom of hard : "Источники: IAP, награждаемая реклама,\\nспециальные события, достижения"
@enduml

```

![image.png](attachment:838bd222-c6a9-46b7-bac7-222cad4affda:image.png)

### Источники и Стоки Валюты

```yaml
Gold_Economy:
  sources:
    product_sales: 85% # Primary income
    offline_earnings: 10% # Retention mechanic
    achievements: 3% # Milestone rewards
    events: 2% # LiveOps integration

  sinks:
    building_upgrades: 60% # Core progression
    npc_hiring: 20% # Automation
    tool_upgrades: 15% # Active enhancement
    misc_upgrades: 5% # QoL improvements

Gem_Economy:
  sources:
    iap: 70% # Primary monetization
    rewarded_ads: 20% # F2P path
    achievements: 7% # Milestone rewards
    events: 3% # Special occasions

  sinks:
    time_skips: 40% # Impatience monetization
    boosters: 30% # Efficiency enhancement
    premium_content: 20% # Exclusive access
    convenience: 10% # QoL features

```

## 5.4 Масштабирование Ценности Производства

### Формула Умножения Ценности

```
Processed_Value = Base_Value × Processing_Multiplier × Chain_Depth_Bonus × Quality_Modifier

Where:
- Processing_Multiplier: 3.0-5.0 (depends on processing complexity)
- Chain_Depth_Bonus: 1.0 + (0.2 × chain_position)
- Quality_Modifier: 1.0-1.5 (building level dependent)

```

### Примеры Расчетов

```yaml
Bread_Production_Example:
  wheat_base: 1 gold

  flour_calculation:
    base_value: 1
    processing_multiplier: 3.5
    chain_depth_bonus: 1.0
    quality_modifier: 1.1 (level 5 mill)
    result: 1 × 3.5 × 1.0 × 1.1 = 3.85 gold

  bread_calculation:
    base_value: 3.85
    processing_multiplier: 4.0
    chain_depth_bonus: 1.2
    quality_modifier: 1.2 (level 8 bakery)
    result: 3.85 × 4.0 × 1.2 × 1.2 = 22.2 gold

```

## 5.5 Масштабирование Стоимости Апгрейдов

### Динамическая Прогрессия Стоимости

```
@startuml
!theme plain

rectangle phases [
  Фазы Масштабирования Стоимости
  --
  Ранняя Игра (Уровни 1-10)
  Темп Роста: 1.12x
  Быстрая прогрессия
  --
  Средняя Игра (Уровни 11-20)
  Темп Роста: 1.15x
  Умеренный темп
  --
  Поздняя Игра (Уровни 21+)
  Темп Роста: 1.18x
  Долгосрочные цели
]

rectangle formula [
  Формула Стоимости

  Базовая Стоимость × (Темп Роста ^ Уровень)
  --
  Мягкие Потолки на:
  • Уровень 25 (здания)
  • Уровень 20 (инструменты)
  • Уровень 15 (персонаж)
]

phases --> formula
@enduml
```

![image.png](attachment:5d9f603d-1b9c-4876-b043-1a2a3bc04797:image.png)

### Примеры Стоимости Апгрейдов

```yaml
Building_Costs:
  mill_upgrades:
    level_1: 500 gold
    level_5: 500 × (1.12^4) = 787 gold
    level_10: 500 × (1.15^5) = 1,005 gold (rate change at level 11)
    level_15: 1,005 × (1.15^5) = 2,021 gold
    level_20: 2,021 × (1.18^5) = 4,618 gold
    level_25: 4,618 × (1.18^5) = 10,554 gold

Tool_Costs:
  scythe_line:
    basic_sickle: 100 gold
    iron_sickle: 100 × (1.12^3) = 141 gold
    steel_scythe: 141 × (1.15^3) = 215 gold
    master_scythe: 215 × (1.18^3) = 354 gold

```

## 5.6 Время как Экономический Ресурс

### Механики на Основе Времени

```
@startuml
!theme plain

entity "Инвестиции Времени" as time {
  Длительность Переработки
  Строительство Апгрейдов
  Обучение NPC
  Офлайн Накопление
}

entity "Ускорение Времени" as accel {
  Активное Участие Игрока
  Апгрейды Эффективности
  Премиум Бустеры
  Системы Автоматизации
}

entity "Интеграция Монетизации" as money {
  Мгновенное Завершение [Камни]
  Бустеры Скорости [Реклама/IAP]
  Офлайн Множители
  Премиум Автоматизация
}

time --> accel : "Игровые решения"
time --> money : "Решения монетизации"
accel --> money : "Премиум улучшение"

note bottom of time : "Основная точка трения\nСоздает мотивацию к апгрейдам"
note bottom of accel : "Заработанная прогрессия\nНаграды за навыки и инвестиции"
note bottom of money : "Опциональные сокращения\nМонетизация удобства"
@enduml
```

![image.png](attachment:f72ec64b-1175-4e2f-b55d-f3451503f35e:image.png)

### Формула Экономики Времени

```yaml
Time_Systems:
  processing_times:
    base_formula: "base_time × (0.95 ^ building_level)"
    wheat_to_flour: "10 seconds base → 5 seconds at level 10"
    flour_to_bread: "15 seconds base → 7 seconds at level 10"

  offline_earnings:
    max_duration: "4 hours"
    efficiency_rate: "50% of active production"
    formula: "min(offline_hours, 4) × production_rate × 0.5"

  acceleration_options:
    player_presence: "100% efficiency"
    premium_booster: "200% efficiency for 30 minutes"
    instant_complete: "gems = time_remaining_minutes × 2"

```

## 5.7 Фреймворк Экономического Баланса

### Баланс Источников и Стоков

```
@startuml
!theme plain

entity "Источники Дохода" as income {
  Продажи Продуктов [85%]
  Офлайн Доходы [10%]
  Достижения [3%]
  События [2%]
}

entity "Стоки Расходов" as sinks {
  Апгрейды Зданий [60%]
  Системы NPC [20%]
  Инструменты и Оборудование [15%]
  Разное [5%]
}

entity "Механизмы Баланса" as balance {
  Динамическое Ценообразование
  Ограничения Апгрейдов
  Мягкие Потолки
  Сбросы Престижа
}

income --> sinks : Поток дохода
sinks --> balance : Контроль расходов
balance --> income : Экономическая корректировка

note bottom : Цель: 75-80% коэффициент реинвестирования\nВсегда есть доступный следующий шаг
@enduml
```

![image.png](attachment:c24627f1-bf3c-426e-aae5-d70605287d0a:image.png)

### Метрики Здоровья Экономики

```yaml
Balance_KPIs:
  player_satisfaction:
    upgrade_accessibility: ">80% players have affordable upgrade"
    progression_feeling: ">4.0/5 satisfaction rating"
    economic_fairness: "<10% complaints about costs"

  monetization_health:
    f2p_progression: "All content accessible without payment"
    premium_value: "Clear time savings for paying players"
    whale_protection: "Spending caps and warnings"

  retention_correlation:
    economic_engagement: "Strong correlation with D7 retention"
    upgrade_frequency: "Optimal 2-5 minute gaps between upgrades"
    long_term_goals: "Visible progress toward major milestones"

```

## 5.8 Системы Экономической Безопасности

### Меры Против Инфляции

```
@startuml
!theme plain

entity "Риски Инфляции" as risks {
  Экспоненциальный Рост
  Переполнение Чисел
  Путаница Игроков
  Нарушение Баланса
}

entity "Системы Безопасности" as safety {
  Сбросы Престижа
  Мягкие Потолки
  Логарифмическое Масштабирование
  Мета-Прогрессия
}

entity "Инструменты Мониторинга" as monitoring {
  Экономическая Аналитика
  Отслеживание Прогрессии Игроков
  Автоматические Оповещения
  Фреймворк A/B Тестирования
}

risks --> safety : Системы предотвращения
safety --> monitoring : Валидация и корректировка
monitoring --> risks : Раннее обнаружение

note bottom : Поддерживать экономическую стабильность\nПредотвращать неконтролируемую инфляцию\nОбеспечивать долгосрочную устойчивость
@enduml
```

![image.png](attachment:8b1f7500-1951-4fae-94bd-6511016fee2e:image.png)

### Экономические Ограничения


Safety_Rules:
  hard_constraints:
    no_paywalls: "All content accessible through gameplay"
    no_mandatory_iap: "Premium only accelerates, never gates"
    fair_f2p: "Free players can achieve 90% of paying player efficiency"

  soft_constraints:
    upgrade_pacing: "Maintain 2-5 minute upgrade intervals"
    resource_diversity: "All resources have minimum 2 uses"
    economic_transparency: "Clear cost/benefit for all purchases"

  monitoring_thresholds:
    inflation_rate: "<20% cost increase per week"
    progression_stalls: "<5% of players stuck >24 hours"
    economic_complaints: "<2% of reviews mention unfair costs"
