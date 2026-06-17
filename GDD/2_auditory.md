# **2. Целевая Аудитория и Платформенная Стратегия**

# Целевая Аудитория и Платформенная Стратегия

## 2.1 Профиль Основной Аудитории

### Демография

* **Возраст** : 18-35 лет
* **Платформы** : iOS/Android (приоритет iOS для монетизации)
* **География** : Страны первого уровня (США, Канада, Западная Европа, Япония)
* **Доход** : Средний+ (готовность к микротранзакциям)

### Игровой Опыт

**Обязательный опыт:**

* Idle Miner Tycoon
* Lumber Empire / Idle Wood Inc
* AdVenture Capitalist
* Cookie Clicker (понимание инкрементальных механик)

**Желательный опыт:**

* Аниме/манга культура
* Mobile RPG (для понимания прогрессии персонажа)
* City builders (SimCity, Township)

### Поведенческий Профиль

```
@startuml
!theme plain
skinparam actorStyle awesome

actor "Mid-Core Player" as player
usecase "Daily Sessions" as daily
usecase "Automation Focus" as auto
usecase "Micro Investments" as micro
usecase "Progress Optimization" as opt

player --> daily : "2-4 раза в день\\n3-10 минут"
player --> auto : "Ценит эффективность\\nНе любит рутину"
player --> micro : "$1-20 в месяц\\nИмпульсивные покупки"
player --> opt : "Любит оптимизировать\\nИщет лучшие стратегии"

note right of daily : "Короткие сессии\\nВысокая частота"
note bottom of auto : "Готов платить за\\nавтоматизацию"
note left of micro : "Starter packs\\nTime savers"
note top of opt : "Теорикрафтинг\\nМин-максинг"
@enduml

```

![image.png](attachment:6807d558-f980-4e3e-8868-1dfd310fed4b:image.png)

## 2.2 Вторичная Аудитория

### Казуальные Поклонники Аниме

* **Привлечение** : Визуальный стиль
* **Удержание** : Простой старт + постепенное усложнение
* **Конверсия** : Косметические покупки

### Новички в Idle Играх

* **Привлечение** : Аниме тематика
* **Удержание** : Понятный основной цикл
* **Конверсия** : Стартовые наборы

## 2.3 Фреймворк Мотивации Игроков

### Основные Мотивации (Таксономия Бартла)

```
@startuml
!theme plain

package "Мотивации Игроков" {
    [Достиженцы] as ACH
    [Исследователи] as EXP
    [Социализаторы] as SOC
    [Убийцы] as KIL
}

package "Игровые Системы" {
    [Прогрессия] as PROG
    [Открытие Контента] as DISC
    [Таблицы Лидеров] as LEAD
    [Соревнование] as COMP
}

ACH --> PROG : "Числовой рост\\nДостижения"
EXP --> DISC : "Новые цепочки\\nСкрытые механики"
SOC --> LEAD : "Сравнение прогресса\\nГильдии"
KIL --> COMP : "PvP события\\nРейтинги"

note bottom of ACH : "70% аудитории"
note bottom of EXP : "20% аудитории"
note bottom of SOC : "8% аудитории"
note bottom of KIL : "2% аудитории"
@enduml

```

![image.png](attachment:a8d7627a-94f8-4a84-a800-cd74284921ba:image.png)

### Retention Drivers

1. **Видимый прогресс** - числа растут постоянно
2. **Открытие нового** - регулярные разблокировки
3. **Оптимизация** - поиск лучших стратегий
4. **Визуальное развитие** - деревня растет и красивеет
5. **"Еще один апгрейд"** - классический idle-хук

## 2.4 Стратегия Дизайна Сессий

### Типы Сессий и Длительность

