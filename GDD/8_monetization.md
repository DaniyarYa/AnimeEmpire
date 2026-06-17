# 8. **Стратегия Монетизации**

# Стратегия Монетизации

## 8.1 Философия и Этика Монетизации

### Основные Принципы Монетизации

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

rectangle "Справедливая Монетизация" as fair {
  :Ускорение, Не Блокировка;
  :Опциональное Удобство;
  :Прозрачная Ценность;
  :Уважение к Времени Игрока;
}

rectangle "Преимущества для Игрока" as benefits {
  :Экономия Времени;
  :Улучшенный Опыт;
  :Эксклюзивная Косметика;
  :Премиум Удобство;
}

rectangle "Устойчивость Бизнеса" as business {
  :Долгосрочный LTV;
  :Положительные Отзывы;
  :Рост через Сарафанное Радио;
  :Устойчивый Доход;
}

fair --> benefits : "Этичный дизайн\\nсоздает ценность"
benefits --> business : "Счастливые игроки\\nтратят больше"
business --> fair : "Успех позволяет\\nсправедливые практики"

note bottom : "Монетизация улучшает геймплей\\nНикогда не заменяет и не блокирует его"
@enduml

```

![image.png](attachment:31ca2b1e-c5b3-4364-8d39-b128a384d062:image.png)

### Фреймворк Этики Монетизации

```yaml
Ethical_Guidelines:
  mandatory_principles:
    no_paywalls: "All content accessible through gameplay"
    no_p2w_mechanics: "Paying players get convenience, not power"
    transparent_pricing: "Clear value proposition for all purchases"
    respect_f2p: "Free players are valued community members"

  prohibited_practices:
    forced_ads: "No mandatory advertisement viewing"
    predatory_pricing: "No exploitative pricing strategies"
    gambling_mechanics: "No loot boxes or gacha systems"
    progress_blocking: "No artificial barriers requiring payment"

  encouraged_practices:
    value_demonstration: "Show clear benefits before purchase"
    spending_limits: "Protect players from overspending"
    f2p_respect: "Ensure free players have great experience"
    community_building: "Foster positive player relationships"

```

## 8.2 Архитектура Потоков Дохода

### Основные Каналы Монетизации

```
@startuml
!theme plain

package "Награждаемая Реклама" as ads #lightblue {
  rectangle "2x Офлайн Доход" as offline
  rectangle "Мгновенное Завершение" as instant
  rectangle "Бесплатные Бустеры" as boosters
  rectangle "Временные NPC" as temp_npc
}

package "Внутриигровые Покупки" as iap #lightgreen {
  rectangle "Стартовые Наборы" as starter
  rectangle "Твердая Валюта" as gems
  rectangle "Удаление Рекламы" as remove_ads
  rectangle "Премиум Пропуски" as passes
}

package "Ограниченные Предложения" as offers #lightyellow {
  rectangle "Вехи Прогресса" as milestones
  rectangle "Сезонные События" as seasonal
  rectangle "Персонализированные Сделки" as personalized
}

package "Модель Подписки" as subscription #lightcoral {
  rectangle "Премиум Членство" as premium
  rectangle "Боевой Пропуск" as battlepass
  rectangle "VIP Преимущества" as vip
}

ads --> iap : "Воронка конверсии\\nРеклама в IAP"
iap --> offers : "Поведение покупок\\nзапускает предложения"
offers --> subscription : "Высокоценные игроки\\nпереходят на подписку"

note bottom : "Множественные потоки дохода\\nСнижают риск зависимости\\nУчитывают разные типы игроков"
@enduml

```

![image.png](attachment:572faffd-09a2-48b4-b6c5-cd61d4e9749e:image.png)

### Цели Распределения Дохода

```yaml
Revenue_Targets:
  channel_distribution:
    rewarded_ads: 45% # Primary revenue driver
    starter_packs: 20% # High conversion, low value
    hard_currency: 15% # Core IAP system
    limited_offers: 12% # High-value moments
    subscriptions: 5% # Premium players
    remove_ads: 3% # Quality of life

  player_segment_contribution:
    whales_1_percent: 40% # High spenders
    dolphins_9_percent: 35% # Medium spenders
    minnows_40_percent: 20% # Light spenders
    f2p_50_percent: 5% # Ad revenue only

  timeline_targets:
    month_1: "$0.08 ARPDAU"
    month_3: "$0.12 ARPDAU"
    month_6: "$0.15 ARPDAU"
    month_12: "$0.18 ARPDAU"

