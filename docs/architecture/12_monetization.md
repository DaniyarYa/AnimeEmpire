# 12 · Монетизация

## Каналы

Из `GDD/8`:
- 45% rewarded ads
- 20% starter packs
- 15% hard currency (gems)
- 12% limited offers
- 5% subscription
- 3% remove ads (one-time)

Целевой ARPDAU: $0.08 (старт) → $0.18 (12 месяцев).

## MonetizationService (autoload)

Фасад над тремя подсистемами:
1. **IAP** — платформенные покупки
2. **Ads** — rewarded / interstitial через mediation
3. **Subscription** — авторекуррентная подписка

```gdscript
extends Node

signal purchase_completed(sku: String, reward: Dictionary)
signal purchase_failed(sku: String, reason: String)
signal ad_completed(placement: String, reward: Dictionary)
signal subscription_changed(active: bool)

func purchase(sku: String) -> void
func show_rewarded_ad(placement: String) -> void
func show_interstitial(placement: String) -> void
func restore_purchases() -> void
func has_active_subscription() -> bool
```

## IAP

### SKUs (MVP)
| SKU | Тип | Цена USD | Содержимое |
|-----|-----|----------|------------|
| `gem_pack_small` | consumable | 0.99 | 100 gems |
| `gem_pack_medium` | consumable | 4.99 | 600 gems |
| `gem_pack_large` | consumable | 9.99 | 1500 gems |
| `gem_pack_mega` | consumable | 19.99 | 3500 gems |
| `starter_pack` | non-consumable | 2.99 | 500 gems + 5K gold + exclusive NPC (7-day) |
| `remove_ads` | non-consumable | 4.99 | без interstitial навсегда |
| `subscription_monthly` | auto-renew | 4.99/mo | 2x offline, 50% speed, 50 gems/day, exclusive NPCs, ad-free |

### Flow покупки

```
UI → MonetizationService.purchase(sku)
  ↓
платформенный API (StoreKit / Google Billing)
  ↓
получение receipt
  ↓
POST /iap/validate { sku, receipt, store } → backend
  ↓
сервер проверяет в Apple/Google verifyReceipt
  ↓
{ valid: true, reward: {...} }
  ↓
EconomySim.grant(reward) + EventBus.emit("iap_completed")
  ↓
консьюм (для consumables)
```

### Pending purchases
- Сохранять незаконченные транзакции в SaveService
- При старте — попытка завершить (нативный API даёт `getPurchaseHistory`)

### Restore (Apple требование)
- Кнопка `Restore Purchases` в Settings
- Все non-consumables восстанавливаются (`remove_ads`, `starter_pack`)
- Подписка восстанавливается из app store

## Ads

### Mediation
- AdMob или IronSource (выбор на Phase 3)
- Конфиги частоты — через RemoteConfig

### Placements
| Placement | Тип | Триггер | Награда |
|-----------|-----|---------|---------|
| `offline_2x` | rewarded | После модалки оффлайн-дохода | x2 к собранному |
| `instant_finish` | rewarded | На таймере цикла здания | пропуск 30 с |
| `free_booster_15min` | rewarded | Кнопка в HUD | +50% speed 15 мин |
| `temp_npc_60min` | rewarded | Кнопка в NPC экране | временный NPC |
| `prestige_bonus` | rewarded | На экране престижа | +1 prestige point |
| `level_complete` | interstitial | После Lv5/10/20 апгрейда | — |
| `daily_login` | rewarded | Бонус удваивается | x2 daily |

### Правила показа
- Никаких ads первые 5 минут игры (онбординг)
- Никаких ads до первого апгрейда
- Не больше 1 interstitial за 5 минут
- Подписка / `remove_ads` отключают interstitial (rewarded остаётся)
- Cooldown между rewarded одного placement — 60 с

### Caching
- Pre-load следующего ad после показа
- При failure — fallback (показать чуть позже без callback)

## Subscription

### Состояние
- `subscription_until: int` (unix timestamp) хранится в сейве
- Сервер периодически валидирует (refresh status, проверка отмены)
- При expiration — баннер с предложением продлить

### Грейс-период
- 3 дня после окончания — бонусы ещё доступны (политика Apple/Google)
- В клиенте отражаем как `is_grace_period`

## Limited offers

- Время-ограниченные пакеты (24-72 ч)
- Триггер: первый день, после ачивки, в день выплаты
- Прогрессивная скидка: первый запуск — 50%, повторный — 30%, после покупки — стандарт
- Хранятся в RemoteConfig events

## Anti-cheat

- Серверная валидация receipt (см. `10_backend.md`)
- Receipt-spoofing на клиенте → backend выдаёт `valid: false` → reward не выдаётся
- При повторных подозрительных запросах — soft-ban (см. `10_backend.md`)

## Привязка к UX

- Покупки не агрессивны: цель — convenience, не P2W (см. `GDD/8`)
- Оффер показывается раз в сессию, не блокирует gameplay
- Closing X всегда доступен, ≥ 24 px, не маскируется под кнопку "OK"
- A/B-тестим цены, окна показа, тексты CTA

## Этика

Из GDD: no paywalls, no gacha, no P2W, F2P достигает 90% эффективности. Документация PR должна проверять, что новые офферы не нарушают это правило.

## Открытые вопросы

- [ ] AdMob vs IronSource — окончательный выбор на Phase 3
- [ ] Regional pricing — настраиваем вручную или используем платформенные tiers?
- [ ] Bundle покупки (несколько SKU за раз) — приоритет post-launch