```
@startuml
!theme plain
skinparam backgroundColor #FFFFFF

rectangle "Короткие Сессии" as short #lightblue
rectangle "Средние Сессии" as medium #lightgreen
rectangle "Длинные Сессии" as long #lightyellow

short : 30 сек - 3 мин
short : • Сбор офлайн дохода
short : • Быстрые апгрейды
short : • Использование бустеров
short : • Проверка прогресса

medium : 5-10 мин
medium : • Открытие новых зданий
medium : • Оптимизация цепочек
medium : • Ручное ускорение
medium : • Планирование развития

long : 15+ мин
long : • Новые производственные этапы
long : • Крупные апгрейды
long : • Кат-сцены и события
long : • Престиж и мета-прогрессия

short -down-> medium : "Втягивание"
medium -down-> long : "Глубокое погружение"
long -up-> short : "Возврат к рутине"
@enduml

```

![image.png](attachment:06a33a92-526d-4a79-8c78-b3b27f1b11b5:image.png)

### Оптимизация Потока Сессии

* **Хук (0-10 сек)** : Офлайн доходы + визуальные изменения
* **Основной Цикл (10 сек - 3 мин)** : Основные действия
* **Хук Удержания (последние 30 сек)** : Прогресс к следующей цели

## 2.5 Платформенная Стратегия

### Приоритет Целевых Платформ

1. **iOS** (приоритет) - высокая монетизация
2. **Android** - массовая аудитория

### Technical Requirements

```yaml
Platform_Specs:
  orientation: portrait
  input_method: one_hand_touch
  performance_target:
    fps: 30-60
    battery_life: minimal_impact
    device_support: low_end_compatible

  iOS_specific:
    min_version: iOS 12
    target_devices: [iPhone_6s+, iPad_Air_2+]

  Android_specific:
    min_api: 21 (Android 5.0)
    target_ram: 2GB+
    gpu: Adreno_530+ / Mali_G71+

```

### Совместимость Монетизации по Платформам

* **iOS** : Премиум IAP, Удаление Рекламы, Боевой Пропуск
* **Android** : Фокус на награждаемой рекламе, Микро-IAP
* **Обе** : Стартовые Наборы, Ограниченные Предложения

## 2.6 Конкурентный Анализ

### Прямые Конкуренты

| Game                 | Strength         | Weakness          | Our Advantage  |
| -------------------- | ---------------- | ----------------- | -------------- |
| Lumber Empire        | Proven mechanics | Generic visuals   | Anime style    |
| Idle Miner Tycoon    | Polish & scale   | Complex UI        | Simplicity     |
| AdVenture Capitalist | Addictive loop   | Outdated graphics | Modern visuals |

### Позиционирование на Рынке

```
@startuml
!theme plain

rectangle "Высокое Визуальное Качество" as visual_high
rectangle "Низкое Визуальное Качество" as visual_low
rectangle "Простые Механики" as mech_simple
rectangle "Сложные Механики" as mech_complex

visual_high -down-> visual_low : "Визуальное Качество"
mech_simple -right-> mech_complex : "Механическая Сложность"

note "Anime Empire\\n(Target Position)" as target
target .> visual_high
target .> mech_simple

note "Lumber Empire" as lumber
lumber .> visual_low
lumber .> mech_simple

note "Idle Miner Tycoon" as miner
miner .> visual_low
miner .> mech_complex

note "Premium Idle Games" as premium
premium .> visual_high
premium .> mech_complex
@enduml

```

![image.png](attachment:b394fee6-7e39-44b0-8e1a-2f0518ce9f29:image.png)

## 2.7 Метрики Успеха по Аудитории

### KPI Основной Аудитории

* **D1 Retention** : ≥45%
* **D7 Retention** : ≥18%
* **ARPDAU** : ≥$0.12
* **Session Frequency** : ≥3 per day

### KPI Вторичной Аудитории

* **Tutorial Completion** : ≥90%
* **First Purchase Rate** : ≥5%
* **Organic Sharing** : ≥2% (anime appeal)
* **Visual Rating** : ≥4.5/5
