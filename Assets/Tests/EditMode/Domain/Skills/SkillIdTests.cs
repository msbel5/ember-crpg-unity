using EmberCrpg.Domain.Skills;
using NUnit.Framework;

// Design note:
// These tests pin SkillId as a data-driven skill identity primitive.
// They do not test skill definitions, skill values, XP, rust, categories, or formulas.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Skills
{
    /// <summary>
    /// Verifies Ember's data-driven skill identity value.
    /// </summary>
    public sealed class SkillIdTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var id = new SkillId("field.lockwork");

            Assert.That(id.Value, Is.EqualTo("field.lockwork"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var id = default(SkillId);

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var id = new SkillId("");

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new SkillId("combat.blade.short");
            var right = new SkillId("combat.blade.short");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new SkillId("combat.blade.short");
            var right = new SkillId("science.xenobiology");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var id = new SkillId("magic.embercraft");

            Assert.That(id.ToString(), Is.EqualTo("SkillId(magic.embercraft)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var id = default(SkillId);

            Assert.That(id.ToString(), Is.EqualTo("SkillId.Empty"));
        }
    }
}