using System;
using EmberCrpg.Domain.Components;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin labor assignment as a deterministic pure selector.
// They do not test pathfinding, schedule reservation, actor mutation, job mutation, reactions, inventory, or AI.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic labor assignment selection.
    /// </summary>
    public sealed class LaborAssignmentTests
    {
        [Test]
        public void SelectBest_EmptyCandidates_ReturnsNoSelection()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));

            var result = LaborAssignment.SelectBest(
                job,
                new PositionComponent(0, 0),
                Array.Empty<LaborCandidate>());

            Assert.That(result.HasSelection, Is.False);
            Assert.That(result.SelectedActorId.IsEmpty, Is.True);
        }

        [Test]
        public void SelectBest_NullCandidates_ReturnsNoSelection()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));

            var result = LaborAssignment.SelectBest(
                job,
                new PositionComponent(0, 0),
                null);

            Assert.That(result.HasSelection, Is.False);
        }

        [Test]
        public void SelectBest_FiltersMissingRequiredSkill()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 0, 0, true, new SkillId("medical.surgery"), 10),
                NewCandidate(2UL, 5, 0, true, new SkillId("craft.smithing"), 2)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.HasSelection, Is.True);
            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
        }

        [Test]
        public void SelectBest_FiltersUnavailableCandidates()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 0, 0, false, new SkillId("craft.smithing"), 10),
                NewCandidate(2UL, 5, 0, true, new SkillId("craft.smithing"), 2)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
        }

        [Test]
        public void SelectBest_ClosestEligibleCandidateWins()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 3, 0, true, new SkillId("craft.smithing"), 10),
                NewCandidate(2UL, 1, 0, true, new SkillId("craft.smithing"), 2)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
            Assert.That(result.Distance, Is.EqualTo(1));
        }

        [Test]
        public void SelectBest_EqualDistance_HigherEffectiveSkillWins()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 1, 0, true, new SkillId("craft.smithing"), 3),
                NewCandidate(2UL, 0, 1, true, new SkillId("craft.smithing"), 8)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
            Assert.That(result.EffectiveSkillLevel, Is.EqualTo(8));
        }

        [Test]
        public void SelectBest_EqualDistanceAndSkill_LowerActorIdWins()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(9UL, 1, 0, true, new SkillId("craft.smithing"), 5),
                NewCandidate(3UL, 0, 1, true, new SkillId("craft.smithing"), 5)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(3UL)));
        }

        [Test]
        public void SelectBest_PrdExample_SelectsClosestHighestSkillAmongEquidistant()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 3, 0, true, new SkillId("craft.smithing"), 5),
                NewCandidate(2UL, 1, 0, true, new SkillId("craft.smithing"), 8),
                NewCandidate(3UL, 0, 1, true, new SkillId("craft.smithing"), 3)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
            Assert.That(result.Distance, Is.EqualTo(1));
            Assert.That(result.EffectiveSkillLevel, Is.EqualTo(8));
        }

        [Test]
        public void SelectBest_EmptyRequiredSkill_AllAvailableCandidatesAreEligible()
        {
            var job = NewQueuedJob(default(SkillId));
            var candidates = new[]
            {
                NewCandidate(1UL, 5, 0, true, new SkillId("craft.smithing"), 10),
                new LaborCandidate(new ActorId(2UL), new PositionComponent(1, 0), SkillSet.Empty, true)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(2UL)));
            Assert.That(result.EffectiveSkillLevel, Is.EqualTo(0));
        }

        [Test]
        public void SelectBest_NoEligibleCandidate_ReturnsNoSelection()
        {
            var job = NewQueuedJob(new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 0, 0, true, new SkillId("medical.surgery"), 10),
                NewCandidate(2UL, 1, 0, false, new SkillId("craft.smithing"), 10)
            };

            var result = LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates);

            Assert.That(result.HasSelection, Is.False);
        }

        [Test]
        public void SelectBest_NonQueuedJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Active, new SkillId("craft.smithing"));
            var candidates = new[]
            {
                NewCandidate(1UL, 0, 0, true, new SkillId("craft.smithing"), 5)
            };

            Assert.Throws<InvalidOperationException>(() =>
                LaborAssignment.SelectBest(job, new PositionComponent(0, 0), candidates));
        }

        [Test]
        public void SelectBest_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                LaborAssignment.SelectBest(null, new PositionComponent(0, 0), Array.Empty<LaborCandidate>()));
        }

        private static JobRecord NewQueuedJob(SkillId skillId)
        {
            return NewJob(JobStatus.Queued, skillId);
        }

        private static JobRecord NewJob(JobStatus status, SkillId skillId)
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                status,
                default(ActorId),
                skillId,
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                0,
                new[] { "production", "metal" });
        }

        private static LaborCandidate NewCandidate(
            ulong actorId,
            int x,
            int y,
            bool isAvailable,
            SkillId skillId,
            int effectiveLevel)
        {
            var level = effectiveLevel;
            var skills = SkillSet.Empty;

            if (!skillId.IsEmpty)
            {
                skills = new SkillSet(new[]
                {
                    new SkillRecord(skillId, level * 500, level, 0, 0)
                });
            }

            return new LaborCandidate(
                new ActorId(actorId),
                new PositionComponent(x, y),
                skills,
                isAvailable);
        }
    }
}