# 06 · Арт-направление

## Стилевой стержень

- **Аниме / chibi** — мягкие пропорции, большие глаза, выразительные эмоции
- **Low-poly mobile** — для гейм-пея: 500-1000 трисов на NPC, без сложного PBR
- **Hi-res portraits** — для UI: рисованный аниме-стиль 1024×1024
- **Light & cheerful** — яркие цвета, насыщенность, ощущение прогресса

## Референсы

- **Стиль NPC:** *Animal Crossing*, *Ni no Kuni*, *Genshin* chibi-аватары
- **Сцены:** *Lumber Empire*, *Idle Miner Tycoon* — пайплайн, *Farm Town* — палитра
- **UI:** *Honkai*, *Princess Connect* — цветовые акценты, рамки карточек

## Цветовая палитра (базовая)

| Роль | Hex | Использование |
|------|-----|---------------|
| Primary | `#FFB84D` | Gold-related, акценты |
| Secondary | `#5BC0EB` | Gems, премиум |
| Accent green | `#7FD56F` | Успех, апгрейд доступен |
| Accent red | `#EF6F6C` | Ошибка, недоступно |
| Background warm | `#FFF4E0` | Земля, фон UI |
| Background cool | `#E8F4F8` | Небо, переходы |
| Text dark | `#2D2A2E` | Основной текст |
| Text light | `#FFFFFF` | На тёмных фонах |
| UI grey | `#9C9398` | Disabled, второстепенное |

Регионы — собственные palette-варианты:
- Mountain: холодные синие + серые
- Coast: бирюза + песочный
- Desert: терракот + охра
- Magical: фиолетовый + неон-розовый
- Tech: серебро + неон-синий

## Типографика

- **Latin:** Nunito (Google Fonts) — мягкий, читаемый
- **CJK:** Noto Sans CJK
- **Cyrillic:** Nunito (есть кириллица)
- **Числа:** Tabular-figures для счётчиков
- **Иерархия:** см. `05_ui_ux_spec.md` (шрифт-размеры)

## Анимэ-портреты (UI)

- Формат: 1024×1024 PNG, без фона (либо мягкий gradient)
- 3-4 эмоции на персонажа: neutral, happy, surprised, sad
- Стиль: detailed, soft-shading, без heavy outline

Используются в:
- NPC roster
- Магазин персонажей
- Награды событий
- Story dialogues (если будут)

## Промпты 3DAI Studio

### Шаблон базового NPC

```
Style: chibi anime, low-poly 3D game model, 500-1000 triangles, soft shading
Character: <gender> <profession> in <era> setting
Pose: T-pose, ready for rigging
Outfit: <description>, <2-3 specific items>
Colors: <hex1>, <hex2>, <hex3>
Accessory: <tool/item in hand or attached>
Texture: 512×512 diffuse only, no PBR
Camera: front 3/4 view, neutral lighting
Output: GLB with skeleton, suitable for Mixamo retargeting
```

### Пример: Gatherer Farmer

```
Style: chibi anime, low-poly 3D game model, ~800 triangles, soft shading
Character: friendly young woman peasant in medieval European setting
Pose: T-pose, ready for rigging
Outfit: simple linen dress in warm beige, brown apron, leather sandals, straw hat
Colors: #D4A574 dress, #6B4226 apron, #FFD66B straw
Accessory: small wooden sickle in right hand strap
Texture: 512x512 diffuse, no PBR maps
Camera: front 3/4 view, soft neutral lighting
Output: GLB with humanoid skeleton
```

### Пример: Manager Production (UI-only, портрет)

```
Style: detailed anime portrait, soft shading, no heavy lineart
Character: confident male overseer, late 30s, slightly idealistic vibe
Outfit: tailored vest, white shirt, dark trousers, simple cap
Expression: friendly determined smile (neutral)
Background: transparent or soft gradient orange→cream
Output: 1024x1024 PNG, character bust framed
```

## Анимации

Из `architecture/08_asset_pipeline.md`. Список по NPC:

| Анимация | Длительность | Loop |
|----------|--------------|------|
| `idle` | 2-3 c | да |
| `walk` | 1 c (cycle) | да |
| `run` (опц.) | 0.8 c | да |
| `work_harvest` | 1-2 c | да |
| `work_chop` | 1-2 c | да |
| `work_mine` | 1-2 c | да |
| `carry_idle` | 2 c | да |
| `carry_walk` | 1 c | да |
| `deliver` | 1 c | нет |
| `celebrate` | 1.5 c | нет |

Для Player avatar:
- Все NPC анимации + `tap_response` (короткое weave при тапе на здание)
- Costume-варианты: prestige badge, speed outfit, capacity backpack

## Здания

- Стиль: low-poly stylized, soft edges, минимум деталей
- 4-5 визуальных тиров: каждый следующий — больше деталей, новые элементы
- Анимация работы: вращение колеса (мельница), дым из трубы (пекарня), мерцание окон

Источник: ручная Blender или asset-pack (CC0 / купленный).

## Окружение

- Базовая деревня — flat terrain, лёгкие холмы, деревья stylized
- Skybox — gradient sky с парой облаков
- Цикл дня/ночи: опционально на Phase 5 (визуальный, без gameplay-эффекта)
- Растительность — billboard sprites или very low-poly

## Эффекты

- Floating text: цветной (gold yellow, exp blue, gem cyan)
- Particles: монетки при продаже, искры при апгрейде, листва при сборе
- Используем `CPUParticles3D` (Compatibility renderer)
- Эффекты — короткие, не отвлекающие

## UI-графика

- Иконки — flat с лёгким shading, 64×64 source
- Атлас на ~30 иконок
- Рамки карточек — rounded 12pt + soft shadow

## Аудио-визуал синхронизация

См. `07_audio_design.md`. Каждое визуальное событие имеет SFX.

## Открытые вопросы

- [ ] Concept art перед 3DAI (помогает консистентности) — нужен ли?
- [ ] Стиль-гайд PDF — формат и owner
