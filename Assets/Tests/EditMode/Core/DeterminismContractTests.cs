using EmberCrpg.Domain.Core;
using NUnit.Framework;

namespace EmberCrpg.Tests.EditMode.Core
{
    public sealed class DeterminismContractTests
    {
        [Test]
        public void SameSeedValuesAreEqual()
        {
            var left = new DeterministicSeed(777UL);
            var right = new DeterministicSeed(777UL);

            Assert.AreEqual(left, right);
        }

        [Test]
        public void SameTickValuesAreEqual()
        {
            var left = new GameTick(100L);
            var right = new GameTick(100L);

            Assert.AreEqual(left, right);
        }
    }
}