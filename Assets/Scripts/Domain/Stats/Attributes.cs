using System;
using StatAttribute = EmberCrpg.Domain.Stats.Attribute;

// Design note:
// Attributes is Ember's immutable six-stat value container.
// It stores canonical 0..100 stat values only; formulas, rolls, skills, and derived stats live elsewhere.
namespace EmberCrpg.Domain.Stats
{
    /// <summary>
    /// Immutable six-stat value container for an actor.
    /// </summary>
    public readonly struct Attributes
    {
        /// <summary>
        /// Minimum allowed value for each Ember attribute.
        /// </summary>
        public const int MinValue = 0;

        /// <summary>
        /// Maximum allowed value for each Ember attribute.
        /// </summary>
        public const int MaxValue = 100;

        /// <summary>
        /// Default baseline value for an average actor.
        /// </summary>
        public const int BaseValue = 50;

        /// <summary>
        /// Returns an average six-stat set with every attribute at 50.
        /// </summary>
        public static Attributes Base50
        {
            get { return new Attributes(BaseValue, BaseValue, BaseValue, BaseValue, BaseValue, BaseValue); }
        }

        /// <summary>
        /// Might: physical power, melee force, carry strength, and intimidation pressure.
        /// </summary>
        public readonly int Mig;

        /// <summary>
        /// Agility: speed, reflexes, precision, evasion, and ranged handling.
        /// </summary>
        public readonly int Agi;

        /// <summary>
        /// Endurance: stamina, resilience, pain tolerance, poison resistance, and fatigue depth.
        /// </summary>
        public readonly int End;

        /// <summary>
        /// Mind: intellect, memory, learning, spell structure, and technical reasoning.
        /// </summary>
        public readonly int Mnd;

        /// <summary>
        /// Insight: perception, intuition, awareness, will checks, and social reading.
        /// </summary>
        public readonly int Ins;

        /// <summary>
        /// Presence: force of personality, leadership, social pressure, and command aura.
        /// </summary>
        public readonly int Pre;

        /// <summary>
        /// Creates an immutable six-stat value container.
        /// </summary>
        public Attributes(int mig, int agi, int end, int mnd, int ins, int pre)
        {
            Mig = Validate(mig, nameof(mig));
            Agi = Validate(agi, nameof(agi));
            End = Validate(end, nameof(end));
            Mnd = Validate(mnd, nameof(mnd));
            Ins = Validate(ins, nameof(ins));
            Pre = Validate(pre, nameof(pre));
        }

        /// <summary>
        /// Returns the value of the requested Ember attribute.
        /// </summary>
        public int Get(StatAttribute attribute)
        {
            switch (attribute)
            {
                case StatAttribute.Mig: return Mig;
                case StatAttribute.Agi: return Agi;
                case StatAttribute.End: return End;
                case StatAttribute.Mnd: return Mnd;
                case StatAttribute.Ins: return Ins;
                case StatAttribute.Pre: return Pre;
                default: throw new ArgumentOutOfRangeException(nameof(attribute), attribute, "Unknown attribute.");
            }
        }

        /// <summary>
        /// Returns a copy with one attribute changed.
        /// </summary>
        public Attributes With(StatAttribute attribute, int value)
        {
            switch (attribute)
            {
                case StatAttribute.Mig: return new Attributes(value, Agi, End, Mnd, Ins, Pre);
                case StatAttribute.Agi: return new Attributes(Mig, value, End, Mnd, Ins, Pre);
                case StatAttribute.End: return new Attributes(Mig, Agi, value, Mnd, Ins, Pre);
                case StatAttribute.Mnd: return new Attributes(Mig, Agi, End, value, Ins, Pre);
                case StatAttribute.Ins: return new Attributes(Mig, Agi, End, Mnd, value, Pre);
                case StatAttribute.Pre: return new Attributes(Mig, Agi, End, Mnd, Ins, value);
                default: throw new ArgumentOutOfRangeException(nameof(attribute), attribute, "Unknown attribute.");
            }
        }

        private static int Validate(int value, string paramName)
        {
            if (value < MinValue || value > MaxValue)
                throw new ArgumentOutOfRangeException(paramName, "Attribute values must be between 0 and 100.");

            return value;
        }
    }
}