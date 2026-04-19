using System;

// Design note:
// UniverseId separates content packs, physics laws, and reality rules. It exists so
// fantasy, sci-fi, vampire gothic, and space opera worlds can share one engine.
namespace EmberCrpg.Domain.Core
{
    /// <summary>
    /// Stable handle to a universe or ruleset scope.
    /// </summary>
    public readonly struct UniverseId : IEquatable<UniverseId>
    {
        private readonly ulong _value;

        /// <summary>
        /// Creates a universe handle from its raw stable identifier.
        /// </summary>
        public UniverseId(ulong value)
        {
            _value = value;
        }

        /// <summary>
        /// Raw stable identifier carried by this universe handle.
        /// </summary>
        public ulong Value
        {
            get { return _value; }
        }

        /// <summary>
        /// True when this handle is the empty no-universe sentinel.
        /// </summary>
        public bool IsEmpty
        {
            get { return _value == 0UL; }
        }

        /// <summary>
        /// Returns true when both universe handles carry the same raw identifier.
        /// </summary>
        public bool Equals(UniverseId other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Returns true when the object is a universe handle with the same raw identifier.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is UniverseId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the raw stable identifier.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a compact debug label for this universe handle.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "UniverseId.Empty" : $"UniverseId({_value})";
        }

        /// <summary>
        /// Returns true when both universe handles carry the same raw identifier.
        /// </summary>
        public static bool operator ==(UniverseId left, UniverseId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when universe handles carry different raw identifiers.
        /// </summary>
        public static bool operator !=(UniverseId left, UniverseId right)
        {
            return !left.Equals(right);
        }
    }
}