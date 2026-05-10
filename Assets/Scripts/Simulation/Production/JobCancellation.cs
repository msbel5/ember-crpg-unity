using System;
using System.Collections.Generic;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Production;

// Design note:
// JobCancellation turns a cancellable job into a deterministic cancellation result.
// It does not mutate inventory, restore stockpiles, create items, or write event logs.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// A tagged input that should be refunded or released by a later inventory/store mutation step.
    /// </summary>
    public sealed class JobCancellationRefund
    {
        /// <summary>
        /// Refunded material or item tag.
        /// </summary>
        public readonly string Tag;

        /// <summary>
        /// Refunded quantity.
        /// </summary>
        public readonly int Quantity;

        /// <summary>
        /// Creates a refund descriptor.
        /// </summary>
        public JobCancellationRefund(string tag, int quantity)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Refund tag cannot be empty.", nameof(tag));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Refund quantity must be positive.");

            Tag = tag.Trim();
            Quantity = quantity;
        }

        /// <summary>
        /// Returns a copy with additional refunded quantity.
        /// </summary>
        public JobCancellationRefund WithAdditionalQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Additional refund quantity must be positive.");

            var value = (long)Quantity + quantity;
            var clamped = value > int.MaxValue ? int.MaxValue : (int)value;
            return new JobCancellationRefund(Tag, clamped);
        }

        /// <summary>
        /// Returns a compact debug label for this refund.
        /// </summary>
        public override string ToString()
        {
            return $"JobCancellationRefund({Tag} x{Quantity})";
        }
    }

    /// <summary>
    /// Deterministic result of cancelling a job.
    /// </summary>
    public sealed class JobCancellationResult
    {
        /// <summary>
        /// Cancelled job record.
        /// </summary>
        public readonly JobRecord CancelledJob;

        /// <summary>
        /// Refund descriptors that a later inventory/store step may apply.
        /// </summary>
        public readonly IReadOnlyList<JobCancellationRefund> Refunds;

        /// <summary>
        /// Creates a deterministic job cancellation result.
        /// </summary>
        public JobCancellationResult(JobRecord cancelledJob, IReadOnlyList<JobCancellationRefund> refunds)
        {
            CancelledJob = cancelledJob ?? throw new ArgumentNullException(nameof(cancelledJob));
            Refunds = refunds ?? Array.Empty<JobCancellationRefund>();
        }

        /// <summary>
        /// Returns true when this cancellation result includes a refund for the supplied tag.
        /// </summary>
        public bool HasRefund(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            var normalized = tag.Trim();
            for (var i = 0; i < Refunds.Count; i++)
            {
                if (string.Equals(Refunds[i].Tag, normalized, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Pure deterministic job cancellation helper.
    /// </summary>
    public static class JobCancellation
    {
        /// <summary>
        /// Cancels a queued, assigned, or active job and returns refundable input descriptors.
        /// </summary>
        public static JobCancellationResult Cancel(JobRecord job, ReactionDef reaction)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (reaction == null)
                throw new ArgumentNullException(nameof(reaction));

            if (job.Status == JobStatus.Completed || job.Status == JobStatus.Cancelled)
                throw new InvalidOperationException("Completed or already-cancelled jobs cannot be cancelled.");

            if (!JobStatusRules.CanTransition(job.Status, JobStatus.Cancelled))
                throw new InvalidOperationException("Job status cannot transition to cancelled.");

            var refunds = RefundsFor(job.Status, reaction);
            var cancelledJob = job.WithStatus(JobStatus.Cancelled);

            return new JobCancellationResult(cancelledJob, refunds);
        }

        private static IReadOnlyList<JobCancellationRefund> RefundsFor(JobStatus status, ReactionDef reaction)
        {
            var includeConsumedInputs = status != JobStatus.Active;
            var refunds = new List<JobCancellationRefund>();

            for (var i = 0; i < reaction.InputMaterials.Count; i++)
            {
                var input = reaction.InputMaterials[i];
                if (input == null)
                    continue;

                if (includeConsumedInputs || !input.Consumed)
                    AddRefund(refunds, input.Tag, input.Quantity);
            }

            return refunds.AsReadOnly();
        }

        private static void AddRefund(List<JobCancellationRefund> refunds, string tag, int quantity)
        {
            for (var i = 0; i < refunds.Count; i++)
            {
                if (string.Equals(refunds[i].Tag, tag, StringComparison.Ordinal))
                {
                    refunds[i] = refunds[i].WithAdditionalQuantity(quantity);
                    return;
                }
            }

            refunds.Add(new JobCancellationRefund(tag, quantity));
        }
    }
}