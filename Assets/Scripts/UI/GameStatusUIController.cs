namespace Horticultist.Scripts.UI
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Horticultist.Scripts.Mechanics;

    public class GameStatusUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text vesselGrowthText;
        [SerializeField] private TMP_Text cultSizeText;
        [SerializeField] private TMP_Text objectivesText;
        private int cultSize;

        private void OnEnable() {
            StartCoroutine(OnEnableCoroutine());
        }

        private IEnumerator OnEnableCoroutine()
        {
            while(TownEventBus.Instance == null)
            {
                yield return new WaitForFixedUpdate();
            }
            TownEventBus.Instance.OnTreeGrowthChange += OnTreeGrowthChange;
            TownEventBus.Instance.OnCultistJoin += OnCultistJoin;
            TownEventBus.Instance.OnCultistLeave += OnCultistLeave;
            TownEventBus.Instance.OnObjectiveUpdate += OnObjectiveUpdate;
        }

        private void OnDisable() {
            TownEventBus.Instance.OnTreeGrowthChange -= OnTreeGrowthChange;
            TownEventBus.Instance.OnCultistJoin -= OnCultistJoin;
            TownEventBus.Instance.OnCultistLeave -= OnCultistLeave;
            TownEventBus.Instance.OnObjectiveUpdate -= OnObjectiveUpdate;
        }

        private void Start() {
            cultSizeText.text = "Cult size: 0 members";
            vesselGrowthText.text = "Tree height: 0.1m";
        }

        private void OnTreeGrowthChange(float value, int stage)
        {
            var textVal = (value/10).ToString("F2");
            vesselGrowthText.text = $"Tree Height: {textVal}m";
        }

        private void OnCultistJoin(NpcController npc)
        {
            cultSize += 1;
            cultSizeText.text = $"Cult size: {cultSize} members";
        }
        private void OnCultistLeave(NpcController npc)
        {
            cultSize -= 1;
            cultSizeText.text = $"Cult size: {cultSize} members";
        }

        private void OnObjectiveUpdate(IEnumerable<string> objectives)
        {
            var sb = new System.Text.StringBuilder();
            foreach(var objective in objectives)
            {
                sb.Append("- " + objective);
                sb.AppendLine();
            }
            sb.Remove(sb.Length - 1, 1);
            objectivesText.text = sb.ToString();
        }
    }
}