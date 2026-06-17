# Contributing — Anime Empire

Гайд для нового разработчика на проекте.

## 1 · Стек

- **Движок:** Godot 4.x (Mobile / Compatibility renderer, GLES3)
- **Язык:** GDScript 2.0
- **OS:** macOS / Linux / Windows
- **Git:** + Git LFS (для бинарных ассетов)
- **Python 3.11+:** для tooling (`gdtoolkit`)
- **Blender 4.x:** для очистки 3D-ассетов (опционально)

## 2 · Setup за 5 минут

```bash
# 1. Клонировать
git clone <repo-url>
cd AnimeEmpire

# 2. Установить инструменты
make bootstrap
# или вручную:
brew install git-lfs godot                  # macOS
git lfs install
pip install gdtoolkit==4.*

# 3. Подтянуть LFS-ассеты
git lfs pull

# 4. Открыть проект
godot godot/project.godot
# или вручную: открой Godot, "Импорт" → выбери godot/project.godot
```

## 3 · Запуск проекта

- **В Godot:** жми **F5** (запустить весь проект) или **F6** (только текущая сцена)
- **Headless (CI / build):** `make import` затем `godot --headless --path godot`

## 4 · Daily commands

```bash
make test           # запустить unit-тесты
make lint           # gdlint
make format         # gdformat (форматирует код)
make format-check   # gdformat --check (не правит)
make clean          # удалить .godot/imported кеш
```

## 5 · Структура репозитория

```
AnimeEmpire/
├── GDD/             ← геймдизайн (read-only от кода)
├── docs/            ← архитектура / дизайн / TODO
│   ├── README.md         ← начни отсюда
│   ├── architecture/
│   ├── design/
│   └── todo/
├── godot/           ← Godot 4.x проект
│   ├── autoload/         ← синглтоны (EventBus, GameState, ...)
│   ├── scenes/           ← .tscn сцены
│   ├── scripts/          ← логика по слоям
│   ├── resources/        ← .tres data
│   ├── assets/           ← LFS: 3D / 2D / аудио
│   ├── themes/
│   ├── locales/
│   └── tests/            ← unit-тесты
├── tools/           ← bootstrap.sh, 3DAI brief, QA checklist
├── .github/         ← CI (GitHub Actions)
├── CODE_STYLE.md    ← конвенции GDScript
├── CONTRIBUTING.md  ← этот файл
├── Makefile         ← daily commands
└── README.md
```

## 6 · С чего начать

1. Прочитай **`docs/README.md`** — навигация
2. **`docs/architecture/00_overview.md`** — высокоуровневая архитектура
3. **`docs/architecture/02_project_layout.md`** — структура Godot-проекта
4. **`CODE_STYLE.md`** — конвенции GDScript
5. **`docs/todo/00_milestones.md`** — карта фаз
6. **`docs/todo/01_phase0_preprod.md`** (или актуальная фаза) — конкретные задачи

## 7 · Workflow

### Брать задачу

1. Открыть `docs/todo/<фаза>.md`
2. Выбрать `[ ]` (pending) задачу из текущей фазы
3. Если в команде — спросить в чате / claim в tracker (если используется)

### Делать задачу

1. Создать feature-ветку: `git checkout -b feat/<id>-<short-name>` (например `feat/p1-024-npc-fsm`)
2. Писать код согласно `CODE_STYLE.md`
3. Перед commit: `make lint && make format-check && make test`
4. Commit format: Conventional Commits (`feat:`, `fix:`, `refactor:`, ...) с ID задачи в скобках
   ```
   feat: add NPC behavior tree (P1-024)
   ```
5. Push, открыть PR
6. После merge → пометить задачу `[x]` в `docs/todo/` с датой

### Code review

- Минимум 1 ревьюер (если в команде)
- CI должен быть зелёным
- Документация обновлена, если меняешь архитектуру или GDD-связанное поведение

## 8 · Что делать с ошибками

### Godot не открывает проект
- Проверь версию: `godot --version` — должна быть 4.x
- Перенакопировать кеш: `make clean && make import`
- Проверь `godot/project.godot` — должен быть валидный

### LFS файлы пустые / `pointer` вместо bytes
- `git lfs install`
- `git lfs pull`
- Проверь, что `git-lfs` v2.13+

### Тесты падают
- Запусти локально `make test` — смотри Output
- Логи в `godot/.godot/` (включи verbose в project.godot если надо)

### CI красный
- Жми на failed job в GitHub Actions → Logs
- Чаще всего: `gdformat` (запусти `make format` локально)
- Или: missing test (запусти `make test` локально)

## 9 · Конвенции

### Файлы и naming
См. `CODE_STYLE.md` и `docs/architecture/02_project_layout.md`.

### Commit messages
Conventional Commits с ID задачи:
```
<type>: <subject> (<task-id>)

<тело — почему, не что>
```

Типы:
- `feat:` — новая фича
- `fix:` — баг-фикс
- `refactor:` — без изменения поведения
- `docs:` — только документация
- `test:` — только тесты
- `chore:` — инфраструктура, билд, deps
- `perf:` — оптимизация
- `style:` — форматирование

### Ветки
- `main` — стабильно, защищён
- `feat/<id>-<short-name>` — фича
- `fix/<id>-<short-name>` — баг
- `chore/<id>-<short-name>` — инфра

### PR
- Title формата как commit
- Body: что сделано, как тестилось, что проверить
- Link на ID в todo
- Скриншоты / видео для UI-изменений

## 10 · Asset workflow

Подробно: `docs/architecture/08_asset_pipeline.md`, `tools/3dai/phase0_brief.md`.

Кратко для новых 3D-ассетов:
1. Промпт в `tools/3dai/<категория>.md`
2. Генерация через 3DAI Studio / Meshy
3. Blender чистка по `tools/asset_qa_checklist.md`
4. Положить в `godot/assets/characters/<id>/v<N>/`
5. README.md в папке версии (промпт + дата + проблемы)
6. Импортировать в Godot, проверить

## 11 · Платформенный билд

### Android
1. Установить Android Studio + SDK
2. Editor → Editor Settings → Export → Android — указать пути
3. Создать debug keystore (см. `godot/export_presets.cfg`)
4. Project → Export → Android Debug → Build

### iOS (с Phase 3)
- Apple Developer Program нужен ($99/год)
- Подробности — `docs/todo/05_phase4_launch.md`

## 12 · Помощь

- **Документация:** `docs/`
- **Tickets/issues:** GitHub Issues (когда заведём)
- **Slack/Discord:** TBD

## 13 · Лицензия

MIT (см. `LICENSE`). Контрибьюторы автоматически соглашаются на лицензирование своего вклада под теми же условиями.
