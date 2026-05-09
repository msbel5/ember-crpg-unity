using EmberCrpg.Domain.Production;
using NUnit.Framework;

// Design note:
// These tests pin crafted output quality as a pure deterministic formula.
// They do not test job completion, XP awards, item creation, registries, or random number generators.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Production
{
    /// <summary>
    /// Verifies skill-based crafted output quality selection.
    /// </summary>
    public sealed class CraftQualityTests
    {
        [Test]
        public void QualityLevel_ValuesAreStable()
        {
            Assert.That((int)QualityLevel.Ordinary, Is.EqualTo(0));
            Assert.That((int)QualityLevel.WellCrafted, Is.EqualTo(1));
            Assert.That((int)QualityLevel.FinelyCrafted, Is.EqualTo(2));
            Assert.That((int)QualityLevel.Superior, Is.EqualTo(3));
            Assert.That((int)QualityLevel.Exceptional, Is.EqualTo(4));
            Assert.That((int)QualityLevel.Masterwork, Is.EqualTo(5));
        }

        [Test]
        public void FromEffectiveSkill_LowSkillAtPointFive_ReturnsOrdinary()
        {
            var quality = CraftQuality.FromEffectiveSkill(0, 0.50);

            Assert.That(quality, Is.EqualTo(QualityLevel.Ordinary));
        }

        [Test]
        public void FromEffectiveSkill_LowSkillAtPointNinetyFive_ReturnsWellCrafted()
        {
            var quality = CraftQuality.FromEffectiveSkill(0, 0.95);

            Assert.That(quality, Is.EqualTo(QualityLevel.WellCrafted));
        }

        [Test]
        public void FromEffectiveSkill_SkillFiveAtPointEightyFive_ReturnsFinelyCrafted()
        {
            var quality = CraftQuality.FromEffectiveSkill(5, 0.85);

            Assert.That(quality, Is.EqualTo(QualityLevel.FinelyCrafted));
        }

        [Test]
        public void FromEffectiveSkill_SkillEightAtPointNinetySix_ReturnsExceptional()
        {
            var quality = CraftQuality.FromEffectiveSkill(8, 0.96);

            Assert.That(quality, Is.EqualTo(QualityLevel.Exceptional));
        }

        [Test]
        public void FromEffectiveSkill_SkillFourteenAtPointZero_ReturnsWellCrafted()
        {
            var quality = CraftQuality.FromEffectiveSkill(14, 0.00);

            Assert.That(quality, Is.EqualTo(QualityLevel.WellCrafted));
        }

        [Test]
        public void FromEffectiveSkill_LegendaryAtPointFive_ReturnsMasterwork()
        {
            var quality = CraftQuality.FromEffectiveSkill(15, 0.50);

            Assert.That(quality, Is.EqualTo(QualityLevel.Masterwork));
        }

        [Test]
        public void FromEffectiveSkill_NegativeSkill_UsesLowSkillBracket()
        {
            var quality = CraftQuality.FromEffectiveSkill(-5, 0.95);

            Assert.That(quality, Is.EqualTo(QualityLevel.WellCrafted));
        }

        [Test]
        public void FromEffectiveSkill_RngBelowZero_ClampsToZero()
        {
            var quality = CraftQuality.FromEffectiveSkill(9, -0.25);

            Assert.That(quality, Is.EqualTo(QualityLevel.Ordinary));
        }

        [Test]
        public void FromEffectiveSkill_RngAtOne_ClampsBelowOne()
        {
            var quality = CraftQuality.FromEffectiveSkill(0, 1.00);

            Assert.That(quality, Is.EqualTo(QualityLevel.WellCrafted));
        }
    }
}