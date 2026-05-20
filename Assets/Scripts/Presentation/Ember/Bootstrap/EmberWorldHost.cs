using System.Collections.Generic;
using EmberCrpg.Presentation.Ember.Tick;
using EmberCrpg.Presentation.Ember.UI;
using UnityEngine;

namespace EmberCrpg.Presentation.Ember.Bootstrap
{
    /// <summary>
    /// Single MonoBehaviour entrypoint per scene. Owns the tick driver and exposes the
    /// adapter sources the UI panels read. Replace the placeholder sources with
    /// simulation-backed implementations once the domain is wired.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EmberWorldHost : MonoBehaviour, EmberTickDriver.ITickListener,
        IEmberHudSource, IJobQueueSource, IColonyNeedsSource
    {
        private EmberTickDriver _tick;
        private readonly PlaceholderModel _model = new PlaceholderModel();

        private void Awake()
        {
            _tick = GetComponent<EmberTickDriver>() ?? gameObject.AddComponent<EmberTickDriver>();
            _tick.Listener = this;

            BindUiPanels();
        }

        private void BindUiPanels()
        {
            foreach (var hud in UnityEngine.Object.FindObjectsByType<EmberHud>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                hud.Source = this;
            foreach (var q in UnityEngine.Object.FindObjectsByType<JobQueuePanel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                q.Source = this;
            foreach (var n in UnityEngine.Object.FindObjectsByType<ColonyNeedsPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                n.Source = this;
        }

        public void OnTick(int tickIndex) => _model.Advance(tickIndex);

        public string GetHudText() => _model.GetHudText();
        public IReadOnlyList<JobQueueRow> GetRows() => _model.GetJobRows();
        IReadOnlyList<ColonyNeedsRow> IColonyNeedsSource.GetRows() => _model.GetNeedRows();

        /// <summary>
        /// Visual-only stand-in until a Domain adapter lands. Produces deterministic
        /// rows so the panels show data while the real simulation is being wired.
        /// </summary>
        private sealed class PlaceholderModel
        {
            private int _tick;
            private readonly List<JobQueueRow> _jobs = new List<JobQueueRow>();
            private readonly List<ColonyNeedsRow> _needs = new List<ColonyNeedsRow>();

            public void Advance(int tick)
            {
                _tick = tick;
                _jobs.Clear();
                _jobs.Add(new JobQueueRow("Smith_A", "smith", tick % 8 < 4 ? "active"   : "queued", 0));
                _jobs.Add(new JobQueueRow("Smith_B", "smith", tick % 8 < 4 ? "queued"   : "active", 1));

                _needs.Clear();
                _needs.Add(new ColonyNeedsRow("Innkeeper", Clamp(20 + tick),       Clamp(10 + tick / 2), Clamp(8 + tick / 3),  Clamp(78 - tick / 4)));
                _needs.Add(new ColonyNeedsRow("Beggar",    Clamp(55 + tick * 2),  Clamp(30 + tick),     Clamp(40 + tick / 2), Clamp(42 - tick / 2)));
                _needs.Add(new ColonyNeedsRow("Guard",     Clamp(15 + tick / 2),  Clamp(20 + tick / 2), Clamp(10 + tick / 3), Clamp(85 - tick / 5)));
            }

            public string GetHudText()
            {
                var day = 1 + _tick / 240;
                var season = SeasonOf(day);
                return $"Tick {_tick}   Day {day}   {season}";
            }

            public IReadOnlyList<JobQueueRow> GetJobRows()   => _jobs;
            public IReadOnlyList<ColonyNeedsRow> GetNeedRows() => _needs;

            private static int Clamp(int v) => Mathf.Clamp(v, 0, 100);

            private static string SeasonOf(int day) =>
                (((day - 1) / 30) % 4) switch
                {
                    0 => "Spring",
                    1 => "Summer",
                    2 => "Autumn",
                    _ => "Winter",
                };
        }
    }
}
