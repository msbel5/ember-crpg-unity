using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin ActivitySiteId as the stable identity primitive for local activity anchors.
// They do not test rooms, roles, quality, jobs, reactions, bonuses, or pathfinding.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.World
{
    /// <summary>
    /// Verifies activity site identity behavior.
    /// </summary>
    public sealed class ActivitySiteIdTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var id = new ActivitySiteId("activity_site.iron_forge.001");

            Assert.That(id.Value, Is.EqualTo("activity_site.iron_forge.001"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var id = default(ActivitySiteId);

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var id = new ActivitySiteId("");

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new ActivitySiteId("activity_site.iron_forge.001");
            var right = new ActivitySiteId("activity_site.iron_forge.001");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new ActivitySiteId("activity_site.iron_forge.001");
            var right = new ActivitySiteId("activity_site.owned_bed.001");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var id = new ActivitySiteId("activity_site.iron_forge.001");

            Assert.That(id.ToString(), Is.EqualTo("ActivitySiteId(activity_site.iron_forge.001)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var id = default(ActivitySiteId);

            Assert.That(id.ToString(), Is.EqualTo("ActivitySiteId.Empty"));
        }
    }
}