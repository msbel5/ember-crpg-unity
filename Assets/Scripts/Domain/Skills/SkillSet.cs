using System;
using System.Collections.Generic;

// Design note:
// SkillSet stores one actor's data-driven skill records.
// It provides deterministic lookup and copy-on-write updates; progression formulas live elsewhere.
namespace EmberCrpg.Domain.Skills
{
    /// <summary>
    /// Actor-local collection of runtime skill records.
    /// </summary>
    public sealed class SkillSet
    {
        private readonly IReadOnlyList<SkillRecord> _records;

        /// <summary>
        /// Empty actor-local skill collection.
        /// </summary>
        public static SkillSet Empty
        {
            get { return new SkillSet(Array.Empty<SkillRecord>()); }
        }

        /// <summary>
        /// Number of skill records in this collection.
        /// </summary>
        public int Count
        {
            get { return _records.Count; }
        }

        /// <summary>
        /// Creates an actor-local skill collection.
        /// </summary>
        public SkillSet(IReadOnlyList<SkillRecord> records)
        {
            var copy = new List<SkillRecord>(records ?? Array.Empty<SkillRecord>());
            EnsureUniqueSkillIds(copy);
            _records = copy.AsReadOnly();
        }

        /// <summary>
        /// Returns true when this actor has a record for the skill.
        /// </summary>
        public bool Contains(SkillId skillId)
        {
            return TryGet(skillId, out _);
        }

        /// <summary>
        /// Attempts to find a skill record by id.
        /// </summary>
        public bool TryGet(SkillId skillId, out SkillRecord record)
        {
            for (var i = 0; i < _records.Count; i++)
            {
                if (_records[i].SkillId == skillId)
                {
                    record = _records[i];
                    return true;
                }
            }

            record = default(SkillRecord);
            return false;
        }

        /// <summary>
        /// Returns the effective level for a skill, or zero when absent.
        /// </summary>
        public int EffectiveLevel(SkillId skillId)
        {
            return TryGet(skillId, out var record) ? record.EffectiveLevel : 0;
        }

        /// <summary>
        /// Returns a copy with the supplied skill record inserted or replaced.
        /// </summary>
        public SkillSet With(SkillRecord record)
        {
            var copy = new List<SkillRecord>(_records);
            for (var i = 0; i < copy.Count; i++)
            {
                if (copy[i].SkillId == record.SkillId)
                {
                    copy[i] = record;
                    return new SkillSet(copy);
                }
            }

            copy.Add(record);
            return new SkillSet(copy);
        }

        private static void EnsureUniqueSkillIds(IReadOnlyList<SkillRecord> records)
        {
            for (var i = 0; i < records.Count; i++)
            {
                for (var j = i + 1; j < records.Count; j++)
                {
                    if (records[i].SkillId == records[j].SkillId)
                        throw new ArgumentException("SkillSet cannot contain duplicate skill ids.", nameof(records));
                }
            }
        }
    }
}