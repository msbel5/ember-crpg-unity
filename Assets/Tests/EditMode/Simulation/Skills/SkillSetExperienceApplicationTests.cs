using System;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Simulation.Skills;
using NUnit.Framework;

// Design note:
// These tests pin XP application to an actor-local SkillSet.
// They do not test JobCompletion, ActorRecord mutation, event logging, inventory, reactions, or all-skill rust ticking.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Skills
{
    /// <summary>
    /// Verifies deterministic SkillSet XP application behavior.
    /// </summary>
    public sealed class SkillSetExperienceApplicationTests
    {
        [Test]
        public void Apply_ExistingSkill_UpdatesSkillRecord()
        {
            var set = NewSkillSet(1000, 1, 0, 0);

            var result = SkillSetExperienceApplication.Apply(
                set,
                new SkillId("craft.smithing"),
                100);

            Assert.That(result.WasApplied, Is.True);
            Assert.That(result.CreatedNewRecord, Is.False);
            Assert.That(result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record), Is.True);
            Assert.That(record.Xp, Is.EqualTo(1100));
            Assert.That(record.Level, Is.EqualTo(2));
        }

        [Test]
        public void Apply_ExistingSkill_UsesSkillAndReducesRust()
        {
            var set = NewSkillSet(3500, 5, 2, 50);

            var result = SkillSetExperienceApplication.Apply(
                set,
                new SkillId("craft.smithing"),
                100);

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.RustyLevel, Is.EqualTo(1));
            Assert.That(record.UnusedCounter, Is.EqualTo(0));
        }

        [Test]
        public void Apply_MissingSkill_CreatesNewRecord()
        {
            var set = SkillSet.Empty;

            var result = SkillSetExperienceApplication.Apply(
                set,
                new SkillId("science.xenobiology"),
                500);

            Assert.That(result.WasApplied, Is.True);
            Assert.That(result.CreatedNewRecord, Is.True);
            Assert.That(result.SkillSet.TryGet(new SkillId("science.xenobiology"), out var record), Is.True);
            Assert.That(record.Xp, Is.EqualTo(500));
            Assert.That(record.Level, Is.EqualTo(1));
        }

        [Test]
        public void Apply_EmptySkillId_ReturnsUnchangedSet()
        {
            var set = NewSkillSet(1000, 1, 0, 0);

            var result = SkillSetExperienceApplication.Apply(
                set,
                default(SkillId),
                100);

            Assert.That(result.WasApplied, Is.False);
            Assert.That(result.CreatedNewRecord, Is.False);
            Assert.That(result.SkillSet, Is.EqualTo(set));
        }

        [Test]
        public void Apply_NullSkillSet_TreatsAsEmpty()
        {
            var result = SkillSetExperienceApplication.Apply(
                null,
                new SkillId("craft.smithing"),
                100);

            Assert.That(result.WasApplied, Is.True);
            Assert.That(result.CreatedNewRecord, Is.True);
            Assert.That(result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record), Is.True);
            Assert.That(record.Xp, Is.EqualTo(100));
        }

        [Test]
        public void Apply_ZeroXpStillMarksExistingSkillUsed()
        {
            var set = NewSkillSet(3500, 5, 2, 50);

            var result = SkillSetExperienceApplication.Apply(
                set,
                new SkillId("craft.smithing"),
                0);

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(result.WasApplied, Is.True);
            Assert.That(record.Xp, Is.EqualTo(3500));
            Assert.That(record.RustyLevel, Is.EqualTo(1));
            Assert.That(record.UnusedCounter, Is.EqualTo(0));
        }

        [Test]
        public void Apply_ZeroXpMissingSkill_DoesNotCreateRecord()
        {
            var result = SkillSetExperienceApplication.Apply(
                SkillSet.Empty,
                new SkillId("craft.smithing"),
                0);

            Assert.That(result.WasApplied, Is.False);
            Assert.That(result.CreatedNewRecord, Is.False);
            Assert.That(result.SkillSet.Count, Is.EqualTo(0));
        }

        [Test]
        public void Apply_NegativeXp_ThrowsArgumentOutOfRangeException()
        {
            var set = NewSkillSet(1000, 1, 0, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SkillSetExperienceApplication.Apply(
                    set,
                    new SkillId("craft.smithing"),
                    -1));
        }

        [Test]
        public void Apply_ReturnsLevelUpMetadata()
        {
            var set = NewSkillSet(1000, 1, 0, 0);

            var result = SkillSetExperienceApplication.Apply(
                set,
                new SkillId("craft.smithing"),
                100);

            Assert.That(result.PreviousLevel, Is.EqualTo(1));
            Assert.That(result.NewLevel, Is.EqualTo(2));
            Assert.That(result.LeveledUp, Is.True);
        }

        [Test]
        public void Apply_PreservesOtherSkills()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 1000, 1, 0, 0);
            var surgery = new SkillRecord(new SkillId("medical.surgery"), 3500, 5, 0, 0);
            var set = new SkillSet(new[] { smithing, surgery });

            var result = SkillSetExperienceApplication.Apply(
                set,
                new SkillId("craft.smithing"),
                100);

            Assert.That(result.SkillSet.TryGet(new SkillId("medical.surgery"), out var preserved), Is.True);
            Assert.That(preserved.Xp, Is.EqualTo(3500));
            Assert.That(preserved.Level, Is.EqualTo(5));
        }

        private static SkillSet NewSkillSet(int xp, int level, int rustyLevel, int unusedCounter)
        {
            return new SkillSet(new[]
            {
                new SkillRecord(
                    new SkillId("craft.smithing"),
                    xp,
                    level,
                    rustyLevel,
                    unusedCounter)
            });
        }
    }
}