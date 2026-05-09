using System;
using EmberCrpg.Domain.Core;
using NUnit.Framework;

// Design note:
// These tests pin GameTick as deterministic simulation order, not in-world calendar time.
namespace EmberCrpg.Tests.EditMode.Core
{
    /// <summary>
    /// Verifies Ember's deterministic simulation tick primitive.
    /// </summary>
    public sealed class GameTickTests
    {
        /// <summary>
        /// A constructed tick exposes the supplied raw simulation tick value.
        /// </summary>
        [Test]
        public void Constructor_StoresRawValue()
        {
            var tick = new GameTick(12L);

            Assert.That(tick.Value, Is.EqualTo(12L));
        }

        /// <summary>
        /// Negative simulation ticks are rejected.
        /// </summary>
        [Test]
        public void Constructor_NegativeValue_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GameTick(-1L));
        }

        /// <summary>
        /// The default tick is tick zero.
        /// </summary>
        [Test]
        public void DefaultTick_IsZero()
        {
            var tick = default(GameTick);

            Assert.That(tick.Value, Is.EqualTo(0L));
        }

        /// <summary>
        /// Add returns a new tick advanced by simulation ticks.
        /// </summary>
        [Test]
        public void Add_ReturnsAdvancedTick()
        {
            var tick = new GameTick(10L);

            var advanced = tick.Add(5L);

            Assert.That(advanced.Value, Is.EqualTo(15L));
        }

        /// <summary>
        /// Add rejects movement before tick zero through the constructor invariant.
        /// </summary>
        [Test]
        public void Add_BeforeZero_ThrowsArgumentOutOfRange()
        {
            var tick = new GameTick(3L);

            Assert.Throws<ArgumentOutOfRangeException>(() => tick.Add(-4L));
        }

        /// <summary>
        /// Equal raw values are equal ticks.
        /// </summary>
        [Test]
        public void EqualValues_AreEqual()
        {
            var left = new GameTick(3L);
            var right = new GameTick(3L);

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        /// <summary>
        /// Different raw values are different ticks.
        /// </summary>
        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new GameTick(3L);
            var right = new GameTick(4L);

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        /// <summary>
        /// CompareTo orders ticks by raw simulation order.
        /// </summary>
        [Test]
        public void CompareTo_OrdersTicks()
        {
            var early = new GameTick(1L);
            var late = new GameTick(2L);

            Assert.That(early.CompareTo(late), Is.LessThan(0));
        }

        /// <summary>
        /// Relational operators compare raw simulation order.
        /// </summary>
        [Test]
        public void RelationalOperators_CompareRawValues()
        {
            var early = new GameTick(1L);
            var late = new GameTick(2L);

            Assert.That(early < late, Is.True);
            Assert.That(late > early, Is.True);
            Assert.That(early <= early, Is.True);
            Assert.That(late >= early, Is.True);
        }

        /// <summary>
        /// The plus operator advances by simulation ticks.
        /// </summary>
        [Test]
        public void Operator_Plus_AddsTicks()
        {
            var result = new GameTick(10L) + 5L;

            Assert.That(result.Value, Is.EqualTo(15L));
        }

        /// <summary>
        /// The minus operator returns the signed delta in simulation ticks.
        /// </summary>
        [Test]
        public void Operator_Minus_ReturnsDeltaTicks()
        {
            var delta = new GameTick(30L) - new GameTick(10L);

            Assert.That(delta, Is.EqualTo(20L));
        }

        /// <summary>
        /// ToString returns a compact debug label.
        /// </summary>
        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            Assert.That(new GameTick(42L).ToString(), Is.EqualTo("GameTick(42)"));
        }
    }
}