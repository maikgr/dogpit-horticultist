namespace Horticultist.Scripts.Mechanics
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    
    [RequireComponent(typeof(Camera))]
    public class TherapyCameraController : MonoBehaviour
    {
        [SerializeField] private float buttonSpeed;
        [SerializeField] private float mouseDragSpeed;
        [SerializeField] private Vector2 MinCameraPos;
        [SerializeField] private Vector2 MaxCameraPos;
        [SerializeField] private GameObject IndicatorUp;
        [SerializeField] private GameObject IndicatorRight;
        [SerializeField] private GameObject IndicatorBottom;
        [SerializeField] private GameObject IndicatorLeft;
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

            gameInput.Camera.Move.performed += OnCameraMoveStarted;
            gameInput.Camera.Move.canceled += OnCameraMoveCanceled;
            gameInput.Camera.Move.Enable();

            gameInput.Camera.Drag.started += OnCameraDragStarted;
            gameInput.Camera.Drag.canceled += OnCameraDragCanceled;
            gameInput.Camera.Drag.Enable();
        }

        private void OnDisable()
        {
            gameInput.Camera.Move.performed -= OnCameraMoveStarted;
            gameInput.Camera.Move.canceled -= OnCameraMoveCanceled;
            gameInput.Camera.Move.Disable();

            gameInput.Camera.Drag.started -= OnCameraDragStarted;
            gameInput.Camera.Drag.canceled -= OnCameraDragCanceled;
            gameInput.Camera.Drag.Disable();
        }
        
        private void LateUpdate()
        {
            if (buttonDirection != Vector2.zero)
            {
                MoveCamera(new Vector2(
                    transform.position.x + buttonDirection.x * Time.deltaTime * buttonSpeed,
                    transform.position.y + buttonDirection.y * Time.deltaTime * buttonSpeed
                ));
                UpdateIndicators();
            }
            else if (isMouseDrag)
            {
                MoveCamera(new Vector2(
                    transform.position.x - dragOffset.x * mouseDragSpeed * Time.deltaTime,
                    transform.position.y - dragOffset.y * mouseDragSpeed * Time.deltaTime
                ));
                UpdateIndicators();
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

        private void UpdateIndicators()
        {
            IndicatorUp.gameObject.SetActive(transform.position.y < MaxCameraPos.y);
            IndicatorBottom.gameObject.SetActive(transform.position.y > MinCameraPos.y);
            IndicatorRight.gameObject.SetActive(transform.position.x < MaxCameraPos.x);
            IndicatorLeft.gameObject.SetActive(transform.position.x > MinCameraPos.x);
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
    }
}