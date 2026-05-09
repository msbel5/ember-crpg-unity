using System;

// Design note:
// SkillRecord stores one actor's runtime state for one data-driven skill.
// It keeps XP, level, rust, and unused ticks only; formulas and job rewards live in separate systems.
namespace EmberCrpg.Domain.Skills
{
    /// <summary>
    /// Runtime state for one actor's progress in one skill.
    /// </summary>
    public readonly struct SkillRecord
    {
        /// <summary>
        /// Stable data-driven skill id.
        /// </summary>
        public readonly SkillId SkillId;

        /// <summary>
        /// Cumulative XP earned in this skill.
        /// </summary>
        public readonly int Xp;

        /// <summary>
        /// Base level derived from cumulative XP.
        /// </summary>
        public readonly int Level;

        /// <summary>
        /// Temporary levels lost to rust from disuse.
        /// </summary>
        public readonly int RustyLevel;

        /// <summary>
        /// Ticks since this skill was last used.
        /// </summary>
        public readonly int UnusedCounter;

        /// <summary>
        /// Level after rust is applied.
        /// </summary>
        public int EffectiveLevel
        {
            get { return Math.Max(0, Level - RustyLevel); }
        }

        /// <summary>
        /// Creates runtime state for one skill.
        /// </summary>
        public SkillRecord(SkillId skillId, int xp, int level, int rustyLevel, int unusedCounter)
        {
            if (skillId.IsEmpty)
                throw new ArgumentException("SkillRecord skill id cannot be empty.", nameof(skillId));

            SkillId = skillId;
            Xp = ValidateNonNegative(xp, nameof(xp));
            Level = ValidateNonNegative(level, nameof(level));
            RustyLevel = ValidateNonNegative(rustyLevel, nameof(rustyLevel));
            UnusedCounter = ValidateNonNegative(unusedCounter, nameof(unusedCounter));
        }

        /// <summary>
        /// Returns a copy with changed XP.
        /// </summary>
        public SkillRecord WithXp(int xp)
        {
            return new SkillRecord(SkillId, xp, Level, RustyLevel, UnusedCounter);
        }

        /// <summary>
        /// Returns a copy with changed rust state.
        /// </summary>
        public SkillRecord WithRust(int rustyLevel, int unusedCounter)
        {
            return new SkillRecord(SkillId, Xp, Level, rustyLevel, unusedCounter);
        }

        private static int ValidateNonNegative(int value, string paramName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName, "Skill values cannot be negative.");

            return value;
        }
    }
}