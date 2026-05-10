using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin JobRecord assignment copy behavior.
// They do not test labor selection, candidate filtering, pathfinding, schedule reservation, activation, ticking, or reactions.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Production
{
    /// <summary>
    /// Verifies immutable job assignment behavior.
    /// </summary>
    public sealed class JobRecordAssignmentTests
    {
        [Test]
        public void AssignTo_QueuedJob_ReturnsAssignedCopyWithAssignee()
        {
            var job = NewQueuedJob();

            var assigned = job.AssignTo(new ActorId(7UL));

            Assert.That(assigned.Status, Is.EqualTo(JobStatus.Assigned));
            Assert.That(assigned.AssigneeId, Is.EqualTo(new ActorId(7UL)));
        }

        [Test]
        public void AssignTo_DoesNotMutateOriginalJob()
        {
            var job = NewQueuedJob();

            var assigned = job.AssignTo(new ActorId(7UL));

            Assert.That(job.Status, Is.EqualTo(JobStatus.Queued));
            Assert.That(job.AssigneeId.IsEmpty, Is.True);
            Assert.That(assigned.AssigneeId, Is.EqualTo(new ActorId(7UL)));
        }

        [Test]
        public void AssignTo_PreservesJobData()
        {
            var job = NewQueuedJob();

            var assigned = job.AssignTo(new ActorId(7UL));

            Assert.That(assigned.Id, Is.EqualTo(job.Id));
            Assert.That(assigned.Kind, Is.EqualTo(job.Kind));
            Assert.That(assigned.Priority, Is.EqualTo(job.Priority));
            Assert.That(assigned.SkillId, Is.EqualTo(job.SkillId));
            Assert.That(assigned.RoomId, Is.EqualTo(job.RoomId));
            Assert.That(assigned.ActivitySiteId, Is.EqualTo(job.ActivitySiteId));
            Assert.That(assigned.CompletionTicks, Is.EqualTo(job.CompletionTicks));
            Assert.That(assigned.ElapsedTicks, Is.EqualTo(job.ElapsedTicks));
        }

        [Test]
        public void AssignTo_EmptyActorId_ThrowsArgumentException()
        {
            var job = NewQueuedJob();

            Assert.Throws<ArgumentException>(() => job.AssignTo(default(ActorId)));
        }

        [Test]
        public void AssignTo_NonQueuedJob_ThrowsInvalidOperationException()
        {
            var job = NewQueuedJob().AssignTo(new ActorId(7UL));

            Assert.Throws<InvalidOperationException>(() => job.AssignTo(new ActorId(8UL)));
        }

        [Test]
        public void AssignTo_QueuedJobAlreadyCarryingAssignee_ThrowsInvalidOperationException()
        {
            var job = new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                new ActorId(99UL),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                0,
                new[] { "production", "metal" });

            Assert.Throws<InvalidOperationException>(() => job.AssignTo(new ActorId(7UL)));
        }

        [Test]
        public void WithAssignee_ReturnsCopyWithoutChangingStatus()
        {
            var job = NewQueuedJob();

            var changed = job.WithAssignee(new ActorId(7UL));

            Assert.That(changed.Status, Is.EqualTo(JobStatus.Queued));
            Assert.That(changed.AssigneeId, Is.EqualTo(new ActorId(7UL)));
        }

        [Test]
        public void WithAssignee_EmptyActorId_ClearsAssignee()
        {
            var assigned = NewQueuedJob().WithAssignee(new ActorId(7UL));

            var cleared = assigned.WithAssignee(default(ActorId));

            Assert.That(cleared.AssigneeId.IsEmpty, Is.True);
        }

        private static JobRecord NewQueuedJob()
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                default(ActorId),
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