// Design note:
// NullLog is Ember's default no-op logging sink for Simulation tests and quiet runtime paths.
// It preserves the ILog dependency boundary without storing, formatting, or emitting diagnostics.
namespace EmberCrpg.Simulation.Logging
{
    /// <summary>
    /// No-op structured logging sink used when diagnostics are intentionally ignored.
    /// </summary>
    public sealed class NullLog : ILog
    {
        /// <summary>
        /// Shared reusable no-op logging sink.
        /// </summary>
        public static readonly NullLog Instance = new NullLog();

        private NullLog()
        {
        }

        /// <summary>
        /// Accepts a structured diagnostic event and intentionally ignores it.
        /// </summary>
        public void Write(LogEvent logEvent)
        {
        }
    }
}