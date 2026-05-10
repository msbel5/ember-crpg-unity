using System;
using EmberCrpg.Domain.Components;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Skills;

// Design note:
// LaborCandidate is a minimal actor projection for labor assignment.
// It keeps assignment logic independent from full ActorRecord, biology, needs, AI, inventory, schedule, and rendering.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Minimal actor projection used by labor assignment.
    /// </summary>
    public sealed class LaborCandidate
    {
        /// <summary>
        /// Actor represented by this candidate.
        /// </summary>
        public readonly ActorId ActorId;

        /// <summary>
        /// Deterministic local position used for proximity sorting.
        /// </summary>
        public readonly PositionComponent Position;

        /// <summary>
        /// Actor skill state used for eligibility and tie-breaking.
        /// </summary>
        public readonly SkillSet Skills;

        /// <summary>
        /// True when this actor may be assigned work.
        /// </summary>
        public readonly bool IsAvailable;

        /// <summary>
        /// Creates a labor assignment candidate.
        /// </summary>
        public LaborCandidate(
            ActorId actorId,
            PositionComponent position,
            SkillSet skills,
            bool isAvailable)
        {
            if (actorId.IsEmpty)
                throw new ArgumentException("Labor candidate actor id cannot be empty.", nameof(actorId));

            ActorId = actorId;
            Position = position;
            Skills = skills ?? SkillSet.Empty;
            IsAvailable = isAvailable;
        }

        /// <summary>
        /// Returns true when this candidate can work a job requiring the supplied skill.
        /// Empty skill means no skill requirement.
        /// </summary>
        public bool IsEligibleFor(SkillId requiredSkillId)
        {
            if (!IsAvailable)
                return false;

            if (requiredSkillId.IsEmpty)
                return true;

            return Skills.Contains(requiredSkillId);
        }

        /// <summary>
        /// Returns effective skill level for the supplied skill, or zero when absent.
        /// </summary>
        public int EffectiveSkillFor(SkillId skillId)
        {
            if (skillId.IsEmpty)
                return 0;

            return Skills.EffectiveLevel(skillId);
        }

        /// <summary>
        /// Returns Manhattan distance from this candidate to a target position.
        /// </summary>
        public long DistanceTo(PositionComponent targetPosition)
        {
            return Position.ManhattanDistanceTo(targetPosition);
        }

        /// <summary>
        /// Returns a copy with changed availability.
        /// </summary>
        public LaborCandidate WithAvailability(bool isAvailable)
        {
            return new LaborCandidate(ActorId, Position, Skills, isAvailable);
        }
    }
}