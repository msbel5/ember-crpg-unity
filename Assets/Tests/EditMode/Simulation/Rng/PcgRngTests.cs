using System;
using EmberCrpg.Simulation.Rng;
using NUnit.Framework;

// Design note:
// These tests pin PcgRng as Ember's concrete deterministic RNG implementation.
// They verify reproducibility, stream separation, range bounds, chance semantics, and fork behavior.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Rng
{
    /// <summary>
    /// Verifies Ember's PCG-backed deterministic RNG implementation.
    /// </summary>
    public sealed class PcgRngTests
    {
        [Test]
        public void SameSeedAndStream_ProducesSameSequence()
        {
            var left = new PcgRng(12345UL, 0UL);
            var right = new PcgRng(12345UL, 0UL);

            Assert.That(left.NextUInt32(), Is.EqualTo(right.NextUInt32()));
            Assert.That(left.NextUInt32(), Is.EqualTo(right.NextUInt32()));
            Assert.That(left.NextUInt32(), Is.EqualTo(right.NextUInt32()));
        }

        [Test]
        public void KnownSeed_ProducesPinnedFirstValues()
        {
            var rng = new PcgRng(12345UL, 0UL);

            Assert.That(rng.NextUInt32(), Is.EqualTo(2765504305U));
            Assert.That(rng.NextUInt32(), Is.EqualTo(953180831U));
            Assert.That(rng.NextUInt32(), Is.EqualTo(1208723287U));
        }

        [Test]
        public void DifferentStream_ProducesDifferentSequence()
        {
            var first = new PcgRng(12345UL, 0UL);
            var second = new PcgRng(12345UL, 99UL);

            Assert.That(first.NextUInt32(), Is.Not.EqualTo(second.NextUInt32()));
        }

        [Test]
        public void Range_ReturnsValueInsideInclusiveExclusiveBounds()
        {
            var rng = new PcgRng(7UL, 1UL);

            for (var i = 0; i < 32; i++)
            {
                var value = rng.Range(10, 20);

                Assert.That(value, Is.GreaterThanOrEqualTo(10));
                Assert.That(value, Is.LessThan(20));
            }
        }

        [Test]
        public void Range_InvalidBounds_ThrowsArgumentOutOfRange()
        {
            var rng = new PcgRng(7UL, 1UL);

            Assert.Throws<ArgumentOutOfRangeException>(() => rng.Range(10, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.Range(20, 10));
        }

        [Test]
        public void Chance_ZeroAndHundred_AreDeterministicBoundaries()
        {
            var rng = new PcgRng(7UL, 1UL);

            Assert.That(rng.Chance(0), Is.False);
            Assert.That(rng.Chance(100), Is.True);
        }

        [Test]
        public void Chance_InvalidPercent_ThrowsArgumentOutOfRange()
        {
            var rng = new PcgRng(7UL, 1UL);

            Assert.Throws<ArgumentOutOfRangeException>(() => rng.Chance(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.Chance(101));
        }

        [Test]
        public void Fork_SameStreamFromSameState_ProducesSameSubSequence()
        {
            var left = new PcgRng(12345UL, 0UL);
            var right = new PcgRng(12345UL, 0UL);

            var leftFork = left.Fork(55UL);
            var rightFork = right.Fork(55UL);

            Assert.That(leftFork.NextUInt32(), Is.EqualTo(rightFork.NextUInt32()));
            Assert.That(leftFork.NextUInt32(), Is.EqualTo(rightFork.NextUInt32()));
        }
    }
}