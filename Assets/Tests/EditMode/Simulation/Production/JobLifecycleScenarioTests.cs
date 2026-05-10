using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Components;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin a deterministic single-job lifecycle scenario.
// They do not test inventory mutation, item creation, inherited material resolution, event logging, AI, or global world stores.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic orchestration of one job through assignment, activation, ticking, completion, and XP application.
    /// </summary>
    public sealed class JobLifecycleScenarioTests
    {
        [Test]
        public void RunToCompletion_SelectsBestCandidateAndCompletesJob()
        {
            var job = NewQueuedJob();
            var reaction = NewSmeltReaction();
            var candidates = NewCandidates();

            var result = JobLifecycleScenario.RunToCompletion(
                job,
                new PositionComponent(0, 0),
                candidates,
                reaction,
                new GameTick(0),
                100,
                10,
                0.50);

            Assert.That(result.WasAssigned, Is.True);
            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
            Assert.That(result.FinalJob.Status, Is.EqualTo(JobStatus.Completed));
            Assert.That(result.Completion.XpGained, Is.EqualTo(132));
            Assert.That(result.TicksAdvanced, Is.EqualTo(100));
        }

        [Test]
        public void RunToCompletion_UpdatesSelectedActorSkillSet()
        {
            var job = NewQueuedJob();
            var reaction = NewSmeltReaction();
            var candidates = NewCandidates();

            var result = JobLifecycleScenario.RunToCompletion(
                job,
                new PositionComponent(0, 0),
                candidates,
                reaction,
                new GameTick(0),
                100,
                10,
                0.50);

            Assert.That(result.UpdatedSelectedSkills.TryGet(new SkillId("craft.smithing"), out var record), Is.True);
            Assert.That(record.Xp, Is.EqualTo(3632));
            Assert.That(record.Level, Is.EqualTo(SkillProgression.LevelFromXp(3632)));
        }

        [Test]
        public void RunToCompletion_UsesSelectedCandidateEffectiveSkillForQuality()
        {
            var job = NewQueuedJob();
            var reaction = NewSmeltReaction();
            var candidates = NewCandidates();

            var result = JobLifecycleScenario.RunToCompletion(
                job,
                new PositionComponent(0, 0),
                candidates,
                reaction,
                new GameTick(0),
                100,
                0,
                0.95);

            Assert.That(result.SelectedEffectiveSkillLevel, Is.EqualTo(8));
            Assert.That(result.Completion.Quality, Is.EqualTo(QualityLevel.Exceptional));
        }

        [Test]
        public void RunToCompletion_NoEligibleCandidate_ReturnsUnassignedResult()
        {
            var job = NewQueuedJob();
            var reaction = NewSmeltReaction();
            var candidates = new[]
            {
                NewCandidate(1UL, 0, 0, true, new SkillId("medical.surgery"), 5, 3500)
            };

            var result = JobLifecycleScenario.RunToCompletion(
                job,
                new PositionComponent(0, 0),
                candidates,
                reaction,
                new GameTick(0),
                100,
                0,
                0.50);

            Assert.That(result.WasAssigned, Is.False);
            Assert.That(result.SelectedActorId.IsEmpty, Is.True);
            Assert.That(result.FinalJob.Status, Is.EqualTo(JobStatus.Queued));
            Assert.That(result.Completion, Is.Null);
            Assert.That(result.TicksAdvanced, Is.EqualTo(0));
        }

        [Test]
        public void RunToCompletion_HalfSpeed_TakesTwoHundredSimulationTicksForHundredWorkTicks()
        {
            var job = NewQueuedJob();
            var reaction = NewSmeltReaction();
            var candidates = NewCandidates();

            var result = JobLifecycleScenario.RunToCompletion(
                job,
                new PositionComponent(0, 0),
                candidates,
                reaction,
                new GameTick(0),
                50,
                0,
                0.50);

            Assert.That(result.FinalJob.Status, Is.EqualTo(JobStatus.Completed));
            Assert.That(result.TicksAdvanced, Is.EqualTo(200));
        }

        [Test]
        public void RunToCompletion_DoubleSpeed_TakesFiftySimulationTicksForHundredWorkTicks()
        {
            var job = NewQueuedJob();
            var reaction = NewSmeltReaction();
            var candidates = NewCandidates();

            var result = JobLifecycleScenario.RunToCompletion(
                job,
                new PositionComponent(0, 0),
                candidates,
                reaction,
                new GameTick(0),
                200,
                0,
                0.50);

            Assert.That(result.FinalJob.Status, Is.EqualTo(JobStatus.Completed));
            Assert.That(result.TicksAdvanced, Is.EqualTo(50));
        }

        [Test]
        public void RunToCompletion_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                JobLifecycleScenario.RunToCompletion(
                    null,
                    new PositionComponent(0, 0),
                    Array.Empty<LaborCandidate>(),
                    NewSmeltReaction(),
                    new GameTick(0),
                    100,
                    0,
                    0.50));
        }

        [Test]
        public void RunToCompletion_NullReaction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                JobLifecycleScenario.RunToCompletion(
                    NewQueuedJob(),
                    new PositionComponent(0, 0),
                    Array.Empty<LaborCandidate>(),
                    null,
                    new GameTick(0),
                    100,
                    0,
                    0.50));
        }

        [Test]
        public void RunToCompletion_NonQueuedJob_ThrowsInvalidOperationException()
        {
            var job = NewQueuedJob().AssignTo(new ActorId(2UL));

            Assert.Throws<InvalidOperationException>(() =>
                JobLifecycleScenario.RunToCompletion(
                    job,
                    new PositionComponent(0, 0),
                    NewCandidates(),
                    NewSmeltReaction(),
                    new GameTick(0),
                    100,
                    0,
                    0.50));
        }

        [Test]
        public void RunToCompletion_ZeroSpeed_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                JobLifecycleScenario.RunToCompletion(
                    NewQueuedJob(),
                    new PositionComponent(0, 0),
                    NewCandidates(),
                    NewSmeltReaction(),
                    new GameTick(0),
                    0,
                    0,
                    0.50));
        }

        private static JobRecord NewQueuedJob()
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
                0,
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

        private static LaborCandidate[] NewCandidates()
        {
            return new[]
            {
                NewCandidate(1UL, 3, 0, true, new SkillId("craft.smithing"), 5, 3500),
                NewCandidate(2UL, 1, 0, true, new SkillId("craft.smithing"), 8, 3500),
                NewCandidate(3UL, 0, 1, true, new SkillId("craft.smithing"), 3, 1800)
            };
        }

        private static LaborCandidate NewCandidate(
            ulong actorId,
            int x,
            int y,
            bool isAvailable,
            SkillId skillId,
            int level,
            int xp)
        {
            var skills = new SkillSet(new[]
            {
                new SkillRecord(skillId, xp, level, 0, 0)
            });

            return new LaborCandidate(
                new ActorId(actorId),
                new PositionComponent(x, y),
                skills,
                isAvailable);
        }
    }
}