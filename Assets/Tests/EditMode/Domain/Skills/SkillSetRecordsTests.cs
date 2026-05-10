using System.Collections.Generic;
using EmberCrpg.Domain.Skills;
using NUnit.Framework;

// Design note:
// These tests pin SkillSet record projection for deterministic batch systems.
// They do not test XP application, rust ticking, actor mutation, jobs, reactions, or world ticks.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Skills
{
    /// <summary>
    /// Verifies deterministic read-only SkillSet record projection.
    /// </summary>
    public sealed class SkillSetRecordsTests
    {
        [Test]
        public void Records_EmptySkillSet_ReturnsEmptyList()
        {
            var set = SkillSet.Empty;

            Assert.That(set.Records.Count, Is.EqualTo(0));
        }

        [Test]
        public void Records_ReturnsRecordsInConstructorOrder()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 500, 1, 0, 0);
            var surgery = new SkillRecord(new SkillId("medical.surgery"), 1100, 2, 0, 0);

            var set = new SkillSet(new[] { smithing, surgery });

            Assert.That(set.Records.Count, Is.EqualTo(2));
            Assert.That(set.Records[0].SkillId, Is.EqualTo(new SkillId("craft.smithing")));
            Assert.That(set.Records[1].SkillId, Is.EqualTo(new SkillId("medical.surgery")));
        }

        [Test]
        public void Records_ConstructorCopiesSourceList()
        {
            var source = new List<SkillRecord>
            {
                new SkillRecord(new SkillId("craft.smithing"), 500, 1, 0, 0)
            };

            var set = new SkillSet(source);

            source.Clear();

            Assert.That(set.Records.Count, Is.EqualTo(1));
            Assert.That(set.Records[0].SkillId, Is.EqualTo(new SkillId("craft.smithing")));
        }

        [Test]
        public void Records_WithExistingSkill_PreservesOriginalOrder()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 500, 1, 0, 0);
            var surgery = new SkillRecord(new SkillId("medical.surgery"), 1100, 2, 0, 0);
            var set = new SkillSet(new[] { smithing, surgery });

            var changed = set.With(new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 0));

            Assert.That(changed.Records.Count, Is.EqualTo(2));
            Assert.That(changed.Records[0].SkillId, Is.EqualTo(new SkillId("craft.smithing")));
            Assert.That(changed.Records[0].Level, Is.EqualTo(5));
            Assert.That(changed.Records[1].SkillId, Is.EqualTo(new SkillId("medical.surgery")));
        }

        [Test]
        public void Records_WithNewSkill_AppendsAtEnd()
        {
            var smithing = new SkillRecord(new SkillId("craft.smithing"), 500, 1, 0, 0);
            var set = new SkillSet(new[] { smithing });

            var changed = set.With(new SkillRecord(new SkillId("science.xenobiology"), 1100, 2, 0, 0));

            Assert.That(changed.Records.Count, Is.EqualTo(2));
            Assert.That(changed.Records[0].SkillId, Is.EqualTo(new SkillId("craft.smithing")));
            Assert.That(changed.Records[1].SkillId, Is.EqualTo(new SkillId("science.xenobiology")));
        }
    }
}