```

## 8.3 Стратегия Награждаемой Рекламы

### Дизайн Интеграции Рекламы

```
@startuml
!theme plain

start
:Игрок Встречает Время Ожидания;
note right: Переработка, офлайн возврат, и т.д.
:Система Предлагает Опцию Рекламы;
note right: "Посмотреть рекламу для удвоения/пропуска?"
if (Игрок Принимает?) then (да)
  :Показать Награждаемое Видео;
  :Выдать Обещанное Преимущество;
  :Отследить Вовлеченность;
  :Запустить Таймер Перезарядки;
else (нет)
  :Продолжить Обычный Геймплей;
  :Штраф Не Применен;
endif
:Возобновить Основной Геймплей;
stop

@enduml

```

![image.png](attachment:04cf13e9-d693-48f1-89a5-649ef83576b8:image.png)

> Реклама решает проблемы игроков.
> Никогда не прерывает основной геймплей.
> Всегда опциональна и полезна

### Сценарии Награждаемой Рекламы

```yaml
Ad_Integration_Points:
  offline_income_doubler:
    trigger: "Player returns after 30+ minutes offline"
    offer: "Watch ad to double offline earnings"
    cooldown: "4 hours"
    value_proposition: "Significant income boost"

  instant_processing:
    trigger: "Processing time >30 seconds remaining"
    offer: "Watch ad to complete instantly"
    cooldown: "15 minutes"
    value_proposition: "Immediate gratification"

  free_booster:
    trigger: "Player hasn't used booster in 2+ hours"
    offer: "Watch ad for 30-minute speed boost"
    cooldown: "2 hours"
    value_proposition: "Enhanced efficiency"

  temporary_npc:
    trigger: "Player struggling with manual tasks"
    offer: "Watch ad for 1-hour helper NPC"
    cooldown: "6 hours"
    value_proposition: "Automation preview"

  extra_prestige_points:
    trigger: "Player about to prestige"
    offer: "Watch ad for +1 prestige point"
    cooldown: "Once per prestige"
    value_proposition: "Meta progression boost"

```

### Оптимизация Опыта Рекламы

```yaml
Ad_UX_Guidelines:
  presentation_standards:
    clear_value_proposition: "Exactly what player gets"
    no_surprise_ads: "Player always chooses to watch"
    immediate_reward: "Benefit delivered instantly"
    visual_confirmation: "Clear feedback on reward"

  frequency_management:
    max_ads_per_session: 5
    min_time_between_ads: "3 minutes"
    daily_ad_limit: 20
    respect_player_decline: "Don't re-offer immediately"

  technical_requirements:
    ad_loading_time: "<3 seconds"
    fallback_ads: "Always have backup ad ready"
    reward_guarantee: "Reward even if ad fails"
    skip_option: "Allow skip after 5 seconds for video ads"

```

## 8.4 Стратегия Внутриигровых Покупок

### Дизайн Стартового Набора

```
@startuml
!theme plain

entity "Содержимое Стартового Набора" as contents {
  :500 Камней (Премиум Валюта);
  :5,000 Золота (Мягкая Валюта);
  :Эксклюзивный Помощник NPC;
  :3x Бустер Скорости (30 мин);
  :Специальная Кожа Здания;
}

entity "Стратегия Ценообразования" as pricing {
  :Цена: $2.99;
  :Ценность: $8+ если куплено отдельно;
  :Одноразовая Покупка;
  :Доступно Первые 7 Дней;
}

entity "Тактики Конверсии" as conversion {
  :Выдающееся Размещение UI;
  :Четкая Коммуникация Ценности;
  :Давление Ограниченного Времени;
  :Интеграция в Туториал;
}

