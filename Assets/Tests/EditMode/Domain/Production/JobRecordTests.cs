using System;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Domain.World;
using NUnit.Framework;

// Design note:
// These tests pin JobRecord as future-proof runtime state for one job.
// They do not test labor assignment, reactions, material consumption, XP, quality, or item output.
namespace EmberCrpg.Tests.EditMode.Tests.EditMode.Domain.Production
{
    /// <summary>
    /// Verifies production/activity job runtime state.
    /// </summary>
    public sealed class JobRecordTests
    {
        [Test]
        public void Constructor_StoresFields()
        {
            var job = NewForgeJob();

            Assert.That(job.Id, Is.EqualTo(new JobId("job.forge.001")));
            Assert.That(job.Kind, Is.EqualTo("forge"));
            Assert.That(job.Priority, Is.EqualTo(2));
            Assert.That(job.Status, Is.EqualTo(JobStatus.Queued));
            Assert.That(job.AssigneeId, Is.EqualTo(new ActorId(7UL)));
            Assert.That(job.SkillId, Is.EqualTo(new SkillId("craft.smithing")));
            Assert.That(job.RoomId, Is.EqualTo(new RoomId("room.blacksmith_workshop.001")));
            Assert.That(job.ActivitySiteId, Is.EqualTo(new ActivitySiteId("activity_site.iron_forge.001")));
            Assert.That(job.InputTags[0], Is.EqualTo("ore"));
            Assert.That(job.OutputTags[0], Is.EqualTo("ingot"));
            Assert.That(job.CompletionTicks, Is.EqualTo(100));
            Assert.That(job.ElapsedTicks, Is.EqualTo(25));
            Assert.That(job.Tags[0], Is.EqualTo("production"));
        }

        [Test]
        public void Constructor_AllowsEmptyRoomAndActivitySiteForAbstractJob()
        {
            var job = new JobRecord(
                new JobId("job.think.001"),
                "think",
                3,
                JobStatus.Queued,
                default(ActorId),
                new SkillId("science.theory"),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                10,
                0,
                new[] { "abstract" });

            Assert.That(job.RoomId.IsEmpty, Is.True);
            Assert.That(job.ActivitySiteId.IsEmpty, Is.True);
        }

        [Test]
        public void Constructor_EmptyJobId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new JobRecord(
                default(JobId),
                "forge",
                2,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                100,
                0,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_EmptyKind_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new JobRecord(
                new JobId("job.forge.001"),
                "",
                2,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                100,
                0,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_PriorityBelowOne_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                0,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                100,
                0,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_PriorityAboveFive_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                6,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                100,
                0,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_CompletionTicksZero_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                0,
                0,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_ElapsedTicksNegative_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                Array.Empty<string>(),
                Array.Empty<string>(),
                100,
                -1,
                Array.Empty<string>()));
        }

        [Test]
        public void Constructor_NullTagLists_NormalizeToEmptyLists()
        {
            var job = new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                default(ActorId),
                default(SkillId),
                default(RoomId),
                default(ActivitySiteId),
                null,
                null,
                100,
                0,
                null);

            Assert.That(job.InputTags.Count, Is.EqualTo(0));
            Assert.That(job.OutputTags.Count, Is.EqualTo(0));
            Assert.That(job.Tags.Count, Is.EqualTo(0));
        }

        [Test]
        public void IsComplete_WhenElapsedBelowCompletion_ReturnsFalse()
        {
            var job = NewForgeJob();

            Assert.That(job.IsComplete, Is.False);
        }

        [Test]
        public void IsComplete_WhenElapsedEqualsCompletion_ReturnsTrue()
        {
            var job = NewForgeJob().WithElapsedTicks(100);

            Assert.That(job.IsComplete, Is.True);
        }

        [Test]
        public void ProgressFraction_ReturnsClampedFraction()
        {
            var job = NewForgeJob().WithElapsedTicks(150);

            Assert.That(job.ProgressFraction, Is.EqualTo(1.0));
        }

        [Test]
        public void WithStatus_ValidTransition_ReturnsChangedCopy()
        {
            var job = NewForgeJob();

            var assigned = job.WithStatus(JobStatus.Assigned);

            Assert.That(assigned.Status, Is.EqualTo(JobStatus.Assigned));
            Assert.That(job.Status, Is.EqualTo(JobStatus.Queued));
        }

        [Test]
        public void WithStatus_InvalidTransition_ThrowsInvalidOperationException()
        {
            var job = NewForgeJob();

            Assert.Throws<InvalidOperationException>(() => job.WithStatus(JobStatus.Active));
        }

        [Test]
        public void WithElapsedTicks_ReturnsChangedCopy()
        {
            var job = NewForgeJob();

            var changed = job.WithElapsedTicks(75);

            Assert.That(changed.ElapsedTicks, Is.EqualTo(75));
            Assert.That(job.ElapsedTicks, Is.EqualTo(25));
        }

        private static JobRecord NewForgeJob()
        {
            return new JobRecord(
                new JobId("job.forge.001"),
                "forge",
                2,
                JobStatus.Queued,
                new ActorId(7UL),
                new SkillId("craft.smithing"),
                new RoomId("room.blacksmith_workshop.001"),
                new ActivitySiteId("activity_site.iron_forge.001"),
                new[] { "ore", "fuel" },
                new[] { "ingot" },
                100,
                25,
                new[] { "production", "metal" });
        }
    }
}