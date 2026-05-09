using System;
using EmberCrpg.Domain.Skills;
using NUnit.Framework;

// Design note:
// These tests pin SkillSet as an actor-local skill collection.
// They do not test XP thresholds, rust ticking, job assignment, quality rolls, or actor records.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Skills
{
    /// <summary>
    /// Verifies actor-local skill collection behavior.
    /// </summary>
    public sealed class SkillSetTests
    {
        [Test]
        public void Empty_ReturnsNoRecords()
        {
            var set = SkillSet.Empty;

            Assert.That(set.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_StoresRecords()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 0);

            var set = new SkillSet(new[] { smithing });

            Assert.That(set.Count, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_DuplicateSkillId_ThrowsArgumentException()
        {
            var first = new SkillRecord(new SkillId("craft.smithing"), 0, 0, 0, 0);
            var second = new SkillRecord(new SkillId("craft.smithing"), 500, 1, 0, 0);

            Assert.Throws<ArgumentException>(() => new SkillSet(new[] { first, second }));
        }

        [Test]
        public void Contains_ExistingSkill_ReturnsTrue()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 0);
            var set = new SkillSet(new[] { smithing });

            Assert.That(set.Contains(new SkillId("craft.smithing")), Is.True);
        }

        [Test]
        public void Contains_MissingSkill_ReturnsFalse()
        {
            var set = SkillSet.Empty;

            Assert.That(set.Contains(new SkillId("craft.smithing")), Is.False);
        }

        [Test]
        public void TryGet_ExistingSkill_ReturnsRecord()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 0);
            var set = new SkillSet(new[] { smithing });

            var found = set.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(found, Is.True);
            Assert.That(record.Level, Is.EqualTo(5));
        }

        [Test]
        public void EffectiveLevel_MissingSkill_ReturnsZero()
        {
            var set = SkillSet.Empty;

            Assert.That(set.EffectiveLevel(new SkillId("craft.smithing")), Is.EqualTo(0));
        }

        [Test]
        public void With_ExistingSkill_ReplacesRecord()
        {
            var original = new SkillRecord(new SkillId("craft.smithing"), 0, 0, 0, 0);
            var changed = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 1, 0);
            var set = new SkillSet(new[] { original });

            var next = set.With(changed);

            Assert.That(next.Count, Is.EqualTo(1));
            Assert.That(next.EffectiveLevel(new SkillId("craft.smithing")), Is.EqualTo(4));
        }

        [Test]
        public void With_NewSkill_AppendsRecord()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 0, 0, 0, 0);
            var set = SkillSet.Empty;

            var next = set.With(smithing);

            Assert.That(next.Count, Is.EqualTo(1));
            Assert.That(next.Contains(new SkillId("craft.smithing")), Is.True);
        }
    }
}