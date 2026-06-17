# 01 · Технологический стек

## Движок

- **Godot 4.x stable** (на момент старта — последняя стабильная минорная)
- **Renderer:** Compatibility (GLES3) — обязательно для целевых устройств из `GDD/2_auditory.md` (iPhone 6s+, Galaxy S8+, API 21+)
- **Язык:** GDScript 2.0 (основной). C# через Mono — только если профайлер покажет хот-споты, недопустимые в GDScript

## Целевые ТТХ

| Параметр | Цель | Источник |
|----------|------|----------|
| FPS | 60 на iPhone 8 / Galaxy S8 | GDD/11 |
| RAM | < 200 МБ | GDD/11 |
| Cold start | < 3 с | GDD/11 |
| Scene transition | < 1 с | GDD/11 |
| Размер билда (download) | < 150 МБ (iOS), < 100 МБ (APK base) | проектный |
| Батарея | < 8% / 30 мин активной игры | проектный |

## Включённые фичи Godot

- 3D Mesh, Skeleton3D + анимации (для NPC)
- Control + Theme (UI)
- AudioStreamPlayer + buses
- TranslationServer (i18n)
- HTTPRequest, WebSocketPeer
- Resource / ResourceSaver / ResourceLoader (для `.tres`)

## Выключенные / запрещённые фичи

- ❌ SDFGI, объёмный туман, screen-space reflections (тяжело для GLES3)
- ❌ Forward+/Mobile рендеры — только Compatibility
- ❌ Particles 3D с GPU shader compute (использовать CPU particles или 2D effects поверх)
- ❌ Тяжёлые post-process цепочки (≤ 1 эффект на сцене)

## Зависимости / плагины

Минимизируем сторонние плагины. Допустимые:

| Плагин | Назначение | Альтернатива |
|--------|------------|--------------|
| `Beehave` (опц.) | Behavior Trees для NPC | Свой FSM на GDScript |
| Godot Mobile Export | Сборка iOS/Android | — |
| `gdUnit4` | Юнит-тесты | Godot built-in |

Сторонние SDK (нативные):
- IAP: платформенные API через GDExtension/плагины комьюнити
- Ads: AdMob (через [Poing Studios AdMob plugin](https://github.com/Poing-Studios/godot-admob-plugin) или аналог)
- Analytics: Firebase / GameAnalytics (плагин)

## Инструментарий

| Назначение | Инструмент |
|------------|------------|
| Версионирование | Git + Git LFS (для `.glb`, `.png`, `.wav`) |
| CI | GitHub Actions: lint GDScript, импорт-проверка, сборка debug-билдов |
| Линтер | `gdlint` (gdtoolkit) |
| Форматтер | `gdformat` |
| Тесты | `gdUnit4` или `gut` |
| Профайлер | Godot встроенный + Xcode Instruments, Android Studio Profiler |
| 3D-чистка | Blender 4.x |
| Графика 2D | Aseprite (UI ассеты), Figma (UX) |
| Аудио | Audacity, Reaper |

## Структура билдов

- **dev** — отладка, аналитика выключена, чит-меню доступно
- **qa** — внутренний, аналитика в dev-проект, чит-меню под флагом
- **prod** — релиз, аналитика в prod-проект, чит-меню отключено

Управляется через export presets Godot + `OS.is_debug_build()`.

## Открытые вопросы

- [ ] Использовать ли GDExtension для критичных циклов симуляции (если 10 Hz × 100 NPC окажется тяжело)?
- [ ] AdMob vs IronSource — решение на фазе 3 (soft-launch).
