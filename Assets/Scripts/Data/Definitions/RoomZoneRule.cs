using System;
using System.Collections.Generic;
using EmberCrpg.Domain.World;

// Design note:
// RoomZoneRule is a data-driven requirement row for local room and zone functions.
// It generalizes furniture requirements into activity-site roles and tags for fantasy, sci-fi, colony, ritual, home, and medical spaces.
namespace EmberCrpg.Data.Definitions
{
    /// <summary>
    /// Data definition describing what a room or zone needs to fulfill a function.
    /// </summary>
    public sealed class RoomZoneRule
    {
        /// <summary>
        /// Stable zone type such as bedroom, dining, workshop, hospital, temple, bridge, market, or crypt.
        /// </summary>
        public readonly string ZoneType;

        /// <summary>
        /// Required activity site roles that must be present in the room.
        /// </summary>
        public readonly IReadOnlyList<ActivitySiteRole> RequiredSiteRoles;

        /// <summary>
        /// Required activity site tags that must be present in the room.
        /// </summary>
        public readonly IReadOnlyList<string> RequiredSiteTags;

        /// <summary>
        /// Optional activity site tags that may improve future quality scoring.
        /// </summary>
        public readonly IReadOnlyList<string> OptionalSiteTags;

        /// <summary>
        /// Minimum room/activity-site quality required by this zone rule.
        /// </summary>
        public readonly int MinimumQuality;

        /// <summary>
        /// Creates a data-driven room/zone rule.
        /// </summary>
        public RoomZoneRule(
            string zoneType,
            IReadOnlyList<ActivitySiteRole> requiredSiteRoles,
            IReadOnlyList<string> requiredSiteTags,
            IReadOnlyList<string> optionalSiteTags,
            int minimumQuality)
        {
            if (string.IsNullOrWhiteSpace(zoneType))
                throw new ArgumentException("Room zone type cannot be empty.", nameof(zoneType));
            if (minimumQuality < 0)
                throw new ArgumentOutOfRangeException(nameof(minimumQuality), "Minimum quality cannot be negative.");

            var roles = CopyDistinctRoles(requiredSiteRoles);
            var tags = CopyDistinctTags(requiredSiteTags);

            if (roles.Count == 0 && tags.Count == 0)
                throw new ArgumentException("Room zone rule must require at least one role or tag.");

            ZoneType = zoneType.Trim();
            RequiredSiteRoles = roles;
            RequiredSiteTags = tags;
            OptionalSiteTags = CopyDistinctTags(optionalSiteTags);
            MinimumQuality = minimumQuality;
        }

        /// <summary>
        /// Returns true when this rule requires the supplied activity site role.
        /// </summary>
        public bool RequiresRole(ActivitySiteRole role)
        {
            for (var i = 0; i < RequiredSiteRoles.Count; i++)
            {
                if (RequiredSiteRoles[i] == role)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true when this rule requires the supplied activity site tag.
        /// </summary>
        public bool RequiresTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            var normalized = tag.Trim();
            for (var i = 0; i < RequiredSiteTags.Count; i++)
            {
                if (string.Equals(RequiredSiteTags[i], normalized, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static IReadOnlyList<ActivitySiteRole> CopyDistinctRoles(IReadOnlyList<ActivitySiteRole> roles)
        {
            if (roles == null)
                return Array.Empty<ActivitySiteRole>();

            var copy = new List<ActivitySiteRole>();
            for (var i = 0; i < roles.Count; i++)
            {
                if (roles[i].IsEmpty || ContainsRole(copy, roles[i]))
                    continue;

                copy.Add(roles[i]);
            }

            return copy.AsReadOnly();
        }

        private static IReadOnlyList<string> CopyDistinctTags(IReadOnlyList<string> tags)
        {
            if (tags == null)
                return Array.Empty<string>();

            var copy = new List<string>();
            for (var i = 0; i < tags.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(tags[i]))
                    continue;

                var normalized = tags[i].Trim();
                if (!ContainsTag(copy, normalized))
                    copy.Add(normalized);
            }

            return copy.AsReadOnly();
        }

        private static bool ContainsRole(IReadOnlyList<ActivitySiteRole> roles, ActivitySiteRole role)
        {
            for (var i = 0; i < roles.Count; i++)
            {
                if (roles[i] == role)
                    return true;
            }

            return false;
        }

        private static bool ContainsTag(IReadOnlyList<string> tags, string tag)
        {
            for (var i = 0; i < tags.Count; i++)
            {
                if (string.Equals(tags[i], tag, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }
    }
}