using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin ActivitySiteRole as a data-driven function tag for activity sites.
// They do not test activity site records, rooms, jobs, reactions, bonuses, or actor assignment.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.World
{
    /// <summary>
    /// Verifies activity site role identity behavior.
    /// </summary>
    public sealed class ActivitySiteRoleTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var role = new ActivitySiteRole("work");

            Assert.That(role.Value, Is.EqualTo("work"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var role = default(ActivitySiteRole);

            Assert.That(role.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var role = new ActivitySiteRole("");

            Assert.That(role.IsEmpty, Is.True);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new ActivitySiteRole("ritual");
            var right = new ActivitySiteRole("ritual");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new ActivitySiteRole("work");
            var right = new ActivitySiteRole("home");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void RoleVocabulary_IsDataDriven()
        {
            var shipConsole = new ActivitySiteRole("command");
            var bloodAltar = new ActivitySiteRole("ritual");
            var marketStall = new ActivitySiteRole("trade");

            Assert.That(shipConsole.IsEmpty, Is.False);
            Assert.That(bloodAltar.IsEmpty, Is.False);
            Assert.That(marketStall.IsEmpty, Is.False);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var role = new ActivitySiteRole("medical");

            Assert.That(role.ToString(), Is.EqualTo("ActivitySiteRole(medical)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var role = default(ActivitySiteRole);

            Assert.That(role.ToString(), Is.EqualTo("ActivitySiteRole.Empty"));
        }
    }
}