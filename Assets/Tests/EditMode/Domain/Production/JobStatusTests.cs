using EmberCrpg.Domain.Production;
using NUnit.Framework;

// Design note:
// These tests pin job lifecycle status transitions.
// They do not test JobRecord fields, labor assignment, ticking, reaction completion, XP, or item output.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Production
{
    /// <summary>
    /// Verifies job status identity and lifecycle transition rules.
    /// </summary>
    public sealed class JobStatusTests
    {
        [Test]
        public void JobStatus_ValuesAreStable()
        {
            Assert.That((int)JobStatus.Queued, Is.EqualTo(0));
            Assert.That((int)JobStatus.Assigned, Is.EqualTo(1));
            Assert.That((int)JobStatus.Active, Is.EqualTo(2));
            Assert.That((int)JobStatus.Completed, Is.EqualTo(3));
            Assert.That((int)JobStatus.Cancelled, Is.EqualTo(4));
        }

        [Test]
        public void CanTransition_QueuedToAssigned_IsValid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Queued, JobStatus.Assigned), Is.True);
        }

        [Test]
        public void CanTransition_AssignedToActive_IsValid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Assigned, JobStatus.Active), Is.True);
        }

        [Test]
        public void CanTransition_ActiveToCompleted_IsValid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Active, JobStatus.Completed), Is.True);
        }

        [Test]
        public void CanTransition_ActiveToCancelled_IsValid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Active, JobStatus.Cancelled), Is.True);
        }

        [Test]
        public void CanTransition_QueuedToActive_IsInvalid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Queued, JobStatus.Active), Is.False);
        }

        [Test]
        public void CanTransition_CompletedToAnyStatus_IsInvalid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Completed, JobStatus.Queued), Is.False);
            Assert.That(JobStatusRules.CanTransition(JobStatus.Completed, JobStatus.Assigned), Is.False);
            Assert.That(JobStatusRules.CanTransition(JobStatus.Completed, JobStatus.Active), Is.False);
            Assert.That(JobStatusRules.CanTransition(JobStatus.Completed, JobStatus.Cancelled), Is.False);
        }

        [Test]
        public void CanTransition_CancelledToAnyStatus_IsInvalid()
        {
            Assert.That(JobStatusRules.CanTransition(JobStatus.Cancelled, JobStatus.Queued), Is.False);
            Assert.That(JobStatusRules.CanTransition(JobStatus.Cancelled, JobStatus.Assigned), Is.False);
            Assert.That(JobStatusRules.CanTransition(JobStatus.Cancelled, JobStatus.Active), Is.False);
            Assert.That(JobStatusRules.CanTransition(JobStatus.Cancelled, JobStatus.Completed), Is.False);
        }
    }
}