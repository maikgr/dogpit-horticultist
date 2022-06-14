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

        [Header("Spawn Area")]
        [SerializeField] private Vector2 minAreaPoint;
        [SerializeField] private Vector2 maxAreaPoint;
        private DefaultInputActions defaultInput;
        private InputAction inputAction;

        private string[] firstNames;
        private string[] lastNames;
        private void Awake() {
            firstNames = JsonConvert.DeserializeObject<string[]>(firstNameJson.text);
            lastNames = JsonConvert.DeserializeObject<string[]>(lastNameJson.text);
            defaultInput = new DefaultInputActions();
        }

        private void OnEnable() {
            inputAction = defaultInput.Player.Fire;
            inputAction.Enable();

            defaultInput.Player.Fire.performed += OnInputPerformed;
            defaultInput.Player.Fire.Enable();
        }

        private void OnDisable() {
            defaultInput.Player.Fire.Disable();
            inputAction.Disable();
        }

        private void OnInputPerformed(InputAction.CallbackContext context)
        {
        }

        public void GenerateNPC()
        {
            var npc = Instantiate(npcPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            var hasHeadgear = Random.Range(0f, 1f) < 0.5f;
            npc.GenerateVisitor(
                firstNames.GetRandom(), lastNames.GetRandom(),
                bodySprites.GetRandom(), hasHeadgear ? headgearSprites.GetRandom() : null
            );
        }

        private Vector2 GetRandomSpawnPoint()
        {
            var posX = Random.Range(minAreaPoint.x, maxAreaPoint.x);
            var posY = Random.Range(minAreaPoint.y, maxAreaPoint.y);

            return new Vector2(posX, posY);
        }
    }
}
