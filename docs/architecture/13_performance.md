# 13 · Производительность

## Бюджеты

| Метрика | Бюджет | Минимум | Замер |
|---------|--------|---------|-------|
| FPS | 60 на iPhone 8 / Galaxy S8 | 30 | DisplayServer FPS counter |
| RAM peak | < 200 МБ | 250 МБ | Xcode Instruments / Android Studio Profiler |
| Cold start | < 3 с | 4 с | Stopwatch от запуска до Boot scene ready |
| Scene transition | < 1 с | 1.5 с | Time.get_ticks_msec до scene_changed |
| Battery | < 8% / 30 мин | 12% | Замер вручную |
| Download size | < 150 МБ (iOS), < 100 МБ APK base | 200 МБ | Build inspect |
| Draw calls | < 80 типовая сцена | 120 | Godot debug panel |
| Vertices on screen | < 100K | 200K | Godot debug panel |
| Texture memory | < 80 МБ | 120 МБ | Godot debug panel |

## Целевые устройства

- iPhone 8 (A11, 2GB RAM, iOS 12+) — baseline iOS
- iPhone 6s / iPad Air 2 — min-spec, можно деградировать качество
- Samsung Galaxy S8 (Exynos 8895, 4GB RAM, Android 8+) — baseline Android
- Galaxy S5 / Xiaomi Redmi Note 4 — min-spec

## Стратегии оптимизации

### Рендер
- Compatibility renderer (GLES3) обязательно
- Один материал на NPC (batching)
- Static lighting baked где возможно (lightmaps)
- LOD на здания (2-3 уровня)
- Frustum + occlusion culling включены
- Тени — только от ключевых источников, разрешение 1024
- Никаких screen-space эффектов
- Disable MSAA на мобиле, опционально FXAA

### Память
- Texture compression: ETC2 (Android), ASTC (iOS), PVRTC (старые iOS)
- Атласы UI-иконок
- Сжатие звука: Ogg Vorbis (BGM), WAV PCM 16-bit короткие SFX (или OGG для длинных)
- Pooling для NPC, projectiles, floating-text
- Освобождение неиспользуемых сцен через `queue_free()` + `await get_tree().process_frame`

### CPU
- Симуляция 10 Hz (не 60)
- NPC batched-update (тик каждого через раз при >50)
- Избегать `find_node` в горячих циклах
- Кешировать ссылки на узлы в `_ready()`
- `_process` отключён там, где не нужен (`set_process(false)`)

### Размер билда
- Asset stripping (удалять неиспользуемые в `.tres`)
- Локализации в виде сжатых `.po` (gzip)
- Splash + UI текстуры в атласах
- Шейдеры — не больше 3-4 уникальных
- Android App Bundle (динамическая доставка по архитектуре)

### Старт
- Boot scene минимальна (logo + загрузка)
- Heavy инициализация (config fetch, save load) — асинхронно
- Прелоад только тех ассетов, что нужны для первого экрана

## Профилирование

### Регулярно
- Раз в спринт — прогон performance-чеклиста на минимальном устройстве
- При добавлении новой механики — замер до/после
- CI smoke-тест на эмуляторе (хотя бы FPS не упал ниже порога)

### Инструменты
- Godot встроенный профайлер (CPU, memory)
- Xcode Instruments: Time Profiler, Allocations, Energy
- Android Studio Profiler: CPU, Memory, Network
- RenderDoc (если нужен фрейм-анализ)

## Дисплей и качество

`Settings.graphics_quality` уровни:
- **Low:** shadows off, particles off, 720p render scale, simplified shaders
- **Medium:** shadows 512, basic particles, 1080p, default shaders
- **High:** shadows 1024, full particles, native res

Auto-detect: дешёвые устройства (по CPU/RAM) → low по умолчанию.

## Acceptance criteria для PR

При значительной механической фиче PR должен включать:
- [ ] Profiler-замер на target device (FPS не упал ≥ 5)
- [ ] Memory: не выросла на > 10 МБ
- [ ] Build size delta задокументирована

## Известные потенциальные узкие места

| Узкое место | Митигация |
|-------------|-----------|
| Много NPC одновременно в кадре | Pool + LOD на скелеты, simplified animation away from camera |
| Floating-text спам при больших доходах | Aggregate (за тик показываем сумму, а не каждый дроп) |
| Long offline calc после возврата | Async с прогресс-баром, разбит на чанки |
| Save serialization лагает | Backup в фоне через `Thread` |
| Скачивание ассетов | Не используем downloadable content на MVP |

## Открытые вопросы

- [ ] Поддержка ProMotion (120 FPS) на новых iPhone — приоритет post-launch
- [ ] Нативный код (GDExtension) для критичных циклов — отложить до явного hot-spot
