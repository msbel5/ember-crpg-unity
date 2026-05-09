using System;

// Design note:
// ActivitySiteId is Ember's stable identity primitive for local activity anchors.
// It supports beds, forges, lab benches, altars, consoles, campfires, stalls, and other multiverse interaction sites.
namespace EmberCrpg.Domain.World
{
    /// <summary>
    /// Stable handle to a local activity anchor.
    /// </summary>
    public readonly struct ActivitySiteId : IEquatable<ActivitySiteId>
    {
        private readonly string _value;

        /// <summary>
        /// Creates an activity site identifier from a stable value.
        /// </summary>
        public ActivitySiteId(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Stable activity site identifier value.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this handle does not point to an activity site.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns true when both activity site ids carry the same stable value.
        /// </summary>
        public bool Equals(ActivitySiteId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is an activity site id with the same stable value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is ActivitySiteId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable activity site id.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this activity site id.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "ActivitySiteId.Empty" : $"ActivitySiteId({Value})";
        }

        /// <summary>
        /// Returns true when both activity site ids carry the same stable value.
        /// </summary>
        public static bool operator ==(ActivitySiteId left, ActivitySiteId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when activity site ids carry different stable values.
        /// </summary>
        public static bool operator !=(ActivitySiteId left, ActivitySiteId right)
        {
            return !left.Equals(right);
        }
    }
}