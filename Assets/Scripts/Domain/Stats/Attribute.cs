// Design note:
// EmberAttribute defines Ember's canonical six-stat vocabulary.
// It identifies stat axes only; values, ranges, formulas, and universe-specific labels live elsewhere.
namespace EmberCrpg.Domain.Stats
{
    /// <summary>
    /// Canonical Ember attribute identifiers used by actors, formulas, skills, and saves.
    /// </summary>
    public enum Attribute
    {
        /// <summary>
        /// Might: physical power, melee force, carry strength, and intimidation pressure.
        /// </summary>
        Mig = 0,

        /// <summary>
        /// Agility: speed, reflexes, precision, evasion, and ranged handling.
        /// </summary>
        Agi = 1,

        /// <summary>
        /// Endurance: stamina, resilience, pain tolerance, poison resistance, and fatigue depth.
        /// </summary>
        End = 2,

        /// <summary>
        /// Mind: intellect, memory, learning, spell structure, and technical reasoning.
        /// </summary>
        Mnd = 3,

        /// <summary>
        /// Insight: perception, intuition, awareness, will checks, and social reading.
        /// </summary>
        Ins = 4,

        /// <summary>
        /// Presence: force of personality, leadership, social pressure, and command aura.
        /// </summary>
        Pre = 5
    }
}