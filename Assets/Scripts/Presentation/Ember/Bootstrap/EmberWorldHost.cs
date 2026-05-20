using System.Collections.Generic;
using EmberCrpg.Presentation.Ember.Adapters;
using EmberCrpg.Presentation.Ember.Sprites;
using EmberCrpg.Presentation.Ember.Tick;
using EmberCrpg.Presentation.Ember.UI;
using EmberCrpg.Presentation.Ember.Views;
using UnityEngine;

namespace EmberCrpg.Presentation.Ember.Bootstrap
{
    /// <summary>
    /// Single MonoBehaviour entrypoint per generated scene. It owns the tick driver,
    /// resolves the active simulation adapter, and binds every visual panel to DTO-only
    /// source interfaces.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EmberWorldHost : MonoBehaviour, EmberTickDriver.ITickListener,
        IEmberHudSource, IJobQueueSource, IColonyNeedsSource, IDialogSource,
        IInventorySource, ISpriteByName, IFactionSource, ICombatHudSource
    {
        [SerializeField] private SpriteRegistry _spriteRegistry;

        private static readonly IReadOnlyList<string> Topics = new List<string> { "rumors", "work", "trade", "fate" };

        private EmberTickDriver _tick;
        private IDomainSimulationAdapter _adapter;
        private ActorView[] _actorViews;
        private WorksiteView[] _worksiteViews;
        private string _selectedTopic = "rumors";

        private void Awake()
        {
            _tick = GetComponent<EmberTickDriver>() ?? gameObject.AddComponent<EmberTickDriver>();
            _tick.Listener = this;

            _adapter = EmberDomainAdapterLocator.Current ?? CreateFallbackAdapter();
            EmberDomainAdapterLocator.Register(_adapter);
            _adapter.AdvanceTick(0);

            _actorViews = Object.FindObjectsByType<ActorView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            _worksiteViews = Object.FindObjectsByType<WorksiteView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            BindUiPanels();
            PushWorldViews();
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(EmberDomainAdapterLocator.Current, _adapter))
                EmberDomainAdapterLocator.Clear();
        }

        private void BindUiPanels()
        {
            foreach (var hud in Object.FindObjectsByType<EmberHud>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                hud.Source = this;
            foreach (var q in Object.FindObjectsByType<JobQueuePanel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                q.Source = this;
            foreach (var n in Object.FindObjectsByType<ColonyNeedsPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                n.Source = this;
            foreach (var d in Object.FindObjectsByType<DialogBoxPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                d.Source = this;
            foreach (var inventory in Object.FindObjectsByType<InventoryGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                inventory.Source = this;
                inventory.SpriteLookup = this;
            }
            foreach (var faction in Object.FindObjectsByType<FactionPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                faction.Source = this;
            foreach (var combat in Object.FindObjectsByType<CombatHud>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                combat.Source = this;
        }

        public void OnTick(int tickIndex)
        {
            _adapter.AdvanceTick(tickIndex);
            PushWorldViews();
        }

        private void PushWorldViews()
        {
            for (int i = 0; i < _actorViews.Length; i++)
            {
                var actor = _actorViews[i];
                if (_adapter.TryReadActor(actor.name, out var state))
                    actor.SetTarget(state);
            }

            for (int i = 0; i < _worksiteViews.Length; i++)
            {
                var worksite = _worksiteViews[i];
                if (_adapter.TryReadWorksite(worksite.name, out var state))
                    worksite.SetState(state);
            }
        }

        public string GetHudText() => _adapter.HudText;
        IReadOnlyList<JobQueueRow> IJobQueueSource.GetRows() => _adapter.JobQueueRows;
        IReadOnlyList<ColonyNeedsRow> IColonyNeedsSource.GetRows() => _adapter.ColonyNeedsRows;
        IReadOnlyList<FactionRow> IFactionSource.GetRows() => _adapter.FactionRows;
        public IReadOnlyList<InventorySlot> GetSlots() => _adapter.InventorySlots;
        CombatHudState ICombatHudSource.Read() => _adapter.CombatHud;
        public Sprite GetSprite(string name) => _spriteRegistry != null ? _spriteRegistry.GetSprite(name) : null;

        public string GetCurrentLine()
        {
            switch (_selectedTopic)
            {
                case "work": return "The forge queue is moving. Watch the left panel for job state.";
                case "trade": return "Caravans shift prices as stock moves between settlements.";
                case "fate": return "The oracle can surface a deterministic world query without mutating state.";
                default: return "Ask clean questions. The world remembers what matters.";
            }
        }

        public IReadOnlyList<string> GetTopics() => Topics;

        public void SelectTopic(string topicId)
        {
            if (!string.IsNullOrEmpty(topicId))
                _selectedTopic = topicId;
        }

        private static IDomainSimulationAdapter CreateFallbackAdapter()
        {
            var type = System.Type.GetType(
                "EmberCrpg.Presentation.Ember.Adapters.PlaceholderSimulationAdapter, EmberCrpg.Presentation");
            if (type != null && typeof(IDomainSimulationAdapter).IsAssignableFrom(type))
                return (IDomainSimulationAdapter)System.Activator.CreateInstance(type);

            return new EmptySimulationAdapter();
        }

        private sealed class EmptySimulationAdapter : IDomainSimulationAdapter
        {
            private static readonly IReadOnlyList<JobQueueRow> EmptyJobs = System.Array.Empty<JobQueueRow>();
            private static readonly IReadOnlyList<ColonyNeedsRow> EmptyNeeds = System.Array.Empty<ColonyNeedsRow>();
            private static readonly IReadOnlyList<FactionRow> EmptyFactions = System.Array.Empty<FactionRow>();
            private static readonly IReadOnlyList<InventorySlot> EmptyInventory = System.Array.Empty<InventorySlot>();

            public void AdvanceTick(int tickIndex) { }
            public string HudText => "Tick 0   Day 1   Spring";
            public IReadOnlyList<JobQueueRow> JobQueueRows => EmptyJobs;
            public IReadOnlyList<ColonyNeedsRow> ColonyNeedsRows => EmptyNeeds;
            public IReadOnlyList<FactionRow> FactionRows => EmptyFactions;
            public IReadOnlyList<InventorySlot> InventorySlots => EmptyInventory;
            public CombatHudState CombatHud => new CombatHudState(0, 100, 0, 100, 0, 100, string.Empty);
            public bool TryReadActor(string actorName, out ActorViewState state) { state = default; return false; }
            public bool TryReadWorksite(string siteName, out WorksiteViewState state) { state = default; return false; }
        }
    }
}
