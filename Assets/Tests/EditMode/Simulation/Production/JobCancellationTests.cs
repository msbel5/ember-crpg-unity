using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin job cancellation as a deterministic result builder.
// They do not test inventory mutation, stockpile restoration, item creation, XP, quality, or event logging.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic job cancellation result generation.
    /// </summary>
    public sealed class JobCancellationTests
    {
        [Test]
        public void Cancel_QueuedJob_ReturnsCancelledJob()
        {
            var job = NewJob(JobStatus.Queued);
            var reaction = NewSmeltReaction();

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.CancelledJob.Status, Is.EqualTo(JobStatus.Cancelled));
            Assert.That(job.Status, Is.EqualTo(JobStatus.Queued));
        }

        [Test]
        public void Cancel_AssignedJob_RefundsAllReactionInputs()
        {
            var job = NewJob(JobStatus.Assigned);
            var reaction = NewSmeltReaction();

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.Refunds.Count, Is.EqualTo(3));
            Assert.That(result.Refunds[0].Tag, Is.EqualTo("ore"));
            Assert.That(result.Refunds[0].Quantity, Is.EqualTo(2));
            Assert.That(result.Refunds[1].Tag, Is.EqualTo("fuel"));
            Assert.That(result.Refunds[1].Quantity, Is.EqualTo(1));
            Assert.That(result.Refunds[2].Tag, Is.EqualTo("anvil"));
            Assert.That(result.Refunds[2].Quantity, Is.EqualTo(1));
        }

        [Test]
        public void Cancel_ActiveJob_RefundsOnlyNonConsumedInputs()
        {
            var job = NewJob(JobStatus.Active);
            var reaction = NewSmeltReaction();

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.Refunds.Count, Is.EqualTo(1));
            Assert.That(result.Refunds[0].Tag, Is.EqualTo("anvil"));
            Assert.That(result.Refunds[0].Quantity, Is.EqualTo(1));
        }

        [Test]
        public void Cancel_ActiveJobWithOnlyConsumedInputs_ReturnsNoRefunds()
        {
            var job = NewJob(JobStatus.Active);
            var reaction = new ReactionDef(
                new ReactionId("reaction.cook_meal"),
                "Cook Meal",
                "campfire",
                new ActivitySiteRole("work"),
                new SkillId("craft.cooking"),
                new[]
                {
                    new MaterialRequirement("food", 2, true),
                    new MaterialRequirement("fuel", 1, true)
                },
                new[]
                {
                    new ProductOutput("meal", ProductOutput.InheritMaterialId, 1)
                },
                30,
                ReactionQualityFormula.Fixed,
                new[] { "cooking" });

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.Refunds.Count, Is.EqualTo(0));
        }

        [Test]
        public void Cancel_AggregatesDuplicateRefundTags()
        {
            var job = NewJob(JobStatus.Assigned);
            var reaction = new ReactionDef(
                new ReactionId("reaction.bundle_cloth"),
                "Bundle Cloth",
                "work_table",
                new ActivitySiteRole("work"),
                new SkillId("craft.tailoring"),
                new[]
                {
                    new MaterialRequirement("cloth", 2, true),
                    new MaterialRequirement("cloth", 3, false)
                },
                Array.Empty<ProductOutput>(),
                20,
                ReactionQualityFormula.Fixed,
                new[] { "textile" });

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.Refunds.Count, Is.EqualTo(1));
            Assert.That(result.Refunds[0].Tag, Is.EqualTo("cloth"));
            Assert.That(result.Refunds[0].Quantity, Is.EqualTo(5));
        }

        [Test]
        public void Result_HasRefund_ReturnsTrueForRefundedTag()
        {
            var job = NewJob(JobStatus.Active);
            var reaction = NewSmeltReaction();

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.HasRefund("anvil"), Is.True);
        }

        [Test]
        public void Result_HasRefund_ReturnsFalseForConsumedTag()
        {
            var job = NewJob(JobStatus.Active);
            var reaction = NewSmeltReaction();

            var result = JobCancellation.Cancel(job, reaction);

            Assert.That(result.HasRefund("ore"), Is.False);
        }

        [Test]
        public void Cancel_CompletedJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Completed);
            var reaction = NewSmeltReaction();

            Assert.Throws<InvalidOperationException>(() => JobCancellation.Cancel(job, reaction));
        }

        [Test]
        public void Cancel_CancelledJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Cancelled);
            var reaction = NewSmeltReaction();

            Assert.Throws<InvalidOperationException>(() => JobCancellation.Cancel(job, reaction));
        }

        [Test]
        public void Cancel_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JobCancellation.Cancel(null, NewSmeltReaction()));
        }

        [Test]
        public void Cancel_NullReaction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JobCancellation.Cancel(NewJob(JobStatus.Active), null));
        }

        private static JobRecord NewJob(JobStatus status)
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                status,
                new ActorId(7UL),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel", "anvil" },
                new[] { "ingot" },
                100,
                status == JobStatus.Active || status == JobStatus.Completed ? 50 : 0,
                new[] { "production", "metal" });
        }

        private static ReactionDef NewSmeltReaction()
        {
            return new ReactionDef(
                new ReactionId("reaction.smelt_iron_ingot"),
                "Smelt Iron Ingot",
                "iron_forge",
                new ActivitySiteRole("work"),
                new SkillId("craft.smithing"),
                new[]
                {
                    new MaterialRequirement("ore", 2, true),
                    new MaterialRequirement("fuel", 1, true),
                    new MaterialRequirement("anvil", 1, false)
                },
                new[]
                {
                    new ProductOutput("iron_ingot", ProductOutput.InheritMaterialId, 1)
                },
                120,
                ReactionQualityFormula.WeightedRandom,
                new[] { "metal", "production" });
        }
    }
}