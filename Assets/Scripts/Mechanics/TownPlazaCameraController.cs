namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using DG.Tweening;

    [RequireComponent(typeof(Camera))]
    public class TownPlazaCameraController : MonoBehaviour
    {
        [SerializeField] private float buttonSpeed;
        [SerializeField] private float mouseDragSpeed;
        [SerializeField] private float MinZoomValue;
        [SerializeField] private float MaxZoomValue;
        [SerializeField] private float ZoomStepValue;
        private NpcController trackingNpc;
        private Vector2 MinCameraPos
        {
            get
            {
                return new Vector2(
                    -(13.5f - (cameraControl.orthographicSize * 1.75f)),
                    -(9.65f - (cameraControl.orthographicSize * 1f))
                );
            }
        }
        private Vector2 MaxCameraPos
        {
            get
            {
                return new Vector2(
                    13.5f - (cameraControl.orthographicSize * 1.75f),
                    9.65f - (cameraControl.orthographicSize * 1f)
                );
            }
        }
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
            if (trackingNpc != null && trackingNpc.transform != null)
            {
                MoveCamera(new Vector2(
                    trackingNpc.transform.position.x,
                    trackingNpc.transform.position.y
                ));
            }
            else if (buttonDirection != Vector2.zero)
            {
                MoveCamera(new Vector2(
                    transform.position.x + buttonDirection.x * Time.deltaTime * buttonSpeed,
                    transform.position.y + buttonDirection.y * Time.deltaTime * buttonSpeed
                ));
            }
            else if (isMouseDrag)
            {
                MoveCamera(new Vector2(
                    transform.position.x - dragOffset.x * mouseDragSpeed * Time.deltaTime,
                    transform.position.y - dragOffset.y * mouseDragSpeed * Time.deltaTime
                ));
            }
        }

        private void MoveCamera(Vector2 target)
        {
            transform.position = new Vector3(
                Mathf.Clamp(target.x, MinCameraPos.x, MaxCameraPos.x),
                Mathf.Clamp(target.y, MinCameraPos.y, MaxCameraPos.y),
                transform.position.z
            );
        }

        private void ZoomCamera(float zoomValue)
        {
            DOVirtual.Float(cameraControl.orthographicSize, zoomValue, 0.1f, (val) => {
                cameraControl.orthographicSize = val;
            });
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

            ZoomCamera(
                Mathf.Clamp(
                    cameraControl.orthographicSize - (wheelDelta * ZoomStepValue),
                    MinZoomValue,
                    MaxZoomValue
                )
            );

            // Make sure zoom doesn't reveal out of boundary area
            MoveCamera(
                new Vector2(
                    transform.position.x,
                    transform.position.y
                )
            );
        }

        public void TrackNpc(NpcController npc)
        {
            transform.DOMove(
                new Vector3(
                    Mathf.Clamp(npc.transform.position.x, MinCameraPos.x, MaxCameraPos.x),
                    Mathf.Clamp(npc.transform.position.y, MinCameraPos.y, MaxCameraPos.y),
                    transform.position.z
                ),
                0.2f
            )
            .SetEase(Ease.Linear)
            .OnComplete(() => trackingNpc = npc);
        }

        public void StopTrackNpc()
        {
            trackingNpc = null;
        }

        public void ZoomToNpc(NpcController npc)
        {
            ZoomCamera(MinZoomValue);
            MoveCamera(npc.transform.position);
        }
    }
}
