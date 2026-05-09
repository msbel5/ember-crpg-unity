using EmberCrpg.Domain.Production;
using NUnit.Framework;

// Design note:
// These tests pin JobId as the stable identity primitive for production jobs.
// They do not test JobRecord fields, lifecycle transitions, ticking, actor assignment, or reactions.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Production
{
    /// <summary>
    /// Verifies production job identity behavior.
    /// </summary>
    public sealed class JobIdTests
    {
        [Test]
        public void Constructor_StoresValue()
        {
            var id = new JobId("job.forge.001");

            Assert.That(id.Value, Is.EqualTo("job.forge.001"));
        }

        [Test]
        public void DefaultValue_IsEmpty()
        {
            var id = default(JobId);

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void EmptyString_IsEmpty()
        {
            var id = new JobId("");

            Assert.That(id.IsEmpty, Is.True);
        }

        [Test]
        public void SameValues_AreEqual()
        {
            var left = new JobId("job.forge.001");
            var right = new JobId("job.forge.001");

            Assert.That(left, Is.EqualTo(right));
            Assert.That(left == right, Is.True);
        }

        [Test]
        public void DifferentValues_AreNotEqual()
        {
            var left = new JobId("job.forge.001");
            var right = new JobId("job.haul.001");

            Assert.That(left, Is.Not.EqualTo(right));
            Assert.That(left != right, Is.True);
        }

        [Test]
        public void ToString_ReturnsDebugLabel()
        {
            var id = new JobId("job.forge.001");

            Assert.That(id.ToString(), Is.EqualTo("JobId(job.forge.001)"));
        }

        [Test]
        public void ToString_ForEmpty_ReturnsEmptyDebugLabel()
        {
            var id = default(JobId);

            Assert.That(id.ToString(), Is.EqualTo("JobId.Empty"));
        }
    }
}