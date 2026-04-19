using EmberCrpg.Domain.Core;
using NUnit.Framework;

namespace EmberCrpg.Tests.EditMode.Core
{
    public sealed class CoreContractTests
    {
        [Test]
        public void ActorIdAndItemIdAreDifferentTypes()
        {
            Assert.AreNotEqual(typeof(ActorId), typeof(ItemId));
        }

        [Test]
        public void EntityIdExistsAsGenericIdentitySpine()
        {
            var entityId = new EntityId(1UL);

            Assert.AreEqual(1UL, entityId.Value);
        }

        [Test]
        public void UniverseIdExistsForMultiverseScoping()
        {
            var universeId = new UniverseId(1UL);

            Assert.AreEqual(1UL, universeId.Value);
        }

        [Test]
        public void DeterministicSeedStoresRawSeed()
        {
            var seed = new DeterministicSeed(123UL);

            Assert.AreEqual(123UL, seed.Value);
        }
    }
}