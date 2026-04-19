using EmberCrpg.Domain.Core;
using NUnit.Framework;

namespace EmberCrpg.Tests.EditMode.Core
{
    public sealed class ActorIdTests
    {
        [Test]
        public void ConstructorStoresRawValue()
        {
            var id = new ActorId(42UL);

            Assert.AreEqual(42UL, id.Value);
        }

        [Test]
        public void EqualValuesAreEqual()
        {
            var left = new ActorId(7UL);
            var right = new ActorId(7UL);

            Assert.AreEqual(left, right);
            Assert.IsTrue(left == right);
        }

        [Test]
        public void DifferentValuesAreNotEqual()
        {
            var left = new ActorId(7UL);
            var right = new ActorId(8UL);

            Assert.AreNotEqual(left, right);
            Assert.IsTrue(left != right);
        }

        [Test]
        public void DefaultValueIsEmpty()
        {
            var id = default(ActorId);

            Assert.IsTrue(id.IsEmpty);
            Assert.AreEqual(0UL, id.Value);
        }

        [Test]
        public void HashCodeIsStableForSameValue()
        {
            var left = new ActorId(99UL);
            var right = new ActorId(99UL);

            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }
    }
}