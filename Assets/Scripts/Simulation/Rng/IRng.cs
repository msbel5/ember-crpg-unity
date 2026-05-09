// Design note:
// IRng is Ember's single deterministic randomness contract for Simulation.
// Systems receive this interface instead of calling UnityEngine.Random, System.Random,
// wall-clock time, frame count, or hidden global state.
namespace EmberCrpg.Simulation.Rng
{
    /// <summary>
    /// Deterministic random number generator interface used by simulation systems.
    /// </summary>
    public interface IRng
    {
        /// <summary>
        /// Returns the next unsigned 32-bit random value and advances this RNG stream.
        /// </summary>
        uint NextUInt32();

        /// <summary>
        /// Returns a deterministic integer in the range [minInclusive, maxExclusive).
        /// </summary>
        int Range(int minInclusive, int maxExclusive);

        /// <summary>
        /// Returns true when a deterministic d100-style roll is below the given percent.
        /// </summary>
        bool Chance(int percent);

        /// <summary>
        /// Creates a deterministic sub-stream for a specific system, event, or entity.
        /// </summary>
        IRng Fork(ulong streamId);
    }
}