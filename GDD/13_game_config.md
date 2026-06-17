# 13. Конфигурация Игры и Данные Баланса

# Конфигурация Игры и Данные Баланса

## 13.1 Обзор Системы Конфигурации

### Архитектура Конфигурации

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

package "Источники Конфигурации" as sources {
  entity "Статическая Конфигурация" as static {
    :Основные Правила Игры;
    :Базовые Значения;
    :Системные Константы;
    :Настройки по Умолчанию;
  }

  entity "Удаленная Конфигурация" as remote {
    :Корректировки Баланса;
    :Параметры Событий;
    :Варианты A/B Тестов;
    :Флаги Функций;
  }

  entity "Конфигурация Игрока" as player {
    :Личные Настройки;
    :Состояние Прогресса;
    :Предпочтения;
    :Кастомизации;
  }
}

package "Управление Конфигурацией" as management {
  entity "Система Валидации" as validation {
    :Валидация Схемы;
    :Проверка Диапазонов;
    :Верификация Зависимостей;
    :Безопасность Отката;
  }

  entity "Система Распределения" as distribution {
    :Контроль Версий;
    :Постепенное Развертывание;
    :Экстренные Обновления;
    :Механизмы Отката;
  }
}

sources --> management : "Все источники конфигурации\\nпроходят через управление"

note bottom : "Централизованная система конфигурации\\nВключает быструю итерацию и тестирование"
@enduml

```

![image.png](attachment:06b19fd0-e091-4bef-80ad-2470ad02bd42:image.png)

### Принципы Конфигурации

```yaml
Configuration_Principles:
  flexibility:
    remote_adjustable: "All balance values configurable remotely"
    a_b_testable: "Support for multivariate testing"
    gradual_rollout: "Phased deployment of configuration changes"
    instant_rollback: "Immediate reversion capability"

  safety:
    validation_rules: "Strict validation for all configuration changes"
    dependency_checking: "Verify configuration consistency"
    fallback_values: "Default values if remote config fails"
    change_logging: "Complete audit trail of all changes"

  performance:
    local_caching: "Cache configuration locally for performance"
    delta_updates: "Only download changed values"
    compression: "Minimize bandwidth usage"
    background_sync: "Update configuration without interrupting gameplay"

  developer_experience:
    version_control: "Track all configuration changes"
    documentation: "Clear documentation for all parameters"
    testing_tools: "Easy testing of configuration changes"
    rollback_tools: "Simple rollback mechanisms"

```

## 13.2 Конфигурация Баланса Основной Игры

### Конфигурация Системы Зданий

```yaml
Building_Config:
  wheat_farm:
    base_stats:
      production_rate: 1.0 # wheat per second
      capacity: 10 # maximum wheat storage
      build_cost: 0 # free starter building
      upgrade_base_cost: 100 # gold for first upgrade

    scaling_formulas:
      production_scaling: "base_rate × (1 + 0.25 × level)"
      capacity_scaling: "base_capacity × (1 + 0.2 × level)"
      cost_scaling: "base_cost × (1.15 ^ level)"

    upgrade_limits:
      max_level: 25
      soft_cap_level: 20 # diminishing returns after this
      prestige_reset_level: 1 # level after prestige

  mill:
    base_stats:
      processing_time: 10.0 # seconds to convert wheat to flour
      input_capacity: 5 # wheat storage
      output_capacity: 5 # flour storage
      build_cost: 500 # gold to construct
      unlock_requirement: "player_level >= 3"

    scaling_formulas:
      speed_scaling: "base_time × (0.95 ^ level)" # faster processing
      capacity_scaling: "base_capacity × (1 + 0.3 × level)"
      cost_scaling: "base_cost × (1.12 ^ level)"

    processing_rules:
      input_ratio: 2 # wheat per flour
      output_ratio: 1 # flour per conversion
      quality_bonus: "1.0 + (0.02 × level)" # higher level = better quality

  bakery:
    base_stats:
      processing_time: 15.0 # seconds to convert flour to bread
      input_capacity: 3 # flour storage
      output_capacity: 3 # bread storage
      build_cost: 2000 # gold to construct
      unlock_requirement: "mill_level >= 5"

    scaling_formulas:
      speed_scaling: "base_time × (0.93 ^ level)"
      capacity_scaling: "base_capacity × (1 + 0.25 × level)"
      cost_scaling: "base_cost × (1.18 ^ level)"

    processing_rules:
      input_ratio: 1 # flour per bread
      output_ratio: 1 # bread per conversion
      quality_bonus: "1.0 + (0.03 × level)"

