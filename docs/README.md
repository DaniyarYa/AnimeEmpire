# Anime Empire — Документация проекта

Слой проектной документации поверх `GDD/`. GDD — что мы делаем и почему (геймдизайн). `docs/` — как мы это строим (архитектура, дизайн-спеки, план работ).

## Стек

- **Движок:** Godot 4.x (Mobile / Compatibility renderer, GLES3)
- **Платформы:** iOS (приоритет), Android
- **Язык:** GDScript 2.0
- **Бэкенд:** AWS (Cognito + API Gateway + Lambda + PostgreSQL/Redis); для MVP допустим Firebase
- **3D-ассеты:** [3daistudio.com](https://www.3daistudio.com/) → Blender clean-up → Godot `.glb`
- **Источник правды по геймдизайну:** папка `GDD/` (13 файлов)

> **Override GDD.** В `GDD/11_tech_reqs.md` указан Unity URP. Принятое решение — Godot 4.x. Обоснование и последствия см. `architecture/00_overview.md` и `architecture/01_engine_stack.md`.

## Структура

```
docs/
├── architecture/   как построено (15 файлов)
├── design/         как спроектировано (11 файлов)
└── todo/           что и в каком порядке делать (9 файлов)
```

## С чего начать

| Роль | Читать в порядке |
|------|------------------|
| Новый разработчик | `architecture/00_overview.md` → `architecture/02_project_layout.md` → `todo/00_milestones.md` |
| Геймдизайнер | `GDD/1_vision.md` → `design/00_overview.md` → `design/01_core_loop_spec.md` |
| Художник / 3D | `design/06_art_direction.md` → `architecture/08_asset_pipeline.md` |
| Продюсер / PM | `todo/00_milestones.md` → `todo/01_phase0_preprod.md` |
| Бэкенд | `architecture/10_backend.md` → `architecture/06_save_sync.md` → `architecture/07_remote_config.md` |
| QA | `architecture/13_performance.md` → `architecture/14_security.md` → `todo/04_phase3_soft_launch.md` |

## Карта документов

### architecture/
- `00_overview.md` — системная диаграмма, слои, override GDD
- `01_engine_stack.md` — Godot 4.x Compat, целевые ТТХ
- `02_project_layout.md` — структура проекта Godot, naming
- `03_core_systems.md` — autoload-сервисы, EventBus, паттерн узлов
- `04_simulation.md` — производственные цепочки, NPC AI, оффлайн
- `05_data_model.md` — `.tres` ресурсы, схема сейва, миграции
- `06_save_sync.md` — локальный AES-сейв, облачный sync
- `07_remote_config.md` — удалённый конфиг, A/B флаги
- `08_asset_pipeline.md` — 3DAI Studio → Godot
- `09_ui_architecture.md` — Control, тема, локализация, тач
- `10_backend.md` — endpoints, инфраструктура
- `11_analytics.md` — события, KPI, воронки
- `12_monetization.md` — IAP, реклама, подписка, anti-cheat
- `13_performance.md` — бюджеты, профилирование
- `14_security.md` — шифрование, GDPR/COPPA, обфускация

### design/
- `00_overview.md` — связь с GDD
- `01_core_loop_spec.md` — лупы в Godot-сущностях
- `02_economy_tuning.md` — формулы scaling
- `03_npc_design.md` — стат-блоки, AI states
- `04_progression.md` — здания, цепочки, престиж, регионы
- `05_ui_ux_spec.md` — экраны, флоу, wireframes
- `06_art_direction.md` — стиль, промпты 3DAI, палитра
- `07_audio_design.md` — SFX, музыка
- `08_liveops_spec.md` — события, календарь
- `09_meta_progression.md` — престиж, ветки апгрейдов
- `10_localization.md` — i18n в Godot

### todo/
- `README.md` — конвенции
- `00_milestones.md` — карта фаз
- `01_phase0_preprod.md` — препрод (2 нед)
- `02_phase1_vertical.md` — вертикальный срез (4 нед)
- `03_phase2_mvp.md` — MVP (8 нед)
- `04_phase3_soft_launch.md` — софт-лонч (4 нед)
- `05_phase4_launch.md` — релиз (2 нед)
- `06_phase5_postlaunch.md` — пост-лонч (6+ мес)
- `07_backlog.md` — идеи без приоритета

## Конвенции

- Документы — markdown, UTF-8, LF
- Имена файлов: `<NN>_snake_case.md` (нумерация задаёт порядок чтения)
- Ссылки на GDD: `GDD/<N>_<name>.md`
- Решения с большими последствиями фиксируются как ADR-блок внутри соответствующего документа: `> ADR-NNN: <решение>. **Дата:** YYYY-MM-DD. **Причина:** ...`
- Список открытых вопросов — внутри документа в секции `## Открытые вопросы`

## Обновление документов

Документация — часть кода. PR с фичей должен обновить релевантные разделы. Если GDD меняется — синхронизировать `design/` и (если затронуто) `architecture/`.
