using System;
using NUnit.Framework;
using Attribute = EmberCrpg.Domain.Stats.Attribute;

// Design note:
// These tests pin Ember's six canonical attributes.
// They do not test values, modifiers, character creation, or universe-specific display names.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Stats
{
    /// <summary>
    /// Verifies Ember's canonical six-stat attribute identifiers.
    /// </summary>
    public sealed class AttributeTests
    {
        [Test]
        public void Enum_ContainsSixCanonicalAttributes()
        {
            var values = Enum.GetValues(typeof(Attribute));

            Assert.That(values.Length, Is.EqualTo(6));
        }

        [Test]
        public void Enum_OrderMatchesEmberStatOrder()
        {
            Assert.That((int)Attribute.Mig, Is.EqualTo(0));
            Assert.That((int)Attribute.Agi, Is.EqualTo(1));
            Assert.That((int)Attribute.End, Is.EqualTo(2));
            Assert.That((int)Attribute.Mnd, Is.EqualTo(3));
            Assert.That((int)Attribute.Ins, Is.EqualTo(4));
            Assert.That((int)Attribute.Pre, Is.EqualTo(5));
        }

        [Test]
        public void Enum_NamesExposeEmberVocabulary()
        {
            Assert.That(Enum.IsDefined(typeof(Attribute), "Mig"), Is.True);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Agi"), Is.True);
            Assert.That(Enum.IsDefined(typeof(Attribute), "End"), Is.True);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Mnd"), Is.True);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Ins"), Is.True);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Pre"), Is.True);
        }

        [Test]
        public void Enum_DoesNotContainDndOrDfuNames()
        {
            Assert.That(Enum.IsDefined(typeof(Attribute), "Strength"), Is.False);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Dexterity"), Is.False);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Constitution"), Is.False);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Intelligence"), Is.False);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Willpower"), Is.False);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Personality"), Is.False);
            Assert.That(Enum.IsDefined(typeof(Attribute), "Luck"), Is.False);
        }
    }
}