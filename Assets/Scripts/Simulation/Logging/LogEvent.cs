using System.Collections.Generic;

// Design note:
// LogEvent is Ember's structured diagnostic payload for Simulation logging.
// It is not a gameplay WorldEvent, replay event, ReasonTrace, or Unity console adapter.
namespace EmberCrpg.Simulation.Logging
{
    /// <summary>
    /// Severity level for structured simulation diagnostics.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Very detailed diagnostic information, such as RNG draws or per-tick traces.
        /// </summary>
        Trace,

        /// <summary>
        /// Important state transition, gameplay event, or system-level occurrence.
        /// </summary>
        Event,

        /// <summary>
        /// Recoverable inconsistency or suspicious condition.
        /// </summary>
        Warn,

        /// <summary>
        /// Invariant violation or unrecoverable simulation error.
        /// </summary>
        Error
    }

    /// <summary>
    /// Immutable structured diagnostic event emitted by simulation services.
    /// </summary>
    public readonly struct LogEvent
    {
        /// <summary>
        /// Severity level of this diagnostic event.
        /// </summary>
        public readonly LogLevel Level;

        /// <summary>
        /// Logical source category, such as Rng, Combat, Inventory, or Save.
        /// </summary>
        public readonly string Category;

        /// <summary>
        /// Human-readable diagnostic message.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Structured key/value fields for filtering, tests, and later DM reason inspection.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> Fields;

        /// <summary>
        /// Creates a structured diagnostic event.
        /// </summary>
        public LogEvent(
            LogLevel level,
            string category,
            string message,
            IReadOnlyDictionary<string, string> fields)
        {
            Level = level;
            Category = category;
            Message = message;
            Fields = fields;
        }

        /// <summary>
        /// Returns a compact debug label for this diagnostic event.
        /// </summary>
        public override string ToString()
        {
            return $"[{Level}] {Category}: {Message}";
        }
    }
}