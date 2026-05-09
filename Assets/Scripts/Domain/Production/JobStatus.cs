// Design note:
// JobStatus defines the lifecycle states for production jobs.
// Transition validation is pure here; job ticking, actor assignment, XP, and item output live elsewhere.
namespace EmberCrpg.Domain.Production
{
    /// <summary>
    /// Lifecycle status for a production job.
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Job is waiting for an eligible actor.
        /// </summary>
        Queued = 0,

        /// <summary>
        /// Job has an assigned actor but is not actively ticking yet.
        /// </summary>
        Assigned = 1,

        /// <summary>
        /// Job is currently being worked.
        /// </summary>
        Active = 2,

        /// <summary>
        /// Job finished successfully.
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Job was cancelled before completion.
        /// </summary>
        Cancelled = 4
    }

    /// <summary>
    /// Pure transition rules for production job lifecycle states.
    /// </summary>
    public static class JobStatusRules
    {
        /// <summary>
        /// Returns true when a job may move from one status to another.
        /// </summary>
        public static bool CanTransition(JobStatus from, JobStatus to)
        {
            switch (from)
            {
                case JobStatus.Queued:
                    return to == JobStatus.Assigned || to == JobStatus.Cancelled;

                case JobStatus.Assigned:
                    return to == JobStatus.Active || to == JobStatus.Cancelled;

                case JobStatus.Active:
                    return to == JobStatus.Completed || to == JobStatus.Cancelled;

                case JobStatus.Completed:
                case JobStatus.Cancelled:
                    return false;

                default:
                    return false;
            }
        }
    }
}