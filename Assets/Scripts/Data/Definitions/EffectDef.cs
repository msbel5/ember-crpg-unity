using System;

// Design note:
// EffectDef is a data-driven definition for buffs, debuffs, syndromes, and
// timed alterations applied to actors or world entities. It is data only;
// runtime behaviour is the job of EffectsSystem reading these rows.
//
// Status: stub (2026-05-11). Fields will land once Faz 8 (data-driven magic)
// scaffold from the ChatGPT big-picture pass is approved. For now this file
// exists so the namespace + assembly compile cleanly.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// Data definition stub for a runtime effect (buff, debuff, syndrome, timed alteration).
    /// </summary>
    public sealed class EffectDef
    {
        // TODO(faz-8): EffectId Id, string Label, EffectKind Kind,
        // IReadOnlyList<EffectOperation> Operations, magnitude formula,
        // duration model, target rule, save/resist rule.
    }
}