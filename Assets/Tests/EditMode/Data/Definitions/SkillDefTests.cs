using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Skills;
using NUnit.Framework;
using StatAttribute = EmberCrpg.Domain.Stats.Attribute;

// Design note:
// These tests pin SkillDef as a data-driven skill definition row.
// They do not test skill XP, actor skill state, registry loading, JSON parsing, or formulas.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Data.Definitions
{
    /// <summary>
    /// Verifies Ember's data-driven skill definition contract.
    /// </summary>
    public sealed class SkillDefTests
    {
        [Test]
        public void Constructor_StoresIdentityAndLabels()
        {
            var def = new SkillDef(
                new SkillId("field.lockwork"),
                "Lockwork",
                "field",
                StatAttribute.Agi,
                new[] { "locks", "traps" });

            Assert.That(def.Id, Is.EqualTo(new SkillId("field.lockwork")));
            Assert.That(def.DisplayName, Is.EqualTo("Lockwork"));
            Assert.That(def.Category, Is.EqualTo("field"));
        }

        [Test]
        public void Constructor_StoresGoverningAttribute()
        {
            var def = new SkillDef(
                new SkillId("science.xenobiology"),
                "Xenobiology",
                "science",
                StatAttribute.Mnd,
                Array.Empty<string>());

            Assert.That(def.GoverningAttribute, Is.EqualTo(StatAttribute.Mnd));
        }

        [Test]
        public void Constructor_StoresTags()
        {
            var def = new SkillDef(
                new SkillId("magic.embercraft"),
                "Embercraft",
                "magic",
                StatAttribute.Ins,
                new[] { "spell", "ritual" });

            Assert.That(def.Tags.Count, Is.EqualTo(2));
            Assert.That(def.Tags[0], Is.EqualTo("spell"));
            Assert.That(def.Tags[1], Is.EqualTo("ritual"));
        }

        [Test]
        public void Constructor_NullTags_NormalizesToEmptyList()
        {
            var def = new SkillDef(
                new SkillId("combat.sidearm"),
                "Sidearm",
                "combat",
                StatAttribute.Agi,
                null);

            Assert.That(def.Tags.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_EmptySkillId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SkillDef(
                default(SkillId),
                "Broken",
                "test",
                StatAttribute.Mnd,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyDisplayName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SkillDef(
                new SkillId("broken.skill"),
                "",
                "test",
                StatAttribute.Mnd,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyCategory_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SkillDef(
                new SkillId("broken.skill"),
                "Broken",
                "",
                StatAttribute.Mnd,
                Array.Empty<string>()));
        }
    }
}