using EmberCrpg.Domain.Production;
using NUnit.Framework;

// Design note:
// These tests pin ReactionId as the stable identity primitive for reactions/process definitions.
// They do not test ReactionDef fields, inputs, outputs, execution, inventory, jobs, or registries.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Production
{
    /// <summary>
    /// Verifies reaction identity behavior.
    /// </summary>
    public sealed class ReactionIdTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var id = new ReactionId("reaction.smelt_iron_ingot");

            Assert.That(id.Value, Is.EqualTo("reaction.smelt_iron_ingot"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var id = default(ReactionId);

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var id = new ReactionId("");

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void NullString_IsEmpty()
        {
            var id = new ReactionId(null);

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new ReactionId("reaction.smelt_iron_ingot");
            var right = new ReactionId("reaction.smelt_iron_ingot");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new ReactionId("reaction.smelt_iron_ingot");
            var right = new ReactionId("reaction.repair_plasma_rifle");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var id = new ReactionId("reaction.blood_rite");

            Assert.That(id.ToString(), Is.EqualTo("ReactionId(reaction.blood_rite)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var id = default(ReactionId);

            Assert.That(id.ToString(), Is.EqualTo("ReactionId.Empty"));
        }
    }
}