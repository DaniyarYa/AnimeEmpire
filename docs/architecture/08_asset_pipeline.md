# 08 · Пайплайн ассетов: 3DAI Studio → Godot

## Цели

- Быстрая генерация большого количества стилистически согласованных персонажей
- Воспроизводимость (тот же промпт → похожий результат)
- Версионирование и совместная работа в малой команде
- Лёгкий рантайм-импорт в Godot (минимум ручного труда)

## Workflow

```
1. Концепт         2. Генерация         3. Чистка           4. Импорт          5. Использование
┌─────────┐    ┌─────────────────┐   ┌─────────────┐   ┌──────────────┐   ┌────────────────┐
│ Промпт  │ ─▶ │ 3daistudio.com  │ ─▶│  Blender    │ ─▶│  Godot       │ ─▶│  In-game NPC/  │
│ + ref   │    │ .fbx / .glb     │   │ rig/anim/UV │   │ .glb import  │   │  Building      │
└─────────┘    └─────────────────┘   └─────────────┘   └──────────────┘   └────────────────┘
   ↑                                       ↑                  ↑
   │                                       │                  │
 design/06_art_direction.md           QA-чеклист         .gitattributes (LFS)
```

## Шаг 1 — Концепт

Source of truth: `design/06_art_direction.md`.

Для каждого нового персонажа:
- ID персонажа (`gatherer_farmer`)
- Категория (Gatherer / Carrier / Operator / Specialist / Manager)
- Базовое описание (200-400 символов): пол, профессия, эпоха, ключевые элементы (одежда, инструмент, аксессуары)
- Палитра (2-3 hex-цвета)
- Референс-арт (опционально 1-2 изображения для style transfer)

## Шаг 2 — Генерация (3DAI Studio)

### Стилевые ограничения
- Стиль: chibi / low-poly anime
- Tris: 500-1000 на gameplay-модель
- Текстуры: 512×512 (diffuse + базовая)
- Без сложного PBR (целевой Compatibility renderer не тянет)

### Промпт-шаблон
См. `design/06_art_direction.md` (раздел «Промпты 3DAI»).

### Что генерируем
- Базовая статическая модель (T-pose)
- Скелет (если 3DAI поддерживает auto-rig — используем)
- Анимации: idle / walk / work / harvest / carry / celebrate
  - Если 3DAI не даёт нужный набор — берём из библиотеки Mixamo (после rig-mapping в Blender)

### Что НЕ генерируем через 3DAI
- Здания (статика) — Blender вручную или asset-store mobile-pack
- UI-иконки, портреты в высоком разрешении — отдельно (DALL-E / Midjourney / арт-команда)
- Окружение, VFX

### Выход
- `<npc_id>_base.fbx` или `.glb`
- `<npc_id>_<anim>.fbx` для каждой анимации
- `<npc_id>_diffuse_512.png`

## Шаг 3 — Чистка (Blender)

Обязательный QA-чек перед коммитом:

- [ ] Геометрия очищена от non-manifold, дубликатов вершин
- [ ] Tris ≤ 1000 (decimate если нужно)
- [ ] Origin в ногах персонажа (0,0,0), масштаб 1:1 (1 unit = 1 м, рост ~1.5 м для chibi)
- [ ] Skeleton использует унифицированный rig (см. ниже)
- [ ] Bone names соответствуют стандарту: `Hips, Spine, Chest, Head, UpperArm.L/R, ...`
- [ ] Анимации имеют loop-friendly start/end
- [ ] UV развёрнут, без перекрытий
- [ ] Только одна материал-группа (для batching)
- [ ] Экспорт в `.glb` (Godot-родной формат, лёгкий)

### Унифицированный rig
Минимальный набор костей (для совместимости анимаций между NPC):
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

Дополнительные кости (для аксессуаров) — опционально, не ломают базовые анимации.

## Шаг 4 — Импорт в Godot

### Файловая структура
```
godot/assets/characters/<npc_id>/
├── v1/
│   ├── <npc_id>.glb           # модель + материалы
│   ├── animations/
│   │   ├── idle.glb
│   │   ├── walk.glb
│   │   ├── work_harvest.glb
│   │   └── celebrate.glb
│   ├── diffuse_512.png
│   └── README.md              # промпт, дата, автор
└── current → v1               # симлинк (или копия) на актуальную версию
```

### Импорт-настройки (Godot)
- Mesh: Save to file, use named skin
- Material: Save to file (для замены текстур потом)
- Animation: Optimize, Loop on import
- Generate Tangents: off для базовой модели

### Готовая `.tscn` сцена
Создаётся одна базовая `npc_<id>.tscn`:
```
CharacterBody3D
├── NavigationAgent3D
├── AnimationPlayer
├── Skeleton3D
│   └── Mesh
└── Anchor (Node3D, для груза)
```

Скрипт `npc.gd` (общий для всех NPC) читает `NPCDef` и подгружает нужную модель.

## Шаг 5 — Использование

В рантайме NPC создаётся через `NPCSystem.spawn(npc_def)`:
```gdscript
var scene = load(npc_def.model_scene.resource_path)  # PackedScene из NPCDef
var npc = scene.instantiate()
npc.apply_def(npc_def)
```

## Версионирование

- `v1/`, `v2/`, … — параллельные версии моделей
- `current/` — указатель на актуальную (для упрощения путей в `NPCDef`)
- `<npc_id>/v<N>/README.md` — обязательно: промпт, дата генерации, автор, что изменилось

## Кеширование и LFS

- Все бинарные ассеты → Git LFS
- `.gitattributes`:
  ```
  *.glb filter=lfs diff=lfs merge=lfs -text
  *.fbx filter=lfs diff=lfs merge=lfs -text
  *.png filter=lfs diff=lfs merge=lfs -text
  *.wav filter=lfs diff=lfs merge=lfs -text
  *.ogg filter=lfs diff=lfs merge=lfs -text
  ```

## Производительность импорта

- Godot пересоздаёт `.import/` при первом открытии — закидываем `.godot/imported/` в gitignore
- Для CI — отдельный кэш `.godot/imported/` по хешу источника

## Высокоразрешённые портреты

Используются в:
- UI коллекции NPC
- Магазин персонажей
- Награды событий

Папка: `godot/assets/portraits/`. Формат: 1024×1024 PNG, lossless. Источник — 3DAI или отдельный AI-арт пайплайн (Midjourney).

## Open-source альтернативы / fallback

Если 3DAI Studio даст плохой результат для конкретного типа:
- Mixamo для анимаций (бесплатно, но не для коммерции — проверить лицензию)
- Sketchfab CC0 модели
- Ручной арт-аутсорс

## Бюджет на MVP

- Player avatar: 1 модель + 8 анимаций
- NPC: 4 модели × 6 анимаций
- Здания: 8 моделей × 4 визуальных тира (без 3DAI)
- Портреты: 4 NPC + игрок = 5 × 1024×1024
- **Итого:** ~50 единиц контента к началу MVP-теста

## Открытые вопросы

- [ ] Лицензионные условия 3DAI Studio на коммерческое использование сгенерированного контента
- [ ] Watermark / водяные знаки в free-tier — нужен ли paid plan
- [ ] Auto-rig 3DAI достаточно хорош или всегда нужен Blender-пас?