contents --> pricing : "Высокая воспринимаемая ценность\nНизкий ценовой барьер"
pricing --> conversion : "Привлекательное предложение\nСоздает срочность"
conversion --> contents : "Ценное содержимое\nОправдывает покупку"

note bottom : "Первая покупка критична\nЛомает барьер трат\nСтроит доверие к ценности"
@enduml
```

![image.png](attachment:ce33b799-e58a-4aed-9adc-856896051549:image.png)

### Экономика Твердой Валюты (Камни)

```yaml
Gem_Packages:
  small_pack:
    gems: 100
    price: "$0.99"
    bonus: "0% bonus gems"
    target: "Impulse purchases"

  medium_pack:
    gems: 550
    price: "$4.99"
    bonus: "10% bonus gems (50 extra)"
    target: "Regular spenders"

  large_pack:
    gems: 1200
    price: "$9.99"
    bonus: "20% bonus gems (200 extra)"
    target: "Committed players"

  mega_pack:
    gems: 2750
    price: "$19.99"
    bonus: "37.5% bonus gems (500 extra)"
    target: "High-value players"

Gem_Usage_Distribution:
  time_acceleration: 40% # Skip waiting times
  premium_boosters: 25% # Enhanced efficiency
  npc_upgrades: 15% # Automation enhancement
  cosmetic_items: 10% # Visual customization
  emergency_resources: 5% # Solve bottlenecks
  meta_progression: 5% # Prestige acceleration

```

### Система Ограниченных по Времени Предложений

```
@startuml
!theme plain

entity "События Запуска" as triggers {
  :Веха Здания;
  :Достижение Престижа;
  :Обнаружение Застревания;
  :Возврат после Отсутствия;
  :Сезонные События;
}

entity "Генерация Предложения" as generation {
  :Анализ Прогресса Игрока;
  :Идентификация Текущих Потребностей;
  :Расчет Оптимальной Ценности;
  :Создание Персонализированного Предложения;
}

entity "Представление Предложения" as presentation {
  :UI Ограниченного Времени;
  :Четкое Предложение Ценности;
  :Таймер Обратного Отсчета;
  :Одноразовая Покупка;
}

triggers --> generation : "Поведение игрока\nуправляет временем предложения"
generation --> presentation : "Персонализированная ценность\nмаксимизирует конверсию"
presentation --> triggers : "Поведение покупок\nинформирует будущие предложения"

note bottom : "Правильное предложение, правильное время, правильная цена\nПерсонализация движет конверсией"
@enduml
```

![image.png](attachment:fc2bd5f6-a778-4cfb-8dc1-569d66675d6d:image.png)

## 8.5 Подписка и Премиум Сервисы

### Дизайн Премиум Членства

```yaml
Premium_Membership:
  monthly_subscription:
    price: "$4.99/month"
    benefits:
      - "2x offline earnings (permanent)"
      - "50% faster processing times"
      - "Daily gem allowance (50 gems)"
      - "Exclusive premium NPCs"
      - "Priority customer support"
      - "Ad-free experience"

  value_calculation:
    equivalent_gem_value: "$8-10/month"
    time_savings: "Significant gameplay acceleration"
    exclusive_content: "Premium NPCs and cosmetics"
    convenience: "No ads, faster progression"

  target_audience:
    committed_players: "D14+ with high engagement"
    previous_purchasers: "Players who bought starter pack"
    high_session_frequency: "3+ sessions per day"

  conversion_strategy:
    free_trial: "3-day trial period"
    clear_benefits: "Demonstrate value immediately"
    easy_cancellation: "Build trust with transparency"
    retention_focus: "Deliver consistent value"

```

### Система Боевого Пропуска (После Запуска)

```
@startuml
!theme plain

entity "Бесплатный Трек" as free {
  :20 Уровней Наград;
  :Базовые Ресурсы;
  :Обычные NPC;
  :Стандартная Косметика;
  :Доступно Всем Игрокам;
}

