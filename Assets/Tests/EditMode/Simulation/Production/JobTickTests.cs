using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using EmberCrpg.Simulation.Production;
using NUnit.Framework;

// Design note:
// These tests pin job ticking as a deterministic pure simulation behavior.
// They do not test labor assignment, reaction completion, item output, XP, quality, inventory, or actor AI.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Simulation.Production
{
    /// <summary>
    /// Verifies deterministic active job ticking.
    /// </summary>
    public sealed class JobTickTests
    {
        [Test]
        public void Advance_ActiveJobAtNormalSpeed_IncrementsElapsedTicksByOne()
        {
            var job = NewActiveJob(25, 100);

            var result = JobTick.Advance(job, new GameTick(0), 100);

            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(26));
            Assert.That(result.WorkTicksApplied, Is.EqualTo(1));
            Assert.That(result.IsNowComplete, Is.False);
        }

        [Test]
        public void Advance_ActiveJobAtZeroSpeed_DoesNotProgress()
        {
            var job = NewActiveJob(25, 100);

            var result = JobTick.Advance(job, new GameTick(0), 0);

            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(25));
            Assert.That(result.WorkTicksApplied, Is.EqualTo(0));
            Assert.That(result.IsNowComplete, Is.False);
        }

        [Test]
        public void Advance_ActiveJobAtHalfSpeed_ProgressesEveryOtherTick()
        {
            var job = NewActiveJob(25, 100);

            var first = JobTick.Advance(job, new GameTick(0), 50);
            var second = JobTick.Advance(job, new GameTick(1), 50);

            Assert.That(first.WorkTicksApplied, Is.EqualTo(0));
            Assert.That(first.Job.ElapsedTicks, Is.EqualTo(25));

            Assert.That(second.WorkTicksApplied, Is.EqualTo(1));
            Assert.That(second.Job.ElapsedTicks, Is.EqualTo(26));
        }

        [Test]
        public void Advance_ActiveJobAtDoubleSpeed_IncrementsElapsedTicksByTwo()
        {
            var job = NewActiveJob(25, 100);

            var result = JobTick.Advance(job, new GameTick(0), 200);

            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(27));
            Assert.That(result.WorkTicksApplied, Is.EqualTo(2));
        }

        [Test]
        public void Advance_WhenJobReachesCompletion_ReturnsCompleteButKeepsActiveStatus()
        {
            var job = NewActiveJob(99, 100);

            var result = JobTick.Advance(job, new GameTick(0), 100);

            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(100));
            Assert.That(result.IsNowComplete, Is.True);
            Assert.That(result.Job.Status, Is.EqualTo(JobStatus.Active));
        }

        [Test]
        public void Advance_WhenJobExceedsCompletion_ProgressFractionRemainsClamped()
        {
            var job = NewActiveJob(99, 100);

            var result = JobTick.Advance(job, new GameTick(0), 200);

            Assert.That(result.Job.ElapsedTicks, Is.EqualTo(101));
            Assert.That(result.Job.ProgressFraction, Is.EqualTo(1.0));
            Assert.That(result.IsNowComplete, Is.True);
        }

        [Test]
        public void Advance_QueuedJob_ThrowsInvalidOperationException()
        {
            var job = NewQueuedJob();

            Assert.Throws<InvalidOperationException>(() => JobTick.Advance(job, new GameTick(0), 100));
        }

        [Test]
        public void Advance_NullJob_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => JobTick.Advance(null, new GameTick(0), 100));
        }

        [Test]
        public void Advance_NegativeWorkSpeedPercent_ThrowsArgumentOutOfRangeException()
        {
            var job = NewActiveJob(25, 100);

            Assert.Throws<ArgumentOutOfRangeException>(() => JobTick.Advance(job, new GameTick(0), -1));
        }

        [Test]
        public void Advance_TooLargeWorkSpeedPercent_ThrowsArgumentOutOfRangeException()
        {
            var job = NewActiveJob(25, 100);

            Assert.Throws<ArgumentOutOfRangeException>(() => JobTick.Advance(job, new GameTick(0), 10001));
        }

        [Test]
        public void WorkTicksFor_EightyPercent_IsDeterministicallyDistributed()
        {
            Assert.That(JobTick.WorkTicksFor(new GameTick(0), 80), Is.EqualTo(0));
            Assert.That(JobTick.WorkTicksFor(new GameTick(1), 80), Is.EqualTo(1));
            Assert.That(JobTick.WorkTicksFor(new GameTick(2), 80), Is.EqualTo(1));
            Assert.That(JobTick.WorkTicksFor(new GameTick(3), 80), Is.EqualTo(1));
            Assert.That(JobTick.WorkTicksFor(new GameTick(4), 80), Is.EqualTo(1));
        }

        private static JobRecord NewActiveJob(int elapsedTicks, int completionTicks)
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Active,
                new ActorId(7UL),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                completionTicks,
                elapsedTicks,
                new[] { "production", "metal" });
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