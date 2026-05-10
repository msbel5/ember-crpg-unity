using EmberCrpg.Domain.Components;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin LaborCandidate as a minimal actor projection for labor assignment.
// They do not test ActorRecord, pathfinding, job mutation, schedules, needs, AI, or inventory.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies labor assignment candidate projection behavior.
    /// </summary>
    public sealed class LaborCandidateTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var skills = NewSmithingSkills();

            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(3, 4, 0),
                skills,
                true);

            Assert.That(candidate.ActorId, Is.EqualTo(new ActorId(7UL)));
            Assert.That(candidate.Position, Is.EqualTo(new PositionComponent(3, 4, 0)));
            Assert.That(candidate.Skills, Is.EqualTo(skills));
            Assert.That(candidate.IsAvailable, Is.True);
        }

        [Test]
        public void Constructor_EmptyActorId_ThrowsArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => new LaborCandidate(
                default(ActorId),
                new PositionComponent(0, 0),
                SkillSet.Empty,
                true));
        }

        [Test]
        public void Constructor_NullSkills_NormalizesToEmptySkillSet()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                null,
                true);

            Assert.That(candidate.Skills.Count, Is.EqualTo(0));
        }

        [Test]
        public void IsEligibleFor_NoRequiredSkillAndAvailable_ReturnsTrue()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                SkillSet.Empty,
                true);

            Assert.That(candidate.IsEligibleFor(default(SkillId)), Is.True);
        }

        [Test]
        public void IsEligibleFor_NoRequiredSkillButUnavailable_ReturnsFalse()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                SkillSet.Empty,
                false);

            Assert.That(candidate.IsEligibleFor(default(SkillId)), Is.False);
        }

        [Test]
        public void IsEligibleFor_RequiredSkillPresent_ReturnsTrue()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                NewSmithingSkills(),
                true);

            Assert.That(candidate.IsEligibleFor(new SkillId("craft.smithing")), Is.True);
        }

        [Test]
        public void IsEligibleFor_RequiredSkillMissing_ReturnsFalse()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                NewSmithingSkills(),
                true);

            Assert.That(candidate.IsEligibleFor(new SkillId("medical.surgery")), Is.False);
        }

        [Test]
        public void EffectiveSkillFor_ReturnsSkillSetEffectiveLevel()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                NewSmithingSkills(),
                true);

            Assert.That(candidate.EffectiveSkillFor(new SkillId("craft.smithing")), Is.EqualTo(4));
        }

        [Test]
        public void EffectiveSkillFor_MissingSkill_ReturnsZero()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(0, 0),
                NewSmithingSkills(),
                true);

            Assert.That(candidate.EffectiveSkillFor(new SkillId("science.xenobiology")), Is.EqualTo(0));
        }

        [Test]
        public void DistanceTo_ReturnsManhattanDistance()
        {
            var candidate = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(1, 2, 3),
                SkillSet.Empty,
                true);

            Assert.That(candidate.DistanceTo(new PositionComponent(4, 6, 8)), Is.EqualTo(12));
        }

        [Test]
        public void WithAvailability_ReturnsChangedCopy()
        {
            var original = new LaborCandidate(
                new ActorId(7UL),
                new PositionComponent(1, 2),
                NewSmithingSkills(),
                true);

            var changed = original.WithAvailability(false);

            Assert.That(changed.IsAvailable, Is.False);
            Assert.That(original.IsAvailable, Is.True);
            Assert.That(changed.ActorId, Is.EqualTo(original.ActorId));
            Assert.That(changed.Position, Is.EqualTo(original.Position));
        }

        private static SkillSet NewSmithingSkills()
        {
            return new SkillSet(new[]
            {
                new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 1, 0)
            });
        }
    }
}