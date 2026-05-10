using System;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Simulation.Skills;
using NUnit.Framework;

// Design note:
// These tests pin per-tick SkillSet rust behavior.
// They do not test XP awards, job completion, actor mutation, event logging, reactions, or world pipeline orchestration.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Skills
{
    /// <summary>
    /// Verifies deterministic SkillSet rust ticking behavior.
    /// </summary>
    public sealed class SkillSetRustTickTests
    {
        [Test]
        public void Tick_EmptySkillSet_ReturnsEmptySkillSet()
        {
            var result = SkillSetRustTick.Tick(SkillSet.Empty, Array.Empty<SkillId>());

            Assert.That(result.SkillSet.Count, Is.EqualTo(0));
            Assert.That(result.TickedRecords, Is.EqualTo(0));
            Assert.That(result.UsedRecords, Is.EqualTo(0));
            Assert.That(result.RustedRecords, Is.EqualTo(0));
            Assert.That(result.RecoveredRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_NullSkillSet_TreatsAsEmpty()
        {
            var result = SkillSetRustTick.Tick(null, Array.Empty<SkillId>());

            Assert.That(result.SkillSet.Count, Is.EqualTo(0));
            Assert.That(result.TickedRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_NullUsedSkills_TreatsAllSkillsAsUnused()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 0, 10));

            var result = SkillSetRustTick.Tick(set, null);

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.UnusedCounter, Is.EqualTo(11));
            Assert.That(result.UsedRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_UnusedSkill_IncrementsUnusedCounter()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 0, 10));

            var result = SkillSetRustTick.Tick(set, Array.Empty<SkillId>());

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.UnusedCounter, Is.EqualTo(11));
            Assert.That(record.RustyLevel, Is.EqualTo(0));
            Assert.That(result.TickedRecords, Is.EqualTo(1));
        }

        [Test]
        public void Tick_UnusedSkillPastThreshold_IncrementsRustAndResetsCounter()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 0, 200));

            var result = SkillSetRustTick.Tick(set, Array.Empty<SkillId>());

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.UnusedCounter, Is.EqualTo(0));
            Assert.That(record.RustyLevel, Is.EqualTo(1));
            Assert.That(result.RustedRecords, Is.EqualTo(1));
        }

        [Test]
        public void Tick_LegendarySkillAtThreshold_DoesNotRustYet()
        {
            var set = NewSet(NewSkill("craft.legendary_smithing", 18100, 15, 0, 499));

            var result = SkillSetRustTick.Tick(set, Array.Empty<SkillId>());

            result.SkillSet.TryGet(new SkillId("craft.legendary_smithing"), out var record);

            Assert.That(record.UnusedCounter, Is.EqualTo(500));
            Assert.That(record.RustyLevel, Is.EqualTo(0));
            Assert.That(result.RustedRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_UsedSkill_ReducesRustAndResetsCounter()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 2, 50));

            var result = SkillSetRustTick.Tick(
                set,
                new[] { new SkillId("craft.smithing") });

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.RustyLevel, Is.EqualTo(1));
            Assert.That(record.UnusedCounter, Is.EqualTo(0));
            Assert.That(result.UsedRecords, Is.EqualTo(1));
            Assert.That(result.RecoveredRecords, Is.EqualTo(1));
        }

        [Test]
        public void Tick_UsedSkillWithoutRust_ResetsCounterButDoesNotRecover()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 0, 50));

            var result = SkillSetRustTick.Tick(
                set,
                new[] { new SkillId("craft.smithing") });

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.RustyLevel, Is.EqualTo(0));
            Assert.That(record.UnusedCounter, Is.EqualTo(0));
            Assert.That(result.RecoveredRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_DuplicateUsedSkillIds_CountsUsedRecordOnce()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 2, 50));

            var result = SkillSetRustTick.Tick(
                set,
                new[] { new SkillId("craft.smithing"), new SkillId("craft.smithing") });

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.RustyLevel, Is.EqualTo(1));
            Assert.That(result.UsedRecords, Is.EqualTo(1));
        }

        [Test]
        public void Tick_MissingUsedSkill_DoesNotCreateRecord()
        {
            var set = SkillSet.Empty;

            var result = SkillSetRustTick.Tick(
                set,
                new[] { new SkillId("craft.smithing") });

            Assert.That(result.SkillSet.Count, Is.EqualTo(0));
            Assert.That(result.UsedRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_EmptyUsedSkillId_IsIgnored()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 0, 10));

            var result = SkillSetRustTick.Tick(
                set,
                new[] { default(SkillId) });

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var record);

            Assert.That(record.UnusedCounter, Is.EqualTo(11));
            Assert.That(result.UsedRecords, Is.EqualTo(0));
        }

        [Test]
        public void Tick_MultipleSkills_UpdatesEachSkillDeterministically()
        {
            var set = NewSet(
                NewSkill("craft.smithing", 3500, 5, 2, 50),
                NewSkill("medical.surgery", 1100, 2, 0, 200),
                NewSkill("science.xenobiology", 500, 1, 0, 10));

            var result = SkillSetRustTick.Tick(
                set,
                new[] { new SkillId("craft.smithing") });

            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var smithing);
            result.SkillSet.TryGet(new SkillId("medical.surgery"), out var surgery);
            result.SkillSet.TryGet(new SkillId("science.xenobiology"), out var xenobiology);

            Assert.That(smithing.RustyLevel, Is.EqualTo(1));
            Assert.That(smithing.UnusedCounter, Is.EqualTo(0));

            Assert.That(surgery.RustyLevel, Is.EqualTo(1));
            Assert.That(surgery.UnusedCounter, Is.EqualTo(0));

            Assert.That(xenobiology.RustyLevel, Is.EqualTo(0));
            Assert.That(xenobiology.UnusedCounter, Is.EqualTo(11));

            Assert.That(result.TickedRecords, Is.EqualTo(3));
            Assert.That(result.UsedRecords, Is.EqualTo(1));
            Assert.That(result.RustedRecords, Is.EqualTo(1));
            Assert.That(result.RecoveredRecords, Is.EqualTo(1));
        }

        [Test]
        public void Tick_DoesNotMutateOriginalSkillSet()
        {
            var set = NewSet(NewSkill("craft.smithing", 3500, 5, 0, 10));

            var result = SkillSetRustTick.Tick(set, Array.Empty<SkillId>());

            set.TryGet(new SkillId("craft.smithing"), out var original);
            result.SkillSet.TryGet(new SkillId("craft.smithing"), out var changed);

            Assert.That(original.UnusedCounter, Is.EqualTo(10));
            Assert.That(changed.UnusedCounter, Is.EqualTo(11));
        }

        private static SkillSet NewSet(params SkillRecord[] records)
        {
            return new SkillSet(records);
        }

        private static SkillRecord NewSkill(
            string skillId,
            int xp,
            int level,
            int rustyLevel,
            int unusedCounter)
        {
            return new SkillRecord(
                new SkillId(skillId),
                xp,
                level,
                rustyLevel,
                unusedCounter);
        }
    }
}