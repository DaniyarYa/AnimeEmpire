# 05 · UI/UX Спецификация

## Источник

`GDD/9_uiux.md` + `architecture/09_ui_architecture.md`. Здесь — конкретные экраны, флоу, wireframes.

## Экраны (MVP)

1. **Boot / Splash** — лого, прогресс загрузки
2. **World (Main)** — 3D мир + HUD
3. **Shop** — IAP + офферы
4. **NPC Roster** — список нанятых, найм новых
5. **Upgrades** — игрок stats + tools
6. **Settings** — звук, локализация, аккаунт, приватность
7. **Prestige** (Phase 2) — превью + ветки
8. **Events** (Phase 5) — активные ивенты

## Главный экран (World HUD)

```
┌────────────────────────────────────────────┐
│ 1234g   💎50   ⏱ NEXT 12:34   ⚙️         │ ← top bar
│ ▰▰▰▰▰▱▱▱  Lv 5  280/500 XP                │ ← progress (под top bar)
│                                            │
│                                            │
│                                            │
│             [ 3D WORLD VIEW ]              │
│                                            │
│        🌾farm   🏭mill   🏪market           │
│              (NPC walking)                 │
│                                            │
│                                            │
│                              ┌──────────┐  │
│                              │ +10 gold │  │ ← floating text
│                              └──────────┘  │
├────────────────────────────────────────────┤
│ [Shop] [NPCs] [Upgr.] [Events] [Menu]      │ ← bottom action bar
└────────────────────────────────────────────┘
```

## Окно апгрейда здания (tap-модалка)

```
┌────────────────────────────────────────┐
│ 🌾 Wheat Farm                Lv 5  [X]│
├────────────────────────────────────────┤
│   [ 3D preview здания ]                │
│                                        │
│ Output: 1.4 wheat / sec                │
│ Cycle: 0.71 sec                        │
│                                        │
│ ──── Upgrade to Lv 6 ────              │
│ Cost: 1,200 gold      Available: ✅    │
│ Next: 1.6 wheat / sec (+14%)           │
│                                        │
│ NPC slots: 1/2                         │
│ [🧑‍🌾 Hire Farmer (1000g)]               │
│                                        │
│        [ UPGRADE ]                     │
└────────────────────────────────────────┘
```

## Окно NPC

```
┌────────────────────────────────────────┐
│ NPC Roster                     [X]    │
├────────────────────────────────────────┤
│ Hired (3/8)                            │
│ ┌──┬──┬──┐                             │
│ │🧑‍🌾│🧑‍🌾│🚶‍♂️│                             │
│ └──┴──┴──┘                             │
│                                        │
│ Available to hire                      │
│ ┌─────────────────────────────────┐   │
│ │ 🧑‍🌾 Farmer        1000g  [HIRE]  │   │
│ │ 🪓 Lumberjack    1500g  [HIRE]  │   │
│ │ ⛏️ Miner         2500g  [LOCK]  │   │
│ └─────────────────────────────────┘   │
└────────────────────────────────────────┘
```

## Окно престижа (Phase 2)

```
┌────────────────────────────────────────┐
│ Prestige                        [X]   │
├────────────────────────────────────────┤
│  ⚡ Available: 5 points                │
│                                        │
│  If you prestige now:                  │
│    +6 points to total                  │
│    Permanent +18% income               │
│                                        │
│  Tree:                                 │
│   [INCOME +5%] cost 1 pt   [OWNED]    │
│   [INCOME +5%] cost 2 pts  [BUY]      │
│   [SPEED +3%] cost 1 pt    [OWNED]    │
│   [SLOTS +1]  cost 3 pts   [BUY]      │
│                                        │
│       [ PRESTIGE NOW ]                 │
└────────────────────────────────────────┘
```

## Флоу: первая сессия

```
Splash (2 c)
  ↓
Boot → загрузка
  ↓
Welcome modal: «Привет, ты управляющий!»
  ↓
Tutorial step 1: тап на ферму (стрелка)
  ↓
Tutorial step 2: продай на рынке
  ↓
Tutorial step 3: апгрейдни ферму
  ↓
Tutorial step 4: найми крестьянина
  ↓
Tutorial step 5: открой Mill
  ↓
Onboarded → free play
```

Tutorial — без отдельного режима, всё внутри World HUD с подсветкой нужных элементов.

## Флоу: апгрейд здания

```
[Tap здания] → Building Modal
                  ├── есть gold → [UPGRADE] активна
                  │                  ↓
                  │              анимация → новый меш → close modal
                  └── нет gold → [UPGRADE] disabled
                                  ↓
                                подсказка «нужно ещё 200 gold»
```

## Флоу: покупка IAP

```
[Tap shop / offer]
  ↓
Show shop modal
  ↓
[Tap пакет]
  ↓
Confirm modal "Купить за $2.99?"
  ↓
[OK] → Native platform popup
  ↓
Approved → loader "Validating..."
  ↓
Success → reward modal + sparkles
```

## Флоу: rewarded ad

```
[Игрок видит кнопку "+50% boost"]
  ↓
[Tap]
  ↓
Confirm modal "Посмотреть рекламу за +50% на 15 мин?"
  ↓
[OK] → SDK loading
  ↓
Ad полностью просмотрен
  ↓
Reward grant → floating text "+50% activated for 15:00"
```

## Анимации и обратная связь

| Действие | Feedback |
|----------|----------|
| Tap здания | Scale 0.95 → 1.05 → 1.0 (100 мс) |
| Apgrade | Particle burst + screen shake 50 мс + sound `upgrade` |
| Сбор ресурса | Floating text + sparkle |
| Получение gold | Counter tween + coin sound |
| Получение gem | Counter tween + gem sound |
| Ошибка (не хватает gold) | Red flash на кнопке + sound `error` |
| Notification | Slide-in bottom 300 мс |

Все таймины — fast и нерезкие, idle-jrr ритм.

## Touch behavior

- **Single tap:** primary action
- **Long press (500 мс):** info tooltip
- **Drag empty area:** pan camera
- **Pinch:** zoom камеры
- **Double tap:** center on player

## Размеры элементов (мобиль)

- Tap target: ≥ 44×44 pt
- Bottom bar height: 80 pt (включая safe area)
- Top bar height: 60 pt (включая safe area)
- Padding modal: 16 pt
- Радиусы скругления: 8 pt small / 12 pt medium / 20 pt large

## Шрифты

- Display: 28 pt
- Title: 22 pt
- Subtitle: 18 pt
- Body: 16 pt
- Caption: 14 pt
- Минимум: 14 pt

## Локализация

См. `10_localization.md`. Особо учесть:
- Немецкий: тексты на 30% длиннее → auto-wrap, гибкие контейнеры
- Японский / китайский: нет word-spacing, нужен другой font
- Арабский (post-launch): RTL

## Колдстарт UX

- Splash <2 c
- В оффлайне — играть можно, индикатор «нет сети» в HUD
- Перезапуск без потери прогресса (восстановление сейва)

## Открытые вопросы

- [ ] Landscape mode для tablets — добавлять во flow когда?
- [ ] Глобальный UI sound bus — пока через `AudioStreamPlayer` или нативный
