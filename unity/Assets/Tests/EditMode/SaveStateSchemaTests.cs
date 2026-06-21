using System.Collections.Generic;
using AnimeEmpire.Backend;
using NUnit.Framework;

namespace AnimeEmpire.Tests.EditMode
{
    public class SaveStateSchemaTests
    {
        Dictionary<string, object> _state;

        [SetUp]
        public void SetUp() => _state = SaveService.NewState();

        [Test]
        public void NewState_HasSaveVersion1()
        {
            Assert.That(_state.ContainsKey("save_version"));
            Assert.That(_state["save_version"], Is.EqualTo(1));
        }

        [Test]
        public void NewState_HasCurrencies_GoldGemsZero()
        {
            Assert.That(_state.ContainsKey("currencies"));
            var c = (Dictionary<string, object>)_state["currencies"];
            Assert.That(c["gold"], Is.EqualTo(0));
            Assert.That(c["gems"], Is.EqualTo(0));
        }

        [Test]
        public void NewState_HasPrestige_LevelZero()
        {
            var p = (Dictionary<string, object>)_state["prestige"];
            Assert.That(p["level"], Is.EqualTo(0));
        }

        [Test]
        public void NewState_HasTutorial_NotCompleted()
        {
            var t = (Dictionary<string, object>)_state["tutorial"];
            Assert.That(t["step"], Is.EqualTo(0));
            Assert.That(t["completed"], Is.EqualTo(false));
        }
    }
}
