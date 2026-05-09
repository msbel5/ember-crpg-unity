using System.Collections.Generic;
using EmberCrpg.Simulation.Logging;
using NUnit.Framework;

// Design note:
// These tests pin LogEvent as a structured diagnostic payload.
// They do not test console output, Unity logging, replay events, or world mutation.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Logging
{
    /// <summary>
    /// Verifies Ember's structured simulation log event payload.
    /// </summary>
    public sealed class LogEventTests
    {
        [Test]
        public void Constructor_StoresLevel()
        {
            var logEvent = new LogEvent(LogLevel.Event, "Combat", "Hit landed", EmptyFields());

            Assert.That(logEvent.Level, Is.EqualTo(LogLevel.Event));
        }

        [Test]
        public void Constructor_StoresCategory()
        {
            var logEvent = new LogEvent(LogLevel.Warn, "Inventory", "Missing item", EmptyFields());

            Assert.That(logEvent.Category, Is.EqualTo("Inventory"));
        }

        [Test]
        public void Constructor_StoresMessage()
        {
            var logEvent = new LogEvent(LogLevel.Error, "Save", "Round-trip failed", EmptyFields());

            Assert.That(logEvent.Message, Is.EqualTo("Round-trip failed"));
        }

        [Test]
        public void Constructor_AllowsStructuredFields()
        {
            var fields = new Dictionary<string, string>
            {
                { "actor", "ActorId(7)" },
                { "item", "ItemId(3)" }
            };

            var logEvent = new LogEvent(LogLevel.Event, "Inventory", "Item moved", fields);

            Assert.That(logEvent.Fields["actor"], Is.EqualTo("ActorId(7)"));
            Assert.That(logEvent.Fields["item"], Is.EqualTo("ItemId(3)"));
        }

        [Test]
        public void Constructor_AllowsEmptyFields()
        {
            var logEvent = new LogEvent(LogLevel.Trace, "Rng", "Roll", EmptyFields());

            Assert.That(logEvent.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public void ToString_ReturnsCompactDebugLabel()
        {
            var logEvent = new LogEvent(LogLevel.Warn, "Inventory", "Missing item", EmptyFields());

            Assert.That(logEvent.ToString(), Is.EqualTo("[Warn] Inventory: Missing item"));
        }

        private static IReadOnlyDictionary<string, string> EmptyFields()
        {
            return new Dictionary<string, string>();
        }
    }
}