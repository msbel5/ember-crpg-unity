using System.Collections.Generic;
using EmberCrpg.Simulation.Logging;
using NUnit.Framework;

// Design note:
// These tests pin NullLog as Ember's default no-op Simulation log sink.
// They do not test Unity console output, storage, replay events, or diagnostic filtering.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Logging
{
    /// <summary>
    /// Verifies the no-op logging sink used by tests and quiet simulation services.
    /// </summary>
    public sealed class NullLogTests
    {
        [Test]
        public void Instance_ReturnsILog()
        {
            ILog log = NullLog.Instance;

            Assert.That(log, Is.Not.Null);
        }

        [Test]
        public void Write_AcceptsLogEventWithoutThrowing()
        {
            var logEvent = new LogEvent(LogLevel.Event, "Test", "Message", EmptyFields());

            Assert.DoesNotThrow(() => NullLog.Instance.Write(logEvent));
        }

        [Test]
        public void Write_AllowsMultipleEventsWithoutThrowing()
        {
            var first = new LogEvent(LogLevel.Trace, "Rng", "First", EmptyFields());
            var second = new LogEvent(LogLevel.Warn, "Inventory", "Second", EmptyFields());

            Assert.DoesNotThrow(() =>
            {
                NullLog.Instance.Write(first);
                NullLog.Instance.Write(second);
            });
        }

        [Test]
        public void Instance_ReturnsSameReusableObject()
        {
            var first = NullLog.Instance;
            var second = NullLog.Instance;

            Assert.That(first, Is.SameAs(second));
        }

        private static IReadOnlyDictionary<string, string> EmptyFields()
        {
            return new Dictionary<string, string>();
        }
    }
}