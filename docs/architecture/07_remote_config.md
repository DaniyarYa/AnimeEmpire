# 07 · Remote Config & A/B-тестирование

## Цели

- Балансировать экономику без релиза билда
- Включать/выключать LiveOps-события удалённо
- A/B-тестировать варианты (туториал, стартер-пак, цены)
- Управлять фича-флагами (включить новую систему без обновления стора)

## Что конфигурируется удалённо

| Категория | Что меняется | Пример |
|-----------|--------------|--------|
| Экономика | Множители стоимости, цены ресурсов | `cost_growth: 1.15 → 1.13` |
| Производство | Циклы зданий, базовые доходы | Festival event +100% yield |
| NPC | Эффективность, стоимость найма | `gatherer_farmer.efficiency: 0.75 → 0.80` |
| Монетизация | Цены IAP, состав пакетов, окна оффера | Starter pack: $2.99 → $1.99 |
| Реклама | Частота, типы (interstitial, rewarded) | Rewarded после prestige |
| Прогрессия | Уровни разблокировки, требования престижа | Prestige threshold lowered |
| Туториал | Шаги, тексты, длительность | Variant A vs B (быстрый/обычный) |
| События | Активные ивенты, их параметры | Winter event ON 2026-12-01 |
| Фича-флаги | Boolean-выключатели | `enable_friends: false` |

## Архитектура

```
┌──────────┐  GET /config?ver=N&user_segment=X  ┌──────────┐
│  Client  │ ─────────────────────────────────▶ │  Backend │
│  Godot   │ ◀───────────── JSON ──────────────│ + S3 CDN │
└──────────┘                                    └──────────┘
     │
     ▼
 cache в user://config_cache.json
     │
     ▼
 fallback на встроенные .tres
```

### Источник правды
- **dev / qa:** локальный `tools/config/` (JSON, редактируется руками)
- **prod:** S3 + CloudFront. Раскат через CI: PR → merge → деплой JSON в bucket → CDN invalidation

### Версионирование
Конфиг имеет `version: int` и `schema_version: int`.
- `version` — растёт с каждым изменением
- `schema_version` — растёт только при breaking change (новый или удалённый ключ)
- Клиент проверяет `schema_version` — если меньше требуемой, использует fallback и пишет analytics-event

## Формат

```json
{
  "version": 47,
  "schema_version": 3,
  "valid_from": 1718600000,
  "valid_until": 1719600000,
  "values": {
    "economy.cost_growth_early": 1.12,
    "economy.cost_growth_mid": 1.15,
    "economy.cost_growth_late": 1.18,
    "buildings.wheat_farm.base_cycle": 1.0,
    "buildings.wheat_farm.base_cost": 100,
    "monetization.starter_pack.price_usd": 2.99,
    "monetization.starter_pack.window_days": 7,
    "flags.enable_friends": false,
    "flags.enable_battle_pass": true
  },
  "ab_tests": {
    "tutorial_variant": {
      "variants": ["A", "B"],
      "weights": [0.5, 0.5],
      "stickiness": "user_id"
    },
    "starter_pack_price": {
      "variants": ["1.99", "2.99", "4.99"],
      "weights": [0.34, 0.33, 0.33]
    }
  },
  "events": [
    {"id": "farm_festival_2026_06", "start": 1718640000, "end": 1719244800, "params": {"yield_mult": 2.0}}
  ]
}
```

## Применение значений в коде

```gdscript
# RemoteConfig.gd
func get_float(key: String, default: float) -> float:
    return _values.get(key, default)

# использование
var growth = RemoteConfig.get_float("economy.cost_growth_early", 1.12)
```

`BuildingDef` помечает поля как «remote-overridable». При загрузке `BuildingDef.apply_remote_config()` накатывает override.

## A/B-тесты

### Назначение варианта
- Backend назначает на `/auth/login` и пишет в user profile
- Sticky: пользователь остаётся в одном варианте всё время теста
- Альтернатива: клиентский hash `hash(user_id + test_id) % N` — без серверного round-trip, но без точного управления распределением

### Замер
- Каждый event аналитики помечается активными вариантами `{tutorial_variant: "B"}`
- Воронка анализируется в BI (Looker / Metabase / Amplitude)

## Hot-reload

Конфиг можно перезагружать без перезапуска приложения:
- Раз в N минут (например, 15) клиент пингует `/config?ver=cached`
- Backend отвечает `304` или новой версией
- Применение — через `EventBus.config_loaded`, чувствительные системы пересчитывают модификаторы

При активной игровой сессии — не применяем break-changing значения сразу, ждём перехода между сценами / возврата на главный экран.

## Безопасность

- Конфиг — публичный (любой может вытянуть). Не класть туда секреты
- Подпись конфига сервером (HMAC) — чтобы избежать MITM-подмены
- Клиент проверяет подпись перед применением

## Fallback

Если конфиг не загружен и нет кеша — используем встроенные `.tres`. Игра должна быть полностью играбельной без remote config (важно для cold start и оффлайн-первого запуска).

## Открытые вопросы

- [ ] Использовать готовый сервис (Firebase Remote Config, Statsig, GrowthBook) или строить свой?
- [ ] Юзер-сегменты (whales, retention cohort) — кто их рассчитывает: клиент или сервер?
