using EmberCrpg.Editor.Ember.Common;
using UnityEditor;
using UnityEngine;

namespace EmberCrpg.Editor.Ember.SceneBuilders
{
    /// <summary>
    /// Spawns actor and worksite placeholders. Each placeholder is a primitive with a
    /// SpriteRenderer billboard pointing at the camera, so Morrowind-style worlds get an
    /// inhabitable feel before a model pipeline lands. Actor sprites are resolved by name
    /// against <see cref="EmberAssetPaths.CharactersDir"/>.
    /// </summary>
    public static class EmberWorldspaceBuilder
    {
        public static GameObject SpawnActor(
            string actorName,
            string spriteName,
            Vector3 worldPosition,
            Transform parent = null)
        {
            var go = new GameObject(actorName);
            if (parent != null) go.transform.SetParent(parent, worldPositionStays: false);
            go.transform.position = worldPosition;

            var billboardChild = new GameObject("Billboard");
            billboardChild.transform.SetParent(go.transform, worldPositionStays: false);
            billboardChild.transform.localPosition = new Vector3(0f, 0.9f, 0f);

            var renderer = billboardChild.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSpriteByName(spriteName);
            renderer.sortingOrder = 10;

            var capsuleShadow = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsuleShadow.name = "ShadowProxy";
            capsuleShadow.transform.SetParent(go.transform, worldPositionStays: false);
            capsuleShadow.transform.localScale = new Vector3(0.5f, 0.9f, 0.5f);
            capsuleShadow.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            var sr = capsuleShadow.GetComponent<MeshRenderer>();
            if (sr != null) sr.enabled = false;

            return go;
        }

        public static GameObject SpawnWorksiteMarker(
            string siteName,
            Vector3 worldPosition,
            Transform parent = null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = siteName;
            if (parent != null) go.transform.SetParent(parent, worldPositionStays: false);
            go.transform.position = worldPosition;
            go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            return go;
        }

        private static Sprite LoadSpriteByName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName)) return null;
            var path = $"{EmberAssetPaths.CharactersDir}/{spriteName}.png";
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