```

### Конфигурация Ценности Ресурсов

```yaml
Resource_Values:
  base_values: # Gold per unit
    wheat: 1
    flour: 4
    bread: 15
    wood: 1
    planks: 5
    furniture: 22
    stone: 2
    bricks: 9
    buildings: 40

  value_modifiers:
    quality_multiplier: "1.0 + (building_level × 0.02)" # Higher level = better prices
    market_demand: "0.8 to 1.2" # Random daily fluctuation
    bulk_discount: "0.95 ^ (quantity / 10)" # Slight discount for large sales

  processing_multipliers:
    tier_1_processing: 3.5 # wheat → flour
    tier_2_processing: 4.2 # flour → bread
    tier_3_processing: 4.8 # advanced processing

  special_resources: # Event or rare resources
    golden_wheat: 10 # 10x base wheat value
    crystal: 50 # rare mining resource
    gems: 100 # very rare resource

```

### Конфигурация Системы NPC

```yaml
NPC_Config:
  gatherer_npcs:
    farmer:
      hire_cost: 1000
      efficiency: 0.75 # 75% of player efficiency
      work_speed: 1.0 # base speed multiplier
      capacity: 3 # items carried
      specialization: "wheat"
      unlock_requirement: "wheat_farm_level >= 5"

    lumberjack:
      hire_cost: 1200
      efficiency: 0.80
      work_speed: 0.9 # slightly slower but more efficient
      capacity: 2 # wood is heavier
      specialization: "wood"
      unlock_requirement: "forest_level >= 3"

  carrier_npcs:
    porter:
      hire_cost: 800
      movement_speed: 0.8 # 80% of player speed
      capacity: 5 # can carry more items
      efficiency: 0.85
      unlock_requirement: "warehouse_level >= 2"

  specialist_npcs:
    master_miller:
      hire_cost: 5000
      building_bonus: 0.25 # +25% mill efficiency
      specialization: "mill"
      rarity: "rare"
      unlock_requirement: "mill_level >= 10"

  npc_progression:
    upgrade_cost_formula: "hire_cost × (1.3 ^ level)"
    efficiency_scaling: "base_efficiency + (level × 0.015)"
    speed_scaling: "base_speed × (1 + level × 0.1)"
    max_level: 10

```

## 13.3 Баланс Прогрессии и Экономики

### Конфигурация Персонажа Игрока

```yaml
Player_Character_Config:
  base_stats:
    movement_speed: 100 # units per second
    carry_capacity: 1 # number of items
    harvest_efficiency: 1.0 # multiplier for resource collection
    sale_bonus: 0.0 # percentage bonus to sale prices

  upgrade_costs:
    movement_speed:
      base_cost: 500
      cost_scaling: 1.2
      max_level: 20
      effect_per_level: 0.08 # +8% speed per level

    carry_capacity:
      base_cost: 300
      cost_scaling: 1.15
      max_level: 30
      effect_per_level: 0.33 # +1 capacity every 3 levels

    harvest_efficiency:
      base_cost: 400
      cost_scaling: 1.18
      max_level: 25
      effect_per_level: 0.04 # +4% efficiency per level

    sale_bonus:
      base_cost: 1000
      cost_scaling: 1.25
      max_level: 15
      effect_per_level: 1.5 # +1.5% sale bonus per level

```

### Конфигурация Мета-Прогрессии

```yaml
Meta_Progression_Config:
  prestige_requirements:
    first_prestige:
      min_income_per_hour: 100000 # 100K gold/hour
      min_building_levels: 15 # average building level
      min_production_chains: 3 # completed chains

    scaling_requirements:
      income_scaling: 2.5 # each prestige requires 2.5x more income
      building_scaling: 1.2 # slightly higher building requirements
      chain_scaling: 1.0 # same number of chains required

  prestige_points_formula:
    base_formula: "sqrt(max_income / 100000) + (total_building_levels / 100) + (chains × 0.5)"
    time_bonus: "max(0, (14 - days_played) × 0.1)" # bonus for faster prestige

  meta_upgrades:
    income_multiplier:
      cost_per_level: [1, 2, 3, 5, 8, 13, 21, 34, 55, 89] # Fibonacci sequence
      effect_per_level: 0.10 # +10% income per level
      max_level: 10

    speed_multiplier:
      cost_per_level: [2, 3, 5, 8, 13, 21, 34, 55] # Fibonacci sequence
      effect_per_level: 0.08 # +8% speed per level
      max_level: 8

    starting_gold:
      cost_per_tier: [3, 8, 20, 50] # Exponential growth
      gold_per_tier: [1000, 5000, 25000, 100000]
      max_tier: 4

