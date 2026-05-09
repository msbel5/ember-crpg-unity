using System;
using EmberCrpg.Domain.Skills;
using NUnit.Framework;

// Design note:
// These tests pin SkillRecord as one actor's runtime state for one data-driven skill.
// They do not test XP thresholds, job rewards, rust ticking, quality, or actor containers.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Skills
{
    /// <summary>
    /// Verifies runtime state for a single actor skill.
    /// </summary>
    public sealed class SkillRecordTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 500, 1, 0, 25);

            Assert.That(record.SkillId, Is.EqualTo(new SkillId("craft.smithing")));
            Assert.That(record.Xp, Is.EqualTo(500));
            Assert.That(record.Level, Is.EqualTo(1));
            Assert.That(record.RustyLevel, Is.EqualTo(0));
            Assert.That(record.UnusedCounter, Is.EqualTo(25));
        }

        [Test]
        public void Constructor_EmptySkillId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SkillRecord(default(SkillId), 0, 0, 0, 0));
        }

        [Test]
        public void Constructor_NegativeXp_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SkillRecord(new SkillId("craft.smithing"), -1, 0, 0, 0));
        }

        [Test]
        public void Constructor_NegativeLevel_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SkillRecord(new SkillId("craft.smithing"), 0, -1, 0, 0));
        }

        [Test]
        public void EffectiveLevel_SubtractsRust()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 2, 0);

            Assert.That(record.EffectiveLevel, Is.EqualTo(3));
        }

        [Test]
        public void EffectiveLevel_DoesNotGoBelowZero()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 500, 1, 5, 0);

            Assert.That(record.EffectiveLevel, Is.EqualTo(0));
        }

        [Test]
        public void WithXp_ReturnsCopyWithChangedXp()
        {
            var original = new SkillRecord(new SkillId("craft.smithing"), 0, 0, 0, 0);

            var changed = original.WithXp(500);

            Assert.That(changed.Xp, Is.EqualTo(500));
            Assert.That(original.Xp, Is.EqualTo(0));
        }

        [Test]
        public void WithRust_ReturnsCopyWithChangedRust()
        {
            var original = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 10);

            var changed = original.WithRust(1, 0);

            Assert.That(changed.RustyLevel, Is.EqualTo(1));
            Assert.That(changed.UnusedCounter, Is.EqualTo(0));
            Assert.That(original.RustyLevel, Is.EqualTo(0));
        }
    }
}