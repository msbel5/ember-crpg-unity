using System;

// Design note:
// ReactionId is Ember's stable identity primitive for data-driven reactions/process definitions.
// It keeps crafting, research, ritual, medicine, repair, cooking, and sci-fi workflows on one multiverse-safe spine.
namespace EmberCrpg.Domain.Production
{
    /// <summary>
    /// Stable handle to a reaction or process definition.
    /// </summary>
    public readonly struct ReactionId : IEquatable<ReactionId>
    {
        private readonly string _value;

        /// <summary>
        /// Creates a reaction identifier from a stable value.
        /// </summary>
        public ReactionId(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Stable reaction identifier value.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this handle does not point to a reaction definition.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns true when both reaction ids carry the same stable value.
        /// </summary>
        public bool Equals(ReactionId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is a reaction id with the same stable value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is ReactionId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable reaction id.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this reaction id.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "ReactionId.Empty" : $"ReactionId({Value})";
        }

        /// <summary>
        /// Returns true when both reaction ids carry the same stable value.
        /// </summary>
        public static bool operator ==(ReactionId left, ReactionId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when reaction ids carry different stable values.
        /// </summary>
        public static bool operator !=(ReactionId left, ReactionId right)
        {
            return !left.Equals(right);
        }
    }
}