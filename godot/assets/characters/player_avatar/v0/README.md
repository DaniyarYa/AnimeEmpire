# Player Avatar · v0

Стартовая версия аватара главного героя. Генерация: **Meshy AI** (как замена / часть пайплайна 3DAI Studio).

## Метаданные

| Поле | Значение |
|------|----------|
| Дата | 2026-06-17 |
| Источник | Meshy AI "Little Explorer biped" template |
| Формат | FBX (содержит mesh + skeleton + animation per файл) |
| Текстуры | 2048×2048 PBR (diffuse + metallic + normal + roughness) |
| Лицензия | Коммерческое использование разрешено (см. `docs/architecture/08_asset_pipeline.md` ADR-003) |
| Phase | 0 — валидация пайплайна |

## Промпт (для воспроизводимости)

См. `tools/3dai/phase0_brief.md` §«Промпт 1».

Реальная генерация:
> Шаблон "Little Explorer biped" с пакетом анимаций (Idle, Walk, Run, Carry,
> Collect, Victory Cheer). Стиль выбран рамкой Meshy AI character preset.

## Структура

```
v0/
├── README.md                 ← этот файл
├── player_avatar.fbx         ← базовая модель (копия idle.fbx)
├── diffuse_2048.png          ← основная текстура (нужен downsample до 512)
├── animations/
│   ├── idle.fbx              ← idle loop (source для базы)
│   ├── walk.fbx              ← цикл ходьбы
│   ├── walk_inplace.fbx      ← walk без forward-смещения (опц.)
│   ├── run.fbx               ← цикл бега
│   ├── carry_walk.fbx        ← ходьба с грузом
│   ├── work_harvest.fbx      ← сбор / "Collect Object"
│   └── celebrate.fbx         ← победа / апгрейд
└── _unused_pbr/              ← PBR карты, Compatibility renderer их не использует
    ├── metallic.png
    ├── normal.png
    └── roughness.png
```

## Mapping анимаций (Meshy → проект)

| Файл проекта | Источник Meshy | Назначение в игре |
|--------------|----------------|-------------------|
| `idle.fbx` | `Idle_4` | NPC / игрок без задачи |
| `walk.fbx` | `Walking` | Перемещение между точками |
| `walk_inplace.fbx` | `walking_2_inplace` | Опц.: walk без forward-движения (для in-place loops) |
| `run.fbx` | `Running` | Опц.: ускоренное перемещение (если будет boost) |
| `carry_walk.fbx` | `Carry_Heavy_Object_Walk` | NPC несёт ресурс |
| `work_harvest.fbx` | `Collect_Object` | Сбор пшеницы / generic gather |
| `celebrate.fbx` | `Victory_Cheer` | Апгрейд, ачивка, prestige |

## Известные проблемы / TODO для Phase 0 imploймент

- [ ] **Текстура 2048×2048** — нужно даунсэмплить до 512×512 (Blender / Photoshop / `magick convert`). По пайплайну `08_asset_pipeline.md`.
- [ ] **Tris count неизвестен** — проверить в Blender, при необходимости decimate до ≤ 1000.
- [ ] **Каждый FBX содержит withSkin** — дублирование mesh в каждом анимационном файле. В Blender pass надо:
  - Использовать `player_avatar.fbx` как master с mesh + skeleton
  - Из остальных файлов экспортнуть только animation tracks (как `.glb` без mesh)
  - Альтернатива: в Godot ImportSettings → `animation/save_to_file` per-file
- [ ] **PBR maps в `_unused_pbr/`** — для Compatibility renderer не нужны. Если позже мигрируем на Forward+ — можем подключить.
- [ ] **Bone names** — проверить совместимость со стандартом из `architecture/08_asset_pipeline.md` (Hips/Spine/Chest/...). Скорее всего Mixamo-стандарт.
- [ ] **Origin модели** — проверить, что в ногах (Y=0), не в hips.
- [ ] **Loop seamless** для idle / walk / run / carry_walk — проверить.

## Конверсия в GLB

Финальный формат по пайплайну — `.glb`. Конверсия в Blender:
1. Открыть `player_avatar.fbx`
2. Применить QA checklist (`tools/asset_qa_checklist.md`)
3. Экспорт → `player_avatar.glb`
4. Для каждой анимации: открыть FBX, экспортнуть только animation track как `.glb`

После конверсии — удалить или архивировать `.fbx` (LFS-place).

## Следующие шаги

1. Открыть в Godot — auto-import должен сработать
2. Создать тест-сцену с player_avatar, проиграть idle / walk
3. Если выглядит OK — пометить P0-031 и P0-033 как done
4. Blender clean-up + GLB конверсия — отдельной задачей (можно отложить до P1-021)
