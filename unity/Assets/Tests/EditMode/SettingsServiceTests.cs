using AnimeEmpire.Backend;
using AnimeEmpire.Core;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class SettingsServiceTests
    {
        GameObject _host;
        SaveService _save;
        SettingsService _settings;

        [SetUp]
        public void SetUp()
        {
            _host = new GameObject("TestSettings");
            _save = _host.AddComponent<SaveService>();
            _settings = _host.AddComponent<SettingsService>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_host != null) Object.DestroyImmediate(_host);
        }

        [Test]
        public void Sfx_AboveOne_ClampsToOne()
        {
            _settings.Sfx = 2f;
            Assert.That(_settings.Sfx, Is.EqualTo(1f));
        }

        [Test]
        public void Music_BelowZero_ClampsToZero()
        {
            _settings.Music = -0.5f;
            Assert.That(_settings.Music, Is.EqualTo(0f));
        }

        [Test]
        public void Sfx_SameValue_NoChange()
        {
            int changes = 0;
            _settings.SettingsChanged += () => changes++;
            float current = _settings.Sfx;
            _settings.Sfx = current;
            Assert.That(changes, Is.EqualTo(0));
        }

        [Test]
        public void Locale_Switch_PersistsToSaveState()
        {
            _settings.Locale = "ru";
            var state = _save.GetState();
            Assert.That(state.ContainsKey("settings"));
            var s = (System.Collections.Generic.Dictionary<string, object>)state["settings"];
            Assert.That(s["locale"], Is.EqualTo("ru"));
        }

        [Test]
        public void SetSfx_RaisesSettingsChanged()
        {
            int changes = 0;
            _settings.SettingsChanged += () => changes++;
            _settings.Sfx = 0.3f;
            Assert.That(changes, Is.EqualTo(1));
        }
    }
}
