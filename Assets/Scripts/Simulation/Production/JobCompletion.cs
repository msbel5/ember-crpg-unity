using System;
using System.Collections.Generic;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Production;

// Design note:
// JobCompletion finalizes completed job state into a deterministic result payload.
// It does not mutate actors, consume inventory, create items, insert stock, or write event logs.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Product descriptor produced by a completed job.
    /// </summary>
    public sealed class JobCompletionProduct
    {
        /// <summary>
        /// Item definition id for the produced item.
        /// </summary>
        public readonly string ItemDefId;

        /// <summary>
        /// Material id for the produced item, or "inherit" when material must be resolved later.
        /// </summary>
        public readonly string MaterialId;

        /// <summary>
        /// Number of produced items.
        /// </summary>
        public readonly int Quantity;

        /// <summary>
        /// Quality assigned to this produced item descriptor.
        /// </summary>
        public readonly QualityLevel Quality;

        /// <summary>
        /// True when material should be resolved from consumed input materials later.
        /// </summary>
        public bool InheritsMaterial
        {
            get { return string.Equals(MaterialId, ProductOutput.InheritMaterialId, StringComparison.Ordinal); }
        }

        /// <summary>
        /// Creates a completed product descriptor.
        /// </summary>
        public JobCompletionProduct(string itemDefId, string materialId, int quantity, QualityLevel quality)
        {
            if (string.IsNullOrWhiteSpace(itemDefId))
                throw new ArgumentException("Completed product item definition id cannot be empty.", nameof(itemDefId));
            if (string.IsNullOrWhiteSpace(materialId))
                throw new ArgumentException("Completed product material id cannot be empty.", nameof(materialId));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Completed product quantity must be positive.");

            ItemDefId = itemDefId.Trim();
            MaterialId = materialId.Trim();
            Quantity = quantity;
            Quality = quality;
        }
    }

    /// <summary>
    /// Deterministic result of completing a job.
    /// </summary>
    public sealed class JobCompletionResult
    {
        /// <summary>
        /// Completed job record.
        /// </summary>
        public readonly JobRecord CompletedJob;

        /// <summary>
        /// Reaction that produced this completion result.
        /// </summary>
        public readonly ReactionId ReactionId;

        /// <summary>
        /// XP that should be awarded to the assigned actor by a later mutation step.
        /// </summary>
        public readonly int XpGained;

        /// <summary>
        /// Quality level selected for produced outputs.
        /// </summary>
        public readonly QualityLevel Quality;

        /// <summary>
        /// Product descriptors that a later inventory/store step may turn into ItemRecords.
        /// </summary>
        public readonly IReadOnlyList<JobCompletionProduct> Products;

        /// <summary>
        /// Creates a deterministic job completion result.
        /// </summary>
        public JobCompletionResult(
            JobRecord completedJob,
            ReactionId reactionId,
            int xpGained,
            QualityLevel quality,
            IReadOnlyList<JobCompletionProduct> products)
        {
            if (completedJob == null)
                throw new ArgumentNullException(nameof(completedJob));
            if (reactionId.IsEmpty)
                throw new ArgumentException("Reaction id cannot be empty.", nameof(reactionId));
            if (xpGained < 0)
                throw new ArgumentOutOfRangeException(nameof(xpGained), "XP gained cannot be negative.");

            CompletedJob = completedJob;
            ReactionId = reactionId;
            XpGained = xpGained;
            Quality = quality;
            Products = products ?? Array.Empty<JobCompletionProduct>();
        }
    }

    /// <summary>
    /// Pure deterministic job completion helper.
    /// </summary>
    public static class JobCompletion
    {
        /// <summary>
        /// Completes an active, complete job and returns a deterministic result payload.
        /// </summary>
        public static JobCompletionResult Complete(
            JobRecord job,
            ReactionDef reaction,
            int effectiveSkill,
            int mentalBonusPercent,
            double rngValue)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (reaction == null)
                throw new ArgumentNullException(nameof(reaction));

            if (job.Status != JobStatus.Active)
                throw new InvalidOperationException("Only active jobs can be completed.");
            if (!job.IsComplete)
                throw new InvalidOperationException("Cannot complete a job before required work ticks are reached.");

            var quality = ResolveQuality(reaction.QualityFormula, effectiveSkill, rngValue);
            var xpGained = XpFor(reaction.BaseDurationTicks, mentalBonusPercent);
            var completedJob = job.WithStatus(JobStatus.Completed);
            var products = ProductsFor(reaction, quality);

            return new JobCompletionResult(completedJob, reaction.Id, xpGained, quality, products);
        }

        /// <summary>
        /// Computes XP from base duration and deterministic mental bonus percent.
        /// </summary>
        public static int XpFor(int baseDurationTicks, int mentalBonusPercent)
        {
            if (baseDurationTicks <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseDurationTicks), "Base duration ticks must be positive.");

            var multiplierPercent = 100 + mentalBonusPercent;
            if (multiplierPercent <= 0)
                return 0;

            var value = ((long)baseDurationTicks * multiplierPercent) / 100L;
            return value > int.MaxValue ? int.MaxValue : (int)value;
        }

        private static QualityLevel ResolveQuality(
            ReactionQualityFormula formula,
            int effectiveSkill,
            double rngValue)
        {
            if (formula.IsEmpty || formula.IsWeightedRandom)
                return CraftQuality.FromEffectiveSkill(effectiveSkill, rngValue);

            if (formula.IsFixed)
                return QualityLevel.Ordinary;

            throw new NotSupportedException("Custom reaction quality formulas require a formula resolver.");
        }

        private static IReadOnlyList<JobCompletionProduct> ProductsFor(ReactionDef reaction, QualityLevel quality)
        {
            if (reaction.OutputProducts.Count == 0)
                return Array.Empty<JobCompletionProduct>();

            var products = new List<JobCompletionProduct>(reaction.OutputProducts.Count);
            for (var i = 0; i < reaction.OutputProducts.Count; i++)
            {
                var output = reaction.OutputProducts[i];
                products.Add(new JobCompletionProduct(
                    output.ItemDefId,
                    output.MaterialId,
                    output.Quantity,
                    quality));
            }

            return products.AsReadOnly();
        }
    }
}