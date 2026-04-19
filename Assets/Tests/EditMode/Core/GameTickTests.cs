using EmberCrpg.Domain.Core;
using NUnit.Framework;

namespace EmberCrpg.Tests.EditMode.Core
{
    public sealed class GameTickTests
    {
        [Test]
        public void ConstructorStoresRawValue()
        {
            var tick = new GameTick(12L);

            Assert.AreEqual(12L, tick.Value);
        }

        [Test]
        public void DefaultTickIsZero()
        {
            var tick = default(GameTick);

            Assert.AreEqual(0L, tick.Value);
        }

        [Test]
        public void AddReturnsAdvancedTick()
        {
            var tick = new GameTick(10L);

            var advanced = tick.Add(5L);

            Assert.AreEqual(15L, advanced.Value);
        }

        [Test]
        public void EqualValuesAreEqual()
        {
            var left = new GameTick(3L);
            var right = new GameTick(3L);

            Assert.AreEqual(left, right);
            Assert.IsTrue(left == right);
        }

        [Test]
        public void CompareToOrdersTicks()
        {
            var early = new GameTick(1L);
            var late = new GameTick(2L);

            Assert.Less(early.CompareTo(late), 0);
        }
    }
}