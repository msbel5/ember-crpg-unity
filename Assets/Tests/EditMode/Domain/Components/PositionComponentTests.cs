using EmberCrpg.Domain.Components;
using NUnit.Framework;

// Design note:
// These tests pin PositionComponent as a deterministic integer coordinate component.
// They do not test pathfinding, room connectivity, movement systems, Unity transforms, or rendering.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Components
{
    /// <summary>
    /// Verifies deterministic local position behavior.
    /// </summary>
    public sealed class PositionComponentTests
    {
        [Test]
        public void Constructor_StoresCoordinates()
        {
            var position = new PositionComponent(3, 4, 5);

            Assert.That(position.X, Is.EqualTo(3));
            Assert.That(position.Y, Is.EqualTo(4));
            Assert.That(position.Z, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_DefaultZ_IsZero()
        {
            var position = new PositionComponent(3, 4);

            Assert.That(position.X, Is.EqualTo(3));
            Assert.That(position.Y, Is.EqualTo(4));
            Assert.That(position.Z, Is.EqualTo(0));
        }

        [Test]
        public void DefaultPosition_IsOrigin()
        {
            var position = default(PositionComponent);

            Assert.That(position.X, Is.EqualTo(0));
            Assert.That(position.Y, Is.EqualTo(0));
            Assert.That(position.Z, Is.EqualTo(0));
            Assert.That(position.IsOrigin, Is.True);
        }

        [Test]
        public void ManhattanDistanceTo_UsesAllAxes()
        {
            var left = new PositionComponent(1, 2, 3);
            var right = new PositionComponent(4, 6, 8);

            Assert.That(left.ManhattanDistanceTo(right), Is.EqualTo(12));
        }

        [Test]
        public void ManhattanDistanceTo_IsSymmetric()
        {
            var left = new PositionComponent(-2, 5, 1);
            var right = new PositionComponent(4, -1, 3);

            Assert.That(left.ManhattanDistanceTo(right), Is.EqualTo(right.ManhattanDistanceTo(left)));
        }

        [Test]
        public void Translate_ReturnsMovedCopy()
        {
            var position = new PositionComponent(1, 2, 3);

            var moved = position.Translate(4, -1, 2);

            Assert.That(moved, Is.EqualTo(new PositionComponent(5, 1, 5)));
            Assert.That(position, Is.EqualTo(new PositionComponent(1, 2, 3)));
        }

        [Test]
        public void SameCoordinates_AreEqual()
        {
            var left = new PositionComponent(1, 2, 3);
            var right = new PositionComponent(1, 2, 3);

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentCoordinates_AreNotEqual()
        {
            var left = new PositionComponent(1, 2, 3);
            var right = new PositionComponent(1, 2, 4);

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var position = new PositionComponent(1, 2, 3);

            Assert.That(position.ToString(), Is.EqualTo("PositionComponent(1,2,3)"));
        }
    }
}