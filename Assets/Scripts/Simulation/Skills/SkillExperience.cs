using System;
using EmberCrpg.Domain.Skills;

// Design note:
// SkillExperience applies XP to a single SkillRecord deterministically.
// It does not compute job rewards, mutate actors, update SkillSet, run reactions, or write event logs.
namespace EmberCrpg.Simulation.Skills
{
    /// <summary>
    /// Result of applying XP to one skill record.
    /// </summary>
    public sealed class SkillExperienceResult
    {
        /// <summary>
        /// Updated skill record after XP and level recalculation.
        /// </summary>
        public readonly SkillRecord Record;

        /// <summary>
        /// Level before XP application.
        /// </summary>
        public readonly int PreviousLevel;

        /// <summary>
        /// Level after XP application.
        /// </summary>
        public readonly int NewLevel;

        /// <summary>
        /// True when XP application increased the base skill level.
        /// </summary>
        public bool LeveledUp
        {
            get { return NewLevel > PreviousLevel; }
        }

        /// <summary>
        /// Creates a skill experience result.
        /// </summary>
        public SkillExperienceResult(SkillRecord record, int previousLevel, int newLevel)
        {
            if (previousLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(previousLevel), "Previous level cannot be negative.");
            if (newLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(newLevel), "New level cannot be negative.");

            Record = record;
            PreviousLevel = previousLevel;
            NewLevel = newLevel;
        }
    }

    /// <summary>
    /// Pure deterministic skill XP application helper.
    /// </summary>
    public static class SkillExperience
    {
        /// <summary>
        /// Applies XP to a skill record and recomputes its base level.
        /// </summary>
        public static SkillExperienceResult ApplyXp(SkillRecord record, int xpGained)
        {
            if (xpGained < 0)
                throw new ArgumentOutOfRangeException(nameof(xpGained), "XP gained cannot be negative.");

            var previousLevel = record.Level;
            var newXp = AddClamped(record.Xp, xpGained);
            var newLevel = SkillProgression.LevelFromXp(newXp);

            var updated = new SkillRecord(
                record.SkillId,
                newXp,
                newLevel,
                record.RustyLevel,
                record.UnusedCounter);

            return new SkillExperienceResult(updated, previousLevel, newLevel);
        }

        /// <summary>
        /// Applies XP and also marks this skill as used for rust recovery.
        /// </summary>
        public static SkillExperienceResult ApplyXpAndUse(SkillRecord record, int xpGained)
        {
            var xpResult = ApplyXp(record, xpGained);
            var usedRecord = SkillProgression.TickRust(xpResult.Record, true);

            return new SkillExperienceResult(
                usedRecord,
                xpResult.PreviousLevel,
                xpResult.NewLevel);
        }

        private static int AddClamped(int left, int right)
        {
            var value = (long)left + right;
            return value > int.MaxValue ? int.MaxValue : (int)value;
        }
    }
}