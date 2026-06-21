using AnimeEmpire.Core;
using NUnit.Framework;

namespace AnimeEmpire.Tests.EditMode
{
    public class EventBusResetTests
    {
        [Test]
        public void Raise_DoesNotThrow_WhenNoSubscribers()
        {
            Assert.DoesNotThrow(() =>
            {
                EventBus.RaiseResourceProduced("x", "y", 1);
                EventBus.RaiseResourceSold("y", 1, 1);
                EventBus.RaiseSaveDirty();
                EventBus.RaiseSavePersisted();
                EventBus.RaiseCurrencyChanged("gold", 0);
                EventBus.RaiseScreenChanged("a", "b");
                EventBus.RaiseConfigLoaded(0);
                EventBus.RaiseIapCompleted("sku", false);
                EventBus.RaisePrestigeTriggered(1);
                EventBus.RaiseNpcHired("n", "i");
                EventBus.RaiseNpcTaskCompleted("i", "t");
                EventBus.RaiseBuildingUpgraded("b", 1);
            });
        }

        [Test]
        public void Subscribe_ReceivesEmission()
        {
            int count = 0;
            void Handler(string _, string __, int ___) => count++;
            EventBus.ResourceProduced += Handler;
            try
            {
                EventBus.RaiseResourceProduced("a", "b", 1);
                Assert.That(count, Is.EqualTo(1));
            }
            finally
            {
                EventBus.ResourceProduced -= Handler;
            }
        }
    }
}
