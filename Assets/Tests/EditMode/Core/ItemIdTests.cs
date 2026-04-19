using EmberCrpg.Domain.Core;
using NUnit.Framework;

namespace EmberCrpg.Tests.EditMode.Core
{
    public sealed class ItemIdTests
    {
        [Test]
        public void ConstructorStoresRawValue()
        {
            var id = new ItemId(42UL);

            Assert.AreEqual(42UL, id.Value);
        }

        [Test]
        public void EqualValuesAreEqual()
        {
            var left = new ItemId(7UL);
            var right = new ItemId(7UL);

            Assert.AreEqual(left, right);
            Assert.IsTrue(left == right);
        }

        [Test]
        public void DifferentValuesAreNotEqual()
        {
            var left = new ItemId(7UL);
            var right = new ItemId(8UL);

            Assert.AreNotEqual(left, right);
            Assert.IsTrue(left != right);
        }

        [Test]
        public void DefaultValueIsEmpty()
        {
            var id = default(ItemId);

            Assert.IsTrue(id.IsEmpty);
            Assert.AreEqual(0UL, id.Value);
        }

        [Test]
        public void HashCodeIsStableForSameValue()
        {
            var left = new ItemId(99UL);
            var right = new ItemId(99UL);

            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }
    }
}