entity "Премиум Трек" as premium {
  :20 Дополнительных Уровней;
  :Редкие Ресурсы;
  :Эксклюзивные NPC;
  :Премиум Косметика;
  :$9.99 Разблокировка;
}

entity "Система Прогрессии" as progression {
  :Ежедневные Вызовы;
  :Еженедельные Цели;
  :Сезонные События;
  :Продвижение на Основе XP;
}

free --> progression : "Вовлеченность\nдвижет прогрессией"
premium --> progression : "Улучшенные награды\nза те же активности"
progression --> free : "Регулярный контент\nдля всех игроков"
progression --> premium : "Премиум ценность\nдля платящих игроков"

note bottom : "Боевой Пропуск вводится только после\nсильных метрик удержания и вовлеченности"
@enduml
```

![image.png](attachment:f7876d5f-d30e-4146-add2-74b2813054a6:image.png)

## 8.6 Интеграция Монетизации с Основными Системами

### Выравнивание Прогрессии-Монетизации

```
@startuml
!theme plain

package "Основная Прогрессия" as core {
  [Апгрейды Зданий] as buildings
  [Автоматизация NPC] as npcs
  [Мета-Прогрессия] as meta
  [Разблокировки Контента] as content
}

package "Точки Контакта Монетизации" as monetization {
  [Ускорение Времени] as time
  [Бустеры Ресурсов] as resources
  [Премиум Контент] as premium
  [Функции Удобства] as convenience
}

buildings --> time : "Пропустить таймеры апгрейдов"
buildings --> resources : "Усилить эффективность зданий"
npcs --> premium : "Эксклюзивные варианты NPC"
npcs --> convenience : "Улучшенная автоматизация"
meta --> time : "Ускорить циклы престижа"
meta --> premium : "Эксклюзивные мета-апгрейды"
content --> premium : "Ранний доступ к регионам"
content --> convenience : "Пропустить требования разблокировки"

note bottom : "Монетизация улучшает каждую систему\\nБез замены основной прогрессии"
@enduml

```

![image.png](attachment:f4ca0ad4-2158-4de1-9366-f0da78596956:image.png)

### Точки Трения Монетизации

```yaml
Designed_Friction_Points:
  processing_wait_times:
    friction: "10-60 second processing delays"
    f2p_solution: "Wait or use earned boosters"
    monetization: "Instant completion with gems"

  offline_earnings_cap:
    friction: "4-hour maximum offline accumulation"
    f2p_solution: "Return regularly to collect"
    monetization: "Extended offline time with premium"

  inventory_limitations:
    friction: "Limited carrying capacity"
    f2p_solution: "Upgrade bags with gold"
    monetization: "Premium bags with gems"

  npc_efficiency:
    friction: "NPCs work at 70-90% player efficiency"
    f2p_solution: "Upgrade NPCs with gold"
    monetization: "Premium NPCs with higher base efficiency"

  prestige_timing:
    friction: "Optimal prestige timing requires planning"
    f2p_solution: "Learn through experience"
    monetization: "Prestige calculators and guides"

```

## 8.7 Сегментация Игроков и Персонализация

### Сегменты Поведения Трат

```
@startuml
!theme plain

rectangle "F2P Игроки (50%)" as f2p {
  :$0 пожизненные траты;
  :Только доход от рекламы;
  :Высокий потенциал вовлеченности;
  :Цель конверсии;
}

rectangle "Мелкие Рыбки (40%)" as minnows {
  :$1-10 пожизненные траты;
  :Покупатели стартовых наборов;
  :Периодические покупки камней;
  :Возможности допродажи;
}

rectangle "Дельфины (9%)" as dolphins {
  :$10-100 пожизненные траты;
  :Регулярные покупатели;
  :Кандидаты на подписку;
  :Фокус на удержании;
}

rectangle "Киты (1%)" as whales {
  :$100+ пожизненные траты;
  :Фокус на премиум сервисе;
  :Доступ к эксклюзивному контенту;
  :VIP обращение;
}

f2p --> minnows : "Конверсия\\nстартового набора"
minnows --> dolphins : "Обновление\\nподписки"
dolphins --> whales : "Вовлеченность\\nпремиум контентом"

