using EmberCrpg.Data.Definitions;
using NUnit.Framework;

// Design note:
// These tests pin ReactionQualityFormula as a data-driven selector.
// They do not test quality calculation, RNG, jobs, reactions, skill records, or item outputs.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Data.Definitions
{
    /// <summary>
    /// Verifies reaction quality formula selector behavior.
    /// </summary>
    public sealed class ReactionQualityFormulaTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var formula = new ReactionQualityFormula("weighted_random");

            Assert.That(formula.Value, Is.EqualTo("weighted_random"));
        }

        [Test]
        public void Constructor_TrimsValue()
        {
            var formula = new ReactionQualityFormula("  fixed  ");

            Assert.That(formula.Value, Is.EqualTo("fixed"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var formula = default(ReactionQualityFormula);

            Assert.That(formula.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var formula = new ReactionQualityFormula("");

            Assert.That(formula.IsEmpty, Is.True);
        }

        [Test]
        public void NullString_IsEmpty()
        {
            var formula = new ReactionQualityFormula(null);

            Assert.That(formula.IsEmpty, Is.True);
        }

        [Test]
        public void WeightedRandom_ReturnsCanonicalFormula()
        {
            var formula = ReactionQualityFormula.WeightedRandom;

            Assert.That(formula.Value, Is.EqualTo("weighted_random"));
            Assert.That(formula.IsWeightedRandom, Is.True);
            Assert.That(formula.IsFixed, Is.False);
        }

        [Test]
        public void Fixed_ReturnsCanonicalFormula()
        {
            var formula = ReactionQualityFormula.Fixed;

            Assert.That(formula.Value, Is.EqualTo("fixed"));
            Assert.That(formula.IsFixed, Is.True);
            Assert.That(formula.IsWeightedRandom, Is.False);
        }

        [Test]
        public void CustomFormula_IsAllowedForFutureUniverses()
        {
            var formula = new ReactionQualityFormula("ritual_omen");

            Assert.That(formula.Value, Is.EqualTo("ritual_omen"));
            Assert.That(formula.IsEmpty, Is.False);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new ReactionQualityFormula("weighted_random");
            var right = new ReactionQualityFormula("weighted_random");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new ReactionQualityFormula("weighted_random");
            var right = new ReactionQualityFormula("fixed");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var formula = new ReactionQualityFormula("factory_grade");

            Assert.That(formula.ToString(), Is.EqualTo("ReactionQualityFormula(factory_grade)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var formula = default(ReactionQualityFormula);

            Assert.That(formula.ToString(), Is.EqualTo("ReactionQualityFormula.Empty"));
        }
    }
}