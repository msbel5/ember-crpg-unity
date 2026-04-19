using System;

// Design note:
// GameTick is the smallest deterministic simulation clock unit. It does not use
// DateTime, Unity Time, frame count, or wall-clock time.
namespace EmberCrpg.Domain.Core
{
    /// <summary>
    /// Deterministic simulation tick. Value type; default value is tick zero.
    /// </summary>
    public readonly struct GameTick : IEquatable<GameTick>, IComparable<GameTick>
    {
        private readonly long _value;

        /// <summary>
        /// Creates a game tick from its raw tick value.
        /// </summary>
        public GameTick(long value)
        {
            _value = value;
        }

        /// <summary>
        /// Raw deterministic tick value.
        /// </summary>
        public long Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Creates a new tick advanced by the given amount.
        /// </summary>
        public GameTick Add(long ticks)
        {
            return new GameTick(_value + ticks);
        }

        /// <summary>
        /// Compares this tick with another tick.
        /// </summary>
        public int CompareTo(GameTick other)
        {
            return _value.CompareTo(other._value);
        }

        /// <summary>
        /// Returns true when both ticks carry the same raw value.
        /// </summary>
        public bool Equals(GameTick other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Returns true when the object is a game tick with the same raw value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is GameTick other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the raw tick.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a compact debug label for this tick.
        /// </summary>
        public override string ToString()
        {
            return $"GameTick({_value})";
        }

        /// <summary>
        /// Returns true when both ticks carry the same raw value.
        /// </summary>
        public static bool operator ==(GameTick left, GameTick right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when ticks carry different raw values.
        /// </summary>
        public static bool operator !=(GameTick left, GameTick right)
        {
            return !left.Equals(right);
        }
    }
}