```

## 13.4 Конфигурация Монетизации

### Конфигурация IAP

```yaml
IAP_Config:
  gem_packages:
    small_pack:
      gems: 100
      price_usd: 0.99
      bonus_gems: 0

    medium_pack:
      gems: 550
      price_usd: 4.99
      bonus_gems: 50 # 10% bonus

    large_pack:
      gems: 1200
      price_usd: 9.99
      bonus_gems: 200 # 20% bonus

    mega_pack:
      gems: 2750
      price_usd: 19.99
      bonus_gems: 500 # 22% bonus

  starter_pack:
    price_usd: 2.99
    contents:
      gems: 500
      gold: 5000
      exclusive_npc: "Helper Bot"
      speed_booster: 3 # 30-minute boosters
      building_skin: "Premium Farm Skin"
    availability_window: 7 # days from first play

  premium_subscription:
    price_usd: 4.99
    billing_period: "monthly"
    benefits:
      offline_multiplier: 2.0 # 2x offline earnings
      processing_speed_bonus: 0.5 # +50% processing speed
      daily_gems: 50
      exclusive_npcs: true
      ad_removal: true

```

### Конфигурация Интеграции Рекламы

```yaml
Ad_Config:
  rewarded_ads:
    offline_income_doubler:
      cooldown_minutes: 240 # 4 hours
      reward_multiplier: 2.0
      max_uses_per_day: 6

    instant_completion:
      cooldown_minutes: 15
      min_time_remaining: 30 # seconds
      max_uses_per_session: 5

    free_booster:
      cooldown_minutes: 120 # 2 hours
      booster_duration: 30 # minutes
      booster_effect: 2.0 # 2x speed
      max_uses_per_day: 12

  ad_frequency_limits:
    max_ads_per_session: 5
    min_time_between_ads: 180 # 3 minutes
    daily_ad_limit: 20
    respect_decline_period: 300 # 5 minutes before re-offering

  ad_placement_rules:
    never_interrupt_core_gameplay: true
    always_player_initiated: true
    clear_value_proposition: true
    immediate_reward_delivery: true

```

## 13.5 Конфигурация Событий и LiveOps

### Конфигурация Системы Событий

```yaml
Event_Config:
  harvest_festival:
    duration_hours: 120 # 5 days
    production_bonus: 1.0 # +100% farm production
    special_resource: "golden_wheat"
    special_spawn_interval: 30 # minutes
    event_currency: "festival_tokens"

    reward_tiers:
      tier_1: {tokens_required: 100, reward: "500 gems"}
      tier_2: {tokens_required: 300, reward: "Harvest NPC"}
      tier_3: {tokens_required: 600, reward: "Festival Building Skin"}
      tier_4: {tokens_required: 1000, reward: "Legendary Farmer NPC"}

  double_income_weekend:
    duration_hours: 48
    income_multiplier: 2.0
    combo_system:
      enabled: true
      max_combo: 10
      combo_bonus_per_level: 0.1 # +10% per combo level
      combo_decay_time: 30 # seconds without sale

  seasonal_events:
    winter_wonderland:
      start_date: "2024-12-15"
      end_date: "2024-12-29"
      theme_resources: ["ice", "pine_cones", "cocoa_beans"]
      special_chains: ["hot_cocoa_production"]
      weather_effects:
        snow_slowdown: 0.8 # 20% slower production
        quality_bonus: 1.3 # 30% higher quality

```

### Конфигурация A/B Тестирования

```yaml
AB_Testing_Config:
  tutorial_flow_test:
    test_name: "tutorial_v2"
    traffic_split: 0.5 # 50% of new players
    variants:
      control: "original_tutorial"
      treatment: "streamlined_tutorial"
    success_metrics: ["tutorial_completion_rate", "d1_retention"]

  starter_pack_pricing:
    test_name: "starter_pack_price"
    traffic_split: 0.3 # 30% of eligible players
    variants:
      control: {price: 2.99}
      treatment_a: {price: 1.99}
      treatment_b: {price: 4.99}
    success_metrics: ["conversion_rate", "revenue_per_user"]

  building_upgrade_costs:
    test_name: "building_costs_v3"
    traffic_split: 0.2 # 20% of players
    variants:
      control: {cost_scaling: 1.15}
      treatment: {cost_scaling: 1.12}
    success_metrics: ["progression_speed", "retention", "monetization"]

