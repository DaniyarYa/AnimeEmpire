using System;
using System.Collections.Generic;
using AnimeEmpire.Backend;
using NUnit.Framework;

namespace AnimeEmpire.Tests.EditMode
{
    public class CrashReporterTests
    {
        class MockReporter : ICrashReporter
        {
            public int ExceptionCount;
            public int ErrorCount;
            public Exception LastException;
            public string LastError;
            public readonly Dictionary<string, string> Tags = new();

            public void ReportException(Exception ex, IReadOnlyDictionary<string, string> extra = null)
            { ExceptionCount++; LastException = ex; }
            public void ReportError(string message, IReadOnlyDictionary<string, string> extra = null)
            { ErrorCount++; LastError = message; }
            public void SetUserId(string userId) => Tags["user_id"] = userId;
            public void SetTag(string key, string value) => Tags[key] = value;
        }

        ICrashReporter _restore;

        [SetUp]
        public void SetUp() { _restore = CrashReporter.Current; }

        [TearDown]
        public void TearDown() { CrashReporter.SetProvider(_restore); }

        [Test]
        public void DefaultProvider_IsLocalLogReporter()
        {
            CrashReporter.SetProvider(null);
            Assert.That(CrashReporter.Current, Is.InstanceOf<LocalLogReporter>());
        }

        [Test]
        public void SetProvider_Null_FallsBackToLocal()
        {
            CrashReporter.SetProvider(new MockReporter());
            CrashReporter.SetProvider(null);
            Assert.That(CrashReporter.Current, Is.InstanceOf<LocalLogReporter>());
        }

        [Test]
        public void SetProvider_Swap_RoutesReports()
        {
            var mock = new MockReporter();
            CrashReporter.SetProvider(mock);
            CrashReporter.Current.ReportException(new InvalidOperationException("test"));
            Assert.That(mock.ExceptionCount, Is.EqualTo(1));
            Assert.That(mock.LastException, Is.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void SetUserId_PropagatesToTags()
        {
            var mock = new MockReporter();
            CrashReporter.SetProvider(mock);
            CrashReporter.Current.SetUserId("user_123");
            Assert.That(mock.Tags["user_id"], Is.EqualTo("user_123"));
        }

        [Test]
        public void LocalLogReporter_ReportNullException_DoesNotThrow()
        {
            var r = new LocalLogReporter();
            Assert.DoesNotThrow(() => r.ReportException(null));
        }
    }
}
