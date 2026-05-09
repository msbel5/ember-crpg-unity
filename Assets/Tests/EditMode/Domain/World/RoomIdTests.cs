using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin RoomId as the stable identity primitive for local rooms and zones.
// They do not test room bounds, room quality, actor assignment, facilities, jobs, or pathfinding.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.World
{
    /// <summary>
    /// Verifies room and zone identity behavior.
    /// </summary>
    public sealed class RoomIdTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var id = new RoomId("room.blacksmith_workshop.001");

            Assert.That(id.Value, Is.EqualTo("room.blacksmith_workshop.001"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var id = default(RoomId);

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var id = new RoomId("");

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new RoomId("room.blacksmith_workshop.001");
            var right = new RoomId("room.blacksmith_workshop.001");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new RoomId("room.blacksmith_workshop.001");
            var right = new RoomId("room.bedroom.001");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var id = new RoomId("room.blacksmith_workshop.001");

            Assert.That(id.ToString(), Is.EqualTo("RoomId(room.blacksmith_workshop.001)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var id = default(RoomId);

            Assert.That(id.ToString(), Is.EqualTo("RoomId.Empty"));
        }
    }
}