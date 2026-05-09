using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin RoomZoneRule as a data-driven room/zone requirement row.
// They do not test room validation, activity site lookup, actor assignment, morale, needs, jobs, or reactions.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Data.Definitions
{
    /// <summary>
    /// Verifies data-driven room/zone rule behavior.
    /// </summary>
    public sealed class RoomZoneRuleTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var rule = new RoomZoneRule(
                "bedroom",
                new[] { new ActivitySiteRole("home"), new ActivitySiteRole("rest") },
                new[] { "bed" },
                new[] { "dresser" },
                2);

            Assert.That(rule.ZoneType, Is.EqualTo("bedroom"));
            Assert.That(rule.RequiredSiteRoles.Count, Is.EqualTo(2));
            Assert.That(rule.RequiredSiteTags.Count, Is.EqualTo(1));
            Assert.That(rule.OptionalSiteTags.Count, Is.EqualTo(1));
            Assert.That(rule.MinimumQuality, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_AllowsRoleOnlyRule()
        {
            var rule = new RoomZoneRule(
                "bridge",
                new[] { new ActivitySiteRole("command") },
                Array.Empty<string>(),
                Array.Empty<string>(),
                0);

            Assert.That(rule.RequiresRole(new ActivitySiteRole("command")), Is.True);
            Assert.That(rule.RequiredSiteTags.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_AllowsTagOnlyRule()
        {
            var rule = new RoomZoneRule(
                "dining",
                Array.Empty<ActivitySiteRole>(),
                new[] { "table", "chair" },
                Array.Empty<string>(),
                0);

            Assert.That(rule.RequiresTag("table"), Is.True);
            Assert.That(rule.RequiresTag("chair"), Is.True);
            Assert.That(rule.RequiredSiteRoles.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_TrimsZoneAndTags()
        {
            var rule = new RoomZoneRule(
                "  temple  ",
                Array.Empty<ActivitySiteRole>(),
                new[] { "  altar  " },
                new[] { "  candles  " },
                0);

            Assert.That(rule.ZoneType, Is.EqualTo("temple"));
            Assert.That(rule.RequiredSiteTags[0], Is.EqualTo("altar"));
            Assert.That(rule.OptionalSiteTags[0], Is.EqualTo("candles"));
        }

        [Test]
        public void Constructor_EmptyZoneType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new RoomZoneRule(
                "",
                new[] { new ActivitySiteRole("home") },
                Array.Empty<string>(),
                Array.Empty<string>(),
                0));
        }

        [Test]
        public void Constructor_NoRequiredRolesOrTags_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new RoomZoneRule(
                "empty_zone",
                Array.Empty<ActivitySiteRole>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                0));
        }

        [Test]
        public void Constructor_NegativeMinimumQuality_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RoomZoneRule(
                "bedroom",
                new[] { new ActivitySiteRole("home") },
                Array.Empty<string>(),
                Array.Empty<string>(),
                -1));
        }

        [Test]
        public void Constructor_DeduplicatesRequiredRolesAndTags()
        {
            var rule = new RoomZoneRule(
                "workshop",
                new[] { new ActivitySiteRole("work"), new ActivitySiteRole("work") },
                new[] { "forge", "forge" },
                Array.Empty<string>(),
                0);

            Assert.That(rule.RequiredSiteRoles.Count, Is.EqualTo(1));
            Assert.That(rule.RequiredSiteTags.Count, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_DropsEmptyOptionalTags()
        {
            var rule = new RoomZoneRule(
                "market",
                new[] { new ActivitySiteRole("trade") },
                Array.Empty<string>(),
                new[] { "", "stall", "   " },
                0);

            Assert.That(rule.OptionalSiteTags.Count, Is.EqualTo(1));
            Assert.That(rule.OptionalSiteTags[0], Is.EqualTo("stall"));
        }

        [Test]
        public void RequiresRole_MissingRole_ReturnsFalse()
        {
            var rule = new RoomZoneRule(
                "hospital",
                new[] { new ActivitySiteRole("medical") },
                new[] { "bed" },
                Array.Empty<string>(),
                0);

            Assert.That(rule.RequiresRole(new ActivitySiteRole("trade")), Is.False);
        }

        [Test]
        public void RequiresTag_MissingTag_ReturnsFalse()
        {
            var rule = new RoomZoneRule(
                "temple",
                Array.Empty<ActivitySiteRole>(),
                new[] { "altar" },
                Array.Empty<string>(),
                0);

            Assert.That(rule.RequiresTag("console"), Is.False);
        }
    }
}