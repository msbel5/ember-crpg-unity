using System;

// Design note:
// PcgRng is Ember's concrete deterministic RNG for simulation code.
// It implements PCG-XSH-RR 64-to-32 and replaces non-deterministic random sources in all game rules.
namespace EmberCrpg.Simulation.Rng
{
    /// <summary>
    /// PCG-XSH-RR deterministic random number generator for simulation systems.
    /// </summary>
    public sealed class PcgRng : IRng
    {
        private const ulong Multiplier = 6364136223846793005UL;
        private const ulong StreamSalt = 0x9E3779B97F4A7C15UL;

        private ulong _state;
        private readonly ulong _increment;

        /// <summary>
        /// Creates a deterministic RNG stream from a world seed.
        /// </summary>
        public PcgRng(ulong worldSeed)
            : this(worldSeed, 0UL)
        {
        }

        /// <summary>
        /// Creates a deterministic RNG stream from a world seed and stream id.
        /// </summary>
        public PcgRng(ulong worldSeed, ulong streamId)
        {
            _state = 0UL;
            _increment = (Mix64(worldSeed ^ streamId) << 1) | 1UL;

            NextUInt32();
            _state += Mix64(worldSeed + StreamSalt + streamId);
            NextUInt32();
        }

        /// <summary>
        /// Returns the next unsigned 32-bit random value and advances this RNG stream.
        /// </summary>
        public uint NextUInt32()
        {
            var oldState = _state;
            _state = unchecked(oldState * Multiplier + _increment);

            var xorShifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
            var rotation = (int)(oldState >> 59);

            return (xorShifted >> rotation) | (xorShifted << ((-rotation) & 31));
        }

        /// <summary>
        /// Returns a deterministic integer in the range [minInclusive, maxExclusive).
        /// </summary>
        public int Range(int minInclusive, int maxExclusive)
        {
            var span = (long)maxExclusive - minInclusive;
            if (span <= 0L || span > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Range bounds must form a positive int-sized span.");

            var bound = (uint)span;
            var threshold = unchecked((0U - bound)) % bound;

            while (true)
            {
                var value = NextUInt32();
                if (value >= threshold)
                    return minInclusive + (int)(value % bound);
            }
        }

        /// <summary>
        /// Returns true when a deterministic d100-style roll is below the given percent.
        /// </summary>
        public bool Chance(int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentOutOfRangeException(nameof(percent), "Percent must be between 0 and 100.");

            if (percent == 0)
                return false;
            if (percent == 100)
                return true;

            return Range(0, 100) < percent;
        }

        /// <summary>
        /// Creates a deterministic sub-stream for a specific system, event, or entity.
        /// </summary>
        public IRng Fork(ulong streamId)
        {
            return new PcgRng(Mix64(_state ^ streamId), streamId);
        }

        private static ulong Mix64(ulong value)
        {
            value ^= value >> 30;
            value *= 0xBF58476D1CE4E5B9UL;
            value ^= value >> 27;
            value *= 0x94D049BB133111EBUL;
            value ^= value >> 31;

            return value;
        }
    }
}