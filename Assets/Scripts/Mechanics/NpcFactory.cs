namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
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

        [Header("Special NPC")]
        [SerializeField] private List<SpecialNpcMap> specialNpcMap;
        [SerializeField] private NpcBodySet radishgonBodySet;

        private NpcPersonalityEnum[] personalities = new NpcPersonalityEnum[] {
            NpcPersonalityEnum.Health,
            NpcPersonalityEnum.Wealth,
            NpcPersonalityEnum.Love
        };

        private NpcNameData firstNames;
        private NpcNameData lastNames;

        // WebGL can't access StreamingAssets
        private void Awake()
        {
            firstNames = JsonUtility.FromJson<NpcNameData>(DialogueAsset.FIRST_NAMES);
            lastNames = JsonUtility.FromJson<NpcNameData>(DialogueAsset.LAST_NAMES);
            this.townDialogueParser = new TownDialogueParser(
                DialogueAsset.GENERIC_DIALOGUES,
                DialogueAsset.CULTIST_DIALOGUES
            );
        }

        private void Start()
        {
            // Synchronize list of spawnable special NPCs
            specialNpcMap = specialNpcMap.Where(map => 
                    !GameStateController.Instance
                        .SpecialSpawnedThisWeek
                        .Any(spawned => spawned == map.SpecialNpcType)
                )
                .ToList();
        }

        public NpcController SpawnGenericNpc()
        {
            var npc = Instantiate(npcPrefab, GetSpawnPoint(), Quaternion.identity);
            var hasHeadgear = Random.Range(0f, 1f) < headgearChance;
            var firstName = firstNames.names.GetRandom();
            var lastName = lastNames.names.GetRandom();
            var personality = personalities.GetRandom();
            npc.ConfigureGeneric(
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

        public NpcController SpawnSpecialNpc(SpecialNpcTypeEnum specialNpcType)
        {
            var toSpawn = specialNpcMap.FirstOrDefault(map => map.SpecialNpcType == specialNpcType);
            var specNpc = Instantiate(toSpawn.npcPrefab, GetSpawnPoint(), Quaternion.identity);
            var specNpcController = specNpc.GetComponent<NpcController>();
            specNpcController.ConfigureSpecial(
                specialNpcType,
                toSpawn.personality,
                toSpawn.bodySet,
                townDialogueParser.GenerateSpecialDialogue(specialNpcType),
                townDialogueParser.GenerateCultistActions(specialNpcType.ToString(), toSpawn.personality),
                toSpawn.expressionSet
            );
            GameStateController.Instance.SpecialSpawnedThisWeek.Add(specialNpcType);
            return specNpcController;
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

    [System.Serializable]
    public class NpcNameData
    {
        public List<string> names;
    }

    [System.Serializable]
    public class SpecialNpcMap
    {
        public SpecialNpcTypeEnum SpecialNpcType;
        public NpcPersonalityEnum personality;
        public NpcBodySet bodySet;
        public NpcExpressionSet expressionSet;
        public NpcController npcPrefab;
    }
}
