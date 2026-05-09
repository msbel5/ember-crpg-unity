using EmberCrpg.Simulation.Logging;
using UnityEngine;

// Design note:
// UnityConsoleLog is the Presentation-only adapter from Ember's structured simulation
// diagnostics to Unity Console output. It is the only layer that may call UnityEngine.Debug.
namespace EmberCrpg.Presentation.Logging
{
    /// <summary>
    /// Writes structured simulation diagnostics to the Unity Console.
    /// </summary>
    public sealed class UnityConsoleLog : ILog
    {
        /// <summary>
        /// Writes a structured diagnostic event to the matching Unity Console channel.
        /// </summary>
        public void Write(LogEvent logEvent)
        {
            var message = logEvent.ToString();

            switch (logEvent.Level)
            {
                case LogLevel.Trace:
                case LogLevel.Event:
                    Debug.Log(message);
                    break;

                case LogLevel.Warn:
                    Debug.LogWarning(message);
                    break;

                case LogLevel.Error:
                    Debug.LogError(message);
                    break;

                default:
                    Debug.Log(message);
                    break;
            }
        }
    }
}