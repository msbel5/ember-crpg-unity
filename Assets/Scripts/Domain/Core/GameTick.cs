using System;

// Design note:
// GameTick is Ember's deterministic simulation ordering primitive. It orders commands,
// events, replay hashes, and system ticks; it is not calendar time, Unity frame time,
// wall-clock time, weather, or schedule logic.
namespace EmberCrpg.Domain.Core
{
    /// <summary>
    /// Deterministic simulation tick. Value type; default value is tick zero.
    /// </summary>
    public readonly struct GameTick : IEquatable<GameTick>, IComparable<GameTick>
    {
        private readonly long _value;

        /// <summary>
        /// Creates a simulation tick from its raw non-negative tick value.
        /// </summary>
        public GameTick(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Game tick cannot be negative.");

            _value = value;
        }

        /// <summary>
        /// Raw deterministic simulation tick value.
        /// </summary>
        public long Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Creates a new tick advanced by the given number of simulation ticks.
        /// </summary>
        public GameTick Add(long ticks)
        {
            return new GameTick(_value + ticks);
        }

        /// <summary>
        /// Compares ticks by their raw simulation order.
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
        /// Returns a new tick advanced by simulation ticks.
        /// </summary>
        public static GameTick operator +(GameTick tick, long ticks)
        {
            return tick.Add(ticks);
        }

        /// <summary>
        /// Returns the signed delta between two simulation ticks.
        /// </summary>
        public static long operator -(GameTick left, GameTick right)
        {
            return left._value - right._value;
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

        /// <summary>
        /// Returns true when the left tick is earlier than the right tick.
        /// </summary>
        public static bool operator <(GameTick left, GameTick right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Returns true when the left tick is later than the right tick.
        /// </summary>
        public static bool operator >(GameTick left, GameTick right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Returns true when the left tick is not later than the right tick.
        /// </summary>
        public static bool operator <=(GameTick left, GameTick right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Returns true when the left tick is not earlier than the right tick.
        /// </summary>
        public static bool operator >=(GameTick left, GameTick right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}