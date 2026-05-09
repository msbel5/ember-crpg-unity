using System;

// Design note:
// RoomId is Ember's stable identity primitive for local rooms and zones.
// It supports homes, workshops, hospitals, temples, ship cabins, labs, and other multiverse spaces.
namespace EmberCrpg.Domain.World
{
    /// <summary>
    /// Stable handle to a local room or zone.
    /// </summary>
    public readonly struct RoomId : IEquatable<RoomId>
    {
        private readonly string _value;

        /// <summary>
        /// Creates a room identifier from a stable value.
        /// </summary>
        public RoomId(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Stable room identifier value.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this handle does not point to a room or zone.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns true when both room ids carry the same stable value.
        /// </summary>
        public bool Equals(RoomId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is a room id with the same stable value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is RoomId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable room id.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this room id.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "RoomId.Empty" : $"RoomId({Value})";
        }

        /// <summary>
        /// Returns true when both room ids carry the same stable value.
        /// </summary>
        public static bool operator ==(RoomId left, RoomId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when room ids carry different stable values.
        /// </summary>
        public static bool operator !=(RoomId left, RoomId right)
        {
            return !left.Equals(right);
        }
    }
}