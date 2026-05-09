using System;

// Design note:
// ActivitySiteRole is Ember's data-driven function tag for local activity anchors.
// It keeps work, home, ritual, trade, medical, command, storage, and future multiverse roles out of hardcoded enums.
namespace EmberCrpg.Domain.World
{
    /// <summary>
    /// Stable data-driven role assigned to an activity site.
    /// </summary>
    public readonly struct ActivitySiteRole : IEquatable<ActivitySiteRole>
    {
        private readonly string _value;

        /// <summary>
        /// Creates an activity site role from a stable value.
        /// </summary>
        public ActivitySiteRole(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Stable role identifier value.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this role has no stable identifier.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns true when both roles carry the same stable value.
        /// </summary>
        public bool Equals(ActivitySiteRole other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is an activity site role with the same stable value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is ActivitySiteRole other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable role value.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this activity site role.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "ActivitySiteRole.Empty" : $"ActivitySiteRole({Value})";
        }

        /// <summary>
        /// Returns true when both roles carry the same stable value.
        /// </summary>
        public static bool operator ==(ActivitySiteRole left, ActivitySiteRole right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when roles carry different stable values.
        /// </summary>
        public static bool operator !=(ActivitySiteRole left, ActivitySiteRole right)
        {
            return !left.Equals(right);
        }
    }
}