using System.Collections.Generic;
using AnimeEmpire.Backend;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class AnalyticsBusSinkTests
    {
        class CaptureSink : IAnalyticsSink
        {
            public readonly List<string> Events = new();
            public string LastUserId;
            public int FlushCount;
            public void Send(string eventName, IReadOnlyDictionary<string, object> parameters) => Events.Add(eventName);
            public void Flush() => FlushCount++;
            public void SetUserId(string userId) => LastUserId = userId;
        }

        IAnalyticsSink _restore;

        [SetUp]
        public void SetUp() { _restore = AnalyticsBus.Sink; }

        [TearDown]
        public void TearDown() { AnalyticsBus.RegisterSink(_restore); }

        [Test]
        public void DefaultSink_IsDebugLogSink()
        {
            AnalyticsBus.RegisterSink(null);
            Assert.That(AnalyticsBus.Sink, Is.InstanceOf<DebugLogSink>());
        }

        [Test]
        public void RegisterSink_Swap_TakesEffect()
        {
            var cap = new CaptureSink();
            AnalyticsBus.RegisterSink(cap);
            Assert.That(AnalyticsBus.Sink, Is.SameAs(cap));
        }

        [Test]
        public void RegisterSink_Null_FallsBackToDebugLog()
        {
            AnalyticsBus.RegisterSink(new CaptureSink());
            AnalyticsBus.RegisterSink(null);
            Assert.That(AnalyticsBus.Sink, Is.InstanceOf<DebugLogSink>());
        }

        [Test]
        public void SetUserId_ForwardsToSink()
        {
            var cap = new CaptureSink();
            AnalyticsBus.RegisterSink(cap);
            cap.SetUserId("user_42");
            Assert.That(cap.LastUserId, Is.EqualTo("user_42"));
        }
    }
}
