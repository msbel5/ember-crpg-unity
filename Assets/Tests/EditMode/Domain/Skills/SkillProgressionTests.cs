using EmberCrpg.Domain.Skills;
using NUnit.Framework;

// Design note:
// These tests pin skill progression formulas.
// They do not test jobs, XP rewards, actor containers, crafted quality, RNG, or data registries.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Skills
{
    /// <summary>
    /// Verifies XP-to-level and rust rules for actor skills.
    /// </summary>
    public sealed class SkillProgressionTests
    {
        [Test]
        public void LevelFromXp_NegativeXp_ReturnsZero()
        {
            Assert.That(SkillProgression.LevelFromXp(-1), Is.EqualTo(0));
        }

        [Test]
        public void LevelFromXp_ThresholdBoundary_ReturnsExpectedLevel()
        {
            Assert.That(SkillProgression.LevelFromXp(1099), Is.EqualTo(1));
            Assert.That(SkillProgression.LevelFromXp(1100), Is.EqualTo(2));
        }

        [Test]
        public void LevelFromXp_ThresholdFourteen_ReturnsGrandMasterLevel()
        {
            Assert.That(SkillProgression.LevelFromXp(16100), Is.EqualTo(14));
        }

        [Test]
        public void LevelFromXp_BeyondThresholdFourteen_ReturnsLegendaryLevels()
        {
            Assert.That(SkillProgression.LevelFromXp(18099), Is.EqualTo(14));
            Assert.That(SkillProgression.LevelFromXp(18100), Is.EqualTo(15));
            Assert.That(SkillProgression.LevelFromXp(20100), Is.EqualTo(16));
        }

        [Test]
        public void LevelName_LevelFifteenOrHigher_ReturnsLegendary()
        {
            Assert.That(SkillProgression.LevelName(15), Is.EqualTo("Legendary"));
        }

        [Test]
        public void RustThreshold_BelowLegendary_ReturnsTwoHundred()
        {
            Assert.That(SkillProgression.RustThreshold(14), Is.EqualTo(200));
        }

        [Test]
        public void RustThreshold_Legendary_ReturnsFiveHundred()
        {
            Assert.That(SkillProgression.RustThreshold(15), Is.EqualTo(500));
        }

        [Test]
        public void TickRust_WhenUsed_ResetsCounterAndReducesRust()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 2, 150);

            var changed = SkillProgression.TickRust(record, true);

            Assert.That(changed.UnusedCounter, Is.EqualTo(0));
            Assert.That(changed.RustyLevel, Is.EqualTo(1));
        }

        [Test]
        public void TickRust_UnusedBelowThreshold_IncrementsCounter()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 199);

            var changed = SkillProgression.TickRust(record, false);

            Assert.That(changed.UnusedCounter, Is.EqualTo(200));
            Assert.That(changed.RustyLevel, Is.EqualTo(0));
        }

        [Test]
        public void TickRust_UnusedPastThreshold_AddsRustAndResetsCounter()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 3500, 5, 0, 200);

            var changed = SkillProgression.TickRust(record, false);

            Assert.That(changed.UnusedCounter, Is.EqualTo(0));
            Assert.That(changed.RustyLevel, Is.EqualTo(1));
        }

        [Test]
        public void TickRust_LegendaryAtThreshold_DoesNotRustYet()
        {
            var record = new SkillRecord(new SkillId("craft.smithing"), 18100, 15, 0, 499);

            var changed = SkillProgression.TickRust(record, false);

            Assert.That(changed.UnusedCounter, Is.EqualTo(500));
            Assert.That(changed.RustyLevel, Is.EqualTo(0));
        }
    }
}