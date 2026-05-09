using System;
using System.Collections.Generic;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;

// Design note:
// ReactionDef is a data-driven process definition.
// It generalizes crafting, cooking, medicine, ritual, research, repair, command, and sci-fi production workflows.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// Data definition for a reaction or process that may be executed by jobs.
    /// </summary>
    public sealed class ReactionDef
    {
        /// <summary>
        /// Stable reaction definition id.
        /// </summary>
        public readonly ReactionId Id;

        /// <summary>
        /// Player-facing reaction label.
        /// </summary>
        public readonly string Label;

        /// <summary>
        /// Required activity site kind, such as iron_forge, campfire, lab_bench, blood_altar, or nav_console.
        /// </summary>
        public readonly string ActivitySiteKind;

        /// <summary>
        /// Required activity site role, such as work, ritual, medical, trade, command, rest, or research.
        /// </summary>
        public readonly ActivitySiteRole RequiredActivitySiteRole;

        /// <summary>
        /// Skill required or exercised by this reaction. Empty means no skill is required.
        /// </summary>
        public readonly SkillId RequiredSkillId;

        /// <summary>
        /// Tagged input requirements checked or consumed by this reaction.
        /// </summary>
        public readonly IReadOnlyList<MaterialRequirement> InputMaterials;

        /// <summary>
        /// Products produced by this reaction.
        /// </summary>
        public readonly IReadOnlyList<ProductOutput> OutputProducts;

        /// <summary>
        /// Base duration in deterministic simulation ticks.
        /// </summary>
        public readonly int BaseDurationTicks;

        /// <summary>
        /// Selector for output quality behavior.
        /// </summary>
        public readonly ReactionQualityFormula QualityFormula;

        /// <summary>
        /// Data tags used by registries, UI, scheduling, colony pressure, and adapter packs.
        /// </summary>
        public readonly IReadOnlyList<string> Tags;

        /// <summary>
        /// Creates a data-driven reaction definition.
        /// </summary>
        public ReactionDef(
            ReactionId id,
            string label,
            string activitySiteKind,
            ActivitySiteRole requiredActivitySiteRole,
            SkillId requiredSkillId,
            IReadOnlyList<MaterialRequirement> inputMaterials,
            IReadOnlyList<ProductOutput> outputProducts,
            int baseDurationTicks,
            ReactionQualityFormula qualityFormula,
            IReadOnlyList<string> tags)
        {
            if (id.IsEmpty)
                throw new ArgumentException("ReactionDef id cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("ReactionDef label cannot be empty.", nameof(label));
            if (string.IsNullOrWhiteSpace(activitySiteKind))
                throw new ArgumentException("ReactionDef activity site kind cannot be empty.", nameof(activitySiteKind));
            if (requiredActivitySiteRole.IsEmpty)
                throw new ArgumentException("ReactionDef required activity site role cannot be empty.", nameof(requiredActivitySiteRole));
            if (baseDurationTicks <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseDurationTicks), "ReactionDef base duration ticks must be positive.");

            Id = id;
            Label = label.Trim();
            ActivitySiteKind = activitySiteKind.Trim();
            RequiredActivitySiteRole = requiredActivitySiteRole;
            RequiredSkillId = requiredSkillId;
            InputMaterials = CopyInputs(inputMaterials);
            OutputProducts = CopyOutputs(outputProducts);
            BaseDurationTicks = baseDurationTicks;
            QualityFormula = qualityFormula.IsEmpty ? ReactionQualityFormula.WeightedRandom : qualityFormula;
            Tags = CopyTags(tags);
        }

        /// <summary>
        /// Returns true when this reaction requires an input with the supplied tag.
        /// </summary>
        public bool RequiresInputTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            var normalized = tag.Trim();
            for (var i = 0; i < InputMaterials.Count; i++)
            {
                if (string.Equals(InputMaterials[i].Tag, normalized, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static IReadOnlyList<MaterialRequirement> CopyInputs(IReadOnlyList<MaterialRequirement> inputMaterials)
        {
            if (inputMaterials == null)
                return Array.Empty<MaterialRequirement>();

            var copy = new List<MaterialRequirement>(inputMaterials.Count);
            for (var i = 0; i < inputMaterials.Count; i++)
            {
                if (inputMaterials[i] != null)
                    copy.Add(inputMaterials[i]);
            }

            return copy.AsReadOnly();
        }

        private static IReadOnlyList<ProductOutput> CopyOutputs(IReadOnlyList<ProductOutput> outputProducts)
        {
            if (outputProducts == null)
                return Array.Empty<ProductOutput>();

            var copy = new List<ProductOutput>(outputProducts.Count);
            for (var i = 0; i < outputProducts.Count; i++)
            {
                if (outputProducts[i] != null)
                    copy.Add(outputProducts[i]);
            }

            return copy.AsReadOnly();
        }

        private static IReadOnlyList<string> CopyTags(IReadOnlyList<string> tags)
        {
            if (tags == null)
                return Array.Empty<string>();

            var copy = new List<string>(tags.Count);
            for (var i = 0; i < tags.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(tags[i]))
                    copy.Add(tags[i].Trim());
            }

            return copy.AsReadOnly();
        }
    }
}