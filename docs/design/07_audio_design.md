# 07 · Аудио-дизайн

## Источник

`GDD/2_auditory.md`. Здесь — конкретные треки, SFX, шины.

## Аудио-шины (AudioBus)

```
Master
├── Music
│   ├── BGM
│   └── Stinger (короткие импульсы при событиях)
├── SFX
│   ├── World (тапы зданий, NPC работа)
│   ├── UI (кнопки, попапы)
│   └── Currency (gold, gems)
├── Ambient (фон деревни, природа)
└── Voice (опционально, для tutorial)
```

Каждая шина — отдельный slider в Settings.

## Музыка

### Стиль
- Лёгкий J-pop / orchestral fusion
- Acoustic instruments: pipa, koto, ukulele, маримба, светлый strings
- Темп 80-110 BPM (расслабленный)
- Мажор, мажор, иногда модальные модуляции (Lydian, Dorian)

### Треки MVP
| Track | Length | Использование |
|-------|--------|---------------|
| `bgm_village_main.ogg` | 2-3 мин loop | Главная деревня |
| `bgm_event.ogg` | 2 мин | Активный ивент |
| `bgm_prestige.ogg` | 1 мин | Окно престижа (cinematic) |
| `bgm_shop.ogg` | 1.5 мин | Меню магазина |

### Треки post-launch (по регионам)
- `bgm_mountain.ogg` — холодные обертоны
- `bgm_coast.ogg` — волны, морской бриз
- `bgm_desert.ogg` — экзотические перкуссии
- `bgm_magic.ogg` — глитч-pad + sparkle
- `bgm_tech.ogg` — synth + сложный ритм

## SFX

### World
| ID | Источник | Длительность |
|----|----------|--------------|
| `tap_building.ogg` | Тап здания | 100 мс |
| `npc_walk.ogg` | Шаги (рандомный pitch) | 50 мс |
| `npc_work_harvest.ogg` | Сбор пшеницы | 300 мс |
| `npc_work_chop.ogg` | Рубка дерева | 400 мс |
| `npc_work_mine.ogg` | Кирка | 350 мс |
| `npc_deliver.ogg` | Положил ресурс | 200 мс |
| `building_upgrade.ogg` | Апгрейд | 600 мс |
| `building_new_tier.ogg` | Открыт новый тир | 1.0 с |

### UI
| ID | Использование |
|----|---------------|
| `ui_tap.ogg` | Тап кнопки |
| `ui_modal_open.ogg` | Открыл попап |
| `ui_modal_close.ogg` | Закрыл попап |
| `ui_purchase.ogg` | Успешная покупка |
| `ui_error.ogg` | Не хватает gold |
| `ui_tab_switch.ogg` | Переключение таба |

### Currency
| ID | Использование |
|----|---------------|
| `gold_drop.ogg` | Получил gold |
| `gold_big_drop.ogg` | Получил много gold (продажа партии) |
| `gem_drop.ogg` | Получил gem |
| `level_up.ogg` | Поднял уровень |

### Stingers (короткие импульсы)
| ID | Использование |
|----|---------------|
| `stinger_achievement.ogg` | Открыл ачивку |
| `stinger_prestige.ogg` | Запустил престиж |
| `stinger_event_start.ogg` | Начало ивента |
| `stinger_quest_complete.ogg` | Завершил квест |

## Формат файлов

- BGM: Ogg Vorbis, 128 kbps, mono для маленьких треков, stereo для main
- SFX: Ogg Vorbis, 96 kbps, mono (за редким исключением)
- Короткие UI SFX: WAV PCM 16-bit (быстрее decode, нет latency)

## Динамическая адаптация

- Музыка адаптируется к фазе игры (опционально):
  - Спокойная — при низкой активности
  - Энергичная — после первого апгрейда / во время ивента
- Громкость BGM auto-duck при модалках (на -6 dB)

## Микширование

- LUFS: -16 (мобильный стандарт)
- True Peak: -1 dB
- Bass roll-off ниже 80 Гц (мелкие динамики телефона)

## Размер бюджета аудио

- BGM (4 трека MVP) ≈ 5-7 МБ
- SFX (~50 файлов) ≈ 2-3 МБ
- Итого: ~10 МБ

## Источники музыки

- Опц. фриланс composer для MVP (бюджет ~$500-2000)
- AssetStore / Sounds.com / Epidemic Sound — для прототипа и backup
- AI generation (Suno, Udio) — экспериментально, проверять лицензии

## Голос / диалоги

- Tutorial голос — нет (только текст). Озвучка слишком дорого для MVP
- Reactive «hai!», «yatta!» для NPC при выполнении задач (короткие сэмплы, безоязычные) — на Phase 5

## Открытые вопросы

- [ ] Композитор: in-house, фриланс, asset-pack?
- [ ] Адаптивные слои BGM (HTI-стиль) — приоритет post-launch?
- [ ] Озвучка NPC (короткие emotes) — стоимость / выгода
