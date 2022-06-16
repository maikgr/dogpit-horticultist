namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Camera))]
    public class TownPlazaCameraController : MonoBehaviour
    {
        [SerializeField] private float buttonSpeed;
        [SerializeField] private float mouseDragSpeed;
        [SerializeField] private Vector2 MinCameraPos;
        [SerializeField] private Vector2 MaxCameraPos;
        [SerializeField] private float MinZoomValue;
        [SerializeField] private float MaxZoomValue;
        [SerializeField] private float ZoomStepValue;
        private HorticultistInputActions gameInput;
        private Camera cameraControl;

        // Camera input control
        private Vector2 buttonDirection = Vector2.zero;
        private bool isMouseDrag;
        private Vector2 dragOffset;
        private void Awake()
        {
            gameInput = new HorticultistInputActions();
            cameraControl = GetComponent<Camera>();
        }

        private void OnEnable()
        {

            gameInput.Camera.Move.started += OnCameraMoveStarted;
            gameInput.Camera.Move.canceled += OnCameraMoveCanceled;
            gameInput.Camera.Move.Enable();

            gameInput.Camera.Drag.started += OnCameraDragStarted;
            gameInput.Camera.Drag.canceled += OnCameraDragCanceled;
            gameInput.Camera.Drag.Enable();

            gameInput.Camera.Zoom.performed += OnCameraZoomPerformed;
            gameInput.Camera.Zoom.Enable();
        }

        private void OnDisable()
        {
            gameInput.Camera.Move.started -= OnCameraMoveStarted;
            gameInput.Camera.Move.canceled -= OnCameraMoveCanceled;
            gameInput.Camera.Move.Disable();

            gameInput.Camera.Drag.started -= OnCameraDragStarted;
            gameInput.Camera.Drag.canceled -= OnCameraDragCanceled;
            gameInput.Camera.Drag.Disable();

            gameInput.Camera.Zoom.performed -= OnCameraZoomPerformed;
            gameInput.Camera.Zoom.Disable();
        }
        
        private void LateUpdate()
        {
            if (buttonDirection != Vector2.zero)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x + buttonDirection.x * Time.deltaTime * buttonSpeed, MinCameraPos.x, MaxCameraPos.x),
                    Mathf.Clamp(transform.position.y + buttonDirection.y * Time.deltaTime * buttonSpeed, MinCameraPos.y, MaxCameraPos.y),
                    transform.position.z
                );
            }
            else if (isMouseDrag)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x - dragOffset.x * mouseDragSpeed * Time.deltaTime, MinCameraPos.x, MaxCameraPos.x),
                    Mathf.Clamp(transform.position.y - dragOffset.y * mouseDragSpeed * Time.deltaTime, MinCameraPos.y, MaxCameraPos.y),
                    transform.position.z
                );
            }
        }

        private void OnCameraMoveStarted(InputAction.CallbackContext context)
        {
            this.buttonDirection = context.ReadValue<Vector2>();
        }

        private void OnCameraMoveCanceled(InputAction.CallbackContext context)
        {
            this.buttonDirection = Vector2.zero;
        }

        private void OnCameraDragStarted(InputAction.CallbackContext context)
        {
            this.dragOffset = context.ReadValue<Vector2>();
            this.isMouseDrag = true;
        }
        private void OnCameraDragCanceled(InputAction.CallbackContext context)
        {
            this.isMouseDrag = false;
            this.dragOffset = Vector2.zero;
        }

        private void OnCameraZoomPerformed(InputAction.CallbackContext context)
        {
            var wheelDelta = context.ReadValue<float>();
            if (wheelDelta > 0) wheelDelta = 1;
            else if (wheelDelta < 0) wheelDelta = -1;

            cameraControl.orthographicSize = Mathf.Clamp(
                cameraControl.orthographicSize - (wheelDelta * ZoomStepValue),
                MinZoomValue,
                MaxZoomValue
            );
        }
    }
}
