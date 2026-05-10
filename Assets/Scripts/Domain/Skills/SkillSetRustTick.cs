using System;
using System.Collections.Generic;
using EmberCrpg.Domain.Skills;

// Design note:
// SkillSetRustTick applies one deterministic rust/update tick across an actor-local SkillSet.
// It does not award XP, mutate actors, inspect jobs, run reactions, or write event logs.
namespace EmberCrpg.Simulation.Skills
{
    /// <summary>
    /// Result of ticking skill rust across a SkillSet.
    /// </summary>
    public sealed class SkillSetRustTickResult
    {
        /// <summary>
        /// Updated actor-local skill set.
        /// </summary>
        public readonly SkillSet SkillSet;

        /// <summary>
        /// Number of records processed.
        /// </summary>
        public readonly int TickedRecords;

        /// <summary>
        /// Number of existing records marked as used this tick.
        /// </summary>
        public readonly int UsedRecords;

        /// <summary>
        /// Number of records that gained rust this tick.
        /// </summary>
        public readonly int RustedRecords;

        /// <summary>
        /// Number of records that recovered one rusty level this tick.
        /// </summary>
        public readonly int RecoveredRecords;

        /// <summary>
        /// Creates a SkillSet rust tick result.
        /// </summary>
        public SkillSetRustTickResult(
            SkillSet skillSet,
            int tickedRecords,
            int usedRecords,
            int rustedRecords,
            int recoveredRecords)
        {
            if (tickedRecords < 0)
                throw new ArgumentOutOfRangeException(nameof(tickedRecords), "Ticked record count cannot be negative.");
            if (usedRecords < 0)
                throw new ArgumentOutOfRangeException(nameof(usedRecords), "Used record count cannot be negative.");
            if (rustedRecords < 0)
                throw new ArgumentOutOfRangeException(nameof(rustedRecords), "Rusted record count cannot be negative.");
            if (recoveredRecords < 0)
                throw new ArgumentOutOfRangeException(nameof(recoveredRecords), "Recovered record count cannot be negative.");

            SkillSet = skillSet ?? SkillSet.Empty;
            TickedRecords = tickedRecords;
            UsedRecords = usedRecords;
            RustedRecords = rustedRecords;
            RecoveredRecords = recoveredRecords;
        }
    }

    /// <summary>
    /// Pure deterministic SkillSet rust ticking helper.
    /// </summary>
    public static class SkillSetRustTick
    {
        /// <summary>
        /// Applies one rust tick across all existing skill records.
        /// </summary>
        public static SkillSetRustTickResult Tick(
            SkillSet skillSet,
            IReadOnlyList<SkillId> usedSkillIds)
        {
            var sourceSet = skillSet ?? SkillSet.Empty;
            var usedIds = usedSkillIds ?? Array.Empty<SkillId>();

            var updatedSet = sourceSet;
            var tickedRecords = 0;
            var usedRecords = 0;
            var rustedRecords = 0;
            var recoveredRecords = 0;

            for (var i = 0; i < sourceSet.Records.Count; i++)
            {
                var before = sourceSet.Records[i];
                var used = ContainsSkillId(usedIds, before.SkillId);
                var after = SkillProgression.TickRust(before, used);

                updatedSet = updatedSet.With(after);
                tickedRecords++;

                if (used)
                    usedRecords++;

                if (after.RustyLevel > before.RustyLevel)
                    rustedRecords++;

                if (after.RustyLevel < before.RustyLevel)
                    recoveredRecords++;
            }

            return new SkillSetRustTickResult(
                updatedSet,
                tickedRecords,
                usedRecords,
                rustedRecords,
                recoveredRecords);
        }

        private static bool ContainsSkillId(IReadOnlyList<SkillId> skillIds, SkillId target)
        {
            if (target.IsEmpty)
                return false;

            for (var i = 0; i < skillIds.Count; i++)
            {
                if (!skillIds[i].IsEmpty && skillIds[i] == target)
                    return true;
            }

            return false;
        }
    }
}