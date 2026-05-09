using System;
using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin ActivitySiteRecord as a local activity anchor.
// They do not test jobs, reactions, need fulfillment, morale, actor assignment, or pathfinding.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.World
{
    /// <summary>
    /// Verifies local activity site record behavior.
    /// </summary>
    public sealed class ActivitySiteRecordTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var record = NewForge();

            Assert.That(record.Id, Is.EqualTo(new ActivitySiteId("activity_site.iron_forge.001")));
            Assert.That(record.RoomId, Is.EqualTo(new RoomId("room.blacksmith_workshop.001")));
            Assert.That(record.Kind, Is.EqualTo("iron_forge"));
            Assert.That(record.Quality, Is.EqualTo(2));
            Assert.That(record.Roles.Count, Is.EqualTo(2));
            Assert.That(record.Tags.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_AllowsEmptyRoomIdForOutdoorOrAreaLevelSite()
        {
            var record = new ActivitySiteRecord(
                new ActivitySiteId("activity_site.campfire.001"),
                default(RoomId),
                "campfire",
                new[] { new ActivitySiteRole("rest"), new ActivitySiteRole("social") },
                1,
                new[] { "outdoor", "fire" });

            Assert.That(record.RoomId.IsEmpty, Is.True);
        }

        [Test]
        public void Constructor_EmptyActivitySiteId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySiteRecord(
                default(ActivitySiteId),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                new[] { new ActivitySiteRole("work") },
                2,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyKind_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "",
                new[] { new ActivitySiteRole("work") },
                2,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_NullRoles_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                null,
                2,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyRoles_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                Array.Empty<ActivitySiteRole>(),
                2,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyRole_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                new[] { default(ActivitySiteRole) },
                2,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_DuplicateRole_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                new[] { new ActivitySiteRole("work"), new ActivitySiteRole("work") },
                2,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_NegativeQuality_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                new[] { new ActivitySiteRole("work") },
                -1,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_NullTags_NormalizesToEmptyList()
        {
            var record = new ActivitySiteRecord(
                new ActivitySiteId("activity_site.owned_bed.001"),
                new RoomId("room.bedroom.001"),
                "owned_bed",
                new[] { new ActivitySiteRole("home"), new ActivitySiteRole("rest") },
                3,
                null);

            Assert.That(record.Tags.Count, Is.EqualTo(0));
        }

        [Test]
        public void HasRole_ExistingRole_ReturnsTrue()
        {
            var record = NewForge();

            Assert.That(record.HasRole(new ActivitySiteRole("work")), Is.True);
        }

        [Test]
        public void HasRole_MissingRole_ReturnsFalse()
        {
            var record = NewForge();

            Assert.That(record.HasRole(new ActivitySiteRole("home")), Is.False);
        }

        private static ActivitySiteRecord NewForge()
        {
            return new ActivitySiteRecord(
                new ActivitySiteId("activity_site.iron_forge.001"),
                new RoomId("room.blacksmith_workshop.001"),
                "iron_forge",
                new[] { new ActivitySiteRole("work"), new ActivitySiteRole("craft") },
                2,
                new[] { "forge", "heat" });
        }
    }
}