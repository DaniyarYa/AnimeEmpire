# 08 · LiveOps & События

## Источник

`GDD/10_liveops_events.md`. Здесь — шаблоны, календарь, награды.

## Каденция

| Тип | Длительность | Частота | Цель |
|-----|--------------|---------|------|
| Micro | 2-6 ч | Несколько раз в день | Boost для активности |
| Short | 24-72 ч | 1-2 раза в неделю | Эмоциональный пик |
| Medium | 3-7 д | 1-2 раза в месяц | Карьера / collectible |
| Major | 7-14 д | Раз в месяц | Большая цель |
| Mega | 14-30 д | Раз в квартал | Сезон |

## Типы событий

### Production events
- Удваивают доходность конкретной цепочки
- Пример: «Farm Festival»: +100% wheat yield 48 ч
- Награды: лидерборд по произведённому количеству

### Progression events
- Тиры с растущими наградами
- Пример: «Building Challenge»: достигни Lv 10 в новом здании за неделю
- Награды: cosmetics, NPC, gems

### Collection events
- Найди / собери N редких предметов
- Пример: «NPC Hunt»: собери 5 specialist карточек
- Награды: legendary NPC

### Seasonal events
- Тематические (Winter, Harvest, Spring)
- Новые ресурсы, цепочки, окружение
- Часто mega-events

## Календарь (пример)

```
Понедельник     — Daily quests rotation
Среда вечер     — Limited offer (24 ч)
Пятница-Воскр.  — Short event (production boost)
Каждые 3 недели — Medium event (progression challenge)
Каждый месяц    — Major event (story/collection)
Каждый квартал  — Mega event (seasonal)
```

Регулярность — ключ retention.

## Конфигурация события

`EventDef` (Resource):
```gdscript
class_name EventDef extends Resource

@export var id: String
@export var display_name_key: String
@export var category: String          # production | progression | collection | seasonal
@export var start_unix: int
@export var end_unix: int
@export var icon: Texture2D
@export var modifiers: Array[ModifierDef]
@export var rewards: Array[RewardTier]
@export var leaderboard_id: String = ""
@export var event_currency_id: String = ""
@export var fomo_message_key: String
```

`RewardTier`:
```gdscript
class_name RewardTier extends Resource

@export var threshold: int          # очки / прогресс для разблокировки
@export var rewards: Dictionary     # {"gold": 1000, "gems": 5, "npc_id": "..."}
@export var is_premium: bool = false  # доступно только при battle-pass
```

## Реализация в коде

- `EventManager` (autoload): загружает активные ивенты из RemoteConfig
- При старте сессии — проверяет, активен ли event
- В UI HUD — индикатор «Event активен», тап → `events_screen`
- Модификаторы применяются через `EconomySim.add_modifier(...)`

## Награды

### Tiers (по `GDD/10`)
- **Participation** (все): минимальная награда за вход
- **Engagement** (50% игроков): средняя — gems, gold, временные buffs
- **Dedication** (80% активных): высокая — NPC, cosmetics
- **Mastery** (100% или leaderboard top): legendary
- **Premium** (paid pass): эксклюзивные cosmetics, exclusive NPC

### Event currency
- Каждый ивент имеет свою валюту (`festival_token`)
- Спустя `end_unix` валюта сгорает (FOMO)
- Магазин ивента — обмен валюты на награды

## Battle Pass (Phase 5)

- Сезонный (4 недели)
- Free track + Premium track ($9.99 / $4.99 со скидкой)
- 50 уровней
- Награды каждые 5 уровней — крупные

## Leaderboards

- Global / friends / regional
- Сезонные (сбрасываются с ивентом)
- Топ-100 — отдельные награды
- Anti-cheat: hash от save_state (см. `architecture/10_backend.md`)

## A/B-тестирование событий

- Разные группы получают разные параметры (yield multiplier 1.5x vs 2x)
- Меряем: участие, потраченное время, retention 24h после ивента, монетизация

## Push-уведомления для ивентов

- За 1 ч до старта: «Через час Farm Festival, +100% пшеница»
- В момент старта (opt-in)
- За 1 ч до конца: «Не упусти награды»

Респектим opt-in / rate-limit. См. `GDD/12`.

## Coordination в команде

- Live-Ops календарь — Notion / shared sheet
- Деплой ивентов — через PR с `.json` конфигом + RemoteConfig
- Hotfix balance — через RC без билда

## Открытые вопросы

- [ ] Минимум 1 ивент в первую неделю после релиза — критично для retention
- [ ] Cross-promotion с другими играми (если есть портфель студии)
