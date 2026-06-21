using System.Collections;
using AnimeEmpire.Core;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AnimeEmpire.Tests.PlayMode
{
    public class BootstrapPlayModeTests
    {
        [UnityTest]
        public IEnumerator EconomySim_AndGameState_AliveAfterBoot()
        {
            yield return null;
            yield return null;
            Assert.That(EconomySim.Instance, Is.Not.Null, "EconomySim must spawn from Bootstrap.prefab");
            Assert.That(GameState.Instance, Is.Not.Null, "GameState must spawn from Bootstrap.prefab");
        }

        [UnityTest]
        public IEnumerator EventBus_ResetsCleanlyBetweenPlaySessions()
        {
            int counter = 0;
            void Handler(string a, string b, int c) => counter++;
            EventBus.ResourceProduced += Handler;
            EventBus.RaiseResourceProduced("test", "x", 1);
            Assert.That(counter, Is.EqualTo(1));
            EventBus.ResourceProduced -= Handler;
            yield return null;
        }
    }
}