```

## 13.6 Техническая Конфигурация

### Конфигурация Производительности

```yaml
Performance_Config:
  rendering_settings:
    target_fps: 60
    min_fps: 30
    quality_levels:
      high: {texture_quality: 1.0, particle_density: 1.0, shadow_quality: "high"}
      medium: {texture_quality: 0.75, particle_density: 0.7, shadow_quality: "medium"}
      low: {texture_quality: 0.5, particle_density: 0.4, shadow_quality: "low"}

  memory_management:
    texture_memory_budget: 100 # MB
    audio_memory_budget: 30 # MB
    garbage_collection_frequency: 30 # seconds

  battery_optimization:
    background_fps_limit: 10
    idle_detection_time: 60 # seconds
    power_saving_mode_threshold: 20 # battery percentage

  network_settings:
    connection_timeout: 10 # seconds
    retry_attempts: 3
    offline_queue_size: 100 # pending requests
    sync_frequency: 300 # seconds

```

### Конфигурация Аналитики

```yaml
Analytics_Config:
  event_tracking:
    core_events:
      - "tutorial_step_completed"
      - "building_upgraded"
      - "npc_hired"
      - "resource_sold"
      - "prestige_activated"

    monetization_events:
      - "iap_purchase_initiated"
      - "iap_purchase_completed"
      - "ad_watched"
      - "offer_presented"
      - "offer_declined"

    retention_events:
      - "session_started"
      - "session_ended"
      - "daily_login"
      - "achievement_unlocked"
      - "social_action"

  data_collection_rules:
    personal_data: false # No PII collection
    device_info: true # Device specs for optimization
    gameplay_data: true # All gameplay interactions
    crash_reports: true # Automatic crash reporting

  privacy_settings:
    gdpr_compliance: true
    coppa_compliance: true
    data_retention_days: 365
    anonymization_enabled: true

```

## 13.7 Конфигурация Локализации

### Конфигурация Поддержки Языков

```yaml
Localization_Config:
  supported_languages:
    tier_1: # Launch languages
      - {code: "en", name: "English", region: "US"}
      - {code: "es", name: "Spanish", region: "ES"}
      - {code: "fr", name: "French", region: "FR"}
      - {code: "de", name: "German", region: "DE"}
      - {code: "ja", name: "Japanese", region: "JP"}

    tier_2: # Post-launch expansion
      - {code: "pt", name: "Portuguese", region: "BR"}
      - {code: "ru", name: "Russian", region: "RU"}
      - {code: "ko", name: "Korean", region: "KR"}
      - {code: "zh-CN", name: "Chinese Simplified", region: "CN"}

  text_formatting:
    number_formatting: "locale_specific" # 1,000.00 vs 1.000,00
    currency_symbols: "locale_specific" # $, €, ¥, etc.
    date_formatting: "locale_specific" # MM/DD/YYYY vs DD/MM/YYYY

  cultural_adaptations:
    color_preferences: "region_specific" # Avoid unlucky colors
    imagery_guidelines: "cultural_sensitivity" # Appropriate imagery
    gameplay_modifications: "minimal" # Keep core gameplay consistent

```

## 13.8 Инструменты Управления Конфигурацией

### Правила Валидации Конфигурации

```yaml
Validation_Rules:
  numeric_ranges:
    building_costs: {min: 0, max: 1000000}
    production_rates: {min: 0.1, max: 100.0}
    upgrade_scaling: {min: 1.01, max: 2.0}
    npc_efficiency: {min: 0.1, max: 1.0}

  dependency_checks:
    building_unlocks: "prerequisite_buildings_exist"
    npc_requirements: "required_buildings_available"
    event_rewards: "reward_items_defined"

  consistency_rules:
    resource_flow: "input_resources_available"
    economic_balance: "inflation_rate_acceptable"
    progression_pacing: "upgrade_frequency_reasonable"

  safety_constraints:
    no_infinite_loops: "prevent_circular_dependencies"
    no_negative_values: "all_costs_positive"
    no_impossible_requirements: "achievable_unlock_conditions"

```

### Процесс Развертывания Конфигурации

```
@startuml
!theme plain

start
:Configuration Change Requested;
:Validate Against Schema;
if (Validation Passes?) then (yes)
  :Stage in Test Environment;
  :Run Automated Tests;
  if (Tests Pass?) then (yes)
    :Deploy to Staging;
    :Manual QA Testing;
    if (QA Approves?) then (yes)
      :Deploy to Production;
      :Monitor Key Metrics;
      if (Metrics Healthy?) then (yes)
        :Mark Deployment Successful;
      else (no)
        :Automatic Rollback;
        :Alert Development Team;
      endif
    else (no)
      :Return to Development;
    endif
  else (no)
    :Fix Test Failures;
  endif
else (no)
  :Fix Validation Errors;
endif
stop

@enduml

```

note right: "Автоматизированный пайплайн обеспечивает\nбезопасное развертывание конфигурации"
