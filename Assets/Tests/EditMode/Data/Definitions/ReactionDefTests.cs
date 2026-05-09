using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin ReactionDef as a data-driven reaction/process definition.
// They do not test reaction execution, inventory consumption, item creation, XP, quality rolls, jobs, or actor eligibility.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Data.Definitions
{
    /// <summary>
    /// Verifies reaction/process definition behavior.
    /// </summary>
    public sealed class ReactionDefTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var def = NewSmeltReaction();

            Assert.That(def.Id, Is.EqualTo(new ReactionId("reaction.smelt_iron_ingot")));
            Assert.That(def.Label, Is.EqualTo("Smelt Iron Ingot"));
            Assert.That(def.ActivitySiteKind, Is.EqualTo("iron_forge"));
            Assert.That(def.RequiredActivitySiteRole, Is.EqualTo(new ActivitySiteRole("work")));
            Assert.That(def.RequiredSkillId, Is.EqualTo(new SkillId("craft.smithing")));
            Assert.That(def.BaseDurationTicks, Is.EqualTo(120));
            Assert.That(def.QualityFormula, Is.EqualTo(ReactionQualityFormula.WeightedRandom));
            Assert.That(def.InputMaterials.Count, Is.EqualTo(2));
            Assert.That(def.OutputProducts.Count, Is.EqualTo(1));
            Assert.That(def.Tags.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_AllowsNoRequiredSkill()
        {
            var def = new ReactionDef(
                new ReactionId("reaction.rest_at_bed"),
                "Rest at Bed",
                "owned_bed",
                new ActivitySiteRole("rest"),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                60,
                ReactionQualityFormula.Fixed,
                new[] { "rest" });

            Assert.That(def.RequiredSkillId.IsEmpty, Is.True);
        }

        [Test]
        public void Constructor_AllowsCustomQualityFormula()
        {
            var def = new ReactionDef(
                new ReactionId("reaction.blood_rite"),
                "Blood Rite",
                "blood_altar",
                new ActivitySiteRole("ritual"),
                new SkillId("occult.blood_rite"),
                new[] { new MaterialRequirement("blood", 1, true) },
                Array.Empty<ProductOutput>(),
                200,
                new ReactionQualityFormula("ritual_omen"),
                new[] { "ritual", "occult" });

            Assert.That(def.QualityFormula.Value, Is.EqualTo("ritual_omen"));
        }

        [Test]
        public void Constructor_EmptyReactionId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ReactionDef(
                default(ReactionId),
                "Broken",
                "forge",
                new ActivitySiteRole("work"),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                100,
                ReactionQualityFormula.WeightedRandom,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyLabel_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ReactionDef(
                new ReactionId("reaction.broken"),
                "",
                "forge",
                new ActivitySiteRole("work"),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                100,
                ReactionQualityFormula.WeightedRandom,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyActivitySiteKind_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ReactionDef(
                new ReactionId("reaction.broken"),
                "Broken",
                "",
                new ActivitySiteRole("work"),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                100,
                ReactionQualityFormula.WeightedRandom,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyActivitySiteRole_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ReactionDef(
                new ReactionId("reaction.broken"),
                "Broken",
                "forge",
                default(ActivitySiteRole),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                100,
                ReactionQualityFormula.WeightedRandom,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_BaseDurationTicksZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReactionDef(
                new ReactionId("reaction.broken"),
                "Broken",
                "forge",
                new ActivitySiteRole("work"),
                default(SkillId),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                0,
                ReactionQualityFormula.WeightedRandom,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyQualityFormula_DefaultsToWeightedRandom()
        {
            var def = new ReactionDef(
                new ReactionId("reaction.cook_meal"),
                "Cook Meal",
                "campfire",
                new ActivitySiteRole("work"),
                new SkillId("craft.cooking"),
                Array.Empty<MaterialRequirement>(),
                Array.Empty<ProductOutput>(),
                30,
                default(ReactionQualityFormula),
                Array.Empty<string>());

            Assert.That(def.QualityFormula, Is.EqualTo(ReactionQualityFormula.WeightedRandom));
        }

        [Test]
        public void Constructor_NullLists_NormalizeToEmptyLists()
        {
            var def = new ReactionDef(
                new ReactionId("reaction.decode_signal"),
                "Decode Signal",
                "nav_console",
                new ActivitySiteRole("command"),
                new SkillId("science.signals"),
                null,
                null,
                80,
                ReactionQualityFormula.Fixed,
                null);

            Assert.That(def.InputMaterials.Count, Is.EqualTo(0));
            Assert.That(def.OutputProducts.Count, Is.EqualTo(0));
            Assert.That(def.Tags.Count, Is.EqualTo(0));
        }

        [Test]
        public void RequiresInputTag_ExistingTag_ReturnsTrue()
        {
            var def = NewSmeltReaction();

            Assert.That(def.RequiresInputTag("ore"), Is.True);
        }

        [Test]
        public void RequiresInputTag_MissingTag_ReturnsFalse()
        {
            var def = NewSmeltReaction();

            Assert.That(def.RequiresInputTag("blood"), Is.False);
        }

        private static ReactionDef NewSmeltReaction()
        {
            return new ReactionDef(
                new ReactionId("reaction.smelt_iron_ingot"),
                "Smelt Iron Ingot",
                "iron_forge",
                new ActivitySiteRole("work"),
                new SkillId("craft.smithing"),
                new[]
                {
                    new MaterialRequirement("ore", 2, true),
                    new MaterialRequirement("fuel", 1, true)
                },
                new[]
                {
                    new ProductOutput("iron_ingot", ProductOutput.InheritMaterialId, 1)
                },
                120,
                ReactionQualityFormula.WeightedRandom,
                new[] { "metal", "production" });
        }
    }
}