using UnityEngine;

namespace EmberCrpg.Editor.Ember.SceneBuilders
{
    /// <summary>
    /// Builds the first-person player rig: a capsule body with a child eye camera.
    /// Runtime control lives on <c>EmberCrpg.Presentation.Ember.Camera.EmberFirstPersonController</c>.
    /// The builder wires that script and the input adapter by reflection-friendly name
    /// so the editor assembly does not need a hard reference on the runtime assembly.
    /// </summary>
    public static class EmberPlayerRigBuilder
    {
        public const float DefaultCapsuleHeight = 1.85f;
        public const float DefaultCapsuleRadius = 0.35f;

        public static GameObject BuildRig(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            var rig = new GameObject("PlayerRig");
            rig.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

            var controller = rig.AddComponent<CharacterController>();
            controller.height = DefaultCapsuleHeight;
            controller.radius = DefaultCapsuleRadius;
            controller.center = new Vector3(0f, DefaultCapsuleHeight / 2f, 0f);
            controller.stepOffset = 0.35f;

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(rig.transform, worldPositionStays: false);
            body.transform.localPosition = new Vector3(0f, DefaultCapsuleHeight / 2f, 0f);
            body.transform.localScale = new Vector3(DefaultCapsuleRadius * 2f, DefaultCapsuleHeight / 2f, DefaultCapsuleRadius * 2f);
            var bodyRenderer = body.GetComponent<MeshRenderer>();
            if (bodyRenderer != null) bodyRenderer.enabled = false;
            var bodyCollider = body.GetComponent<Collider>();
            if (bodyCollider != null) Object.DestroyImmediate(bodyCollider);

            EmberCameraRigBuilder.AddFirstPersonCamera(
                parent: rig,
                fieldOfView: 70f,
                nearClip: 0.05f,
                farClip: 500f,
                localEyePosition: new Vector3(0f, DefaultCapsuleHeight * 0.95f, 0f));

            AddScriptByName(rig, "EmberFirstPersonController");
            return rig;
        }

        private static void AddScriptByName(GameObject host, string scriptName)
        {
            var type = System.Type.GetType($"EmberCrpg.Presentation.Ember.Camera.{scriptName}, EmberCrpg.Presentation");
            if (type != null) host.AddComponent(type);
        }
    }
}
