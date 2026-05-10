using System;
using EmberCrpg.Domain.Skills;

// Design note:
// SkillSetExperienceApplication applies XP to an actor-local SkillSet deterministically.
// It does not know jobs, actors, reactions, inventory, event logs, or global world state.
namespace EmberCrpg.Simulation.Skills
{
    /// <summary>
    /// Result of applying skill XP to a SkillSet.
    /// </summary>
    public sealed class SkillSetExperienceApplicationResult
    {
        /// <summary>
        /// Updated actor-local skill set.
        /// </summary>
        public readonly SkillSet SkillSet;

        /// <summary>
        /// Skill affected by this application, or empty when no XP was applied.
        /// </summary>
        public readonly SkillId SkillId;

        /// <summary>
        /// Level before XP application.
        /// </summary>
        public readonly int PreviousLevel;

        /// <summary>
        /// Level after XP application.
        /// </summary>
        public readonly int NewLevel;

        /// <summary>
        /// True when a previously missing skill record was created.
        /// </summary>
        public readonly bool CreatedNewRecord;

        /// <summary>
        /// True when XP/use was applied to a skill record.
        /// </summary>
        public bool WasApplied
        {
            get { return !SkillId.IsEmpty; }
        }

        /// <summary>
        /// True when the affected skill increased base level.
        /// </summary>
        public bool LeveledUp
        {
            get { return NewLevel > PreviousLevel; }
        }

        /// <summary>
        /// Creates a SkillSet XP application result.
        /// </summary>
        public SkillSetExperienceApplicationResult(
            SkillSet skillSet,
            SkillId skillId,
            int previousLevel,
            int newLevel,
            bool createdNewRecord)
        {
            if (previousLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(previousLevel), "Previous level cannot be negative.");
            if (newLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(newLevel), "New level cannot be negative.");

            SkillSet = skillSet ?? SkillSet.Empty;
            SkillId = skillId;
            PreviousLevel = previousLevel;
            NewLevel = newLevel;
            CreatedNewRecord = createdNewRecord;
        }
    }

    /// <summary>
    /// Pure deterministic SkillSet XP application helper.
    /// </summary>
    public static class SkillSetExperienceApplication
    {
        /// <summary>
        /// Applies XP and use-state to one skill in a SkillSet.
        /// </summary>
        public static SkillSetExperienceApplicationResult Apply(
            SkillSet skillSet,
            SkillId skillId,
            int xpGained)
        {
            if (xpGained < 0)
                throw new ArgumentOutOfRangeException(nameof(xpGained), "XP gained cannot be negative.");

            var sourceSet = skillSet ?? SkillSet.Empty;

            if (skillId.IsEmpty)
                return new SkillSetExperienceApplicationResult(sourceSet, default(SkillId), 0, 0, false);

            var exists = sourceSet.TryGet(skillId, out var record);
            if (!exists && xpGained == 0)
                return new SkillSetExperienceApplicationResult(sourceSet, default(SkillId), 0, 0, false);

            var sourceRecord = exists ? record : new SkillRecord(skillId, 0, 0, 0, 0);
            var experience = SkillExperience.ApplyXpAndUse(sourceRecord, xpGained);
            var updatedSet = sourceSet.With(experience.Record);

            return new SkillSetExperienceApplicationResult(
                updatedSet,
                skillId,
                experience.PreviousLevel,
                experience.NewLevel,
                !exists);
        }
    }
}