using System;
using EmberCrpg.Data.Definitions;
using NUnit.Framework;

// Design note:
// These tests pin ProductOutput as a data-driven reaction output definition.
// They do not test item creation, material inheritance resolution, inventory insertion, jobs, or reaction execution.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Data.Definitions
{
    /// <summary>
    /// Verifies reaction output definition behavior.
    /// </summary>
    public sealed class ProductOutputTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var output = new ProductOutput("iron_sword", "iron", 1);

            Assert.That(output.ItemDefId, Is.EqualTo("iron_sword"));
            Assert.That(output.MaterialId, Is.EqualTo("iron"));
            Assert.That(output.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_DefaultQuantity_IsOne()
        {
            var output = new ProductOutput("iron_ingot", "iron");

            Assert.That(output.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_AllowsInheritedMaterial()
        {
            var output = new ProductOutput("forged_blade", ProductOutput.InheritMaterialId, 1);

            Assert.That(output.MaterialId, Is.EqualTo("inherit"));
            Assert.That(output.InheritsMaterial, Is.True);
        }

        [Test]
        public void Constructor_TrimsItemDefIdAndMaterialId()
        {
            var output = new ProductOutput("  plasma_cell  ", "  refined_lithium  ", 2);

            Assert.That(output.ItemDefId, Is.EqualTo("plasma_cell"));
            Assert.That(output.MaterialId, Is.EqualTo("refined_lithium"));
        }

        [Test]
        public void Constructor_EmptyItemDefId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ProductOutput("", "iron", 1));
        }

        [Test]
        public void Constructor_WhitespaceMaterialId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ProductOutput("iron_sword", "   ", 1));
        }

        [Test]
        public void Constructor_NullMaterialId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ProductOutput("iron_sword", null, 1));
        }

        [Test]
        public void Constructor_QuantityZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProductOutput("iron_sword", "iron", 0));
        }

        [Test]
        public void Constructor_NegativeQuantity_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProductOutput("iron_sword", "iron", -1));
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var output = new ProductOutput("iron_ingot", "inherit", 2);

            Assert.That(output.ToString(), Is.EqualTo("ProductOutput(iron_ingot/inherit x2)"));
        }
    }
}