using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin assignment application as a deterministic bridge from LaborAssignmentResult to JobRecord.
// They do not test candidate selection, pathfinding, schedule reservation, activation, ticking, reactions, or actor mutation.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic application of labor assignment results to jobs.
    /// </summary>
    public sealed class JobAssignmentApplicationTests
    {
        [Test]
        public void Apply_NoSelection_ReturnsUnchangedQueuedJob()
        {
            var job = NewQueuedJob();

            var result = JobAssignmentApplication.Apply(job, LaborAssignmentResult.None);

            Assert.That(result.WasAssigned, Is.False);
            Assert.That(result.Job.Status, Is.EqualTo(JobStatus.Queued));
            Assert.That(result.Job.AssigneeId.IsEmpty, Is.True);
        }

        [Test]
        public void Apply_Selection_AssignsJobToSelectedActor()
        {
            var job = NewQueuedJob();
            var selection = new LaborAssignmentResult(new ActorId(7UL), 3, 5);

            var result = JobAssignmentApplication.Apply(job, selection);

            Assert.That(result.WasAssigned, Is.True);
            Assert.That(result.Job.Status, Is.EqualTo(JobStatus.Assigned));
            Assert.That(result.Job.AssigneeId, Is.EqualTo(new ActorId(7UL)));
        }

        [Test]
        public void Apply_Selection_PreservesSelectionMetadata()
        {
            var job = NewQueuedJob();
            var selection = new LaborAssignmentResult(new ActorId(7UL), 3, 5);

            var result = JobAssignmentApplication.Apply(job, selection);

            Assert.That(result.SelectedActorId, Is.EqualTo(new ActorId(7UL)));
            Assert.That(result.Distance, Is.EqualTo(3));
            Assert.That(result.EffectiveSkillLevel, Is.EqualTo(5));
        }

        [Test]
        public void Apply_Selection_DoesNotMutateOriginalJob()
        {
            var job = NewQueuedJob();
            var selection = new LaborAssignmentResult(new ActorId(7UL), 3, 5);

            var result = JobAssignmentApplication.Apply(job, selection);

            Assert.That(job.Status, Is.EqualTo(JobStatus.Queued));
            Assert.That(job.AssigneeId.IsEmpty, Is.True);
            Assert.That(result.Job.Status, Is.EqualTo(JobStatus.Assigned));
        }

        [Test]
        public void Apply_Selection_PreservesJobData()
        {
            var job = NewQueuedJob();
            var selection = new LaborAssignmentResult(new ActorId(7UL), 3, 5);

            var result = JobAssignmentApplication.Apply(job, selection);

            Assert.That(result.Job.Id, Is.EqualTo(job.Id));
            Assert.That(result.Job.Kind, Is.EqualTo(job.Kind));
            Assert.That(result.Job.Priority, Is.EqualTo(job.Priority));
            Assert.That(result.Job.SkillId, Is.EqualTo(job.SkillId));
            Assert.That(result.Job.RoomId, Is.EqualTo(job.RoomId));
            Assert.That(result.Job.ActivitySiteId, Is.EqualTo(job.ActivitySiteId));
            Assert.That(result.Job.CompletionTicks, Is.EqualTo(job.CompletionTicks));
            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(job.ElapsedTicks));
        }

        [Test]
        public void Apply_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                JobAssignmentApplication.Apply(null, LaborAssignmentResult.None));
        }

        [Test]
        public void Apply_NullSelection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                JobAssignmentApplication.Apply(NewQueuedJob(), null));
        }

        [Test]
        public void Apply_SelectionToNonQueuedJob_ThrowsInvalidOperationException()
        {
            var assignedJob = NewQueuedJob().AssignTo(new ActorId(7UL));
            var selection = new LaborAssignmentResult(new ActorId(8UL), 1, 9);

            Assert.Throws<InvalidOperationException>(() =>
                JobAssignmentApplication.Apply(assignedJob, selection));
        }

        [Test]
        public void Apply_SelectionToQueuedJobWithExistingAssignee_ThrowsInvalidOperationException()
        {
            var brokenQueuedJob = new JobRecord(
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

            var selection = new LaborAssignmentResult(new ActorId(7UL), 3, 5);

            Assert.Throws<InvalidOperationException>(() =>
                JobAssignmentApplication.Apply(brokenQueuedJob, selection));
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