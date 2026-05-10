using System;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Simulation.Skills;
using NUnit.Framework;

// Design note:
// These tests pin XP application as a pure skill-state behavior.
// They do not test job completion, actor mutation, SkillSet mutation, reactions, inventory, or event logging.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Skills
{
    /// <summary>
    /// Verifies deterministic skill XP application behavior.
    /// </summary>
    public sealed class SkillExperienceTests
    {
        [Test]
        public void ApplyXp_AddsXpToRecord()
        {
            var record = NewSkillRecord(100, 0, 0, 0);

            var result = SkillExperience.ApplyXp(record, 50);

            Assert.That(result.Record.Xp, Is.EqualTo(150));
        }

        [Test]
        public void ApplyXp_RecomputesLevelFromNewXp()
        {
            var record = NewSkillRecord(1000, 1, 0, 0);

            var result = SkillExperience.ApplyXp(record, 100);

            Assert.That(result.Record.Xp, Is.EqualTo(1100));
            Assert.That(result.Record.Level, Is.EqualTo(2));
        }

        [Test]
        public void ApplyXp_ReturnsLevelUpMetadata()
        {
            var record = NewSkillRecord(1000, 1, 0, 0);

            var result = SkillExperience.ApplyXp(record, 100);

            Assert.That(result.PreviousLevel, Is.EqualTo(1));
            Assert.That(result.NewLevel, Is.EqualTo(2));
            Assert.That(result.LeveledUp, Is.True);
        }

        [Test]
        public void ApplyXp_NoLevelChange_ReturnsNoLevelUp()
        {
            var record = NewSkillRecord(600, 1, 0, 0);

            var result = SkillExperience.ApplyXp(record, 100);

            Assert.That(result.PreviousLevel, Is.EqualTo(1));
            Assert.That(result.NewLevel, Is.EqualTo(1));
            Assert.That(result.LeveledUp, Is.False);
        }

        [Test]
        public void ApplyXp_ZeroXpKeepsXpAndLevel()
        {
            var record = NewSkillRecord(600, 1, 0, 0);

            var result = SkillExperience.ApplyXp(record, 0);

            Assert.That(result.Record.Xp, Is.EqualTo(600));
            Assert.That(result.Record.Level, Is.EqualTo(1));
            Assert.That(result.LeveledUp, Is.False);
        }

        [Test]
        public void ApplyXp_NegativeXp_ThrowsArgumentOutOfRangeException()
        {
            var record = NewSkillRecord(600, 1, 0, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => SkillExperience.ApplyXp(record, -1));
        }

        [Test]
        public void ApplyXp_Overflow_ClampsXpToIntMaxValue()
        {
            var record = NewSkillRecord(int.MaxValue - 10, 100, 0, 0);

            var result = SkillExperience.ApplyXp(record, 100);

            Assert.That(result.Record.Xp, Is.EqualTo(int.MaxValue));
            Assert.That(result.Record.Level, Is.EqualTo(SkillProgression.LevelFromXp(int.MaxValue)));
        }

        [Test]
        public void ApplyXp_PreservesSkillId()
        {
            var record = NewSkillRecord(1000, 1, 0, 0);

            var result = SkillExperience.ApplyXp(record, 100);

            Assert.That(result.Record.SkillId, Is.EqualTo(new SkillId("craft.smithing")));
        }

        [Test]
        public void ApplyXp_PreservesRustStateByDefault()
        {
            var record = NewSkillRecord(1000, 1, 2, 50);

            var result = SkillExperience.ApplyXp(record, 100);

            Assert.That(result.Record.RustyLevel, Is.EqualTo(2));
            Assert.That(result.Record.UnusedCounter, Is.EqualTo(50));
        }

        [Test]
        public void ApplyXpAndUse_ReducesRustAndResetsUnusedCounter()
        {
            var record = NewSkillRecord(1000, 5, 2, 50);

            var result = SkillExperience.ApplyXpAndUse(record, 100);

            Assert.That(result.Record.RustyLevel, Is.EqualTo(1));
            Assert.That(result.Record.UnusedCounter, Is.EqualTo(0));
        }

        [Test]
        public void ApplyXpAndUse_DoesNotReduceRustBelowZero()
        {
            var record = NewSkillRecord(1000, 5, 0, 50);

            var result = SkillExperience.ApplyXpAndUse(record, 100);

            Assert.That(result.Record.RustyLevel, Is.EqualTo(0));
            Assert.That(result.Record.UnusedCounter, Is.EqualTo(0));
        }

        private static SkillRecord NewSkillRecord(int xp, int level, int rustyLevel, int unusedCounter)
        {
            return new SkillRecord(
                new SkillId("craft.smithing"),
                xp,
                level,
                rustyLevel,
                unusedCounter);
        }
    }
}