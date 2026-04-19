using System;

// Design note:
// EntityId is the generic identity spine for Ember's multiverse. ActorId, ItemId,
// SiteId, and FactionId can point to specialized records, but EntityId lets systems
// talk about any world object without pretending everything is an item.
namespace EmberCrpg.Domain.Core
{
    /// <summary>
    /// Stable generic handle to any entity in the world.
    /// </summary>
    public readonly struct EntityId : IEquatable<EntityId>
    {
        private readonly ulong _value;

        /// <summary>
        /// Creates an entity handle from its raw stable identifier.
        /// </summary>
        public EntityId(ulong value)
        {
            _value = value;
        }

        /// <summary>
        /// Raw stable identifier carried by this entity handle.
        /// </summary>
        public ulong Value
        {
            get { return _value; }
        }

        /// <summary>
        /// True when this handle is the empty no-entity sentinel.
        /// </summary>
        public bool IsEmpty
        {
            get { return _value == 0UL; }
        }

        /// <summary>
        /// Returns true when both entity handles carry the same raw identifier.
        /// </summary>
        public bool Equals(EntityId other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Returns true when the object is an entity handle with the same raw identifier.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is EntityId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the raw stable identifier.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a compact debug label for this entity handle.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "EntityId.Empty" : $"EntityId({_value})";
        }

        /// <summary>
        /// Returns true when both entity handles carry the same raw identifier.
        /// </summary>
        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when entity handles carry different raw identifiers.
        /// </summary>
        public static bool operator !=(EntityId left, EntityId right)
        {
            return !left.Equals(right);
        }
    }
}