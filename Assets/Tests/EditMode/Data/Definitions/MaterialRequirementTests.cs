using System;
using EmberCrpg.Data.Definitions;
using NUnit.Framework;

// Design note:
// These tests pin MaterialRequirement as a data-driven reaction input requirement.
// They do not test inventory consumption, stockpile lookup, item creation, jobs, or reaction execution.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Data.Definitions
{
    /// <summary>
    /// Verifies reaction input requirement behavior.
    /// </summary>
    public sealed class MaterialRequirementTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var requirement = new MaterialRequirement("ore", 2, true);

            Assert.That(requirement.Tag, Is.EqualTo("ore"));
            Assert.That(requirement.Quantity, Is.EqualTo(2));
            Assert.That(requirement.Consumed, Is.True);
        }

        [Test]
        public void Constructor_DefaultConsumed_IsTrue()
        {
            var requirement = new MaterialRequirement("fuel", 1);

            Assert.That(requirement.Consumed, Is.True);
        }

        [Test]
        public void Constructor_AllowsNonConsumedRequirement()
        {
            var requirement = new MaterialRequirement("anvil", 1, false);

            Assert.That(requirement.Tag, Is.EqualTo("anvil"));
            Assert.That(requirement.Quantity, Is.EqualTo(1));
            Assert.That(requirement.Consumed, Is.False);
        }

        [Test]
        public void Constructor_TrimsTag()
        {
            var requirement = new MaterialRequirement("  alien_sample  ", 3, true);

            Assert.That(requirement.Tag, Is.EqualTo("alien_sample"));
        }

        [Test]
        public void Constructor_EmptyTag_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MaterialRequirement("", 1, true));
        }

        [Test]
        public void Constructor_WhitespaceTag_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MaterialRequirement("   ", 1, true));
        }

        [Test]
        public void Constructor_NullTag_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MaterialRequirement(null, 1, true));
        }

        [Test]
        public void Constructor_QuantityZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MaterialRequirement("ore", 0, true));
        }

        [Test]
        public void Constructor_NegativeQuantity_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MaterialRequirement("ore", -1, true));
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var requirement = new MaterialRequirement("fuel", 2, true);

            Assert.That(requirement.ToString(), Is.EqualTo("MaterialRequirement(fuel x2 consumed)"));
        }

        [Test]
        public void ToString_ForNonConsumed_ReturnsDebugLabel()
        {
            var requirement = new MaterialRequirement("anvil", 1, false);

            Assert.That(requirement.ToString(), Is.EqualTo("MaterialRequirement(anvil x1 checked)"));
        }
    }
}