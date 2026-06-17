## Фасад над IAP / Ads / Subscription.
##
## Stub Phase 0: ничего не делает. Полная реализация — Phase 2 (P2-060..P2-066).
## Подробности — docs/architecture/12_monetization.md.
extends Node

signal purchase_completed(sku: String, reward: Dictionary)
signal purchase_failed(sku: String, reason: String)
signal ad_completed(placement: String, reward: Dictionary)
signal subscription_changed(active: bool)


func _ready() -> void:
	print("[MonetizationService] ready (stub)")


func purchase(sku: String) -> void:
	push_warning("[MonetizationService] purchase stub for sku=", sku)
	purchase_failed.emit(sku, "not_implemented")


func show_rewarded_ad(placement: String) -> void:
	push_warning("[MonetizationService] rewarded ad stub for ", placement)
	ad_completed.emit(placement, {})


func show_interstitial(placement: String) -> void:
	push_warning("[MonetizationService] interstitial stub for ", placement)


func restore_purchases() -> void:
	push_warning("[MonetizationService] restore stub")


func has_active_subscription() -> bool:
	return false
