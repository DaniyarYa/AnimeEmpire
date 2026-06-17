# 00 · Milestones

## Карта фаз

| Фаза | Длительность | Главный артефакт | Файл |
|------|--------------|------------------|------|
| **0 · Pre-production** | 2 нед | Прототип, репо, CI | `01_phase0_preprod.md` |
| **1 · Vertical Slice** | 4 нед | Играбельный кусок (1 цепочка, 1 NPC, 5 мин play) | `02_phase1_vertical.md` |
| **2 · MVP** | 8 нед | Полный играбельный продукт без LiveOps | `03_phase2_mvp.md` |
| **3 · Soft-Launch** | 4 нед | Метрики, A/B, баланс | `04_phase3_soft_launch.md` |
| **4 · Launch** | 2 нед | Глобальный релиз | `05_phase4_launch.md` |
| **5 · Post-Launch** | 6+ мес | События, престиж, регионы | `06_phase5_postlaunch.md` |

Сумма до launch: **~20 недель** (~5 мес) при команде 3-4 человека.

## Критерии выхода фаз

### Phase 0 → Phase 1
- [ ] Godot 4.x mobile проект собирается на iOS и Android
- [ ] Git + LFS настроены
- [ ] CI: lint + import check + smoke build
- [ ] Прототип «tap → +gold → upgrade» играбелен
- [ ] Аккаунт 3DAI Studio активен, тест-NPC сгенерирован
- [ ] Соглашения по стилю кода зафиксированы

### Phase 1 → Phase 2
- [ ] Wheat → Bread цепочка работает 5+ минут без багов
- [ ] 1 NPC (Gatherer Farmer) собирает пшеницу
- [ ] Анимации NPC: idle / walk / work / carry
- [ ] HUD: gold, gems, level показывают данные
- [ ] Сейв/загрузка работают
- [ ] Тач-управление: Virtual Joystick для аватара + pinch zoom камеры работают
- [ ] 60 FPS на iPhone 8 / Galaxy S8

### Phase 2 → Phase 3
- [ ] 8 зданий (4 generator + 4 processor) работают
- [ ] 2 цепочки полностью играемы
- [ ] 6 NPC (4 gatherers + 2 carriers)
- [ ] Туториал интегрирован
- [ ] Локальный сейв + базовый облачный sync
- [ ] Оффлайн-доход рассчитывается
- [ ] Базовая аналитика: lifecycle, progression, tutorial
- [ ] IAP подключены (test покупки работают)
- [ ] Rewarded ads подключены (test)
- [ ] Локализация tier-1 (UI strings)

### Phase 3 → Phase 4
- [ ] Soft-launch в 2-3 странах (Канада, Скандинавия)
- [ ] D1 retention ≥ 35% (target 40%)
- [ ] Crash-free sessions ≥ 99%
- [ ] ARPDAU ≥ $0.05 (target $0.08)
- [ ] Critical balance fixes интегрированы
- [ ] 2 A/B-теста проведены

### Phase 4 → Phase 5
- [ ] Глобальный релиз iOS + Android
- [ ] Полная локализация tier-1
- [ ] Push-уведомления работают
- [ ] Marketing-материалы готовы (screenshot, video)
- [ ] Service status page + crisis playbook

### Phase 5 (continuous)
- [ ] LiveOps календарь работает
- [ ] Prestige + 1 регион активны
- [ ] Подписка введена
- [ ] Daily quests
- [ ] Friends + leaderboards

## График (Gantt-ish)

```
Wk 1-2   Wk 3-6        Wk 7-14            Wk 15-18      Wk 19-20   Wk 21+
[P0]     [P1]          [P2 MVP]           [P3 Soft]     [P4 LNCH]  [P5 ─────────────...]
preprod  vertical      8 buildings        analytics     stores     events / prestige / regions
                       NPC AI             A/B           marketing  subscription
                       save sync          balance       launch     friends
                       monetization stub
```

## Команда (рекомендация)

| Роль | FTE | Phase 0-2 | Phase 3-4 | Phase 5 |
|------|-----|-----------|-----------|---------|
| Программист (lead) | 1 | + | + | + |
| Программист (junior/mid) | 0.5-1 | — | + | + |
| Game designer | 0.5 | + | + | + |
| 3D / 2D artist | 0.5 | + | + | + |
| Audio | 0.2 | freelance | freelance | freelance |
| Backend / DevOps | 0.3 | — | + | + |
| QA | 0.5 | — | + | + |
| Product / Marketing | 0.3 | — | + | + |

Минимальный жизнеспособный состав: **2 человека fulltime + 1 part-time дизайнер + фрилансеры**.

## Бюджет MVP (порядок)

- Подписки и сервисы (AWS, Firebase, 3DAI, asset store): $200-500/мес
- Композитор / audio (одноразово): $1000-3000
- Контракт-арт для UI и портретов: $2000-5000
- Soft-launch UA-тест: $1000-5000
- **Итого MVP cash:** $5K-20K (без зарплат)

## Риски

| Риск | Митигация |
|------|-----------|
| 3DAI результат непригоден для коммерции | Backup: ручной арт-фриланс |
| Godot mobile performance не вытянет | Профайлим на Phase 1, готовы перейти на C# |
| Команда меньше 2 — медленно | Скоуп режется в backlog, MVP минимизируется |
| Apple/Google review reject | Резерв 2 нед на ревизию |
| Локализация не готова к запуску | Запуск с en + 1-2 локалями, остальное обновлением |

## Связь с GDD

- KPI цели — из `GDD/12`
- LiveOps план — из `GDD/10`
- Монетизация — из `GDD/8`