note bottom : "Разные сегменты требуют\\nразных подходов к монетизации"
@enduml

```

![image.png](attachment:df39ee31-3d33-42b9-a3c8-a7db53a39a26:image.png)

### Персонализированная Монетизация

```yaml
Personalization_Strategy:
  f2p_players:
    focus: "Ad engagement and starter pack conversion"
    offers: "High-value starter packs, ad bonuses"
    messaging: "Try premium features risk-free"

  minnows:
    focus: "Increase purchase frequency"
    offers: "Small gem packs, limited-time deals"
    messaging: "Enhance your favorite activities"

  dolphins:
    focus: "Subscription conversion and retention"
    offers: "Premium membership, battle pass"
    messaging: "Unlock your full potential"

  whales:
    focus: "VIP experience and exclusive content"
    offers: "Exclusive NPCs, early access, cosmetics"
    messaging: "Premium experience for dedicated players"

  behavioral_triggers:
    stuck_players: "Progress acceleration offers"
    returning_players: "Welcome back bonuses"
    achievement_hunters: "Collection completion packs"
    efficiency_optimizers: "Premium automation tools"

```

## 8.8 Метрики Монетизации и Оптимизация

### Ключевые Показатели Эффективности

```yaml
Monetization_KPIs:
  revenue_metrics:
    arpdau: "$0.08 → $0.18 (12-month target)"
    arpu: "$2.50 → $8.00 (12-month target)"
    ltv: "$15 → $45 (12-month target)"

  conversion_metrics:
    f2p_to_paying: "8% → 12% (target)"
    starter_pack_conversion: "15% → 25% (target)"
    subscription_conversion: "3% → 8% (of paying players)"

  engagement_metrics:
    ad_engagement_rate: "25% → 35% (target)"
    purchase_satisfaction: "4.2/5 → 4.5/5 (target)"
    monetization_complaints: "<2% of reviews"

  retention_correlation:
    paying_player_d30: "45% → 60% (target)"
    subscription_player_d60: "70% → 80% (target)"
    whale_player_d90: "80% → 90% (target)"

```

### Фреймворк A/B Тестирования

```
@startuml
!theme plain

entity "Категории Тестов" as categories {
  :Оптимизация Ценообразования;
  :Время Предложений;
  :Представление UI;
  :Коммуникация Ценности;
}

entity "Реализация Тестов" as implementation {
  :Сегментация Игроков;
  :Статистическая Значимость;
  :Влияние на Доход;
  :Удовлетворенность Игроков;
}

entity "Цикл Оптимизации" as cycle {
  :Формирование Гипотез;
  :Дизайн Тестов;
  :Сбор Данных;
  :Анализ и Решение;
}

categories --> implementation : "Систематическое тестирование\nэлементов монетизации"
implementation --> cycle : "Процесс оптимизации\nна основе данных"
cycle --> categories : "Инсайты информируют\nновые категории тестов"

note bottom : "Непрерывная оптимизация\nНа основе поведения игроков и данных о доходах"
@enduml
```

![image.png](attachment:77d9579b-2b2a-4982-be14-d8df3c3a5980:image.png)

### Мониторинг Безопасности и Этики Монетизации


Safety_Monitoring:
  spending_protection:
    daily_spending_alerts: "Warn at $50+ daily spend"
    weekly_spending_caps: "Suggest breaks at $200+ weekly"
    parental_controls: "Age-appropriate spending limits"

  player_satisfaction:
    monetization_feedback: "Regular surveys on purchase satisfaction"
    complaint_monitoring: "Track monetization-related complaints"
    refund_rate_tracking: "Monitor and minimize refund requests"

  ethical_compliance:
    transparent_pricing: "Clear value proposition for all purchases"
    no_dark_patterns: "Avoid manipulative design practices"
    fair_f2p_experience: "Ensure free players have great gameplay"

  business_sustainability:
    ltv_payback_period: "Target 60-day payback on UA spend"
    revenue_diversification: "No single source >50% of revenue"
    player_lifetime_value: "Focus on long-term value over short-term extraction"
