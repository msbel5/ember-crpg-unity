using System;

// Design note:
// MaterialRequirement is a data-driven input requirement for ReactionDef/ProcessDef.
// It uses tags instead of hardcoded item classes so fantasy, sci-fi, ritual, colony, and medical workflows share one pipeline.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// A single tagged input required by a reaction or process.
    /// </summary>
    public sealed class MaterialRequirement
    {
        /// <summary>
        /// Required material/item tag, such as ore, fuel, cloth, blood, corpse, sample, power_cell, or tool.
        /// </summary>
        public readonly string Tag;

        /// <summary>
        /// Required quantity of matching inputs.
        /// </summary>
        public readonly int Quantity;

        /// <summary>
        /// True when matching inputs are consumed by the reaction; false when only checked as a tool, fixture, or catalyst.
        /// </summary>
        public readonly bool Consumed;

        /// <summary>
        /// Creates a consumed input requirement.
        /// </summary>
        public MaterialRequirement(string tag, int quantity)
            : this(tag, quantity, true)
        {
        }

        /// <summary>
        /// Creates a reaction input requirement.
        /// </summary>
        public MaterialRequirement(string tag, int quantity, bool consumed)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Material requirement tag cannot be empty.", nameof(tag));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Material requirement quantity must be positive.");

            Tag = tag.Trim();
            Quantity = quantity;
            Consumed = consumed;
        }

        /// <summary>
        /// Returns a compact debug label for this requirement.
        /// </summary>
        public override string ToString()
        {
            var mode = Consumed ? "consumed" : "checked";
            return $"MaterialRequirement({Tag} x{Quantity} {mode})";
        }
    }
}