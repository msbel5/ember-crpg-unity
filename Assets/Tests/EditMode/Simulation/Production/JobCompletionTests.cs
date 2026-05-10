using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin job completion as a deterministic result builder.
// They do not test actor mutation, inventory consumption, item creation, stockpile insertion, or event logging.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic job completion result generation.
    /// </summary>
    public sealed class JobCompletionTests
    {
        [Test]
        public void Complete_CompletedActiveJob_ReturnsCompletedJobStatus()
        {
            var job = NewCompleteActiveJob();
            var reaction = NewSmeltReaction();

            var result = JobCompletion.Complete(job, reaction, 5, 0, 0.50);

            Assert.That(result.CompletedJob.Status, Is.EqualTo(JobStatus.Completed));
            Assert.That(job.Status, Is.EqualTo(JobStatus.Active));
        }

        [Test]
        public void Complete_ComputesXpFromBaseDurationAndMentalBonusPercent()
        {
            var job = NewCompleteActiveJob();
            var reaction = NewSmeltReaction();

            var result = JobCompletion.Complete(job, reaction, 5, 10, 0.50);

            Assert.That(result.XpGained, Is.EqualTo(132));
        }

        [Test]
        public void Complete_NegativeMentalBonus_CannotProduceNegativeXp()
        {
            var job = NewCompleteActiveJob();
            var reaction = NewSmeltReaction();

            var result = JobCompletion.Complete(job, reaction, 5, -200, 0.50);

            Assert.That(result.XpGained, Is.EqualTo(0));
        }

        [Test]
        public void Complete_WeightedRandomFormula_UsesCraftQuality()
        {
            var job = NewCompleteActiveJob();
            var reaction = NewSmeltReaction();

            var result = JobCompletion.Complete(job, reaction, 15, 0, 0.50);

            Assert.That(result.Quality, Is.EqualTo(QualityLevel.Masterwork));
        }

        [Test]
        public void Complete_FixedFormula_UsesOrdinaryQuality()
        {
            var job = NewCompleteActiveJob();
            var reaction = new ReactionDef(
                new ReactionId("reaction.rest"),
                "Rest",
                "owned_bed",
                new ActivitySiteRole("rest"),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                60,
                ReactionQualityFormula.Fixed,
                new[] { "rest" });

            var result = JobCompletion.Complete(job, reaction, 0, 0, 0.99);

            Assert.That(result.Quality, Is.EqualTo(QualityLevel.Ordinary));
        }

        [Test]
        public void Complete_CustomQualityFormula_ThrowsNotSupportedException()
        {
            var job = NewCompleteActiveJob();
            var reaction = new ReactionDef(
                new ReactionId("reaction.blood_rite"),
                "Blood Rite",
                "blood_altar",
                new ActivitySiteRole("ritual"),
                new SkillId("occult.blood_rite"),
                new[] { new MaterialRequirement("blood", 1, true) },
                Array.Empty<ProductOutput>(),
                200,
                new ReactionQualityFormula("ritual_omen"),
                new[] { "ritual" });

            Assert.Throws<NotSupportedException>(() => JobCompletion.Complete(job, reaction, 5, 0, 0.50));
        }

        [Test]
        public void Complete_CopiesProductOutputsWithQuality()
        {
            var job = NewCompleteActiveJob();
            var reaction = NewSmeltReaction();

            var result = JobCompletion.Complete(job, reaction, 15, 0, 0.50);

            Assert.That(result.Products.Count, Is.EqualTo(1));
            Assert.That(result.Products[0].ItemDefId, Is.EqualTo("iron_ingot"));
            Assert.That(result.Products[0].MaterialId, Is.EqualTo(ProductOutput.InheritMaterialId));
            Assert.That(result.Products[0].Quantity, Is.EqualTo(1));
            Assert.That(result.Products[0].Quality, Is.EqualTo(QualityLevel.Masterwork));
            Assert.That(result.Products[0].InheritsMaterial, Is.True);
        }

        [Test]
        public void Complete_AllowsReactionWithNoProducts()
        {
            var job = NewCompleteActiveJob();
            var reaction = new ReactionDef(
                new ReactionId("reaction.decode_signal"),
                "Decode Signal",
                "nav_console",
                new ActivitySiteRole("command"),
                new SkillId("science.signals"),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                80,
                ReactionQualityFormula.Fixed,
                new[] { "research" });

            var result = JobCompletion.Complete(job, reaction, 3, 0, 0.20);

            Assert.That(result.Products.Count, Is.EqualTo(0));
        }

        [Test]
        public void Complete_IncompleteJob_ThrowsInvalidOperationException()
        {
            var job = NewIncompleteActiveJob();
            var reaction = NewSmeltReaction();

            Assert.Throws<InvalidOperationException>(() => JobCompletion.Complete(job, reaction, 5, 0, 0.50));
        }

        [Test]
        public void Complete_NonActiveJob_ThrowsInvalidOperationException()
        {
            var job = NewCompleteQueuedJob();
            var reaction = NewSmeltReaction();

            Assert.Throws<InvalidOperationException>(() => JobCompletion.Complete(job, reaction, 5, 0, 0.50));
        }

        [Test]
        public void Complete_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JobCompletion.Complete(null, NewSmeltReaction(), 5, 0, 0.50));
        }

        [Test]
        public void Complete_NullReaction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JobCompletion.Complete(NewCompleteActiveJob(), null, 5, 0, 0.50));
        }

        private static JobRecord NewCompleteActiveJob()
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Active,
                new ActorId(7UL),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                100,
                new[] { "production", "metal" });
        }

        private static JobRecord NewIncompleteActiveJob()
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Active,
                new ActorId(7UL),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                99,
                new[] { "production", "metal" });
        }

        private static JobRecord NewCompleteQueuedJob()
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                default(ActorId),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                100,
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
                    new MaterialRequirement("fuel", 1, true)
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