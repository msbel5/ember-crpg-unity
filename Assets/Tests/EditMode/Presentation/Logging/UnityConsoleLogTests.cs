using System.Collections.Generic;
using EmberCrpg.Presentation.Logging;
using EmberCrpg.Simulation.Logging;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Design note:
// These tests pin UnityConsoleLog as the Presentation-only adapter from structured
// simulation diagnostics to Unity Console messages.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Presentation.Logging
{
    /// <summary>
    /// Verifies Unity Console routing for structured simulation log events.
    /// </summary>
    public sealed class UnityConsoleLogTests
    {
        [Test]
        public void Write_Trace_UsesUnityLog()
        {
            var log = new UnityConsoleLog();
            var logEvent = new LogEvent(LogLevel.Trace, "Rng", "Roll", EmptyFields());

            LogAssert.Expect(LogType.Log, "[Trace] Rng: Roll");

            log.Write(logEvent);
        }

        [Test]
        public void Write_Event_UsesUnityLog()
        {
            var log = new UnityConsoleLog();
            var logEvent = new LogEvent(LogLevel.Event, "Inventory", "Item moved", EmptyFields());

            LogAssert.Expect(LogType.Log, "[Event] Inventory: Item moved");

            log.Write(logEvent);
        }

        [Test]
        public void Write_Warn_UsesUnityWarning()
        {
            var log = new UnityConsoleLog();
            var logEvent = new LogEvent(LogLevel.Warn, "Save", "Missing optional field", EmptyFields());

            LogAssert.Expect(LogType.Warning, "[Warn] Save: Missing optional field");

            log.Write(logEvent);
        }

        [Test]
        public void Write_Error_UsesUnityError()
        {
            var log = new UnityConsoleLog();
            var logEvent = new LogEvent(LogLevel.Error, "Combat", "Invariant failed", EmptyFields());

            LogAssert.Expect(LogType.Error, "[Error] Combat: Invariant failed");

            log.Write(logEvent);
        }

        private static IReadOnlyDictionary<string, string> EmptyFields()
        {
            return new Dictionary<string, string>();
        }
    }
}