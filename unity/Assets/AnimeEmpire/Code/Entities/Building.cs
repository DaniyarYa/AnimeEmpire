using System;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AnimeEmpire.Entities
{
    public class Building : MonoBehaviour, IPointerClickHandler
    {
        public BuildingDef Def;
        public event Action<Building> Clicked;

        ProductionLine _line;
        bool _started;
        NPC _worker;

        void Awake() => BuildingRegistry.Register(this);
        void OnDestroy() => BuildingRegistry.Unregister(this);

        public void StartProduction()
        {
            if (_started || Def == null) return;
            if (Def.Category == "service") { _started = true; return; }
            if (_worker != null) { _started = true; return; }
            if (EconomySim.Instance != null)
                _line = EconomySim.Instance.RegisterLineFromDef(Def, 1);
            _started = true;
            Debug.Log($"[Building] started production: {Def.Id}");
        }

        public bool IsStarted => _started;
        public bool HasWorker => _worker != null;
        public ProductionLine Line => _line;

        public void SetWorker(NPC npc) { _worker = npc; _started = true; }

        public bool AssignWorker(NPC npc)
        {
            if (_worker != null || npc == null) return false;
            if (_line != null) { EconomySim.Instance?.UnregisterLine(_line); _line = null; }
            _worker = npc;
            _started = true;
            npc.Assign(this);
            return true;
        }

        public void DismissWorker()
        {
            if (_worker == null) return;
            _worker.Dismiss();
            _worker = null;
            _started = false;
            if (_line != null) { EconomySim.Instance?.UnregisterLine(_line); _line = null; }
        }

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this);
    }
}
