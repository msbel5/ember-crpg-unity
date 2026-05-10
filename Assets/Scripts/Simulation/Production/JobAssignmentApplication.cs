using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;

// Design note:
// JobAssignmentApplication applies a deterministic labor assignment result to a queued job.
// It does not choose candidates, pathfind, reserve schedules, activate jobs, tick jobs, or run reactions.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Result of applying a labor assignment selection to a job.
    /// </summary>
    public sealed class JobAssignmentApplicationResult
    {
        /// <summary>
        /// Job after assignment application.
        /// </summary>
        public readonly JobRecord Job;

        /// <summary>
        /// Selected actor id, or empty when no assignment was applied.
        /// </summary>
        public readonly ActorId SelectedActorId;

        /// <summary>
        /// Distance reported by labor selection.
        /// </summary>
        public readonly long Distance;

        /// <summary>
        /// Effective skill level reported by labor selection.
        /// </summary>
        public readonly int EffectiveSkillLevel;

        /// <summary>
        /// True when this result assigned the job to an actor.
        /// </summary>
        public bool WasAssigned
        {
            get { return !SelectedActorId.IsEmpty; }
        }

        /// <summary>
        /// Creates an assignment application result.
        /// </summary>
        public JobAssignmentApplicationResult(
            JobRecord job,
            ActorId selectedActorId,
            long distance,
            int effectiveSkillLevel)
        {
            Job = job ?? throw new ArgumentNullException(nameof(job));

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
    /// Pure deterministic bridge from labor selection to job assignment state.
    /// </summary>
    public static class JobAssignmentApplication
    {
        /// <summary>
        /// Applies a labor assignment result to a job.
        /// </summary>
        public static JobAssignmentApplicationResult Apply(
            JobRecord job,
            LaborAssignmentResult selection)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (selection == null)
                throw new ArgumentNullException(nameof(selection));

            if (!selection.HasSelection)
                return new JobAssignmentApplicationResult(job, default(ActorId), 0, 0);

            var assignedJob = job.AssignTo(selection.SelectedActorId);

            return new JobAssignmentApplicationResult(
                assignedJob,
                selection.SelectedActorId,
                selection.Distance,
                selection.EffectiveSkillLevel);
        }
    }
}