namespace Horticultist.Scripts.Mechanics
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Newtonsoft.Json;
    using Horticultist.Scripts.Extensions;

    public class NpcFactory : MonoBehaviour
    {
        [Header("NPC Information")]
        [SerializeField] private NpcController npcPrefab;
        [SerializeField] private TextAsset firstNameJson;
        [SerializeField] private TextAsset lastNameJson;
        [SerializeField] private List<Sprite> bodySprites;
        [SerializeField] private List<Sprite> headgearSprites;
        [SerializeField] private List<Sprite> eyesSprites;
        [SerializeField] private List<Sprite> mouthSprites;
        private HorticultistInputActions gameInput;
        private InputAction inputAction;

        private string[] firstNames;
        private string[] lastNames;
        private void Awake() {
            firstNames = JsonConvert.DeserializeObject<string[]>(firstNameJson.text);
            lastNames = JsonConvert.DeserializeObject<string[]>(lastNameJson.text);
            gameInput = new HorticultistInputActions();
        }

        private void OnEnable() {
            inputAction = gameInput.Player.Fire;
            inputAction.Enable();

            gameInput.Player.Fire.performed += OnInputPerformed;
            gameInput.Player.Fire.Enable();
        }

        private void OnDisable() {
            gameInput.Player.Fire.Disable();
            inputAction.Disable();
        }

        private void OnInputPerformed(InputAction.CallbackContext context)  
        {
        }

        public void GenerateNPC()
        {
            var npc = Instantiate(npcPrefab, TownPlazaAreaController.Instance.GetRandomPoint(), Quaternion.identity);
            var hasHeadgear = Random.Range(0f, 1f) < 0.5f;
            npc.GenerateVisitor(
                $"{firstNames.GetRandom()} {lastNames.GetRandom()}",
                bodySprites.GetRandom(),
                hasHeadgear ? headgearSprites.GetRandom() : null,
                eyesSprites.GetRandom(),
                mouthSprites.GetRandom()
            );
        }
    }
}
