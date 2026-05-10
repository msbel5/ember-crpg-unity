using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;

// Design note:
// JobTick advances active job progress deterministically.
// It does not assign labor, complete reactions, award XP, create items, roll quality, or mutate global world state.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Result of advancing one active job by one simulation tick.
    /// </summary>
    public sealed class JobTickResult
    {
        /// <summary>
        /// Updated job record after applying deterministic work progress.
        /// </summary>
        public readonly JobRecord Job;

        /// <summary>
        /// True when the updated job has reached or exceeded required completion ticks.
        /// </summary>
        public readonly bool IsNowComplete;

        /// <summary>
        /// Number of work ticks applied during this simulation tick.
        /// </summary>
        public readonly int WorkTicksApplied;

        /// <summary>
        /// Creates a job tick result.
        /// </summary>
        public JobTickResult(JobRecord job, int workTicksApplied)
        {
            Job = job ?? throw new ArgumentNullException(nameof(job));
            WorkTicksApplied = workTicksApplied;
            IsNowComplete = job.IsComplete;
        }
    }

    /// <summary>
    /// Pure deterministic job ticking behavior.
    /// </summary>
    public static class JobTick
    {
        /// <summary>
        /// Maximum supported work speed percent for one job tick.
        /// 100 means normal speed, 50 means half speed, 200 means double speed.
        /// </summary>
        public const int MaxWorkSpeedPercent = 10000;

        /// <summary>
        /// Advances an active job by one deterministic simulation tick.
        /// </summary>
        public static JobTickResult Advance(JobRecord job, GameTick tick, int workSpeedPercent)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (job.Status != JobStatus.Active)
                throw new InvalidOperationException("Only active jobs can be advanced.");

            var workTicks = WorkTicksFor(tick, workSpeedPercent);
            var elapsedTicks = AddClamped(job.ElapsedTicks, workTicks);
            var updatedJob = job.WithElapsedTicks(elapsedTicks);

            return new JobTickResult(updatedJob, workTicks);
        }

        /// <summary>
        /// Returns deterministic work ticks for a simulation tick and speed percent.
        /// </summary>
        public static int WorkTicksFor(GameTick tick, int workSpeedPercent)
        {
            if (tick.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(tick), "Game tick cannot be negative for job ticking.");

            if (workSpeedPercent < 0 || workSpeedPercent > MaxWorkSpeedPercent)
                throw new ArgumentOutOfRangeException(nameof(workSpeedPercent), "Work speed percent must be between 0 and 10000.");

            if (workSpeedPercent == 0)
                return 0;

            var currentTickIndex = tick.Value + 1L;
            var previousTickIndex = tick.Value;

            var currentTotal = (currentTickIndex * workSpeedPercent) / 100L;
            var previousTotal = (previousTickIndex * workSpeedPercent) / 100L;
            var delta = currentTotal - previousTotal;

            return delta > int.MaxValue ? int.MaxValue : (int)delta;
        }

        private static int AddClamped(int left, int right)
        {
            var value = (long)left + right;
            return value > int.MaxValue ? int.MaxValue : (int)value;
        }
    }
}