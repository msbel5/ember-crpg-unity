using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin job activation as a deterministic transition from assigned to active.
// They do not test labor selection, pathfinding, schedule reservation, job ticking, reactions, XP, or inventory.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic job activation behavior.
    /// </summary>
    public sealed class JobActivationTests
    {
        [Test]
        public void Activate_AssignedJobWithAssignee_ReturnsActiveJob()
        {
            var job = NewAssignedJob(new ActorId(7UL));

            var result = JobActivation.Activate(job);

            Assert.That(result.WasActivated, Is.True);
            Assert.That(result.Job.Status, Is.EqualTo(JobStatus.Active));
            Assert.That(result.Job.AssigneeId, Is.EqualTo(new ActorId(7UL)));
        }

        [Test]
        public void Activate_DoesNotMutateOriginalJob()
        {
            var job = NewAssignedJob(new ActorId(7UL));

            var result = JobActivation.Activate(job);

            Assert.That(job.Status, Is.EqualTo(JobStatus.Assigned));
            Assert.That(result.Job.Status, Is.EqualTo(JobStatus.Active));
        }

        [Test]
        public void Activate_PreservesJobData()
        {
            var job = NewAssignedJob(new ActorId(7UL));

            var result = JobActivation.Activate(job);

            Assert.That(result.Job.Id, Is.EqualTo(job.Id));
            Assert.That(result.Job.Kind, Is.EqualTo(job.Kind));
            Assert.That(result.Job.Priority, Is.EqualTo(job.Priority));
            Assert.That(result.Job.AssigneeId, Is.EqualTo(job.AssigneeId));
            Assert.That(result.Job.SkillId, Is.EqualTo(job.SkillId));
            Assert.That(result.Job.RoomId, Is.EqualTo(job.RoomId));
            Assert.That(result.Job.ActivitySiteId, Is.EqualTo(job.ActivitySiteId));
            Assert.That(result.Job.CompletionTicks, Is.EqualTo(job.CompletionTicks));
            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(job.ElapsedTicks));
        }

        [Test]
        public void Activate_AssignedJobWithoutAssignee_ThrowsInvalidOperationException()
        {
            var job = NewAssignedJob(default(ActorId));

            Assert.Throws<InvalidOperationException>(() => JobActivation.Activate(job));
        }

        [Test]
        public void Activate_QueuedJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Queued, default(ActorId));

            Assert.Throws<InvalidOperationException>(() => JobActivation.Activate(job));
        }

        [Test]
        public void Activate_ActiveJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Active, new ActorId(7UL));

            Assert.Throws<InvalidOperationException>(() => JobActivation.Activate(job));
        }

        [Test]
        public void Activate_CompletedJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Completed, new ActorId(7UL));

            Assert.Throws<InvalidOperationException>(() => JobActivation.Activate(job));
        }

        [Test]
        public void Activate_CancelledJob_ThrowsInvalidOperationException()
        {
            var job = NewJob(JobStatus.Cancelled, new ActorId(7UL));

            Assert.Throws<InvalidOperationException>(() => JobActivation.Activate(job));
        }

        [Test]
        public void Activate_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JobActivation.Activate(null));
        }

        [Test]
        public void Result_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new JobActivationResult(null, true));
        }

        private static JobRecord NewAssignedJob(ActorId assigneeId)
        {
            return NewJob(JobStatus.Assigned, assigneeId);
        }

        private static JobRecord NewJob(JobStatus status, ActorId assigneeId)
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                status,
                assigneeId,
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                0,
                new[] { "production", "metal" });
        }
    }
}