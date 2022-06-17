namespace Horticultist.Scripts.Mechanics
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Newtonsoft.Json;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.Core;

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

        [Header("NPC Dialogues")]
        [SerializeField] private TextAsset genericDialogueJson;
        [SerializeField] private TextAsset cultistDialogueJson;
        private TownDialogueParser townDialogueParser;
        private HorticultistInputActions gameInput;
        private InputAction inputAction;
        private NpcPersonalityEnum[] personalities = new NpcPersonalityEnum[] {
            NpcPersonalityEnum.Health,
            NpcPersonalityEnum.Wealth,
            NpcPersonalityEnum.Love
        };

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

        private void Start()
        {
            this.townDialogueParser = new TownDialogueParser(
                genericDialogueJson.text,
                cultistDialogueJson.text
            );
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
            var npcName = $"{firstNames.GetRandom()} {lastNames.GetRandom()}";
            var personality = personalities.GetRandom();
            npc.GenerateNpc(
                npcName, personality,
                townDialogueParser.GenerateDialogueSet(personality),
                townDialogueParser.GenerateCultistActions(npcName, personality),
                bodySprites.GetRandom(),
                hasHeadgear ? headgearSprites.GetRandom() : null,
                eyesSprites.GetRandom(),
                mouthSprites.GetRandom()
            );
        }
    }
}
