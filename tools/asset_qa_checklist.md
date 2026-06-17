# Asset QA Checklist (Blender → Godot)

Чек-лист для проверки 3D-моделей персонажей перед коммитом в `assets/characters/`. Применяется к каждому новому ассету или новой версии (`vN/`).

Источник стандартов: `docs/architecture/08_asset_pipeline.md`.

---

## A · Геометрия

- [ ] **Tris ≤ 1000** (для gameplay-моделей; 1500 макс для героев)
  - Проверка: `Statistics` в Blender top-right corner
  - Исправление: `Decimate` modifier (Ratio 0.5-0.8)
- [ ] **Нет non-manifold геометрии**
  - Edit mode → `Select → All by Trait → Non Manifold`
  - Должно быть 0 selected
- [ ] **Нет дубликатов вершин**
  - Edit mode → `Mesh → Merge → By Distance` (default 0.0001)
- [ ] **Нормали направлены наружу**
  - Edit mode → `Mesh → Normals → Recalculate Outside`
- [ ] **Нет zero-area faces**
  - `Select → All by Trait → Faces by Sides → 3` (или другие inval polygons)

## B · Трансформации

- [ ] **Scale = (1, 1, 1)** на объекте
  - Object mode → `Object → Apply → Scale` если иначе
- [ ] **Rotation = (0, 0, 0)** на объекте
  - `Object → Apply → Rotation`
- [ ] **Origin в ногах модели** (X=0, Z=0, Y=0)
  - Edit mode → выделить вершины ступней → `Object → Set Origin → Origin to 3D Cursor` (3D cursor предварительно в (0,0,0))
- [ ] **Масштаб 1 unit = 1 m**
  - Для chibi-героя рост ~1.5 m
  - Для NPC ~1.4-1.6 m
- [ ] **Стоит на плоскости Y=0** (ноги касаются ground)

## C · UV

- [ ] **UV развёрнут**
  - UV Editor → проверить, что вершины разнесены, не схлопнуты в точку
- [ ] **Нет перекрывающихся UV** (overlap)
  - Edit mode → UV Sync Selection → проверить визуально
- [ ] **UV в пределах [0, 1]** (без выхода за квадрат)
- [ ] **Один UV map на mesh** (не несколько)

## D · Материалы

- [ ] **Один материал на персонажа** (для batching)
  - Если несколько — combine в Blender
- [ ] **Текстура 512×512 diffuse** (PNG)
  - Меньше — для distant LOD; больше только если обоснованно
- [ ] **Без PBR maps** (no metalness / roughness / normal — целевой Compatibility renderer)
- [ ] **sRGB color space** для diffuse

## E · Skeleton / Rig

- [ ] **Skeleton привязан** (Armature parent to mesh)
- [ ] **Bones названы по стандарту** (Mixamo-compatible):
  ```
  Hips · Spine · Chest · Neck · Head
  UpperArm.L · LowerArm.L · Hand.L
  UpperArm.R · LowerArm.R · Hand.R
  UpperLeg.L · LowerLeg.L · Foot.L
  UpperLeg.R · LowerLeg.R · Foot.R
  ```
  - Если другая — Blender Rename Bones / Rigify retargeting
- [ ] **Roll правильный** на костях (без флипов при анимации)
- [ ] **Weights нормализованы** (sum = 1.0 на каждой вершине)
  - Weight Paint → `Normalize All`
- [ ] **Нет weights на «вырожденных» вершинах** (ear / tail для базовой модели)
- [ ] **T-pose в Rest** (Armature → Edit mode → Pose at T)

## F · Анимации (если есть)

- [ ] **Loop seamless** для idle / walk / run / carry_*
  - Поза первого кадра === поза последнего
- [ ] **Длительности соответствуют tайному дизайну**:
  - idle: 2-3 c
  - walk: 1 c (cycle)
  - run: 0.8 c
  - work_*: 1-2 c
  - celebrate: 1.5 c (non-loop)
- [ ] **Frame rate 30 fps** (стандарт для mobile)
- [ ] **Имена анимаций** соответствуют стандарту:
  ```
  idle | walk | run | work_harvest | work_chop | work_mine
  carry_idle | carry_walk | deliver | celebrate
  ```

## G · Экспорт GLB

- [ ] **Формат:** glTF Binary (`.glb`)
  - `File → Export → glTF 2.0 (.glb/.gltf)`
- [ ] **Settings:**
  - Format: glTF Binary (.glb)
  - Include: Selected Objects (если экспортишь только модель)
  - Transform: Y Up
  - Geometry: UV, Normals — ON; Tangents — OFF (Godot не требует)
  - Animation: All Actions (если включаем все анимации в один файл) или Active Action (если отдельно)
- [ ] **Без камер / источников света** в экспорте (только Armature + Mesh)

## H · Импорт в Godot

- [ ] **Файл открывается без ошибок**
  - Скопировать в `assets/characters/<npc_id>/v<N>/`
  - Godot auto-import; в Output console — нет warnings/errors
- [ ] **Mesh видим в превью**
- [ ] **Material применён**
- [ ] **Skeleton3D виден в Scene tree**
- [ ] **AnimationPlayer** (если анимации в файле) проигрывает корректно

## I · Файловая структура

- [ ] Файл лежит в `assets/characters/<npc_id>/v<N>/`
- [ ] Анимации в `<npc_id>/v<N>/animations/<anim_name>.glb`
- [ ] Diffuse в `<npc_id>/v<N>/diffuse_512.png`
- [ ] `<npc_id>/v<N>/README.md` присутствует:
  - Дата создания
  - Полный использованный промпт
  - Источник анимаций (3DAI / Mixamo / ручной)
  - Известные проблемы
  - Что изменилось vs предыдущей версии

## J · Git LFS

- [ ] `.glb` и `.png` под LFS (см. `.gitattributes`)
- [ ] `git lfs ls-files` показывает добавленные файлы

---

## Quick run-through (быстрая проверка для итераций)

Для быстрых fixes — минимум:

1. Tris ≤ 1000
2. Scale + Rotation applied
3. Origin в ногах
4. Loop seamless для анимаций (если есть)
5. Импорт в Godot без ошибок

Полный чек — перед commit в main.

---

## Если что-то не проходит

| Проблема | Что делать |
|----------|------------|
| Tris > 1500 | Decimate modifier; если артефакты — manual retopology |
| Origin не в ногах | `Set Origin → 3D Cursor` после позиционирования cursor |
| Кости с неправильными именами | Rename вручную или используй Mixamo Auto-Rig |
| Анимация не loop | Скопировать первый кадр в последний |
| Material несколько | Join в Edit mode, переназначить материал на весь mesh |
| Импорт даёт warning | Прочитать в Output; чаще — отсутствующая текстура или non-standard scale |

Если проблема воспроизводится на нескольких ассетах — обновить этот чек-лист.
