namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using TMPro;
    using DG.Tweening;
    using Horticultist.Scripts.UI;
    using Horticultist.Scripts.Core;

    public class TreeGrowthSceneController : MonoBehaviour
    {
        [SerializeField] private FadeUIController fadeUIController;
        [SerializeField] private TMP_Text GrowthText;
        [SerializeField] private List<TreeVesselStage> stageValueThreshold;
        [SerializeField] private PolygonCollider2D area;
        [SerializeField] private FakeNpcController fakeNpcPrefab;

        private bool sceneEnds = false;
        private bool isChangingScene = false;
        private GameStateController gameState;
        private List<FakeNpcController> fakeNpcList = new List<FakeNpcController>();

        private void Start()
        {
            gameState = GameStateController.Instance;
            gameState.PrevScene = SceneNameConstant.TREE_GROWTH;

            UpdateGrowth(gameState.TreeHeight);

            // Skip if there's no cult members
            SetCultistTreePosition();
            fadeUIController.FadeInScreen(() => {
                if (gameState.CultMembers.Count > 0)
                {
                    GrowthScene();
                }
                else
                {
                    ChangeScene();
                }
            });
        }
        

        private void SetCultistTreePosition()
        {
            gameState.CultMembers.ForEach(npc => {
                var fakeNpcObj = Instantiate(fakeNpcPrefab, GetRandomTreePosition(), Quaternion.identity);
                var fakeNpc = fakeNpcObj.GetComponent<FakeNpcController>();
                fakeNpc.Configure(npc);
                fakeNpcList.Add(fakeNpc);
            });
        }

        private void GrowthScene()
        {
            var totalEf = gameState.CultMembers.Sum(m => m.EfficiencyValue);
            var animGuid = System.Guid.NewGuid().ToString();
            var sequence = DOTween.Sequence();

            // Tree animation
            sequence.SetDelay(1.5f)
                .Append(
                    DOVirtual.Float(
                        gameState.TreeHeight,
                        gameState.TreeHeight + totalEf,
                        3f,
                        (height) => {
                            UpdateGrowth(height);
                        }
                    )
                );
            sequence.SetId(animGuid);

            // Cult members animation
            fakeNpcList.ForEach((npc) =>
            {
                npc.StartAbsorbAnimation(() => {
                    DOTween.Kill(animGuid);
                    sceneEnds = true;
                });
            });
        }

        private void UpdateGrowth(float height)
        {
            var textVal = (height / 10).ToString("F2");
            GrowthText.text = $"{textVal} m";
            
            // Disable all tree sprites
            stageValueThreshold.ForEach(val => val.TreeGameObject.SetActive(false));

            // Calculate tree growth
            var treeStage = stageValueThreshold.Where(val => val.Threshold <= height)
                .OrderByDescending(val => val.Threshold)
                .First();
            
            // Enable matching tree stage
            treeStage.TreeGameObject.SetActive(true);

            gameState.SetTreeStatus(treeStage.Stage, height);
        }

        private void FixedUpdate()
        {
            if (sceneEnds && !isChangingScene)
            {
                isChangingScene = true;
                ChangeScene();
            }
        }

        private void ChangeScene()
        {
            // Assess player actions by the end of every week
            fadeUIController.FadeOutScreen(() => {
                if (gameState.DayNumber >= gameState.DaysPerAssessment)
                {
                    SceneManager.LoadScene(SceneNameConstant.ASSESSMENT);
                }
                else
                {
                    gameState.AddDay();
                    SceneManager.LoadScene(SceneNameConstant.TOWN_PLAZA);
                }
            });
        }

        private Vector2 GetRandomTreePosition()
        {
            var pos = Vector2.zero;
            do
            {
                pos = new Vector2(
                    Random.Range(area.bounds.min.x, area.bounds.max.x),
                    Random.Range(area.bounds.min.y, area.bounds.max.y)
                );
            } while (!area.OverlapPoint(pos));
            return pos;
        }
    }
}