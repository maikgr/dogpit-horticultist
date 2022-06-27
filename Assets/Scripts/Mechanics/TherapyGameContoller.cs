namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.UI;

    public class TherapyGameContoller : MonoBehaviour
    {
        [SerializeField] private GameObject WealthRoomTemplate;
        [SerializeField] private GameObject HealthRoomTemplate;
        [SerializeField] private GameObject LoveRoomTemplate;
        [SerializeField] private Image cursorIconContainer;
        
        [Header("Tool Icons")]
        [SerializeField] private List<TherapyToolIcon> therapyToolIcons;

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
            
            StartCoroutine(OnEnableCoroutine());
        }

        private IEnumerator OnEnableCoroutine()
        {
            while(TherapyEventBus.Instance == null)
            {
                yield return new WaitForFixedUpdate();
            }
            TherapyEventBus.Instance.OnTherapyEnds += OnTherapyEnds;
        }

        private void OnDisable()
        {
            TherapyEventBus.Instance.OnTherapyEnds -= OnTherapyEnds;
            
            Cursor.visible = true;

            gameInput.UI.Point.performed -= OnPoint;
            gameInput.UI.Point.Disable();
            DisableClickEvents();
        }

        private void DisableClickEvents()
        {
            gameInput.Player.Fire.performed -= OnClickDown;
            gameInput.Player.Fire.canceled -= OnClickUp;
            gameInput.Player.Fire.Disable();
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

        private void ResetCursor()
        {
            cursorIconContainer.enabled = false;
            Cursor.visible = true;
        }

        private void SetCursor(ToolTypeEnum setTool)
        {
            cursorIconContainer.sprite = therapyToolIcons.First(tool => tool.toolType.Equals(setTool)).toolIcon;
            cursorIconContainer.enabled = true;
            Cursor.visible = false;
        }

        private MindClutter hoveredClutter;
        private ToolTypeEnum currentActiveTool;
        private void OnPoint(InputAction.CallbackContext context)
        {
            var activeTool = MindToolController.Instance.ActiveToolType;
            if (activeTool != ToolTypeEnum.None && currentActiveTool != activeTool)
            {
                currentActiveTool = activeTool;
                SetCursor(activeTool);
                SfxController.Instance.PlaySfx(SfxEnum.ToolSelect);
            }
            else if (activeTool == ToolTypeEnum.None && cursorIconContainer.enabled)
            {
                ResetCursor();
            }

            if (cursorIconContainer.enabled)
            {
                cursorIconContainer.transform.position = context.ReadValue<Vector2>();
            }
            
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

        private void OnTherapyEnds(NpcTypeEnum npcType, MoodEnum mood)
        {
            DisableClickEvents();
        }

    }

    [System.Serializable]
    public class TherapyToolIcon
    {
        public ToolTypeEnum toolType;
        public Sprite toolIcon;
    }
    
}
