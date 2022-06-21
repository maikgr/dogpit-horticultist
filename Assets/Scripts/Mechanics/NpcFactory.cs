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
        [SerializeField] private List<NpcBodySet> bodySprites;
        [SerializeField] private List<Sprite> headgearSprites;
        [SerializeField] private List<NpcExpressionSet> eyesSet;
        [SerializeField] private List<NpcExpressionSet> mouthSet;
        [SerializeField] private float headgearChance;

        [Header("NPC Dialogues")]
        [SerializeField] private TextAsset genericDialogueJson;
        [SerializeField] private TextAsset cultistDialogueJson;
        private TownDialogueParser townDialogueParser;

        [Header("NPC Area")]
        [SerializeField] private PolygonCollider2D allowedArea;
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
            this.townDialogueParser = new TownDialogueParser(
                genericDialogueJson.text,
                cultistDialogueJson.text
            );
        }

        public NpcController GenerateNpc()
        {
            var npc = Instantiate(npcPrefab, allowedArea.GetRandomPoint(), Quaternion.identity);
            var hasHeadgear = Random.Range(0f, 1f) < headgearChance;
            var firstName = firstNames.GetRandom();
            var lastName = lastNames.GetRandom();
            var personality = personalities.GetRandom();
            npc.GenerateNpc(
                firstName, lastName, personality,
                townDialogueParser.GenerateDialogueSet(personality),
                townDialogueParser.GenerateCultistActions($"{firstName} {lastName}", personality),
                bodySprites.GetRandom(),
                hasHeadgear ? headgearSprites.GetRandom() : null,
                eyesSet.GetRandom(),
                mouthSet.GetRandom(),
                allowedArea
            );

            return npc;
        }
    }
}
