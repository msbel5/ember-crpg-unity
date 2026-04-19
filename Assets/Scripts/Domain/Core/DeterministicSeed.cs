using System;

// Design note:
// DeterministicSeed is the stable input for procedural generation, seeded rolls,
// replay checks, and forked RNG streams. It is data only, not an RNG implementation.
namespace EmberCrpg.Domain.Core
{
    /// <summary>
    /// Stable deterministic seed value used by simulation systems.
    /// </summary>
    public readonly struct DeterministicSeed : IEquatable<DeterministicSeed>
    {
        private readonly ulong _value;

        /// <summary>
        /// Creates a deterministic seed from its raw value.
        /// </summary>
        public DeterministicSeed(ulong value)
        {
            _value = value;
        }

        /// <summary>
        /// Raw seed value.
        /// </summary>
        public ulong Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Returns true when both seeds carry the same raw value.
        /// </summary>
        public bool Equals(DeterministicSeed other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Returns true when the object is a deterministic seed with the same value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is DeterministicSeed other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the raw seed.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a compact debug label for this seed.
        /// </summary>
        public override string ToString()
        {
            return $"DeterministicSeed({_value})";
        }

        /// <summary>
        /// Returns true when both seeds carry the same raw value.
        /// </summary>
        public static bool operator ==(DeterministicSeed left, DeterministicSeed right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when seeds carry different raw values.
        /// </summary>
        public static bool operator !=(DeterministicSeed left, DeterministicSeed right)
        {
            return !left.Equals(right);
        }
    }
}