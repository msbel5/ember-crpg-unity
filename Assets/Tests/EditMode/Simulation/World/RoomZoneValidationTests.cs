using System;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.World;
using NUnit.Framework;

// Design note:
// These tests pin room/zone validation as a pure rule evaluator.
// They do not test actor assignment, morale, need fulfillment, job execution, reaction execution, or room quality scoring.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.World
{
    /// <summary>
    /// Verifies validation of room/zone rules against activity sites.
    /// </summary>
    public sealed class RoomZoneValidationTests
    {
        [Test]
        public void Validate_BedroomWithHomeBed_IsValid()
        {
            var rule = new RoomZoneRule(
                "bedroom",
                new[] { new ActivitySiteRole("home"), new ActivitySiteRole("rest") },
                new[] { "bed" },
                Array.Empty<string>(),
                1);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.owned_bed.001"),
                    new RoomId("room.bedroom.001"),
                    "bed",
                    new[] { new ActivitySiteRole("home"), new ActivitySiteRole("rest") },
                    2,
                    new[] { "sleep" })
            };

            var result = RoomZoneValidation.Validate(rule, sites);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.MissingRequirements.Count, Is.EqualTo(0));
        }

        [Test]
        public void Validate_DiningMissingChair_IsInvalid()
        {
            var rule = new RoomZoneRule(
                "dining",
                Array.Empty<ActivitySiteRole>(),
                new[] { "table", "chair" },
                Array.Empty<string>(),
                0);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.table.001"),
                    new RoomId("room.dining.001"),
                    "table",
                    new[] { new ActivitySiteRole("social") },
                    0,
                    Array.Empty<string>())
            };

            var result = RoomZoneValidation.Validate(rule, sites);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.MissingRequirements.Count, Is.EqualTo(1));
            Assert.That(result.MissingRequirements[0], Is.EqualTo("tag:chair"));
        }

        [Test]
        public void Validate_WorkshopWithWorkRole_IsValid()
        {
            var rule = new RoomZoneRule(
                "workshop",
                new[] { new ActivitySiteRole("work") },
                Array.Empty<string>(),
                Array.Empty<string>(),
                0);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.iron_forge.001"),
                    new RoomId("room.workshop.001"),
                    "iron_forge",
                    new[] { new ActivitySiteRole("work"), new ActivitySiteRole("craft") },
                    2,
                    new[] { "forge", "heat" })
            };

            var result = RoomZoneValidation.Validate(rule, sites);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Validate_CustomBridgeWithCommandRole_IsValid()
        {
            var rule = new RoomZoneRule(
                "bridge",
                new[] { new ActivitySiteRole("command") },
                Array.Empty<string>(),
                Array.Empty<string>(),
                0);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.nav_console.001"),
                    new RoomId("room.bridge.001"),
                    "nav_console",
                    new[] { new ActivitySiteRole("command") },
                    1,
                    new[] { "console", "ship" })
            };

            var result = RoomZoneValidation.Validate(rule, sites);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Validate_EmptySites_ReturnsMissingRoleAndTag()
        {
            var rule = new RoomZoneRule(
                "hospital",
                new[] { new ActivitySiteRole("medical") },
                new[] { "bed" },
                Array.Empty<string>(),
                0);

            var result = RoomZoneValidation.Validate(rule, Array.Empty<ActivitySiteRecord>());

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.MissingRequirements.Count, Is.EqualTo(2));
            Assert.That(result.MissingRequirements[0], Is.EqualTo("role:medical"));
            Assert.That(result.MissingRequirements[1], Is.EqualTo("tag:bed"));
        }

        [Test]
        public void Validate_MinimumQualityNotMet_ReturnsMissingQuality()
        {
            var rule = new RoomZoneRule(
                "temple",
                new[] { new ActivitySiteRole("ritual") },
                new[] { "altar" },
                Array.Empty<string>(),
                3);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.blood_altar.001"),
                    new RoomId("room.temple.001"),
                    "altar",
                    new[] { new ActivitySiteRole("ritual") },
                    2,
                    new[] { "blood" })
            };

            var result = RoomZoneValidation.Validate(rule, sites);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.MissingRequirements.Count, Is.EqualTo(1));
            Assert.That(result.MissingRequirements[0], Is.EqualTo("quality>=3"));
        }

        [Test]
        public void Validate_NullSites_TreatsAsEmpty()
        {
            var rule = new RoomZoneRule(
                "market",
                new[] { new ActivitySiteRole("trade") },
                Array.Empty<string>(),
                Array.Empty<string>(),
                0);

            var result = RoomZoneValidation.Validate(rule, null);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.MissingRequirements[0], Is.EqualTo("role:trade"));
        }

        [Test]
        public void Validate_NullRule_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => RoomZoneValidation.Validate(null, Array.Empty<ActivitySiteRecord>()));
        }

        [Test]
        public void ValidateForRoom_FiltersSitesByRoomId()
        {
            var rule = new RoomZoneRule(
                "bedroom",
                Array.Empty<ActivitySiteRole>(),
                new[] { "bed" },
                Array.Empty<string>(),
                0);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.bed.001"),
                    new RoomId("room.bedroom.001"),
                    "bed",
                    new[] { new ActivitySiteRole("rest") },
                    1,
                    Array.Empty<string>()),

                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.table.001"),
                    new RoomId("room.dining.001"),
                    "table",
                    new[] { new ActivitySiteRole("social") },
                    1,
                    Array.Empty<string>())
            };

            var result = RoomZoneValidation.ValidateForRoom(rule, new RoomId("room.dining.001"), sites);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.MissingRequirements[0], Is.EqualTo("tag:bed"));
        }

        [Test]
        public void Validate_OptionalTagsDoNotBlockValidation()
        {
            var rule = new RoomZoneRule(
                "bedroom",
                new[] { new ActivitySiteRole("rest") },
                new[] { "bed" },
                new[] { "dresser", "art" },
                0);

            var sites = new[]
            {
                new ActivitySiteRecord(
                    new ActivitySiteId("activity_site.bed.001"),
                    new RoomId("room.bedroom.001"),
                    "bed",
                    new[] { new ActivitySiteRole("rest") },
                    1,
                    Array.Empty<string>())
            };

            var result = RoomZoneValidation.Validate(rule, sites);

            Assert.That(result.IsValid, Is.True);
        }
    }
}