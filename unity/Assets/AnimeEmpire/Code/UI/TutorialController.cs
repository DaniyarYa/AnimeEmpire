using System.Collections.Generic;
using AnimeEmpire.Backend;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimeEmpire.UI
{
    public class TutorialController : MonoBehaviour
    {
        public const string TutorialKey = "tutorial";
        public const string StepKey = "step";
        public const string CompletedKey = "completed";

        [SerializeField] TutorialFlow _flow;
        [SerializeField] GameObject _root;
        [SerializeField] TMP_Text _messageLabel;
        [SerializeField] Button _skipButton;
        [SerializeField] Button _tapAnywhereButton;

        int _currentStep;
        bool _completed;

        void Awake()
        {
            if (_skipButton != null) _skipButton.onClick.AddListener(Skip);
            if (_tapAnywhereButton != null) _tapAnywhereButton.onClick.AddListener(OnTapAnywhere);
            EventBus.ResourceProduced += OnResourceProduced;
            EventBus.ResourceSold += OnResourceSold;
        }

        void OnDestroy()
        {
            EventBus.ResourceProduced -= OnResourceProduced;
            EventBus.ResourceSold -= OnResourceSold;
            UnhookBuildings();
        }

        void Start()
        {
            LoadProgress();
            HookBuildings();
            Render();
        }

        void LoadProgress()
        {
            var save = SaveService.Instance;
            if (save == null) return;
            var state = save.GetState();
            if (!state.TryGetValue(TutorialKey, out var raw) || raw is not Dictionary<string, object> dict) return;
            if (dict.TryGetValue(StepKey, out var s)) _currentStep = System.Convert.ToInt32(s);
            if (dict.TryGetValue(CompletedKey, out var c)) _completed = System.Convert.ToBoolean(c);
        }

        void PersistProgress()
        {
            var save = SaveService.Instance;
            if (save == null) return;
            var state = save.GetState();
            if (!state.TryGetValue(TutorialKey, out var raw) || raw is not Dictionary<string, object> dict)
            {
                dict = new Dictionary<string, object>();
                state[TutorialKey] = dict;
            }
            dict[StepKey] = _currentStep;
            dict[CompletedKey] = _completed;
            EventBus.RaiseSaveDirty();
        }

        void HookBuildings()
        {
            for (int i = 0; i < BuildingRegistry.All.Count; i++)
                BuildingRegistry.All[i].Clicked += OnBuildingClicked;
        }

        void UnhookBuildings()
        {
            for (int i = 0; i < BuildingRegistry.All.Count; i++)
                BuildingRegistry.All[i].Clicked -= OnBuildingClicked;
        }

        void Render()
        {
            if (_root == null) return;
            if (_completed || _flow == null || _currentStep >= _flow.Count)
            {
                _root.SetActive(false);
                return;
            }
            var step = _flow.GetStep(_currentStep);
            if (step == null) { _root.SetActive(false); return; }
            _root.SetActive(true);
            if (_messageLabel != null) _messageLabel.text = Localization.T(step.MessageKey);
            if (_tapAnywhereButton != null) _tapAnywhereButton.gameObject.SetActive(step.Advance == TutorialAdvance.TapAnywhere);
        }

        void AdvanceStep()
        {
            if (_flow == null) return;
            _currentStep++;
            if (_currentStep >= _flow.Count) Complete();
            PersistProgress();
            Render();
        }

        void Complete()
        {
            _completed = true;
            _currentStep = _flow != null ? _flow.Count : 0;
        }

        public void Skip()
        {
            Complete();
            PersistProgress();
            Render();
        }

        void OnTapAnywhere()
        {
            var step = CurrentStep();
            if (step != null && step.Advance == TutorialAdvance.TapAnywhere) AdvanceStep();
        }

        void OnResourceProduced(string buildingId, string resourceId, int amount)
        {
            var step = CurrentStep();
            if (step == null || step.Advance != TutorialAdvance.ResourceProduced) return;
            if (!string.IsNullOrEmpty(step.ExpectedResourceId) && step.ExpectedResourceId != resourceId) return;
            AdvanceStep();
        }

        void OnResourceSold(string resourceId, int amount, int gold)
        {
            var step = CurrentStep();
            if (step == null || step.Advance != TutorialAdvance.ResourceSold) return;
            if (!string.IsNullOrEmpty(step.ExpectedResourceId) && step.ExpectedResourceId != resourceId) return;
            AdvanceStep();
        }

        void OnBuildingClicked(Building b)
        {
            var step = CurrentStep();
            if (step == null || step.Advance != TutorialAdvance.BuildingClicked) return;
            if (b == null || b.Def == null) return;
            if (!string.IsNullOrEmpty(step.ExpectedBuildingId) && step.ExpectedBuildingId != b.Def.Id) return;
            AdvanceStep();
        }

        TutorialStep CurrentStep()
        {
            if (_completed || _flow == null) return null;
            return _flow.GetStep(_currentStep);
        }
    }
}
