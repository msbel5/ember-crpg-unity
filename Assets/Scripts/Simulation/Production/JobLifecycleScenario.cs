using System;
using System.Collections.Generic;
using EmberCrpg.Data.Definitions;
using EmberCrpg.Domain.Components;
using EmberCrpg.Domain.Core;
using EmberCrpg.Domain.Production;
using EmberCrpg.Domain.Skills;
using EmberCrpg.Simulation.Skills;

// Design note:
// JobLifecycleScenario composes existing deterministic primitives for one job.
// It is not the world runtime: it does not mutate stores, consume inventory, create items, write events, or run AI.
namespace EmberCrpg.Simulation.Production
{
    /// <summary>
    /// Deterministic result of running one job lifecycle scenario.
    /// </summary>
    public sealed class JobLifecycleScenarioResult
    {
        /// <summary>
        /// Final job state after the scenario.
        /// </summary>
        public readonly JobRecord FinalJob;

        /// <summary>
        /// Selected actor id, or empty when no worker was assigned.
        /// </summary>
        public readonly ActorId SelectedActorId;

        /// <summary>
        /// Selected actor effective skill level for the job skill.
        /// </summary>
        public readonly int SelectedEffectiveSkillLevel;

        /// <summary>
        /// Number of simulation ticks advanced while the job was active.
        /// </summary>
        public readonly int TicksAdvanced;

        /// <summary>
        /// Completion payload, or null when no worker was assigned.
        /// </summary>
        public readonly JobCompletionResult Completion;

        /// <summary>
        /// Updated skill set for the selected actor, or empty when no worker was assigned.
        /// </summary>
        public readonly SkillSet UpdatedSelectedSkills;

        /// <summary>
        /// True when a worker was selected and assigned.
        /// </summary>
        public bool WasAssigned
        {
            get { return !SelectedActorId.IsEmpty; }
        }

        /// <summary>
        /// Creates a job lifecycle scenario result.
        /// </summary>
        public JobLifecycleScenarioResult(
            JobRecord finalJob,
            ActorId selectedActorId,
            int selectedEffectiveSkillLevel,
            int ticksAdvanced,
            JobCompletionResult completion,
            SkillSet updatedSelectedSkills)
        {
            if (finalJob == null)
                throw new ArgumentNullException(nameof(finalJob));
            if (selectedEffectiveSkillLevel < 0)
                throw new ArgumentOutOfRangeException(nameof(selectedEffectiveSkillLevel), "Selected effective skill level cannot be negative.");
            if (ticksAdvanced < 0)
                throw new ArgumentOutOfRangeException(nameof(ticksAdvanced), "Ticks advanced cannot be negative.");

            FinalJob = finalJob;
            SelectedActorId = selectedActorId;
            SelectedEffectiveSkillLevel = selectedEffectiveSkillLevel;
            TicksAdvanced = ticksAdvanced;
            Completion = completion;
            UpdatedSelectedSkills = updatedSelectedSkills ?? SkillSet.Empty;
        }
    }

    /// <summary>
    /// Pure deterministic one-job lifecycle scenario.
    /// </summary>
    public static class JobLifecycleScenario
    {
        /// <summary>
        /// Runs a queued job through assignment, activation, ticking, completion, and selected skill XP application.
        /// </summary>
        public static JobLifecycleScenarioResult RunToCompletion(
            JobRecord job,
            PositionComponent targetPosition,
            IReadOnlyList<LaborCandidate> candidates,
            ReactionDef reaction,
            GameTick startTick,
            int workSpeedPercent,
            int mentalBonusPercent,
            double rngValue)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (reaction == null)
                throw new ArgumentNullException(nameof(reaction));
            if (job.Status != JobStatus.Queued)
                throw new InvalidOperationException("Job lifecycle scenario requires a queued job.");
            if (workSpeedPercent <= 0)
                throw new InvalidOperationException("Job lifecycle scenario requires positive work speed.");

            var selection = LaborAssignment.SelectBest(job, targetPosition, candidates);
            if (!selection.HasSelection)
            {
                return new JobLifecycleScenarioResult(
                    job,
                    default(ActorId),
                    0,
                    0,
                    null,
                    SkillSet.Empty);
            }

            var selectedCandidate = FindSelectedCandidate(candidates, selection.SelectedActorId);
            var assigned = JobAssignmentApplication.Apply(job, selection).Job;
            var active = JobActivation.Activate(assigned).Job;
            var tickResult = TickUntilComplete(active, startTick, workSpeedPercent);

            var completion = JobCompletion.Complete(
                tickResult.Job,
                reaction,
                selection.EffectiveSkillLevel,
                mentalBonusPercent,
                rngValue);

            var updatedSkills = SkillSetExperienceApplication.Apply(
                selectedCandidate.Skills,
                job.SkillId,
                completion.XpGained).SkillSet;

            return new JobLifecycleScenarioResult(
                completion.CompletedJob,
                selection.SelectedActorId,
                selection.EffectiveSkillLevel,
                tickResult.TicksAdvanced,
                completion,
                updatedSkills);
        }

        private static LaborCandidate FindSelectedCandidate(
            IReadOnlyList<LaborCandidate> candidates,
            ActorId selectedActorId)
        {
            var list = candidates ?? Array.Empty<LaborCandidate>();
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].ActorId == selectedActorId)
                    return list[i];
            }

            throw new InvalidOperationException("Selected labor candidate was not found in the candidate list.");
        }

        private static JobLifecycleTickResult TickUntilComplete(
            JobRecord activeJob,
            GameTick startTick,
            int workSpeedPercent)
        {
            var job = activeJob;
            var ticksAdvanced = 0;
            var safetyLimit = job.CompletionTicks * 100;

            while (!job.IsComplete)
            {
                var tick = startTick.Add(ticksAdvanced);
                var tickResult = JobTick.Advance(job, tick, workSpeedPercent);

                job = tickResult.Job;
                ticksAdvanced++;

                if (ticksAdvanced > safetyLimit)
                    throw new InvalidOperationException("Job lifecycle scenario exceeded deterministic safety guard.");
            }

            return new JobLifecycleTickResult(job, ticksAdvanced);
        }
        
        private sealed class JobLifecycleTickResult
        {
            public readonly JobRecord Job;
            public readonly int TicksAdvanced;

            public JobLifecycleTickResult(JobRecord job, int ticksAdvanced)
            {
                Job = job ?? throw new ArgumentNullException(nameof(job));
                TicksAdvanced = ticksAdvanced;
            }
        }
    }
}