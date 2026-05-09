using EmberCrpg.Simulation.Rng;
using NUnit.Framework;

// Design note:
// These tests pin IRng as Ember's deterministic randomness contract.
// They use a tiny test-only implementation so the interface can be verified before PcgRng exists.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Rng
{
    /// <summary>
    /// Verifies the deterministic RNG interface shape and expected semantics.
    /// </summary>
    public sealed class IRngContractTests
    {
        [Test]
        public void NextUInt32_ReturnsDeterministicValueFromImplementation()
        {
            IRng rng = new FixedRng(42U);

            Assert.That(rng.NextUInt32(), Is.EqualTo(42U));
        }

        [Test]
        public void Range_ReturnsValueInsideInclusiveExclusiveBounds()
        {
            IRng rng = new FixedRng(7U);

            var value = rng.Range(10, 20);

            Assert.That(value, Is.GreaterThanOrEqualTo(10));
            Assert.That(value, Is.LessThan(20));
        }

        [Test]
        public void Chance_ReturnsTrueWhenRollIsBelowPercent()
        {
            IRng rng = new FixedRng(24U);

            Assert.That(rng.Chance(25), Is.True);
        }

        [Test]
        public void Chance_ReturnsFalseWhenRollIsNotBelowPercent()
        {
            IRng rng = new FixedRng(25U);

            Assert.That(rng.Chance(25), Is.False);
        }

        [Test]
        public void Fork_ReturnsIndependentStream()
        {
            IRng rng = new FixedRng(10U);

            var forked = rng.Fork(5UL);

            Assert.That(forked.NextUInt32(), Is.EqualTo(15U));
        }

        private sealed class FixedRng : IRng
        {
            private readonly uint _value;

            public FixedRng(uint value)
            {
                _value = value;
            }

            public uint NextUInt32()
            {
                return _value;
            }

            public int Range(int minInclusive, int maxExclusive)
            {
                return minInclusive + (int)(_value % (uint)(maxExclusive - minInclusive));
            }

            public bool Chance(int percent)
            {
                return (_value % 100U) < percent;
            }

            public IRng Fork(ulong streamId)
            {
                return new FixedRng(_value + (uint)streamId);
            }
        }
    }
}