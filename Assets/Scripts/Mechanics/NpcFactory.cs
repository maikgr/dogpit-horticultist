namespace Horticultist.Scripts.Mechanics
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Newtonsoft.Json;
    using Pathfinding;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.Core;

    public class NpcFactory : MonoBehaviour
    {
        [Header("NPC Information")]
        [SerializeField] private NpcController npcPrefab;
        [SerializeField] private List<NpcBodySet> bodySprites;
        [SerializeField] private List<Sprite> headgearSprites;
        [SerializeField] private List<NpcExpressionSet> eyesSet;
        [SerializeField] private List<NpcExpressionSet> mouthSet;
        [SerializeField] private float headgearChance;
        private TownDialogueParser townDialogueParser;

        private NpcPersonalityEnum[] personalities = new NpcPersonalityEnum[] {
            NpcPersonalityEnum.Health,
            NpcPersonalityEnum.Wealth,
            NpcPersonalityEnum.Love
        };

        private string[] firstNames;
        private string[] lastNames;
        private void Awake() {
            var firstNameJson = Resources.Load<TextAsset>("firstnames");
            var lastNameJson = Resources.Load<TextAsset>("lastnames");
            var cultistDialogueJson = Resources.Load<TextAsset>("town-cultist2");
            var genericDialogueJson = Resources.Load<TextAsset>("town-generalnpc2");
            firstNames = JsonConvert.DeserializeObject<string[]>(firstNameJson.text);
            lastNames = JsonConvert.DeserializeObject<string[]>(lastNameJson.text);
            this.townDialogueParser = new TownDialogueParser(
                genericDialogueJson.text,
                cultistDialogueJson.text
            );

            Resources.UnloadAsset(firstNameJson);
            Resources.UnloadAsset(lastNameJson);
            Resources.UnloadAsset(cultistDialogueJson);
            Resources.UnloadAsset(genericDialogueJson);
        }

        public NpcController GenerateNpc()
        {
            var npc = Instantiate(npcPrefab, GetSpawnPoint(), Quaternion.identity);
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
                mouthSet.GetRandom()
            );

            return npc;
        }

        private Vector2 GetSpawnPoint()
        {
            GraphNode targetNode;
            var maxTries = 20;
            do
            {
                var grid = AstarPath.active.data.gridGraph;

                targetNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
                maxTries -= 1;
            }
            while (!targetNode.Walkable || maxTries < 0);
            var worldPos = (Vector3)targetNode.position;
            return new Vector2(
                worldPos.x,
                worldPos.y
            );
        }
    }
}
