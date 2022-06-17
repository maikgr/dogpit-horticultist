namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;


    public class MindClutter : MonoBehaviour
    {

        [SerializeField] private List<GameObject> toolInteractions;
        private HorticultistInputActions gameInput;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            gameInput = new HorticultistInputActions();

            var availableToolInteractions = toolInteractions.Select((interaction) => interaction.GetComponent<ToolClutterInteraction>()).ToList();
            var allowedTools = availableToolInteractions.Select((interaction) => interaction.MindTool.GetComponent<ToolType>().ThisToolType).ToList();
            Debug.Log(allowedTools[0]);
            Debug.Log(allowedTools[1]);
        }

        private void OnEnable()
        {

            // gameInput.UI.Click.started += OnClickDown;
            // gameInput.UI.Click.canceled += OnClickUp;
            // gameInput.UI.Click.Enable();

            gameInput.Player.Fire.performed += OnClickDown;
            gameInput.Player.Fire.Enable();

        }

        private void OnClickDown(InputAction.CallbackContext context)
        {
            var activeController = MindToolController.Instance.ActiveToolType;
            var worldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var clutter = hit.collider.GetComponent<MindClutter>();
                if (clutter != null)
                {
                    Debug.Log(activeController);

                    Debug.Log("click down");
                }
            }

        }

        private void OnClickUp(InputAction.CallbackContext context)
        {
            Debug.Log("click up");
        }
    }
}