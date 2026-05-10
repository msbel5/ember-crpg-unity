using System;
using EmberCrpg.Domain.Production;

// Design note:
// JobActivation applies the assigned -> active lifecycle transition.
// It does not choose labor, reserve schedules, pathfind, tick jobs, execute reactions, or mutate global world state.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Result of activating an assigned job.
    /// </summary>
    public sealed class JobActivationResult
    {
        /// <summary>
        /// Job after activation.
        /// </summary>
        public readonly JobRecord Job;

        /// <summary>
        /// True when activation was applied.
        /// </summary>
        public readonly bool WasActivated;

        /// <summary>
        /// Creates a job activation result.
        /// </summary>
        public JobActivationResult(JobRecord job, bool wasActivated)
        {
            Job = job ?? throw new ArgumentNullException(nameof(job));
            WasActivated = wasActivated;
        }
    }

    /// <summary>
    /// Pure deterministic job activation helper.
    /// </summary>
    public static class JobActivation
    {
        /// <summary>
        /// Activates an assigned job that already has an assignee.
        /// </summary>
        public static JobActivationResult Activate(JobRecord job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (job.Status != JobStatus.Assigned)
                throw new InvalidOperationException("Only assigned jobs can be activated.");

            if (job.AssigneeId.IsEmpty)
                throw new InvalidOperationException("Assigned jobs must have an assignee before activation.");

            var activeJob = job.WithStatus(JobStatus.Active);
            return new JobActivationResult(activeJob, true);
        }
    }
}