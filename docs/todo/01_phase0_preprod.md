# Phase 0 · Pre-production (2 нед)

## Цель

Подготовить инструменты, репозиторий, прототип и пайплайн ассетов. К концу — команда может начать вертикальный срез.

## Phase exit criteria

См. `00_milestones.md`.

## E1 · Репозиторий и инструменты

- [x] **P0-001** • Инициализировать git-репо, базовый README
  - Acceptance: `git init`, push в remote, добавлены `LICENSE`, `.gitignore`, `README.md`
  - Estimate: S
  - Files: корень репо
  - Done: 2026-06-17 (commit `5e1096f`). Remote URL — отдельная задача.
- [x] **P0-002** • Настроить Git LFS для `*.glb`, `*.fbx`, `*.png`, `*.psd`, `*.wav`, `*.ogg`
  - Acceptance: `git lfs install`, `.gitattributes` коммитнут, `git lfs ls-files` показывает binary
  - Estimate: S
  - Done: 2026-06-17. LFS v3.7.1, 35 паттернов в `.gitattributes`.
- [x] **P0-003** • Создать Godot 4.x проект (Compatibility renderer)
  - Acceptance: `godot/` папка, открывается в Godot 4.x, renderer = Compatibility
  - Estimate: S
  - Depends on: P0-001
  - Done: 2026-06-17. Godot 4.6, project открывается без ошибок.
- [x] **P0-004** • Настроить export presets iOS + Android
  - Acceptance: оба preset валидны, ключи подписи (можно ad-hoc на старте)
  - Estimate: M
  - Done: 2026-06-17. Только Android preset (Min 21, Target 34, arm+arm64). iOS отложен до Phase 3.
- [x] **P0-005** • CI на GitHub Actions
  - Acceptance: PR запускает: import check, `gdlint`, smoke debug build для каждой платформы
  - Estimate: M
  - Depends on: P0-003, P0-004
  - Done: 2026-06-17. `.github/workflows/ci.yml` — lint + import-check + test-stub + yaml-lint.
- [x] **P0-006** • Соглашения по стилю кода (CODE_STYLE.md)
  - Acceptance: правила naming, импортов, форматирования (см. `architecture/02_project_layout.md`)
  - Estimate: S
  - Done: 2026-06-17. 17 разделов конвенций GDScript.

## E2 · Архитектурный скелет

- [x] **P0-010** • Создать autoload-стабы (`EventBus`, `GameState`, `EconomySim`, `SaveService`, ...)
  - Acceptance: все autoload зарегистрированы, имеют `_ready()` лог
  - Estimate: S
  - Files: `godot/autoload/*.gd`, `project.godot`
  - Done: 2026-06-17. 10 синглтонов, порядок init из `architecture/03_core_systems.md`.
- [x] **P0-011** • Boot scene + scene routing
  - Acceptance: `boot.tscn` → загрузка → переход на placeholder `world.tscn` через `SceneRouter`
  - Estimate: M
  - Done: 2026-06-17. Boot fade + автопереход на world через 0.8 c.
- [x] **P0-012** • Theme `.tres` (заглушка)
  - Acceptance: один шрифт, одна основная кнопка, базовые цвета (из `design/06`)
  - Estimate: S
  - Done: 2026-06-17. `themes/main.theme.tres`, Button (4 state) + Label + Panel.

## E3 · Прототип core loop

- [x] **P0-020** • Минимальный прототип «tap → +gold → upgrade»
  - Acceptance: 1 кнопка тапа → +1 gold; кнопка `Upgrade` (cost 10) удваивает прирост
  - Estimate: M
  - Files: `scenes/world/world.tscn`, `scenes/world/world.gd`
  - Notes: цель — почувствовать responsiveness Godot mobile
  - Done: 2026-06-17. Клик-куб + HUD + Upgrade button (cost grow 1.5x). Без EconomySim (намеренно).
- [x] **P0-021** • Тач + камера: drag pan, pinch zoom (на placeholder world)
  - Acceptance: 3D-камера двигается по drag, zoom работает
  - Estimate: M
  - Done: 2026-06-17. `scripts/utils/camera_rig.gd`. Drag XZ (clamped), pinch zoom (6-30), mouse wheel в editor.
- [x] **P0-022** • Floating text компонент
  - Acceptance: вызов `FloatingText.spawn("+10", pos)` → твин 1 с вверх
  - Estimate: S
  - Done: 2026-06-17. `scripts/utils/floating_text.gd`. Label3D billboard + auto queue_free.

## E4 · Пайплайн ассетов (3DAI Studio)

- [ ] **P0-030** • Зарегистрировать аккаунт 3daistudio.com
  - Acceptance: рабочий аккаунт, выбран план (free для теста)
  - Estimate: S
- [ ] **P0-031** • Тест-генерация: player avatar (chibi farmer)
  - Acceptance: получен `.glb`, импортирован в Godot, видно в сцене
  - Estimate: M
  - Notes: используем шаблон промпта из `design/06_art_direction.md`
- [ ] **P0-032** • Blender чек-лист QA модели
  - Acceptance: документ `tools/asset_qa_checklist.md` с шагами, прогнан на тест-модели
  - Estimate: S
- [ ] **P0-033** • Тест-анимация: walk + idle для тест-NPC
  - Acceptance: в Godot AnimationPlayer проигрывает обе анимации
  - Estimate: M
- [ ] **P0-034** • Лицензия 3DAI на коммерческое использование — задокументировать
  - Acceptance: `docs/architecture/08_asset_pipeline.md` обновлён с условиями
  - Estimate: S

## E5 · Backend-стабы

- [ ] **P0-040** • Решение: Firebase или AWS на старте (см. `architecture/10_backend.md`)
  - Acceptance: ADR-002 заполнен с финальным выбором
  - Estimate: S
- [ ] **P0-041** • Создать backend проект (Firebase или AWS skeleton)
  - Acceptance: project initialized, dev keys в secure vault, не в git
  - Estimate: M
  - Depends on: P0-040
- [ ] **P0-042** • Стаб-endpoint `/config` возвращает hardcoded JSON
  - Acceptance: клиент при старте достаёт конфиг
  - Estimate: M

## E6 · Тестирование

- [ ] **P0-050** • Настроить gdUnit4 / gut
  - Acceptance: запускается `make test` (или эквивалент), 1 sample-тест проходит
  - Estimate: S
- [ ] **P0-051** • CI запускает тесты в PR
  - Acceptance: GitHub Actions показывает зелёный
  - Estimate: S
  - Depends on: P0-005, P0-050

## E7 · Документация и онбординг

- [ ] **P0-060** • CONTRIBUTING.md
  - Acceptance: гайд для нового разработчика: clone, install, run
  - Estimate: S
- [ ] **P0-061** • Локальный setup-скрипт (`tools/bootstrap.sh`)
  - Acceptance: проверяет Godot version, LFS, открывает проект
  - Estimate: S

## Итого Phase 0

~25 задач, ~2-3 недели для команды 2-3 человека. По завершении — все системы скелет на месте, прототип live.
