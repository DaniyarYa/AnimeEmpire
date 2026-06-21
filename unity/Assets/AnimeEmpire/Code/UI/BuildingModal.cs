using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using AnimeEmpire.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimeEmpire.UI
{
    public class BuildingModal : MonoBehaviour
    {
        [SerializeField] GameObject _root;
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _info;
        [SerializeField] Button _actionButton;
        [SerializeField] Button _assignButton;
        [SerializeField] Button _dismissButton;
        [SerializeField] Button _closeButton;
        [SerializeField] TMP_Text _actionButtonLabel;
        [SerializeField] ResourceDef _wheat;
        [SerializeField] ResourceDef _flour;
        [SerializeField] ResourceDef _bread;

        Building _current;
        float _refreshAccum;

        void Awake()
        {
            if (_root == null) _root = gameObject;
            Hide();
            if (_closeButton != null) _closeButton.onClick.AddListener(OnClosePressed);
            if (_actionButton != null) _actionButton.onClick.AddListener(OnActionPressed);
            if (_assignButton != null) _assignButton.onClick.AddListener(OnAssignPressed);
            if (_dismissButton != null) _dismissButton.onClick.AddListener(OnDismissPressed);
            EventBus.ResourceProduced += OnInventoryChanged;
            EventBus.ResourceSold += OnResourceSold;
        }

        void OnDestroy()
        {
            EventBus.ResourceProduced -= OnInventoryChanged;
            EventBus.ResourceSold -= OnResourceSold;
        }

        void Update()
        {
            if (_root == null || !_root.activeSelf) return;
            _refreshAccum += Time.unscaledDeltaTime;
            if (_refreshAccum >= 0.5f) { _refreshAccum = 0f; Refresh(); }
        }

        public void ShowFor(Building b)
        {
            _current = b;
            Refresh();
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        void Refresh()
        {
            if (_current == null || _current.Def == null)
            {
                if (_title != null) _title.text = "?";
                if (_info != null) _info.text = "";
                if (_actionButton != null) _actionButton.interactable = false;
                return;
            }
            var b = _current.Def;
            string title = Localization.T(b.DisplayNameKey);
            if (title == b.DisplayNameKey) title = Capitalize(b.Id);
            if (_title != null) _title.text = title;

            bool hasWorker = _current.HasWorker;
            if (_dismissButton != null) _dismissButton.gameObject.SetActive(hasWorker);
            bool canAssign = !hasWorker
                && (b.Category == "generator" || b.Category == "processor")
                && NpcRegistry.FindAvailableForCategory(b.Category) != null;
            if (_assignButton != null) _assignButton.gameObject.SetActive(canAssign);

            switch (b.Category)
            {
                case "generator":
                    if (_info != null) _info.text = GeneratorInfo(b);
                    SetActionLabel(ProductionButtonText());
                    if (_actionButton != null) _actionButton.interactable = !_current.IsStarted;
                    break;
                case "processor":
                    if (_info != null) _info.text = ProcessorInfo(b);
                    SetActionLabel(ProductionButtonText());
                    if (_actionButton != null) _actionButton.interactable = !_current.IsStarted;
                    break;
                case "service":
                    if (_info != null) _info.text = ServiceInfo();
                    SetActionLabel("Sell all");
                    int total = (EconomySim.Instance?.GetInventory("wheat") ?? 0)
                              + (EconomySim.Instance?.GetInventory("flour") ?? 0)
                              + (EconomySim.Instance?.GetInventory("bread") ?? 0);
                    if (_actionButton != null) _actionButton.interactable = total > 0;
                    break;
            }
        }

        void SetActionLabel(string txt)
        {
            if (_actionButtonLabel != null) _actionButtonLabel.text = txt;
        }

        string ProductionButtonText()
        {
            if (_current.HasWorker) return "Worker assigned";
            if (_current.IsStarted) return "Producing...";
            return "Start production";
        }

        string GeneratorInfo(BuildingDef b)
        {
            float rate = b.OutputAmount / Mathf.Max(b.BaseCycleSeconds, 0.0001f);
            string outId = b.OutputResource != null ? b.OutputResource.Id : "?";
            int inv = EconomySim.Instance?.GetInventory(outId) ?? 0;
            return $"Output: {FormatRate(rate)} {outId} / sec\nInventory: {inv}";
        }

        string ProcessorInfo(BuildingDef b)
        {
            string inId = b.InputResource != null ? b.InputResource.Id : "?";
            string outId = b.OutputResource != null ? b.OutputResource.Id : "?";
            var econ = EconomySim.Instance;
            int invIn = econ?.GetInventory(inId) ?? 0;
            int invOut = econ?.GetInventory(outId) ?? 0;
            return $"{b.InputAmount} {inId} → {b.OutputAmount} {outId} every {b.BaseCycleSeconds:0}s\nInventory: {inId}={invIn}, {outId}={invOut}";
        }

        string ServiceInfo()
        {
            var econ = EconomySim.Instance;
            int w = econ?.GetInventory("wheat") ?? 0;
            int f = econ?.GetInventory("flour") ?? 0;
            int br = econ?.GetInventory("bread") ?? 0;
            return $"Inventory wheat: {w} × 1g\nInventory flour: {f} × 3g\nInventory bread: {br} × 15g";
        }

        string FormatRate(float r) => r >= 1f ? r.ToString("0.0") : r.ToString("0.00");

        void OnActionPressed()
        {
            if (_current == null || _current.Def == null) return;
            switch (_current.Def.Category)
            {
                case "generator":
                case "processor": _current.StartProduction(); Refresh(); break;
                case "service": SellAll(); break;
            }
        }

        void SellAll()
        {
            var econ = EconomySim.Instance;
            if (econ == null) return;
            if (_wheat != null) econ.SellInventory(_wheat);
            if (_flour != null) econ.SellInventory(_flour);
            if (_bread != null) econ.SellInventory(_bread);
            Refresh();
        }

        void OnAssignPressed()
        {
            if (_current == null || _current.Def == null) return;
            var npc = NpcRegistry.FindAvailableForCategory(_current.Def.Category);
            if (npc == null) return;
            _current.AssignWorker(npc);
            Refresh();
        }

        void OnDismissPressed()
        {
            if (_current == null) return;
            _current.DismissWorker();
            Refresh();
        }

        void OnClosePressed() { _current = null; Hide(); }

        void OnInventoryChanged(string _, string __, int ___) { if (_root != null && _root.activeSelf) Refresh(); }
        void OnResourceSold(string _, int __, int ___) { if (_root != null && _root.activeSelf) Refresh(); }

        static string Capitalize(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);
    }
}
