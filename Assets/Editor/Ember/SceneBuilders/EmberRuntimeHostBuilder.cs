using UnityEngine;

namespace EmberCrpg.Editor.Ember.SceneBuilders
{
    /// <summary>
    /// Adds the one runtime host every generated acceptance scene needs. Recipes build
    /// world geometry and UI; the host binds those views to deterministic placeholder
    /// sources until the backend adapter is connected.
    /// </summary>
    public static class EmberRuntimeHostBuilder
    {
        public static GameObject EnsureHost()
        {
            var existing = GameObject.Find("EmberWorldHost");
            if (existing != null)
                return existing;

            var host = new GameObject("EmberWorldHost");
            AddRuntimeComponent(host, "EmberCrpg.Presentation.Ember.Bootstrap.EmberWorldHost");
            return host;
        }

        private static void AddRuntimeComponent(GameObject host, string fullName)
        {
            var type = ResolveRuntimeType(fullName);
            if (type == null)
            {
                Debug.LogWarning($"Could not resolve runtime component {fullName}");
                return;
            }

            host.AddComponent(type);
        }

        private static System.Type ResolveRuntimeType(string fullName)
        {
            var qualified = System.Type.GetType(fullName + ", EmberCrpg.Presentation");
            if (qualified != null) return qualified;

            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var type = assemblies[i].GetType(fullName);
                if (type != null) return type;
            }
            return null;
        }
    }
}
