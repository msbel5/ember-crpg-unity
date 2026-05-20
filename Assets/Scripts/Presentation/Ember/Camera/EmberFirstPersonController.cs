using UnityEngine;

namespace EmberCrpg.Presentation.Ember.Camera
{
    /// <summary>
    /// Morrowind/Daggerfall-style first-person controller. Yaw is applied to the rig root,
    /// pitch is applied to the eye camera, translation moves the rig along its local plane.
    /// Input is read from the legacy <c>Input</c> module so the controller works without
    /// any package dependency.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EmberFirstPersonController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _mouseSensitivity = 2.1f;
        [SerializeField] private float _pitchMinDegrees = -85f;
        [SerializeField] private float _pitchMaxDegrees = 85f;

        private Transform _eye;
        private float _yawDegrees;
        private float _pitchDegrees;
        private bool _captureCursor = true;

        private void Awake()
        {
            _eye = transform.Find("EyeCamera");
            _yawDegrees = transform.eulerAngles.y;
        }

        private void OnEnable()
        {
            if (_captureCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            ApplyLook();
            ApplyMove();
            if (Input.GetKeyDown(KeyCode.Escape)) ToggleCursor();
        }

        private void ApplyLook()
        {
            var mouseDeltaX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            var mouseDeltaY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            _yawDegrees += mouseDeltaX;
            _pitchDegrees = Mathf.Clamp(_pitchDegrees - mouseDeltaY, _pitchMinDegrees, _pitchMaxDegrees);
            transform.rotation = Quaternion.Euler(0f, _yawDegrees, 0f);
            if (_eye != null) _eye.localRotation = Quaternion.Euler(_pitchDegrees, 0f, 0f);
        }

        private void ApplyMove()
        {
            var forward = Input.GetAxisRaw("Vertical");
            var right = Input.GetAxisRaw("Horizontal");
            var planar = (transform.forward * forward + transform.right * right).normalized;
            transform.position += planar * (_moveSpeed * Time.deltaTime);
        }

        private void ToggleCursor()
        {
            _captureCursor = !_captureCursor;
            Cursor.lockState = _captureCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_captureCursor;
        }
    }
}
