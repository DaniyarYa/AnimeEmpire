# Phase 0 · 3DAI Studio Generation Brief

Цель Phase 0 — **валидация пайплайна**, не production. Генерируем минимум: 1 базовый персонаж + 2 анимации. Если результат непригоден — fallback на Mixamo + ручной арт (см. `docs/architecture/08_asset_pipeline.md`).

Полный список персонажей для Phase 1-2 (4 gatherer + 2 carrier) идёт после успешной валидации Phase 0.

---

## Что генерируем в Phase 0

| # | Артефакт | Задача | Файлы (выход) | Priority |
|---|----------|--------|---------------|----------|
| 1 | Player avatar T-pose | P0-031 | `assets/characters/player_avatar/v0/player_avatar.glb` + `diffuse_512.png` | P0 |
| 2 | Idle animation | P0-033 | `assets/characters/player_avatar/v0/animations/idle.glb` | P0 |
| 3 | Walk animation | P0-033 | `assets/characters/player_avatar/v0/animations/walk.glb` | P0 |

После Blender чистки и импорта в Godot — раздаём имена/UID. Реальная имплементация в сцене — Phase 1 (P1-020..P1-023).

## Что НЕ генерируем в Phase 0

- ❌ NPC (Gatherer Farmer, Lumberjack, и т.д.) — Phase 2 (P2-010)
- ❌ Здания — Blender / asset-pack, не 3DAI (см. `08_asset_pipeline.md`)
- ❌ Дополнительные анимации (work, carry, celebrate) — Phase 1
- ❌ Hi-res портреты — отдельный пайплайн (Midjourney / арт-команда)
- ❌ Cosmetics / costume variants — Phase 5

---

## Стилевые ограничения (для всех генераций)

Источник правды: `docs/design/06_art_direction.md`.

| Параметр | Значение |
|----------|----------|
| Стиль | chibi anime, low-poly 3D, soft shading |
| Tris | 500-1000 на gameplay-модель |
| Текстура | 512×512 diffuse only, no PBR |
| Pose | T-pose, ready for rigging |
| Camera | front 3/4 view, soft neutral lighting |
| Rig | humanoid skeleton (Mixamo-compatible) |
| Output | GLB предпочтительно (Godot-родной), FBX как fallback |
| Scale | 1 unit = 1 m, рост chibi ~1.5 m |
| Origin | в ногах (0,0,0) |

---

## Промпт 1 · Player Avatar (P0-031)

### Контекст
Главный аватар. Должен выглядеть нейтрально-дружелюбно, подходить для аниме-аудитории 18-35. Стартовая профессия — мелкий фермер.

### Промпт (для 3DAI Studio)

```
Style: chibi anime, low-poly 3D game character model, soft cel-shaded look,
       around 800 triangles, suitable for mobile games
Character: friendly young farmer, late teens, gender-ambiguous,
           cheerful expression, cute proportions (large head ~1/3 of body)
Pose: T-pose with arms straight horizontal, ready for humanoid rigging
Outfit: simple linen tunic in warm beige, brown leather suspenders,
        rolled-up trousers, straw hat (small, perched on head),
        leather ankle boots
Accessory: small wooden sickle strapped to right hip belt
Colors:
  - hair: warm chestnut #8B5A2B
  - skin: warm peach #F2C5A4
  - tunic: beige #D4A574
  - suspenders / belt: dark brown #5C3A1E
  - hat: straw yellow #FFD66B
Texture: 512x512 diffuse map only, no PBR
Lighting: soft neutral front 3/4 lighting, white background
Output: GLB with humanoid skeleton (Mixamo-compatible bone names if possible)
Style reference: Animal Crossing villager proportions + Genshin Impact chibi
```

### Acceptance (для модели)
- [ ] Открывается в Blender / Godot без ошибок
- [ ] Tris ≤ 1000 (decimate если нужно)
- [ ] Origin в ногах (0,0,0); пёр персонаж стоит на плоскости Y=0
- [ ] Skeleton с минимальным rig (см. ниже)
- [ ] Diffuse 512×512, sRGB
- [ ] Стиль соответствует референсу (chibi пропорции)

### Минимальный rig (Mixamo-совместимый)

```
Hips
├── Spine
│   └── Chest
│       ├── Neck → Head
│       ├── UpperArm.L → LowerArm.L → Hand.L
│       └── UpperArm.R → LowerArm.R → Hand.R
├── UpperLeg.L → LowerLeg.L → Foot.L
└── UpperLeg.R → LowerLeg.R → Foot.R
```

Если 3DAI генерирует другую структуру — перепривяжем в Blender (Rigify / manual).

---

## Промпт 2 · Idle Animation (P0-033)

### Контекст
Базовый бездействующий цикл. Persistent loop, лёгкое движение для оживления.

### Промпт

```
Animation: idle loop, 2-3 second duration, looping seamlessly
Character: humanoid chibi (use rig from previous generation)
Motion: subtle breathing rise/fall of chest, small head tilt
        and right hand twitch, weight shift between feet every 1.5s
Style: relaxed, friendly, casual stance, slight smile mood
Output: GLB animation track only (compatible with provided rig)
Frame rate: 30 fps
```

