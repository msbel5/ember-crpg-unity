using EmberCrpg.Editor.Ember.Common;
using EmberCrpg.Editor.Ember.SceneBuilders;
using EmberCrpg.Editor.Ember.SceneRecipes;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EmberCrpg.Editor.Ember.Menu
{
    /// <summary>
    /// One MenuItem per faz recipe. Each entry creates a fresh empty scene, runs the
    /// recipe, then saves to <see cref="EmberSceneSavePolicy.ResolveScenePath"/>.
    /// Menu code is intentionally thin so test/automation can call recipes directly.
    /// </summary>
    public static class EmberSceneBuilderMenu
    {
        private const string Root = "Ember/Build Scene/";

        [MenuItem(Root + "Faz 3 — Smithing Overworld")]
        public static void BuildFaz3() => RunRecipe(new Faz3SmithingSceneRecipe());

        [MenuItem(Root + "Faz 4 — Colony Needs")]
        public static void BuildFaz4() => RunRecipe(new Faz4ColonyNeedsSceneRecipe());

        [MenuItem(Root + "Faz 5 — Season Farm")]
        public static void BuildFaz5() => RunRecipe(new Faz5FarmSceneRecipe());

        [MenuItem(Root + "Faz 6 — Trade Market")]
        public static void BuildFaz6() => RunRecipe(new Faz6TradeSceneRecipe());

        [MenuItem(Root + "Faz 7 — Combat Dungeon")]
        public static void BuildFaz7() => RunRecipe(new Faz7CombatSceneRecipe());

        public static void RunRecipe(IEmberSceneRecipe recipe)
        {
            EmberSceneFactory.CreateEmpty();
            recipe.Build();
            EmberRuntimeHostBuilder.EnsureHost();
            var path = EmberSceneSavePolicy.ResolveScenePath(recipe.SceneName);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
            AssetDatabase.Refresh();
        }
    }
}
