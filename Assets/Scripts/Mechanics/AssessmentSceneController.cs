namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using TMPro;
    using Horticultist.Scripts.UI;

    public class AssessmentSceneController : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private FadeUIController fadeUIController;
        [SerializeField] private string townSceneName;
        [SerializeField] private string failedLeaderSceneName;
        [SerializeField] private string pacifistScene;
        [SerializeField] private string ultimateCultScene;
        private HorticultistInputActions gameInputs;
        private GameStateController gameState;
        private bool isTyping;
        private int currentIndex;
        private string nextSceneName;

        private void Awake()
        {
            gameInputs = new HorticultistInputActions();
        }

        private void OnEnable()
        {
            gameInputs.UI.Click.performed += OnClickPerformed;
            gameInputs.UI.Click.Enable();
        }

        private void OnDisable()
        {
            gameInputs.UI.Click.performed -= OnClickPerformed;
            gameInputs.UI.Click.Disable();
        }

        private void Start()
        {
            gameState = GameStateController.Instance;
            fadeUIController.FadeInScreen(
                () =>
                {
                    if (gameState.weekNumber == 0)
                    {
                        Week1Assesment(gameState.dayNumber);
                    }
                    else if (gameState.weekNumber == 1)
                    {
                        Week2Assesment(gameState.dayNumber);
                    }
                    else if (gameState.weekNumber == 2)
                    {
                        Week3Assesment(gameState.dayNumber);
                    }
                    else if (gameState.weekNumber == 3)
                    {
                        Week4Assesment(gameState.dayNumber);
                    }
                    StartCoroutine(ShowDialogue(currentIndex));
                }
            );
        }

        private List<DialogueSceneText> dialogues = new List<DialogueSceneText>();

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() == 0) return;
            if (!isTyping)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex < dialogues.Count)
                {
                    StartCoroutine(ShowDialogue(nextIndex));
                    currentIndex = nextIndex;
                }
                else
                {
                    fadeUIController.FadeOutScreen(() => {
                        gameState.dayNumber += 1;
                        if(gameState.dayNumber > 3)
                        {
                            gameState.weekNumber += 1;
                            gameState.dayNumber = 1;
                        }
                        SceneManager.LoadScene(nextSceneName);
                    });
                }
            }
        }

        private IEnumerator ShowDialogue(int index)
        {
            nameText.text = dialogues[index].name;
            var text = dialogues[index].text;
            var curLetterIdx = 0;
            dialogueText.text = string.Empty;
            while (curLetterIdx < text.Length)
            {
                isTyping = true;
                dialogueText.text += text[curLetterIdx++];
                yield return new WaitForFixedUpdate();
            }
            isTyping = false;
        }

        private void Week1Assesment(int day)
        {
            if (day < 3 && gameState.CultMembers.Count < 10)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I see you've only recruited <color=\"red\">{gameState.CultMembers.Count} people</color> to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Remember, I need you to recruit <color=\"blue\">10 people</color> and you have <color=\"red\">{day - 3} days left</color>."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            else if (day >= 3 && gameState.CultMembers.Count < 10)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I'm disappointed in you, you've only recruited <color=\"red\">{gameState.CultMembers.Count} people</color> to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "I will let it go this time, but make sure you recruit <color=\"blue\">20 people and have at least 5 high ranking cult members</color>, or you will <color=\"red\">face the consequnce</color>."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            else if (day < 3 && gameState.CultMembers.Count >= 10)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited <color=\"green\">{gameState.CultMembers.Count} people</color> to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for<color=\"red\">{day - 3} days left</color> and you will earn your place in the world!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            else if (day >= 3 && gameState.CultMembers.Count >= 10)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited <color=\"green\">{gameState.CultMembers.Count} people</color> to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now for your next task, recruit <color=\"blue\">20 people and have at least 5 high ranking cult members</color>!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            nextSceneName = townSceneName;
        }

        private void Week2Assesment(int day)
        {
            var rank3Count = gameState.CultMembers.Where(mem => mem.CultistRank == Core.CultistRankEnum.Rank3).Count();
            if (day < 3 && (gameState.CultMembers.Count < 20 || rank3Count < 5))
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I see you've only recruited <color=\"red\">{gameState.CultMembers.Count} people</color> and there are only <color=\"red\">{rank3Count} high ranking members in the cult</color>."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Remember, I need you to recruit <color=\"blue\">20 people and have at least 5 high ranking cult members</color> and you have <color=\"red\">{day - 3} days left</color>."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && (gameState.CultMembers.Count < 20 || rank3Count < 5))
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I'm disappointed in you, you've only recruited <color=\"red\">{gameState.CultMembers.Count} people</color> and there are only <color=\"red\">{rank3Count} high ranking members in the cult</color>."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to <color=\"red\">face the consequnce!</color>."
                    }
                };
                nextSceneName = failedLeaderSceneName;
            }
            else if (day < 3 && gameState.CultMembers.Count >= 20 && rank3Count >= 5)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited <color=\"green\">{gameState.CultMembers.Count} people</color> and there are <color=\"green\">{rank3Count} high ranking members in the cult</color> to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for<color=\"red\">{day - 3} days left</color> and you will earn your place in the world!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.CultMembers.Count >= 20 && rank3Count >= 5)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited<color=\"green\">{gameState.CultMembers.Count} people</color> and there are <color=\"green\">{rank3Count} high ranking members in the cult</color> to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now for your next task, I want <color=\"blue\">the tree to be grown</color> as a fitting vessel for me!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
        }

        private void Week3Assesment(int day)
        {
            if (day < 3 && gameState.treeStage < 3)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The tree has only reached <color=\"red\">{gameState.treeHeight.ToString("F2")}m</color>, it is not enough!"

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I need it to <color=\"blue\">grow bigger</color> to be a fitting vessel for me and you have <color=\"red\">{day - 3} days left</color>."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.treeStage < 3)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You failed to grow tree, only reached <color=\"red\">{gameState.treeHeight.ToString("F2")}m</color>."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to <color=\"red\">face the consequnce!</color>."
                    }
                };
                nextSceneName = failedLeaderSceneName;
            }
            else if (day < 3 && gameState.treeStage >= 3)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have grown the tree to <color=\"green\">{gameState.treeHeight.ToString("F2")}m</color> which is big enough to be my vessel."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The more it grows, the stronger I will be! You have <color=\"red\">{day - 3} days left</color>!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.treeStage >= 3)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have grown the tree to <color=\"green\">{gameState.treeHeight.ToString("F2")}m</color> which is big enough to be my vessel."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now sacrifice <color=\"blue\">5 people</color> and you will rule the world together with me!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
        }

        private void Week4Assesment(int day)
        {
            if (day < 3 && gameState.SacrificedMembers.Count < 5)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The tree needs <color=\"blue\">5 sacrifices</color> and you've only sacrificed <color=\"red\">{gameState.SacrificedMembers.Count} people, it is not enough!</color>"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You have <color=\"red\">{day - 3} days left</color> to achieve this, don't disappoint me."

                    }
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.treeStage < 3)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The tree needs <color=\"blue\">5 sacrifices</color> and you've only sacrificed <color=\"red\">{gameState.SacrificedMembers.Count} people, it is not enough!</color>"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to <color=\"red\">face the consequnce!</color>."

                    }
                };
                nextSceneName = pacifistScene;
            }
            else if (day < 3 && gameState.treeStage >= 3)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have grown the tree to <color=\"green\">{gameState.treeHeight.ToString("F2")}m</color> which is big enough to be my vessel."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The more it grows, the stronger I will be! You have <color=\"red\">{day - 3} days left</color>!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you've sacrificed <color=\"green\">{gameState.SacrificedMembers.Count} people</color>, you are worthy to be my servant."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for<color=\"red\">{day - 3} days left</color> and you will earn your place in the world!"

                    }
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.treeStage >= 3)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you've sacrificed <color=\"green\">{gameState.SacrificedMembers.Count} people</color>, you are worthy to be my servant."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You will earn your place in the world!"
                    }
                };
                nextSceneName = ultimateCultScene;
            }
        }
    }
}