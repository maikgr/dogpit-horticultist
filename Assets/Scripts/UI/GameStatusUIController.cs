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

        private void OnEnable()
        {
            StartCoroutine(OnEnableCoroutine());
        }

        private IEnumerator OnEnableCoroutine()
        {
            while (TownEventBus.Instance == null)
            {
                yield return new WaitForFixedUpdate();
            }
            TownEventBus.Instance.OnObjectiveUpdate += OnObjectiveUpdate;
        }

        private void OnDisable()
        {
            TownEventBus.Instance.OnObjectiveUpdate -= OnObjectiveUpdate;
        }

        private void Start()
        {
            cultSizeText.text = $"Cult size: {currMemberCount} members";
        }

        private int currMemberCount;
        private float currTreeHeight;
        private void Update()
        {
            if (currMemberCount != GameStateController.Instance.CultMembers.Count)
            {
                currMemberCount = GameStateController.Instance.CultMembers.Count;
                cultSizeText.text = $"Cult size: {currMemberCount} members";
            }

            if (currTreeHeight != GameStateController.Instance.TreeHeight)
            {
                currTreeHeight = GameStateController.Instance.TreeHeight;
                var textVal = (currTreeHeight / 10).ToString("F2");
                vesselGrowthText.text = $"Tree Height: {textVal}m";
            }
        }

        private void OnObjectiveUpdate(IEnumerable<string> objectives)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var objective in objectives)
            {
                sb.Append("- " + objective);
                sb.AppendLine();
            }
            sb.Remove(sb.Length - 1, 1);
            objectivesText.text = sb.ToString();
        }
    }
}