using System;
using System.Collections.Generic;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.World;

// Design note:
// RoomZoneValidation evaluates data-driven room/zone rules against local activity sites.
// It generalizes furniture checks into activity-site roles, tags, and quality for multiverse spaces.
namespace EmberCrpg.Simulation.World
{
    /// <summary>
    /// Result of validating a room or zone against a rule.
    /// </summary>
    public sealed class RoomZoneValidationResult
    {
        /// <summary>
        /// True when all required roles, tags, and quality constraints are satisfied.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// Deterministic list of missing requirements.
        /// </summary>
        public readonly IReadOnlyList<string> MissingRequirements;

        /// <summary>
        /// Creates a room/zone validation result.
        /// </summary>
        public RoomZoneValidationResult(IReadOnlyList<string> missingRequirements)
        {
            MissingRequirements = missingRequirements ?? Array.Empty<string>();
            IsValid = MissingRequirements.Count == 0;
        }
    }

    /// <summary>
    /// Pure room/zone validation helper.
    /// </summary>
    public static class RoomZoneValidation
    {
        /// <summary>
        /// Validates a rule against a set of activity sites already scoped to a room or zone.
        /// </summary>
        public static RoomZoneValidationResult Validate(
            RoomZoneRule rule,
            IReadOnlyList<ActivitySiteRecord> activitySites)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            var sites = activitySites ?? Array.Empty<ActivitySiteRecord>();
            var missing = new List<string>();

            AddMissingRoles(rule, sites, missing);
            AddMissingTags(rule, sites, missing);
            AddMissingQuality(rule, sites, missing);

            return new RoomZoneValidationResult(missing.AsReadOnly());
        }

        /// <summary>
        /// Filters activity sites by room id before validating a rule.
        /// </summary>
        public static RoomZoneValidationResult ValidateForRoom(
            RoomZoneRule rule,
            RoomId roomId,
            IReadOnlyList<ActivitySiteRecord> allActivitySites)
        {
            var allSites = allActivitySites ?? Array.Empty<ActivitySiteRecord>();
            var scoped = new List<ActivitySiteRecord>();

            for (var i = 0; i < allSites.Count; i++)
            {
                if (allSites[i] != null && allSites[i].RoomId == roomId)
                    scoped.Add(allSites[i]);
            }

            return Validate(rule, scoped);
        }

        private static void AddMissingRoles(
            RoomZoneRule rule,
            IReadOnlyList<ActivitySiteRecord> sites,
            List<string> missing)
        {
            for (var i = 0; i < rule.RequiredSiteRoles.Count; i++)
            {
                var role = rule.RequiredSiteRoles[i];
                if (!HasRole(sites, role))
                    missing.Add("role:" + role.Value);
            }
        }

        private static void AddMissingTags(
            RoomZoneRule rule,
            IReadOnlyList<ActivitySiteRecord> sites,
            List<string> missing)
        {
            for (var i = 0; i < rule.RequiredSiteTags.Count; i++)
            {
                var tag = rule.RequiredSiteTags[i];
                if (!HasTagOrKind(sites, tag))
                    missing.Add("tag:" + tag);
            }
        }

        private static void AddMissingQuality(
            RoomZoneRule rule,
            IReadOnlyList<ActivitySiteRecord> sites,
            List<string> missing)
        {
            if (rule.MinimumQuality <= 0)
                return;

            for (var i = 0; i < sites.Count; i++)
            {
                if (sites[i] != null && sites[i].Quality >= rule.MinimumQuality)
                    return;
            }

            missing.Add("quality>=" + rule.MinimumQuality);
        }

        private static bool HasRole(IReadOnlyList<ActivitySiteRecord> sites, ActivitySiteRole role)
        {
            for (var i = 0; i < sites.Count; i++)
            {
                if (sites[i] != null && sites[i].HasRole(role))
                    return true;
            }

            return false;
        }

        private static bool HasTagOrKind(IReadOnlyList<ActivitySiteRecord> sites, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            var normalized = tag.Trim();

            for (var i = 0; i < sites.Count; i++)
            {
                var site = sites[i];
                if (site == null)
                    continue;

                if (string.Equals(site.Kind, normalized, StringComparison.Ordinal))
                    return true;

                for (var j = 0; j < site.Tags.Count; j++)
                {
                    if (string.Equals(site.Tags[j], normalized, StringComparison.Ordinal))
                        return true;
                }
            }

            return false;
        }
    }
}