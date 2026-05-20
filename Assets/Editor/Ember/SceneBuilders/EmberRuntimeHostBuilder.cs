using UnityEngine;
using UnityEditor;
using EmberCrpg.Editor.Ember.Tools;

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
            {
                AssignSpriteRegistry(existing);
                return existing;
            }

            var host = new GameObject("EmberWorldHost");
            AddRuntimeComponent(host, "EmberCrpg.Presentation.Ember.Bootstrap.EmberWorldHost");
            AssignSpriteRegistry(host);
            return host;
        }

        private static void AssignSpriteRegistry(GameObject host)
        {
            var hostType = ResolveRuntimeType("EmberCrpg.Presentation.Ember.Bootstrap.EmberWorldHost");
            if (hostType == null)
                return;

            var component = host.GetComponent(hostType);
            if (component == null)
                return;

            var registry = AssetDatabase.LoadAssetAtPath<Object>(SpriteRegistryAutoBuilder.RegistryAssetPath);
            if (registry == null)
                return;

            var serialized = new SerializedObject(component);
            var prop = serialized.FindProperty("_spriteRegistry");
            if (prop == null)
                return;

            prop.objectReferenceValue = registry;
            serialized.ApplyModifiedPropertiesWithoutUndo();
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
