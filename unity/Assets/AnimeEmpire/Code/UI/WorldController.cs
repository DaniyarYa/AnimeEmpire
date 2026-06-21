using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Economy;
using AnimeEmpire.Entities;
using AnimeEmpire.Utils;
using TMPro;
using UnityEngine;

namespace AnimeEmpire.UI
{
    public class WorldController : MonoBehaviour
    {
        const float FloatingTextCooldown = 0.6f;
        static readonly Dictionary<string, string> ResourceIcon = new()
        {
            { "wheat", "🌾" }, { "flour", "🌾→" }, { "bread", "🍞" },
        };

        [SerializeField] Player _player;
        [SerializeField] CameraRig _camera;
        [SerializeField] VirtualJoystick _joystick;
        [SerializeField] TMP_Text _goldLabel;
        [SerializeField] TMP_Text _inventoryLabel;
        [SerializeField] BuildingModal _modal;

        readonly Dictionary<string, int> _accAmount = new();
        readonly Dictionary<string, float> _lastSpawnTime = new();

        void Start()
        {
            Debug.Log("[World] ready (Phase 1)");
            if (_camera != null && _player != null) _camera.FollowTarget = _player.transform;
            if (_joystick != null && _player != null) _joystick.DirectionChanged += _player.SetMovementDirection;

            EventBus.CurrencyChanged += OnCurrencyChanged;
            EventBus.ResourceProduced += OnResourceProduced;
            EventBus.ResourceSold += OnResourceSold;

            for (int i = 0; i < BuildingRegistry.All.Count; i++)
                BuildingRegistry.All[i].Clicked += OnBuildingClicked;

            RefreshHud();
        }

        void OnDestroy()
        {
            if (_joystick != null && _player != null) _joystick.DirectionChanged -= _player.SetMovementDirection;
            EventBus.CurrencyChanged -= OnCurrencyChanged;
            EventBus.ResourceProduced -= OnResourceProduced;
            EventBus.ResourceSold -= OnResourceSold;
            for (int i = 0; i < BuildingRegistry.All.Count; i++)
                BuildingRegistry.All[i].Clicked -= OnBuildingClicked;
        }

        void OnBuildingClicked(Building b) { if (_modal != null) _modal.ShowFor(b); }

        void OnCurrencyChanged(string kind, int value) => RefreshHud();

        void OnResourceProduced(string buildingId, string resourceId, int amount)
        {
            RefreshHud();
            AccumulateFloating(buildingId, resourceId, amount);
        }

        void AccumulateFloating(string buildingId, string resourceId, int amount)
        {
            _accAmount.TryGetValue(buildingId, out var prev);
            _accAmount[buildingId] = prev + amount;

            float now = Time.unscaledTime;
            _lastSpawnTime.TryGetValue(buildingId, out var lastTime);
            if (now - lastTime < FloatingTextCooldown) return;

            var b = BuildingRegistry.FindById(buildingId);
            if (b == null) return;
            int accumulated = _accAmount[buildingId];
            string icon = ResourceIcon.TryGetValue(resourceId, out var ic) ? ic : "+";
            FloatingText.Spawn(
                transform,
                b.transform.position + new Vector3(0f, 2.5f, 0f),
                $"+{accumulated} {icon}",
                new Color(1f, 0.85f, 0.3f));
            _accAmount[buildingId] = 0;
            _lastSpawnTime[buildingId] = now;
        }

        void OnResourceSold(string resourceId, int amount, int gold)
        {
            RefreshHud();
            var market = BuildingRegistry.FindById("market");
            if (market != null)
            {
                FloatingText.Spawn(
                    transform,
                    market.transform.position + new Vector3(0f, 2.5f, 0f),
                    $"+{gold} 💰 (−{amount} {resourceId})",
                    new Color(1f, 0.722f, 0.302f));
            }
        }

        void RefreshHud()
        {
            var econ = EconomySim.Instance;
            if (econ == null) return;
            if (_goldLabel != null) _goldLabel.text = $"💰 {econ.GetGold()}";
            if (_inventoryLabel != null)
                _inventoryLabel.text = $"🌾 {econ.GetInventory("wheat")}   🌾→ {econ.GetInventory("flour")}   🍞 {econ.GetInventory("bread")}";
        }
    }
}
