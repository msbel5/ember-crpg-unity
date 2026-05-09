using System;
using System.Collections.Generic;
using EmberCrpg.Domain.Skills;
using StatAttribute = EmberCrpg.Domain.Stats.Attribute;

// Design note:
// SkillDef is a data-driven skill definition row.
// It lets each universe define skill catalogs without hardcoding fantasy, sci-fi, or vampire skills in Domain.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// Data definition for a skill available in a universe or adapter pack.
    /// </summary>
    public sealed class SkillDef
    {
        /// <summary>
        /// Stable skill definition id.
        /// </summary>
        public readonly SkillId Id;

        /// <summary>
        /// Player-facing skill name.
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// Broad grouping such as combat, magic, field, social, science, or occult.
        /// </summary>
        public readonly string Category;

        /// <summary>
        /// Core Ember attribute most associated with this skill.
        /// </summary>
        public readonly StatAttribute GoverningAttribute;

        /// <summary>
        /// Data tags used by jobs, character creation, UI filters, and adapter packs.
        /// </summary>
        public readonly IReadOnlyList<string> Tags;

        /// <summary>
        /// Creates a data-driven skill definition.
        /// </summary>
        public SkillDef(
            SkillId id,
            string displayName,
            string category,
            StatAttribute governingAttribute,
            IReadOnlyList<string> tags)
        {
            if (id.IsEmpty)
                throw new ArgumentException("SkillDef id cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("SkillDef display name cannot be empty.", nameof(displayName));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("SkillDef category cannot be empty.", nameof(category));

            Id = id;
            DisplayName = displayName;
            Category = category;
            GoverningAttribute = governingAttribute;
            Tags = tags ?? Array.Empty<string>();
        }
    }
}