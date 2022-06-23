namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Horticultist.Scripts.Core;

    public class TherapyGameContoller : MonoBehaviour
    {

        [SerializeField] private GameObject WealthRoomTemplate;
        [SerializeField] private GameObject HealthRoomTemplate;
        [SerializeField] private GameObject LoveRoomTemplate;

        private HorticultistInputActions gameInput;
        private Camera mainCamera;
        private MindClutter targettedClutter;


        private void Awake()
        {
            mainCamera = Camera.main;
            gameInput = new HorticultistInputActions();
        }

        private void OnEnable()
        {
            gameInput.Player.Fire.performed += OnClickDown;
            gameInput.Player.Fire.canceled += OnClickUp;
            gameInput.UI.Point.performed += OnPoint;

            gameInput.Player.Fire.Enable();
            gameInput.UI.Point.Enable();
        }

        private void OnDisable()
        {
            gameInput.Player.Fire.performed -= OnClickDown;
            gameInput.Player.Fire.canceled -= OnClickUp;
            gameInput.UI.Point.performed -= OnPoint;

            gameInput.Player.Fire.Disable();
            gameInput.UI.Point.Disable();
        }
        
        private void Start()
        {
            var npc = GameStateController.Instance.SelectedNpc;
            InstantiateRoomTemplate(npc.npcPersonality);
        }

        private void OnClickDown(InputAction.CallbackContext context)
        {
            var worldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                targettedClutter = hit.collider.GetComponent<MindClutter>();
                targettedClutter.OnClickDown();
            }
            else
            {
                targettedClutter = null;
            }
        }

        private void OnClickUp(InputAction.CallbackContext context)
        {
            if (targettedClutter != null) {
                targettedClutter.OnClickUp();
            }
        }

        private MindClutter hoveredClutter;
        private void OnPoint(InputAction.CallbackContext context)
        {
            var worldPos = mainCamera.ScreenToWorldPoint(context.ReadValue<Vector2>());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var clutter = hit.collider.GetComponent<MindClutter>();
                if (clutter != null && clutter.CurrentState == ClutterStateEnum.Dirty)
                {
                    clutter.OnHoverEnter();
                    hoveredClutter = clutter;
                }
                else if (hoveredClutter != null && hoveredClutter != clutter)
                {
                    hoveredClutter.OnHoverExit();
                    hoveredClutter = null;
                }
            }
            else if (hoveredClutter != null)
            {
                hoveredClutter.OnHoverExit();
                hoveredClutter = null;
            }
        }

        private void InstantiateRoomTemplate(NpcPersonalityEnum personality)
        {

            if (personality == NpcPersonalityEnum.Wealth) 
            {
                Instantiate(WealthRoomTemplate, new Vector3(0, 0, -1), Quaternion.identity);
            }
            else if (personality == NpcPersonalityEnum.Health)
            {
                Instantiate(HealthRoomTemplate, new Vector3(0, 0, -1), Quaternion.identity);
            }
            else
            {
                Instantiate(LoveRoomTemplate, new Vector3(0, 0, -1), Quaternion.identity);
            }
        }

    }
    
}
