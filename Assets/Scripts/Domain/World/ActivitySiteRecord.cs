using System;
using System.Collections.Generic;

// Design note:
// ActivitySiteRecord represents a local activity anchor inside an area or room.
// It generalizes worksites, beds, altars, lab benches, consoles, campfires, market stalls, and future multiverse interaction sites.
namespace EmberCrpg.Domain.World
{
    /// <summary>
    /// Runtime/static record for a local activity site.
    /// </summary>
    public sealed class ActivitySiteRecord
    {
        /// <summary>
        /// Stable activity site id.
        /// </summary>
        public readonly ActivitySiteId Id;

        /// <summary>
        /// Room or zone containing this activity site. Empty means area-level or outdoor site.
        /// </summary>
        public readonly RoomId RoomId;

        /// <summary>
        /// Data-driven site kind, such as iron_forge, owned_bed, ritual_circle, lab_bench, or nav_console.
        /// </summary>
        public readonly string Kind;

        /// <summary>
        /// Functional roles this site can satisfy, such as work, home, rest, trade, ritual, medical, or command.
        /// </summary>
        public readonly IReadOnlyList<ActivitySiteRole> Roles;

        /// <summary>
        /// Non-negative quality rating used later by need fulfillment, production speed, morale, or healing modifiers.
        /// </summary>
        public readonly int Quality;

        /// <summary>
        /// Data tags used by jobs, reactions, schedules, UI filters, and adapter packs.
        /// </summary>
        public readonly IReadOnlyList<string> Tags;

        /// <summary>
        /// Creates a local activity site record.
        /// </summary>
        public ActivitySiteRecord(
            ActivitySiteId id,
            RoomId roomId,
            string kind,
            IReadOnlyList<ActivitySiteRole> roles,
            int quality,
            IReadOnlyList<string> tags)
        {
            if (id.IsEmpty)
                throw new ArgumentException("Activity site id cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(kind))
                throw new ArgumentException("Activity site kind cannot be empty.", nameof(kind));
            if (quality < 0)
                throw new ArgumentOutOfRangeException(nameof(quality), "Activity site quality cannot be negative.");

            Id = id;
            RoomId = roomId;
            Kind = kind;
            Roles = CopyAndValidateRoles(roles);
            Quality = quality;
            Tags = CopyTags(tags);
        }

        /// <summary>
        /// Returns true when this site has the requested role.
        /// </summary>
        public bool HasRole(ActivitySiteRole role)
        {
            for (var i = 0; i < Roles.Count; i++)
            {
                if (Roles[i] == role)
                    return true;
            }

            return false;
        }

        private static IReadOnlyList<ActivitySiteRole> CopyAndValidateRoles(IReadOnlyList<ActivitySiteRole> roles)
        {
            if (roles == null || roles.Count == 0)
                throw new ArgumentException("Activity site must have at least one role.", nameof(roles));

            var copy = new List<ActivitySiteRole>(roles.Count);
            for (var i = 0; i < roles.Count; i++)
            {
                if (roles[i].IsEmpty)
                    throw new ArgumentException("Activity site roles cannot contain empty roles.", nameof(roles));

                for (var j = 0; j < copy.Count; j++)
                {
                    if (copy[j] == roles[i])
                        throw new ArgumentException("Activity site roles cannot contain duplicates.", nameof(roles));
                }

                copy.Add(roles[i]);
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
                    copy.Add(tags[i]);
            }

            return copy.AsReadOnly();
        }
    }
}