using AnimeEmpire.Data;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class TutorialFlowTests
    {
        TutorialStep MakeStep(string id, TutorialAdvance advance = TutorialAdvance.TapAnywhere)
        {
            var s = ScriptableObject.CreateInstance<TutorialStep>();
            s.Id = id; s.Advance = advance;
            return s;
        }

        [Test]
        public void GetStep_OutOfRange_ReturnsNull()
        {
            var flow = ScriptableObject.CreateInstance<TutorialFlow>();
            Assert.That(flow.GetStep(0), Is.Null);
            Assert.That(flow.GetStep(-1), Is.Null);
        }

        [Test]
        public void FindById_ReturnsMatching()
        {
            var flow = ScriptableObject.CreateInstance<TutorialFlow>();
            var welcome = MakeStep("welcome");
            var click = MakeStep("click_farm", TutorialAdvance.BuildingClicked);
            flow.Steps.Add(welcome);
            flow.Steps.Add(click);
            Assert.That(flow.FindById("click_farm"), Is.SameAs(click));
            Assert.That(flow.FindById("missing"), Is.Null);
        }

        [Test]
        public void Count_ReflectsStepsList()
        {
            var flow = ScriptableObject.CreateInstance<TutorialFlow>();
            flow.Steps.Add(MakeStep("a"));
            flow.Steps.Add(MakeStep("b"));
            flow.Steps.Add(MakeStep("c"));
            Assert.That(flow.Count, Is.EqualTo(3));
        }

        [Test]
        public void NullEntries_FindByIdSkipsThem()
        {
            var flow = ScriptableObject.CreateInstance<TutorialFlow>();
            flow.Steps.Add(null);
            flow.Steps.Add(MakeStep("x"));
            Assert.That(flow.FindById("x"), Is.Not.Null);
        }
    }
}
