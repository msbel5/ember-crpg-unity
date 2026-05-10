using System;
using System.Collections.Generic;
using EmberCrpg.Domain.Components;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;

// Design note:
// LaborAssignment selects the best worker for a queued job using deterministic rules.
// It does not mutate jobs, reserve schedules, pathfind, run reactions, inspect full.
// It does not mutate jobs, reserve schedules, pathfind, run reactions, inspect full ActorRecord, or use RNG.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Deterministic result of labor assignment selection.
    /// </summary>
    public sealed class LaborAssignmentResult
    {
        /// <summary>
        /// Empty no-selection result.
        /// </summary>
        public static LaborAssignmentResult None
        {
            get { return new LaborAssignmentResult(default(ActorId), 0, 0); }
        }

        /// <summary>
        /// Selected actor id, or empty when no candidate was eligible.
        /// </summary>
        public readonly ActorId SelectedActorId;

        /// <summary>
        /// Manhattan distance from selected candidate to target position.
        /// </summary>
        public readonly long Distance;

        /// <summary>
        /// Selected candidate's effective skill level for the job skill.
        /// </summary>
        public readonly int EffectiveSkillLevel;

        /// <summary>
        /// True when an actor was selected.
        /// </summary>
        public bool HasSelection
        {
            get { return !SelectedActorId.IsEmpty; }
        }

        /// <summary>
        /// Creates a labor assignment selection result.
        /// </summary>
        public LaborAssignmentResult(ActorId selectedActorId, long distance, int effectiveSkillLevel)
        {
            if (!selectedActorId.IsEmpty && distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance), "Assignment distance cannot be negative.");
            if (effectiveSkillLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(effectiveSkillLevel), "Effective skill level cannot be negative.");

            SelectedActorId = selectedActorId;
            Distance = distance;
            EffectiveSkillLevel = effectiveSkillLevel;
        }
    }

    /// <summary>
    /// Pure deterministic labor assignment selector.
    /// </summary>
    public static class LaborAssignment
    {
        /// <summary>
        /// Selects the best available candidate for a queued job.
        /// </summary>
        public static LaborAssignmentResult SelectBest(
            JobRecord job,
            PositionComponent targetPosition,
            IReadOnlyList<LaborCandidate> candidates)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (job.Status != JobStatus.Queued)
                throw new InvalidOperationException("Only queued jobs can receive labor assignment.");

            var list = candidates ?? Array.Empty<LaborCandidate>();
            LaborCandidate bestCandidate = null;
            long bestDistance = 0;
            int bestSkill = 0;

            for (var i = 0; i < list.Count; i++)
            {
                var candidate = list[i];
                if (candidate == null || !candidate.IsEligibleFor(job.SkillId))
                    continue;

                var distance = candidate.DistanceTo(targetPosition);
                var skill = candidate.EffectiveSkillFor(job.SkillId);

                if (bestCandidate == null || IsBetter(candidate, distance, skill, bestCandidate, bestDistance, bestSkill))
                {
                    bestCandidate = candidate;
                    bestDistance = distance;
                    bestSkill = skill;
                }
            }

            if (bestCandidate == null)
                return LaborAssignmentResult.None;

            return new LaborAssignmentResult(bestCandidate.ActorId, bestDistance, bestSkill);
        }

        private static bool IsBetter(
            LaborCandidate candidate,
            long distance,
            int skill,
            LaborCandidate bestCandidate,
            long bestDistance,
            int bestSkill)
        {
            if (distance < bestDistance)
                return true;

            if (distance > bestDistance)
                return false;

            if (skill > bestSkill)
                return true;

            if (skill < bestSkill)
                return false;

            return candidate.ActorId.Value < bestCandidate.ActorId.Value;
        }
    }
}