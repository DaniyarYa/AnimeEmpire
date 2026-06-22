using System.Collections.Generic;
using AnimeEmpire.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimeEmpire.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject _root;
        [SerializeField] GameObject _settingsPanel;
        [SerializeField] Button _openButton;
        [SerializeField] Button _resumeButton;
        [SerializeField] Button _settingsButton;
        [SerializeField] Button _backFromSettingsButton;
        [SerializeField] Button _quitButton;
        [SerializeField] Slider _sfxSlider;
        [SerializeField] Slider _musicSlider;
        [SerializeField] TMP_Dropdown _localeDropdown;
        [SerializeField] TMP_Text _versionLabel;

        float _restoreTimeScale = 1f;

        void Awake()
        {
            if (_root != null) _root.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);
            if (_openButton != null) _openButton.onClick.AddListener(Open);
            if (_resumeButton != null) _resumeButton.onClick.AddListener(Close);
            if (_settingsButton != null) _settingsButton.onClick.AddListener(() => ShowSettings(true));
            if (_backFromSettingsButton != null) _backFromSettingsButton.onClick.AddListener(() => ShowSettings(false));
            if (_quitButton != null) _quitButton.onClick.AddListener(Quit);
            if (_sfxSlider != null) _sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            if (_musicSlider != null) _musicSlider.onValueChanged.AddListener(OnMusicChanged);
            if (_localeDropdown != null) _localeDropdown.onValueChanged.AddListener(OnLocaleChanged);
            if (_versionLabel != null) _versionLabel.text = $"v{Application.version}";
        }

        public void Open()
        {
            if (_root == null) return;
            _restoreTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            _root.SetActive(true);
            ShowSettings(false);
        }

        public void Close()
        {
            if (_root == null) return;
            _root.SetActive(false);
            Time.timeScale = _restoreTimeScale;
        }

        void ShowSettings(bool show)
        {
            if (_settingsPanel == null) return;
            if (show) HydrateSettings();
            _settingsPanel.SetActive(show);
        }

        void HydrateSettings()
        {
            var s = SettingsService.Instance;
            if (s == null) return;
            if (_sfxSlider != null) _sfxSlider.SetValueWithoutNotify(s.Sfx);
            if (_musicSlider != null) _musicSlider.SetValueWithoutNotify(s.Music);
            if (_localeDropdown != null)
            {
                if (_localeDropdown.options.Count == 0)
                {
                    var opts = new List<TMP_Dropdown.OptionData>();
                    foreach (var code in Localization.SupportedLocales) opts.Add(new TMP_Dropdown.OptionData(code));
                    _localeDropdown.options = opts;
                }
                int idx = System.Array.IndexOf(Localization.SupportedLocales, s.Locale);
                _localeDropdown.SetValueWithoutNotify(idx < 0 ? 0 : idx);
            }
        }

        void OnSfxChanged(float v) { if (SettingsService.Instance != null) SettingsService.Instance.Sfx = v; }
        void OnMusicChanged(float v) { if (SettingsService.Instance != null) SettingsService.Instance.Music = v; }
        void OnLocaleChanged(int idx)
        {
            if (idx < 0 || idx >= Localization.SupportedLocales.Length) return;
            if (SettingsService.Instance != null) SettingsService.Instance.Locale = Localization.SupportedLocales[idx];
        }

        void Quit()
        {
            // Phase 1: flush save, then quit.
            Backend.SaveService.Instance?.Flush();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
