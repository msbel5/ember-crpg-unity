using System.Collections.Generic;
using EmberCrpg.Simulation.Logging;
using NUnit.Framework;

// Design note:
// These tests pin ILog as Ember's Simulation logging sink contract.
// They use a tiny test-only implementation; real sinks such as NullLog and UnityConsoleLog come later.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Logging
{
    /// <summary>
    /// Verifies the simulation logging interface can receive structured log events.
    /// </summary>
    public sealed class ILogContractTests
    {
        [Test]
        public void Write_AcceptsStructuredLogEvent()
        {
            ILog log = new CaptureLog();
            var logEvent = new LogEvent(LogLevel.Event, "Test", "Message", EmptyFields());

            log.Write(logEvent);

            var capture = (CaptureLog)log;
            Assert.That(capture.Events.Count, Is.EqualTo(1));
        }

        [Test]
        public void Write_PreservesLogEventPayload()
        {
            ILog log = new CaptureLog();
            var logEvent = new LogEvent(LogLevel.Warn, "Inventory", "Missing item", EmptyFields());

            log.Write(logEvent);

            var capture = (CaptureLog)log;
            Assert.That(capture.Events[0].Level, Is.EqualTo(LogLevel.Warn));
            Assert.That(capture.Events[0].Category, Is.EqualTo("Inventory"));
            Assert.That(capture.Events[0].Message, Is.EqualTo("Missing item"));
        }

        [Test]
        public void Write_PreservesEventOrder()
        {
            ILog log = new CaptureLog();

            log.Write(new LogEvent(LogLevel.Trace, "Rng", "First", EmptyFields()));
            log.Write(new LogEvent(LogLevel.Event, "Rng", "Second", EmptyFields()));

            var capture = (CaptureLog)log;
            Assert.That(capture.Events[0].Message, Is.EqualTo("First"));
            Assert.That(capture.Events[1].Message, Is.EqualTo("Second"));
        }

        private static IReadOnlyDictionary<string, string> EmptyFields()
        {
            return new Dictionary<string, string>();
        }

        private sealed class CaptureLog : ILog
        {
            public readonly List<LogEvent> Events = new List<LogEvent>();

            public void Write(LogEvent logEvent)
            {
                Events.Add(logEvent);
            }
        }
    }
}