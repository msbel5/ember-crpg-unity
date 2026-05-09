using System;

// Design note:
// ReactionQualityFormula is a data-driven selector for how reaction output quality is resolved.
// It does not calculate quality; actual formula execution lives in production systems such as CraftQuality.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// Stable selector for reaction output quality behavior.
    /// </summary>
    public readonly struct ReactionQualityFormula : IEquatable<ReactionQualityFormula>
    {
        /// <summary>
        /// Canonical weighted-random quality formula id.
        /// </summary>
        public const string WeightedRandomValue = "weighted_random";

        /// <summary>
        /// Canonical fixed quality formula id.
        /// </summary>
        public const string FixedValue = "fixed";

        private readonly string _value;

        /// <summary>
        /// Creates a quality formula selector from a stable value.
        /// </summary>
        public ReactionQualityFormula(string value)
        {
            _value = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Canonical weighted-random quality formula selector.
        /// </summary>
        public static ReactionQualityFormula WeightedRandom
        {
            get { return new ReactionQualityFormula(WeightedRandomValue); }
        }

        /// <summary>
        /// Canonical fixed quality formula selector.
        /// </summary>
        public static ReactionQualityFormula Fixed
        {
            get { return new ReactionQualityFormula(FixedValue); }
        }

        /// <summary>
        /// Stable formula selector value.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this selector has no stable formula id.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// True when this selector uses weighted random quality.
        /// </summary>
        public bool IsWeightedRandom
        {
            get { return string.Equals(Value, WeightedRandomValue, StringComparison.Ordinal); }
        }

        /// <summary>
        /// True when this selector uses fixed quality.
        /// </summary>
        public bool IsFixed
        {
            get { return string.Equals(Value, FixedValue, StringComparison.Ordinal); }
        }

        /// <summary>
        /// Returns true when both selectors carry the same stable value.
        /// </summary>
        public bool Equals(ReactionQualityFormula other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is a quality formula selector with the same value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is ReactionQualityFormula other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable selector value.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this quality formula selector.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "ReactionQualityFormula.Empty" : $"ReactionQualityFormula({Value})";
        }

        /// <summary>
        /// Returns true when both selectors carry the same stable value.
        /// </summary>
        public static bool operator ==(ReactionQualityFormula left, ReactionQualityFormula right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when selectors carry different stable values.
        /// </summary>
        public static bool operator !=(ReactionQualityFormula left, ReactionQualityFormula right)
        {
            return !left.Equals(right);
        }
    }
}