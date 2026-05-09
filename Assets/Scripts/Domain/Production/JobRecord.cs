using System;
using System.Collections.Generic;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;

// Design note:
// JobRecord stores future-proof runtime state for one production or activity job.
// It links jobs to optional room/activity context without hardcoding worksite, home, ritual, medical, trade, or command semantics.
namespace EmberCrpg.Domain.Production
{
    /// <summary>
    /// Runtime state for one production or activity job.
    /// </summary>
    public sealed class JobRecord
    {
        /// <summary>
        /// Stable job id.
        /// </summary>
        public readonly JobId Id;

        /// <summary>
        /// Data-driven job kind, such as forge, haul, research, cook, ritual, repair, rest, or trade.
        /// </summary>
        public readonly string Kind;

        /// <summary>
        /// Scheduling priority. Lower value means higher priority.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// Current lifecycle status.
        /// </summary>
        public readonly JobStatus Status;

        /// <summary>
        /// Actor assigned to this job, or empty when unassigned.
        /// </summary>
        public readonly ActorId AssigneeId;

        /// <summary>
        /// Skill exercised by this job, or empty when no skill is required.
        /// </summary>
        public readonly SkillId SkillId;

        /// <summary>
        /// Room or zone where this job is anchored, or empty when not room-bound.
        /// </summary>
        public readonly RoomId RoomId;

        /// <summary>
        /// Activity site where this job is anchored, or empty when not site-bound.
        /// </summary>
        public readonly ActivitySiteId ActivitySiteId;

        /// <summary>
        /// Input tags requested by this job, such as ore, fuel, herb, corpse, sample, or tool.
        /// </summary>
        public readonly IReadOnlyList<string> InputTags;

        /// <summary>
        /// Output tags expected from this job, such as ingot, meal, medicine, rumor, data, or ritual_result.
        /// </summary>
        public readonly IReadOnlyList<string> OutputTags;

        /// <summary>
        /// Total ticks required to complete this job.
        /// </summary>
        public readonly int CompletionTicks;

        /// <summary>
        /// Ticks of work already performed.
        /// </summary>
        public readonly int ElapsedTicks;

        /// <summary>
        /// Data tags used by scheduling, UI filters, colony pressure, and adapter packs.
        /// </summary>
        public readonly IReadOnlyList<string> Tags;

        /// <summary>
        /// True when elapsed work has reached required completion ticks.
        /// </summary>
        public bool IsComplete
        {
            get { return ElapsedTicks >= CompletionTicks; }
        }

        /// <summary>
        /// Completion progress clamped to the range 0..1.
        /// </summary>
        public double ProgressFraction
        {
            get { return Math.Min(1.0, (double)ElapsedTicks / CompletionTicks); }
        }

        /// <summary>
        /// Creates runtime state for one production or activity job.
        /// </summary>
        public JobRecord(
            JobId id,
            string kind,
            int priority,
            JobStatus status,
            ActorId assigneeId,
            SkillId skillId,
            RoomId roomId,
            ActivitySiteId activitySiteId,
            IReadOnlyList<string> inputTags,
            IReadOnlyList<string> outputTags,
            int completionTicks,
            int elapsedTicks,
            IReadOnlyList<string> tags)
        {
            if (id.IsEmpty)
                throw new ArgumentException("Job id cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(kind))
                throw new ArgumentException("Job kind cannot be empty.", nameof(kind));

            Id = id;
            Kind = kind;
            Priority = ValidatePriority(priority);
            Status = status;
            AssigneeId = assigneeId;
            SkillId = skillId;
            RoomId = roomId;
            ActivitySiteId = activitySiteId;
            InputTags = CopyTags(inputTags);
            OutputTags = CopyTags(outputTags);
            CompletionTicks = ValidatePositive(completionTicks, nameof(completionTicks));
            ElapsedTicks = ValidateNonNegative(elapsedTicks, nameof(elapsedTicks));
            Tags = CopyTags(tags);
        }

        /// <summary>
        /// Returns a copy with a validated status transition.
        /// </summary>
        public JobRecord WithStatus(JobStatus status)
        {
            if (!JobStatusRules.CanTransition(Status, status))
                throw new InvalidOperationException("Invalid job status transition.");

            return new JobRecord(
                Id,
                Kind,
                Priority,
                status,
                AssigneeId,
                SkillId,
                RoomId,
                ActivitySiteId,
                InputTags,
                OutputTags,
                CompletionTicks,
                ElapsedTicks,
                Tags);
        }

        /// <summary>
        /// Returns a copy with changed elapsed ticks.
        /// </summary>
        public JobRecord WithElapsedTicks(int elapsedTicks)
        {
            return new JobRecord(
                Id,
                Kind,
                Priority,
                Status,
                AssigneeId,
                SkillId,
                RoomId,
                ActivitySiteId,
                InputTags,
                OutputTags,
                CompletionTicks,
                elapsedTicks,
                Tags);
        }

        private static int ValidatePriority(int value)
        {
            if (value < 1 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(value), "Job priority must be between 1 and 5.");

            return value;
        }

        private static int ValidatePositive(int value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName, "Value must be positive.");

            return value;
        }

        private static int ValidateNonNegative(int value, string paramName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");

            return value;
        }

        private static IReadOnlyList<string> CopyTags(IReadOnlyList<string> tags)
        {
            if (tags == null)
                return Array.Empty<string>();

            var copy = new List<string>(tags.Count);
            for (var i = 0; i < tags.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(tags[i]))
                    copy.Add(tags[i]);
            }

            return copy.AsReadOnly();
        }
    }
}