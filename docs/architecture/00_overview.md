# 00 · Системный обзор

## Назначение

Высокоуровневая карта системы: какие компоненты есть, как они общаются, где границы.

## Override GDD

> **ADR-001: Движок — Godot 4.x вместо Unity URP.**
> **Дата:** 2026-06-17.
> **Причина:** Открытая лицензия, малый рантайм, простой Compatibility-renderer для целевых мобильных устройств (iPhone 6s+, Galaxy S8+). Команда мала, итерации дешевле в Godot.
> **Последствия:** GDD/11_tech_reqs.md (Unity URP, Addressables, C#) применяется концептуально, но не буквально. Эквиваленты см. `01_engine_stack.md`.

## Слои клиента

```
┌──────────────────────────────────────────────┐
│ Presentation                                 │
│   Control (UI) + 3D-сцены, анимации          │
├──────────────────────────────────────────────┤
│ Game Logic (autoload)                        │
│   GameState · EventBus · SceneRouter         │
├──────────────────────────────────────────────┤
│ Simulation                                   │
│   EconomySim · NPCSystem · ProductionTick    │
├──────────────────────────────────────────────┤
│ Data                                         │
│   Resource (.tres) · SaveService · Migrations│
├──────────────────────────────────────────────┤
│ Platform                                     │
│   HTTP · Storage · IAP · Ads · Analytics SDK │
└──────────────────────────────────────────────┘
```

## Внешние интеграции

```
                ┌──────────────────┐
                │   Godot Client   │
                └────────┬─────────┘
                         │ HTTPS
        ┌────────────────┼─────────────────┐
        ▼                ▼                 ▼
  ┌──────────┐    ┌──────────────┐  ┌──────────────┐
  │ Backend  │    │ Store IAP    │  │ Ads Mediator │
  │ AWS/FB   │    │ Apple/Google │  │ (AdMob/etc.) │
  └──────────┘    └──────────────┘  └──────────────┘

  Офлайн-пайплайн (вне рантайма):
  3daistudio.com → Blender → Godot import → git LFS
```

## Autoload-сервисы (синглтоны)

| Имя | Ответственность |
|-----|-----------------|
| `GameState` | Текущее состояние сессии (фаза, активные таймеры) |
| `EventBus` | Pub/sub сигналов между подсистемами |
| `EconomySim` | Производство, доходы, формулы |
| `NPCSystem` | Регистрация NPC, назначение задач |
| `SaveService` | Загрузка/сохранение, шифрование, миграции |
| `RemoteConfig` | Получение конфигов с сервера, фолбэк на локальные `.tres` |
| `AnalyticsBus` | Очередь событий, отправка пачками |
| `MonetizationService` | IAP, реклама, подписка |
| `Localization` | Обёртка над `TranslationServer` |
| `SceneRouter` | Переходы между экранами |

Подробности — `03_core_systems.md`.

## Потоки данных (ключевые)

1. **Тик симуляции:** `ProductionTick (10 Hz)` → `EconomySim.advance(dt)` → `EventBus.emit("resource_produced")` → UI обновляет HUD.
2. **Сохранение:** `EventBus.on("save_dirty")` → `SaveService.write_async()` (debounced 2 с) → локальный файл AES-256 → фоновый sync в backend.
3. **Оффлайн-доход:** `_ready()` → diff `now - last_seen` → `EconomySim.simulate_offline(seconds, cap=4h, eff=0.5)` → модалка результата.
4. **Покупка:** UI → `MonetizationService.purchase(sku)` → платформенный IAP → серверная валидация → `EconomySim.grant(reward)` → save.

## Производительность

Подробности — `13_performance.md`. Ключевые бюджеты:
- 60 FPS на iPhone 8 / Galaxy S8
- < 200 МБ RAM
- < 3 с cold start
- < 80 draw calls на типовом экране

## Открытые вопросы

- [ ] Firebase для MVP или сразу AWS? (см. `10_backend.md`)
- [ ] BehaviorTree через плагин (`Beehave`) или ручной FSM? (см. `04_simulation.md`)
