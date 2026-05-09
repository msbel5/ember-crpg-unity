using System;

// Design note:
// SkillId is Ember's data-driven skill identity primitive.
// It lets each universe define its own skill catalog without hardcoding fantasy, sci-fi, or legacy skill lists in Domain.
namespace EmberCrpg.Domain.Skills
{
    /// <summary>
    /// Stable data-driven handle to a skill definition.
    /// </summary>
    public readonly struct SkillId : IEquatable<SkillId>
    {
        private readonly string _value;

        /// <summary>
        /// Creates a skill identifier from a stable definition id.
        /// </summary>
        public SkillId(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Stable skill definition id.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this handle does not point to a skill definition.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns true when both skill ids carry the same stable value.
        /// </summary>
        public bool Equals(SkillId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is a skill id with the same stable value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is SkillId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable skill id.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this skill id.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "SkillId.Empty" : $"SkillId({Value})";
        }

        /// <summary>
        /// Returns true when both skill ids carry the same stable value.
        /// </summary>
        public static bool operator ==(SkillId left, SkillId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when skill ids carry different stable values.
        /// </summary>
        public static bool operator !=(SkillId left, SkillId right)
        {
            return !left.Equals(right);
        }
    }
}