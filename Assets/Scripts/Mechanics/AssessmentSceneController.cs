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
    using Horticultist.Scripts.Core;

    public class AssessmentSceneController : MonoBehaviour
    {
        [SerializeField] private Transform nameParent;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Transform dialogueParent;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private FadeUIController fadeUIController;
        private HorticultistInputActions gameInputs;
        private GameStateController gameState;
        private bool isTyping;
        private int currentIndex;
        private string nextSceneName;

        Coroutine c;
        bool coroutineHasStartedAtLeastOnce = false;

        private void Awake()
        {
            gameInputs = new HorticultistInputActions();
        }

        private void OnEnable()
        {
            gameInputs.UI.Click.performed += OnClickPerformed;
            gameInputs.UI.Click.Enable();

            nameParent.gameObject.SetActive(false);
            dialogueParent.gameObject.SetActive(false);
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
                    if (gameState.WeekNumber == 0)
                    {
                        Week1Assesment(gameState.DayNumber);
                    }
                    else if (gameState.WeekNumber == 1)
                    {
                        Week2Assesment(gameState.DayNumber);
                    }
                    else if (gameState.WeekNumber == 2)
                    {
                        Week3Assesment(gameState.DayNumber);
                    }
                    else if (gameState.WeekNumber == 3)
                    {
                        Week4Assesment(gameState.DayNumber);
                    }
                    c = StartCoroutine(ShowDialogue(currentIndex));
                    coroutineHasStartedAtLeastOnce = true;
                }
            );
        }

        private List<DialogueSceneText> dialogues = new List<DialogueSceneText>();

        private bool isBlockAction;
        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (isBlockAction) return;
            if (!coroutineHasStartedAtLeastOnce) return;
            if (context.ReadValue<float>() == 0) return;
            if (!isTyping)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex < dialogues.Count)
                {
                    c = StartCoroutine(ShowDialogue(nextIndex));
                    currentIndex = nextIndex;
                }
                else
                {
                    isBlockAction = true;
                    fadeUIController.FadeOutScreen(() => {
                        gameState.AddDay();
                        SceneManager.LoadScene(nextSceneName);
                    });
                }
            }
            else 
            {
                isTyping = false;
                StopCoroutine(c);
                dialogueText.text = dialogues[currentIndex].text.Replace("*", "<color=\"red\">").Replace("]", "</color>").Replace("[", "<color=\"blue\">").Replace("@", "<color=\"green\">");
            }
        }

        private IEnumerator ShowDialogue(int index)
        {
            isTyping = true;

            nameParent.gameObject.SetActive(true);
            nameText.text = dialogues[index].name;
            var text = dialogues[index].text;
            var curLetterIdx = 0;
            dialogueParent.gameObject.SetActive(true);
            dialogueText.text = string.Empty;

            var openTag = "";
            var closeTag = "";

            while (curLetterIdx < text.Length)
            {
                var c = text[curLetterIdx++];

                if (c == '*') 
                {
                    openTag = "<color=\"red\">";
                    closeTag = "</color>";

                }
                else if (c == '[') 
                {
                    openTag = "<color=\"blue\">";
                    closeTag = "</color>";
                }
                else if (c == '@') 
                {
                    openTag = "<color=\"green\">";
                    closeTag = "</color>";

                }
                else if (c == ']') 
                {
                    openTag = "";
                    closeTag = "";

                }
                else
                {
                    dialogueText.text += openTag + c + closeTag;
                }

                yield return new WaitForFixedUpdate();
            }
            isTyping = false;
        }

        private void Week1Assesment(int day)
        {
            if (day < gameState.DaysPerAssessment && gameState.CultMembers.Count < 6)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I see you've only recruited *{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Remember, I need you to recruit [6 people] and you have *{gameState.DaysPerAssessment - day} days left]."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            else if (day >= gameState.DaysPerAssessment && gameState.CultMembers.Count < 6)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I'm disappointed in you, you've only recruited *{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "I will let it go this time, but make sure you recruit [13 people and have at least 3 high ranking cult members], or you will *face the consequnce]."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            else if (day < gameState.DaysPerAssessment && gameState.CultMembers.Count >= 6)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for *{gameState.DaysPerAssessment - day} days left] and you will earn your place in the world!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            else if (day >= gameState.DaysPerAssessment && gameState.CultMembers.Count >= 6)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now for your next task, recruit [13 people and have at least 1 high ranking cult members]!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
            }
            nextSceneName = SceneNameConstant.TOWN_PLAZA;
        }

        private void Week2Assesment(int day)
        {
            var highRankCount = gameState.CultMembers.Where(mem =>
                    mem.CultistRank == Core.CultistRankEnum.Rank3 ||
                    mem.CultistRank == Core.CultistRankEnum.Rank2
                )
                .Count();
            if (day < gameState.DaysPerAssessment && (gameState.CultMembers.Count < 13 || highRankCount < 1))
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I see you've only recruited *{gameState.CultMembers.Count} people] and there are only *{highRankCount} high ranking members in the cult]."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Remember, I need you to recruit [13 people and have at least 1 high ranking cult members] and you have *{gameState.DaysPerAssessment - day} days left]."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
            else if (day >= gameState.DaysPerAssessment && (gameState.CultMembers.Count < 13 || highRankCount < 1))
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I'm disappointed in you, you've only recruited *{gameState.CultMembers.Count} people] and there are only *{highRankCount} high ranking members in the cult]."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to *face the consequnce!]."
                    }
                };
                nextSceneName = SceneNameConstant.ENDING_FAILED;
            }
            else if (day < gameState.DaysPerAssessment && gameState.CultMembers.Count >= 13 && highRankCount >= 1)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] and there are @{highRankCount} high ranking members in the cult] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for *{gameState.DaysPerAssessment - day} days left] and you will earn your place in the world!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
            else if (day >= gameState.DaysPerAssessment && gameState.CultMembers.Count >= 13 && highRankCount >= 1)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] and there are @{highRankCount} high ranking members in the cult] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now for your next task, I want [the tree to be grown] as a fitting vessel for me!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
        }

        private void Week3Assesment(int day)
        {
            if (day < gameState.DaysPerAssessment && gameState.TreeStage < 3)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The tree has only reached *{gameState.TreeHeight.ToString("F2")}m], it is not enough!"

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I need it to [grow bigger] to be a fitting vessel for me and you have *{gameState.DaysPerAssessment - day} days left]."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
            else if (day >= gameState.DaysPerAssessment && gameState.TreeStage < 3)
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You failed to grow tree, only reached *{gameState.TreeHeight.ToString("F2")}m]."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to *face the consequnce!]."
                    }
                };
                nextSceneName = SceneNameConstant.ENDING_FAILED;
            }
            else if (day < gameState.DaysPerAssessment && gameState.TreeStage >= 3)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have grown the tree to @{gameState.TreeHeight.ToString("F2")}m] which is big enough to be my vessel."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The more it grows, the stronger I will be! You have *{gameState.DaysPerAssessment - day} days left]!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
            else if (day >= gameState.DaysPerAssessment && gameState.TreeStage >= 3)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have grown the tree to @{gameState.TreeHeight.ToString("F2")}m] which is big enough to be my vessel."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now sacrifice [5 people] and you will rule the world together with me!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
        }

        private void Week4Assesment(int day)
        {
            if (day < gameState.DaysPerAssessment && gameState.SacrificedMembers.Count < 5)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The tree needs [5 sacrifices] and you've only sacrificed *{gameState.SacrificedMembers.Count} people, it is not enough!]"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You have *{gameState.DaysPerAssessment - day} days left] to achieve this, don't disappoint me."
                    }
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
            else if (day >= gameState.DaysPerAssessment && gameState.SacrificedMembers.Count < 5)
            {
                // Pacifist ending scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The tree needs [5 sacrifices] and you've only sacrificed <{gameState.SacrificedMembers.Count} people, it is not enough!]"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to *face the consequnce!]."

                    }
                };
                nextSceneName = SceneNameConstant.ENDING_PACIFIST;
            }
            else if (day < gameState.DaysPerAssessment && gameState.SacrificedMembers.Count >= 5)
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you have grown the tree to @{gameState.TreeHeight.ToString("F2")}m] which is big enough to be my vessel."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"The more it grows, the stronger I will be! You have *{gameState.DaysPerAssessment - day} days left]!"
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
                        text = $"Good job, you've sacrificed @{gameState.SacrificedMembers.Count} people], you are worthy to be my servant."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for *{gameState.DaysPerAssessment - day} days left] and you will earn your place in the world!"

                    }
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
            else if (day >= gameState.DaysPerAssessment && gameState.SacrificedMembers.Count >= 5)
            {
                // Ulltimate cult ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job, you've sacrificed @{gameState.SacrificedMembers.Count} people], you are worthy to be my servant."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You will earn your place in the world!"
                    }
                };
                nextSceneName = SceneNameConstant.ENDING_ULTIMATE;
            }
        }
    }
}