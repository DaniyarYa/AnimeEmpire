# Phase 1 · Vertical Slice (4 нед)

## Цель

Один полностью играбельный кусок: цепочка пшеница → хлеб, 1 NPC, 5 минут осмысленного геймплея. Доказать концепт, технические допущения и pipeline.

## Phase exit criteria

См. `00_milestones.md`.

## E1 · Core data model

- [x] **P1-001** • Базовый класс `GameResource` (Resource)
  - Files: `scripts/data/game_resource.gd`
  - Estimate: S
  - Done: 2026-06-18. Базовый класс с id + display_name_key.
- [x] **P1-002** • `BuildingDef` ресурс
  - Acceptance: поля как в `architecture/05_data_model.md`
  - Estimate: S
  - Done: 2026-06-18. Все поля включая category, input/output, growth, npc_slots.
- [x] **P1-003** • `ResourceDef` ресурс
  - Estimate: S
  - Done: 2026-06-18.
- [x] **P1-004** • `NPCDef` ресурс
  - Estimate: S
  - Done: 2026-06-18.
- [x] **P1-005** • Создать `.tres` для wheat, flour, bread, wheat_farm, mill, bakery, market, gatherer_farmer
  - Acceptance: 8 файлов в `resources/`, можно открыть в инспекторе
  - Estimate: M
  - Depends on: P1-002..P1-004
  - Done: 2026-06-18. 8 файлов: resources_chain/wheat, flour, bread; buildings/wheat_farm, mill, bakery, market; npcs/gatherer_farmer. Числа из design/02_economy_tuning.md.

## E2 · Симуляция

- [x] **P1-010** • `EconomySim.tick()` с 10 Hz таймером
  - Files: `autoload/economy_sim.gd`
  - Estimate: M
  - Done: 2026-06-18 (фактически в Phase 0 как stub, теперь расширен inventory/gold/sell методами).
- [x] **P1-011** • `ProductionLine` (Resource + рантайм-логика)
  - Acceptance: тик прогрессирует, при достижении 1.0 эмитит `resource_produced`
  - Estimate: M
  - Done: 2026-06-18. Класс `ProductionLine` + 5 unit-тестов (test_production_line.gd).
- [x] **P1-012** • Inventory player (Dictionary {resource_id: amount}, cap)
  - Estimate: S
  - Done: 2026-06-18. `EconomySim._inventory` Dictionary, методы get_inventory/get_inventory_snapshot. Cap пока не реализован (P2-004).
- [x] **P1-013** • Sell action (Market)
  - Acceptance: тап на Market → продаёт всё, gold увеличивается
  - Estimate: M
  - Done: 2026-06-18. `EconomySim.sell_inventory()` + Market в BuildingModal (Sell all wheat/flour/bread).
- [ ] **P1-014** • Building upgrade
  - Acceptance: тап на здание → modal → upgrade → cost списан, level++, output пересчитан
  - Estimate: M
  - Notes: формула `ProductionLine.get_upgrade_cost()` готова + tests (test_building_def.gd). UI кнопка апгрейда в modal — следующая задача.

## E3 · Player + NPC

- [x] **P1-020** • Player avatar 3D + Virtual Joystick управление
  - Files: `scenes/entities/player.tscn`, `scripts/entities/player.gd`, активация `scripts/ui/virtual_joystick.gd`
  - Acceptance: левый палец → joystick управляет аватаром через `CharacterBody3D.velocity`. Тап здания **открывает меню**, не перемещает. Camera follow-mode.
  - Estimate: L
  - Done: 2026-06-18. Player CharacterBody3D с SPEED 4.0, lerp velocity, slerp rotation. Joystick → player.set_movement_direction() через сигнал. Тап здания → Building.clicked → BuildingModal.show_for() (без перемещения игрока).
- [x] **P1-021** • Импорт player модели и анимаций (из Phase 0)
  - Estimate: S
  - Done: 2026-06-18. Player.tscn instance .fbx из Phase 0.
- [x] **P1-022** • Player animation state (idle / walk / carry / work)
  - Estimate: M
  - Done: 2026-06-18. `PlayerAnimationController` — 6 состояний (idle/walk/run/work/carry_walk/celebrate), name hint resolution для FBX armature-prefix имён. Простой `AnimationPlayer.play()` switch (AnimationTree → backlog BL-810).
- [ ] **P1-023** • Player inventory carry visual (нести меш ресурса)
  - Estimate: M
  - Notes: визуал-only задача, отложена. Анимация `carry_walk` готова.
- [ ] **P1-024** • NPC entity + FSM (gatherer)
  - Files: `scenes/entities/npc.tscn`, `scripts/entities/npc.gd`, `scripts/entities/npc_state_machine.gd`
  - Acceptance: NPC автоматически: идёт → собирает → несёт → выгружает → возвращается
  - Estimate: L
- [ ] **P1-025** • NPC импорт модели + анимаций
  - Estimate: M
  - Depends on: P0-031, P0-033

## E4 · Сцена и камера

- [ ] **P1-030** • World scene с базовым лэндшафтом (placeholder ground + skybox)
  - Estimate: M
- [ ] **P1-031** • Размещение зданий (wheat_farm, mill, bakery, market) в сцене
  - Estimate: M
- [ ] **P1-032** • Building scene template (Node3D + Area3D + scripts)
  - Acceptance: тап Area3D эмитит `_on_clicked`
  - Estimate: M
- [x] **P1-033** • Camera follow player (опц. drag для свободного режима)
  - Estimate: M
  - Done: 2026-06-18. `CameraRig` рефакторнут: follow_target + follow_smoothing + pinch zoom (6-30 units) + free_mode_timeout 3s. Drag-pan убран. Mouse wheel zoom для editor.

## E5 · UI HUD

- [ ] **P1-040** • Theme `.tres` (полноценный, не заглушка)
  - Files: `themes/main.theme.tres`
  - Depends on: финальные шрифты и палитра
  - Estimate: M
- [ ] **P1-041** • CurrencyBar widget (gold + gems + level)
  - Estimate: M
- [ ] **P1-042** • Building modal (cost, level, upgrade button)
  - Estimate: L
- [ ] **P1-043** • Floating-text pool на здания (collect / sell)
  - Estimate: S
  - Depends on: P0-022

## E6 · Сейв (минимум)

- [ ] **P1-050** • `SaveService.save()` / `.load()` (JSON, без шифрования)
  - Acceptance: рестарт игры восстанавливает gold, level, building levels
  - Estimate: M
- [ ] **P1-051** • Дебаунс сохранения по `save_dirty`
  - Estimate: S

## E7 · Балансы и проверка

- [ ] **P1-060** • Балансировать первые 10 минут (excel/script)
  - Acceptance: документ `tools/balance_v1.csv`, темп прогрессии описан
  - Estimate: M
- [ ] **P1-061** • Внутренний playtest 5 человек × 30 мин
  - Acceptance: отчёт, что сломано / непонятно
  - Estimate: M
- [ ] **P1-062** • Профайл на iPhone 8 / Galaxy S8
  - Acceptance: 60 FPS на типовой сцене, < 200 МБ RAM
  - Estimate: M

## E8 · Документация

- [ ] **P1-070** • Обновить `design/02_economy_tuning.md` балансовыми числами
  - Estimate: S
- [ ] **P1-071** • ADR: окончательный выбор Beehave vs кастомный FSM
  - Estimate: S
  - Depends on: P1-024

## Итого Phase 1

~30 задач, 4 нед для команды 2-3 человека. По завершении — играбельный prototype, основные технические допущения валидированы.
