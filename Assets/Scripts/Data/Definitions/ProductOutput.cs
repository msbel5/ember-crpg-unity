using System;

// Design note:
// ProductOutput is a data-driven output definition for ReactionDef/ProcessDef.
// It describes what should be produced; item creation, material inheritance resolution, and stock insertion live elsewhere.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// A single product created by a reaction or process.
    /// </summary>
    public sealed class ProductOutput
    {
        /// <summary>
        /// Material id sentinel meaning the output should inherit material from a matching input.
        /// </summary>
        public const string InheritMaterialId = "inherit";

        /// <summary>
        /// Item definition id for the output item.
        /// </summary>
        public readonly string ItemDefId;

        /// <summary>
        /// Material definition id for the output, or "inherit" to resolve from inputs.
        /// </summary>
        public readonly string MaterialId;

        /// <summary>
        /// Number of output items produced.
        /// </summary>
        public readonly int Quantity;

        /// <summary>
        /// True when output material should be inherited from input materials.
        /// </summary>
        public bool InheritsMaterial
        {
            get { return string.Equals(MaterialId, InheritMaterialId, StringComparison.Ordinal); }
        }

        /// <summary>
        /// Creates a product output with quantity one.
        /// </summary>
        public ProductOutput(string itemDefId, string materialId)
            : this(itemDefId, materialId, 1)
        {
        }

        /// <summary>
        /// Creates a product output definition.
        /// </summary>
        public ProductOutput(string itemDefId, string materialId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemDefId))
                throw new ArgumentException("Product output item definition id cannot be empty.", nameof(itemDefId));
            if (string.IsNullOrWhiteSpace(materialId))
                throw new ArgumentException("Product output material id cannot be empty.", nameof(materialId));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Product output quantity must be positive.");

            ItemDefId = itemDefId.Trim();
            MaterialId = materialId.Trim();
            Quantity = quantity;
        }

        /// <summary>
        /// Returns a compact debug label for this product output.
        /// </summary>
        public override string ToString()
        {
            return $"ProductOutput({ItemDefId}/{MaterialId} x{Quantity})";
        }
    }
}