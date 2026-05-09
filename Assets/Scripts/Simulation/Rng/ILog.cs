// Design note:
// ILog is Ember's logging boundary for Simulation systems.
// Systems depend on this interface instead of engine console output, files, or global logging state.
namespace EmberCrpg.Simulation.Logging
{
    /// <summary>
    /// Receives structured diagnostic events emitted by simulation services.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Writes a structured diagnostic event to this logging sink.
        /// </summary>
        void Write(LogEvent logEvent);
    }
}