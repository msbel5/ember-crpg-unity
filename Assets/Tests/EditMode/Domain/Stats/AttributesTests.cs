using System;
using EmberCrpg.Domain.Stats;
using NUnit.Framework;
using StatAttribute = EmberCrpg.Domain.Stats.Attribute;

// Design note:
// These tests pin Attributes as Ember's six-stat immutable value container.
// They do not test formulas, skills, vitals, character creation rolls, or universe-specific labels.
namespace EmberCrpg.Tests.EditMode.Domain.Stats
{
    /// <summary>
    /// Verifies Ember's canonical six-stat value container.
    /// </summary>
    public sealed class AttributesTests
    {
        [Test]
        public void Base50_ReturnsAllStatsAtFifty()
        {
            var attributes = Attributes.Base50;

            Assert.That(attributes.Get(StatAttribute.Mig), Is.EqualTo(50));
            Assert.That(attributes.Get(StatAttribute.Agi), Is.EqualTo(50));
            Assert.That(attributes.Get(StatAttribute.End), Is.EqualTo(50));
            Assert.That(attributes.Get(StatAttribute.Mnd), Is.EqualTo(50));
            Assert.That(attributes.Get(StatAttribute.Ins), Is.EqualTo(50));
            Assert.That(attributes.Get(StatAttribute.Pre), Is.EqualTo(50));
        }

        [Test]
        public void Constructor_StoresAllValues()
        {
            var attributes = new Attributes(60, 55, 50, 45, 40, 35);

            Assert.That(attributes.Mig, Is.EqualTo(60));
            Assert.That(attributes.Agi, Is.EqualTo(55));
            Assert.That(attributes.End, Is.EqualTo(50));
            Assert.That(attributes.Mnd, Is.EqualTo(45));
            Assert.That(attributes.Ins, Is.EqualTo(40));
            Assert.That(attributes.Pre, Is.EqualTo(35));
        }

        [Test]
        public void Get_ReturnsRequestedStat()
        {
            var attributes = new Attributes(60, 55, 50, 45, 40, 35);

            Assert.That(attributes.Get(StatAttribute.Mnd), Is.EqualTo(45));
        }

        [Test]
        public void Constructor_ValueBelowZero_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Attributes(-1, 50, 50, 50, 50, 50));
        }

        [Test]
        public void Constructor_ValueAboveOneHundred_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Attributes(101, 50, 50, 50, 50, 50));
        }

        [Test]
        public void With_ReturnsCopyWithChangedStat()
        {
            var original = Attributes.Base50;

            var changed = original.With(StatAttribute.Mig, 75);

            Assert.That(changed.Mig, Is.EqualTo(75));
            Assert.That(original.Mig, Is.EqualTo(50));
        }

        [Test]
        public void With_ValueOutsideRange_ThrowsArgumentOutOfRange()
        {
            var attributes = Attributes.Base50;

            Assert.Throws<ArgumentOutOfRangeException>(() => attributes.With(StatAttribute.Agi, 101));
        }
    }
}