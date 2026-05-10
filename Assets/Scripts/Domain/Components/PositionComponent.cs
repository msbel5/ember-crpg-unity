using System;

// Design note:
// PositionComponent is a deterministic integer local coordinate component.
// It intentionally avoids Unity transforms, floats, physics, pathfinding, rooms, and world lookup.
namespace EmberCrpg.Domain.Components
{
    /// <summary>
    /// Deterministic local integer position for an entity.
    /// </summary>
    public readonly struct PositionComponent : IEquatable<PositionComponent>
    {
        /// <summary>
        /// Local X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Local Y coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Local Z coordinate. Zero is the default 2D plane.
        /// </summary>
        public readonly int Z;

        /// <summary>
        /// Creates a deterministic local position.
        /// </summary>
        public PositionComponent(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a deterministic local position on the default Z plane.
        /// </summary>
        public PositionComponent(int x, int y)
            : this(x, y, 0)
        {
        }

        /// <summary>
        /// True when this position is the local origin.
        /// </summary>
        public bool IsOrigin
        {
            get { return X == 0 && Y == 0 && Z == 0; }
        }

        /// <summary>
        /// Returns Manhattan distance to another local position.
        /// </summary>
        public long ManhattanDistanceTo(PositionComponent other)
        {
            return AbsDelta(X, other.X) + AbsDelta(Y, other.Y) + AbsDelta(Z, other.Z);
        }

        /// <summary>
        /// Returns a moved copy of this position.
        /// </summary>
        public PositionComponent Translate(int deltaX, int deltaY, int deltaZ)
        {
            return new PositionComponent(
                checked(X + deltaX),
                checked(Y + deltaY),
                checked(Z + deltaZ));
        }

        /// <summary>
        /// Returns true when both positions carry the same coordinates.
        /// </summary>
        public bool Equals(PositionComponent other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Returns true when the object is a position with the same coordinates.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is PositionComponent other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from deterministic coordinates.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) + X.GetHashCode();
                hash = (hash * 31) + Y.GetHashCode();
                hash = (hash * 31) + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns a compact debug label for this position.
        /// </summary>
        public override string ToString()
        {
            return $"PositionComponent({X},{Y},{Z})";
        }

        /// <summary>
        /// Returns true when both positions carry the same coordinates.
        /// </summary>
        public static bool operator ==(PositionComponent left, PositionComponent right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when positions carry different coordinates.
        /// </summary>
        public static bool operator !=(PositionComponent left, PositionComponent right)
        {
            return !left.Equals(right);
        }

        private static long AbsDelta(int left, int right)
        {
            var delta = (long)left - right;
            return delta < 0 ? -delta : delta;
        }
    }
}