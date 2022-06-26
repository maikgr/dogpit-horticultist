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

        private void Start()
        {
            gameState = GameStateController.Instance;
            gameState.PrevScene = SceneNameConstant.TREE_GROWTH;

            // Disable all tree sprites
            stageValueThreshold.ForEach(val => val.TreeGameObject.SetActive(false));
            UpdateGrowth(gameState.TreeHeight);

            // Skip if there's no cult members
            fadeUIController.FadeInScreen(() => {
                if (gameState.CultMembers.Count > 0)
                {
                    SetCultistTreePosition();
                    StartCoroutine(GrowthScene());
                }
                else
                {
                    ChangeScene();
                }
            });

            fakeNpcList = new List<FakeNpcController>();
        }
        

        private List<FakeNpcController> fakeNpcList;
        private void SetCultistTreePosition()
        {
            gameState.CultMembers.ForEach(npc => {
                Debug.Log("instantiating " + npc.DisplayName);
                var fakeNpcObj = Instantiate(fakeNpcPrefab, GetRandomTreePosition(), Quaternion.identity);
                var fakeNpc = fakeNpcObj.GetComponent<FakeNpcController>();
                fakeNpc.Configure(npc);
                fakeNpcList.Add(fakeNpc);
            });
        }

        private IEnumerator GrowthScene()
        {
            while (fakeNpcList.Count != gameState.CultMembers.Count)
            {
                yield return new WaitForFixedUpdate();
            }
            var totalEf = gameState.CultMembers.Sum(m => m.EfficiencyValue);
            var animGuid = System.Guid.NewGuid().ToString();
            var sequence = DOTween.Sequence();

            // Tree animation
            Debug.Log("delay tree");
            sequence.SetDelay(1.5f)
                .Append(
                    DOVirtual.Float(
                        gameState.TreeHeight,
                        gameState.TreeHeight + totalEf,
                        3f,
                        (height) => {
                            Debug.Log("growing tree");
                            GrowthText.text = $"{height.ToString("F2")} m";
                            UpdateGrowth(height);
                        }
                    )
                );
            sequence.SetId(animGuid);

            Debug.Log("absorbing");
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
            // Calculate tree growth
            var treeStage = stageValueThreshold.Where(val => val.Threshold <= height)
                .OrderByDescending(val => val.Threshold)
                .First();
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
            Debug.Log("changing scene");
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
            Debug.Log("find random pos");
            do
            {
                pos = new Vector2(
                    Random.Range(area.bounds.min.x, area.bounds.max.x),
                    Random.Range(area.bounds.min.y, area.bounds.max.y)
                );
            } while (!area.OverlapPoint(pos));
            Debug.Log("Get random pos");
            return pos;
        }
    }
}