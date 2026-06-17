# 11 · Аналитика

## Цели

- Отслеживать ключевые KPI (retention, ARPDAU, conversion)
- Замерять воронки прогрессии (onboarding, первый IAP, первый prestige)
- A/B-тестирование (см. `07_remote_config.md`)
- Выявлять баги через crash-репорты и аномалии

## Стек

| MVP | Prod |
|-----|------|
| Firebase Analytics + Crashlytics | Custom event collector → S3 → Athena + Sentry |
| BigQuery (free quota) | Looker / Metabase |

> Перейти на custom — когда MAU > 100k или нужны кастомные cohort-расчёты.

## Каркас событий

```gdscript
# AnalyticsBus.gd (autoload)
func log(event_name: String, params: Dictionary = {}) -> void:
    var enriched = _enrich(event_name, params)
    _queue.append(enriched)
    if _queue.size() >= 50 or _time_since_flush > 30.0:
        _flush()

func _enrich(name: String, p: Dictionary) -> Dictionary:
    return p.merged({
        "event": name,
        "ts": Time.get_unix_time_from_system(),
        "session_id": GameState.session_id,
        "user_id": GameState.user_id,
        "app_version": ProjectSettings.get_setting("application/config/version"),
        "build": OS.is_debug_build() if "dev" else "prod",
        "platform": OS.get_name(),
        "locale": Localization.current,
        "country": GameState.country_code,
        "ab_variants": RemoteConfig.active_variants(),
    })
```

## Naming convention

`<category>.<subject>.<verb>`. Пример: `economy.building.upgraded`.

Категории:
- `lifecycle.*` — старт/конец сессии, foreground/background
- `progression.*` — уровни, престижи, разблокировки
- `economy.*` — здания, апгрейды, ресурсы
- `tutorial.*` — шаги туториала
- `monetization.*` — показ оффера, IAP, ad
- `social.*` — друзья, лидерборды
- `events.*` — LiveOps
- `error.*` — обрабатываемые ошибки (не crash)

## Минимальный набор событий

| Событие | Параметры | Когда |
|---------|-----------|-------|
| `lifecycle.session.started` | — | старт сессии |
| `lifecycle.session.ended` | duration_s | завершение |
| `tutorial.step.completed` | step, variant | каждый шаг |
| `tutorial.flow.completed` | total_minutes | конец туториала |
| `progression.player.level_up` | new_level | уровень игрока |
| `progression.prestige.triggered` | level, points_earned | престиж |
| `economy.building.constructed` | building_id | первое здание |
| `economy.building.upgraded` | building_id, level, cost | апгрейд |
| `economy.resource.sold` | resource_id, amount, gold | продажа |
| `economy.currency.changed` | kind, delta, balance | gold/gems баланс |
| `npc.hired` | npc_id, category | найм |
| `monetization.offer.shown` | offer_id, variant | показ |
| `monetization.iap.purchased` | sku, price_usd, source | IAP success |
| `monetization.ad.shown` | placement, type | реклама |
| `monetization.ad.completed` | placement, reward | rewarded ad complete |
| `events.live.joined` | event_id | вход в ивент |
| `events.live.reward_claimed` | event_id, tier | награда |
| `error.save.corrupt` | slot | recovery из бэкапа |

## KPI и BI-дашборды

### Retention
- D1 / D7 / D30 (cohort by install date)
- Цель GDD: D1=40%, D7=18%, D30=8%

### Monetization
- ARPDAU = gross_revenue / DAU
- Conversion (% paying)
- ARPU paying users
- LTV (cumulative revenue per cohort)

### Progression
- Time-to-first-prestige (median)
- Distribution by prestige level
- % stuck at level X (детект балансных проблем)

### Tutorial
- Funnel drop-off по каждому шагу
- A/B варианты (skip / quick / full)

## Batching и offline

- Очередь в памяти + бэкап на диск (`user://analytics_queue.json`)
- Флаш при `app_pause` (синхронный, чтобы не потерять)
- При оффлайне очередь растёт; при восстановлении сети — отправляется батчами

## Privacy / GDPR

- В первый запуск — opt-in диалог (для GDPR-стран определяем по IP/locale)
- Без согласия — отправляем только non-PII события (crash, performance) без user_id
- Endpoint `POST /user/data/export` (за полным архивом данных)
- Endpoint `POST /user/data/delete` (полное удаление по запросу)

## Crash / error reporting

- Crashlytics (MVP) / Sentry (prod)
- Custom logger пишет последние 200 строк лога перед крашем (включается в crash report)
- Авто-symbolication для iOS через .dSYM upload в CI

## Performance события

- `perf.fps.drop` (если ниже 30 на >2 c)
- `perf.memory.warning`
- `perf.scene_load_time` (длительность загрузки)

Не флудим: rate-limit `perf.*` событий (max 10/min на устройство).

## Открытые вопросы

- [ ] Server-side validation событий (выкидывать невалидные с клиента)
- [ ] Real-time дашборд для LiveOps команды (через Kinesis?)