### Acceptance
- [ ] Loop seamless (start frame == end frame pose)
- [ ] Длительность 2-3 c
- [ ] Импорт в Godot, AnimationPlayer проигрывает

---

## Промпт 3 · Walk Animation (P0-033)

### Контекст
Цикл ходьбы для перемещения по карте. Должен сочетаться со speed ~2 unit/sec.

### Промпт

```
Animation: walk cycle loop, 1 second duration, looping seamlessly
Character: humanoid chibi (use rig from previous generation)
Motion: relaxed walking gait, gentle arm swing opposite to legs,
        slight head bob, cheerful pace (not running)
Stride: medium step length, ~1.5 steps per second
Style: friendly stride suitable for a chibi farmer carrying nothing
Output: GLB animation track only
Frame rate: 30 fps
```

### Acceptance
- [ ] Loop seamless
- [ ] Скорость соответствует движению ~2 unit/sec при stride ~1.0 m
- [ ] Импорт в Godot, AnimationPlayer проигрывает

---

## Workflow (пошагово)

### 1. Регистрация (P0-030)
1. `https://www.3daistudio.com/`
2. Создать аккаунт
3. Выбрать тариф (free trial для теста, paid если нужно для GLB-экспорта или коммерч. использования)
4. Проверить лицензию на коммерческое использование — задокументировать в `docs/architecture/08_asset_pipeline.md` (P0-034)

### 2. Генерация (P0-031)
1. Вставить **Промпт 1** в 3DAI
2. Получить результат, скачать (GLB / FBX + diffuse texture)
3. Если результат непригоден — итерация промпта; max 3 попытки, иначе fallback на Mixamo + ручной концепт

### 3. Blender чистка (по `tools/asset_qa_checklist.md`)
- Decimate до 800 tris
- Применить scale, transform
- Origin в ногах
- Переименовать кости в стандарт
- UV нормализовать
- Объединить материалы в один (для batching)
- Экспортнуть GLB в `assets/characters/player_avatar/v0/player_avatar.glb`

### 4. Анимации (P0-033)
**Вариант A:** генерация в 3DAI (промпты 2, 3) если функция доступна

**Вариант B (fallback):** Mixamo
1. `https://www.mixamo.com/` — загрузить GLB модели
2. Auto-rig
3. Применить анимации "Idle" + "Walking"
4. Download → FBX with animation
5. Blender: импорт, очистка, экспорт каждой анимации как отдельный GLB

### 5. Импорт в Godot
1. Скопировать GLB в `assets/characters/player_avatar/v0/`
2. Открыть Godot — auto-import
3. В импорт-настройках: Loop on import для idle/walk
4. Создать сцену `scenes/entities/player_avatar.tscn` (или временно — в world)
5. Проверить визуал, проиграть анимацию через AnimationPlayer

### 6. README для версии
Создать `assets/characters/player_avatar/v0/README.md`:
- Дата генерации
- Использованный промпт (точный)
- Источник анимаций (3DAI / Mixamo)
- Известные проблемы
- Что изменилось vs предыдущая версия

---

## Бюджет времени

| Шаг | Время (solo) |
|-----|--------------|
| Регистрация + тариф | 30 мин |
| Промпт 1 + 1-3 итерации | 1-2 ч |
| Скачивание + сохранение | 15 мин |
| Blender чистка | 1-2 ч |
| Анимации (3DAI) | 1 ч |
| Анимации (Mixamo fallback) | 1 ч |
| Импорт в Godot + тест | 30 мин |
| README документация | 15 мин |
| **Итого** | **5-7 часов** (1 день solo) |

## Бюджет денег (ориентир)

- 3DAI Studio: бесплатный trial / paid от $0-30/мес зависит от тарифа
- Mixamo: бесплатно
- Blender: бесплатно

Если 3DAI Studio commercial license требует paid plan — обязательно. Если результат слабый — посчитать концепт-арт фрилансера ($50-150 за персонажа) как опцию.

---

## Критерии успеха Phase 0 (E4)

E4 считается завершённым, когда:

- [ ] Аккаунт 3DAI активен (P0-030)
- [ ] Player avatar GLB в репо, проигрывается в Godot world scene (P0-031)
- [ ] `tools/asset_qa_checklist.md` существует и применён к модели (P0-032)
- [ ] Walk + idle анимации проигрываются (P0-033)
- [ ] `docs/architecture/08_asset_pipeline.md` обновлён с условиями лицензии (P0-034)
- [ ] Решение задокументировано: продолжаем 3DAI или fallback (Mixamo / art outsource)

---

## После Phase 0 — что генерируем в Phase 1-2

### Phase 1 (P1-020 player + 4 NPC проверка)
- Player avatar: добавить анимации `work_harvest`, `carry_idle`, `carry_walk`, `deliver`, `celebrate`
- Тест-NPC Gatherer Farmer (отличается outfit/цветом, тот же rig)

### Phase 2 (P2-010..P2-011)
- 4 Gatherer: Farmer, Lumberjack, Miner, Fisher
- 2 Carrier: Porter, Merchant
- Все с полным набором анимаций (см. `08_asset_pipeline.md`)

### Phase 5 (P5-070..P5-071)
- Specialists (3-5 шт)
- Managers (4 шт)
- Cosmetics variants
