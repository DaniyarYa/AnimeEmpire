# 07 · Backlog

Идеи и задачи без чёткого срока. Из них формируются спринты post-launch.

## Геймдизайн идеи

- [ ] **BL-001** • Pet-система (домашние животные с пассивными бонусами)
- [ ] **BL-002** • Сезонные cosmetics для зданий (зима/весна тематика)
- [ ] **BL-003** • Story-режим с диалогами и катсценами
- [ ] **BL-004** • Mini-games (фишинг как aktive mini-game)
- [ ] **BL-005** • Guilds / clans
- [ ] **BL-006** • Co-op events
- [ ] **BL-007** • PvP режимы (опасно для F2P баланса — отложить)
- [ ] **BL-008** • Random expeditions (отправить NPC на 4-8 ч за наградами)

## UX улучшения

- [ ] **BL-100** • Auto-tap режим
- [ ] **BL-101** • Landscape mode для tablets
- [ ] **BL-102** • Гайд по престижу (in-app tutorial при первом)
- [ ] **BL-103** • Quick-stats виджет (сколько gold/min прямо сейчас)
- [ ] **BL-104** • Customizable HUD layout

## Технические

- [ ] **BL-200** • Миграция Firebase → AWS (когда выручка позволит)
- [ ] **BL-201** • Real-time WebSocket для leaderboard live updates
- [ ] **BL-202** • Cloud sync на iCloud / Google Play Games (доп. к нашему backend)
- [ ] **BL-203** • Apple Game Center / Google Play Achievements интеграция
- [ ] **BL-204** • Apple Watch companion (показывать оффлайн доход)
- [ ] **BL-205** • Daily login локально через notification API
- [ ] **BL-206** • CDN для конфигов (отказоустойчивость)
- [ ] **BL-207** • GraphQL вместо REST (по запросу)
- [ ] **BL-208** • E2E тесты на main flows

## Платформы

- [ ] **BL-300** • Web версия (HTML5 через Godot export)
- [ ] **BL-301** • Steam версия (PC)
- [ ] **BL-302** • Nintendo Switch (если успех на мобиле)

## Маркетинг / community

- [ ] **BL-400** • Referral программа
- [ ] **BL-401** • Cross-promo с другими играми студии
- [ ] **BL-402** • Twitch streams partnership
- [ ] **BL-403** • Fan art contest
- [ ] **BL-404** • Merchandise (плюшевые NPC)

## Аналитика / data

- [ ] **BL-500** • Cohort prediction model (когда уйдёт игрок — превентивный оффер)
- [ ] **BL-501** • Recommendation engine (что купить, какое здание апгрейдить)
- [ ] **BL-502** • Server-side game state hashing для cheat detection ML

## Локализация

- [ ] **BL-600** • Tier-3: it, tr, ar (RTL!), id, vi, th
- [ ] **BL-601** • Voice acting tutorial (на 1-2 главных)
- [ ] **BL-602** • Native review for tier-2 локалей

## Аудио

- [ ] **BL-700** • Adaptive music layers
- [ ] **BL-703** • NPC voice samples ("yatta!", "tasukatta")

## Hotfix queue

> Здесь — задачи, которые срочно нужны для текущей prod-версии. Перебивают спринт-приоритет.

- [ ] (на данный момент пусто)

## Декомпозиция / правила

- Когда BL-задача поднимается в приоритет, она переезжает в файл фазы (P5-xxx) и помечается `[~]` здесь с примечанием
- Раз в квартал — ревью backlog'а: удаляем устаревшее, обновляем приоритеты
