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

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
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

            nameText.text = dialogues[index].name;
            var text = dialogues[index].text;
            var curLetterIdx = 0;
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
            if (day < 3 && gameState.CultMembers.Count < 10)
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
                        text = $"Remember, I need you to recruit [10 people] and you have *{3 - day} days left]."
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
                        text = $"I'm disappointed in you, you've only recruited *{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "I will let it go this time, but make sure you recruit [20 people and have at least 5 high ranking cult members], or you will *face the consequnce]."
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
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for *{3 - day} days left] and you will earn your place in the world!"
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
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now for your next task, recruit [20 people and have at least 5 high ranking cult members]!"
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
                        text = $"I see you've only recruited *{gameState.CultMembers.Count} people] and there are only *{rank3Count} high ranking members in the cult]."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Remember, I need you to recruit [20 people and have at least 5 high ranking cult members] and you have *{3 - day} days left]."
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
                        text = $"I'm disappointed in you, you've only recruited *{gameState.CultMembers.Count} people] and there are only *{rank3Count} high ranking members in the cult]."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Now you have to *face the consequnce!]."
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
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] and there are @{rank3Count} high ranking members in the cult] to serve me."

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Keep this up for *{3 - day} days left] and you will earn your place in the world!"
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
                        text = $"Good job, you have recruited @{gameState.CultMembers.Count} people] and there are @{rank3Count} high ranking members in the cult] to serve me."

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
                nextSceneName = townSceneName;
            }
        }

        private void Week3Assesment(int day)
        {
            if (day < 3 && gameState.TreeStage < 3)
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
                        text = $"I need it to [grow bigger] to be a fitting vessel for me and you have *{3 - day} days left]."
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.TreeStage < 3)
            {
                // Warn scene
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
                nextSceneName = failedLeaderSceneName;
            }
            else if (day < 3 && gameState.TreeStage >= 3)
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
                        text = $"The more it grows, the stronger I will be! You have *{3 - day} days left]!"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!"
                    },
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.TreeStage >= 3)
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
                        text = $"The tree needs [5 sacrifices] and you've only sacrificed *{gameState.SacrificedMembers.Count} people, it is not enough!]"
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"You have *{3 - day} days left] to achieve this, don't disappoint me."
                    }
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.TreeStage < 3)
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
                nextSceneName = pacifistScene;
            }
            else if (day < 3 && gameState.TreeStage >= 3)
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
                        text = $"The more it grows, the stronger I will be! You have *{3 - day} days left]!"
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
                        text = $"Keep this up for *{3 - day} days left] and you will earn your place in the world!"

                    }
                };
                nextSceneName = townSceneName;
            }
            else if (day >= 3 && gameState.TreeStage >= 3)
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
                nextSceneName = ultimateCultScene;
            }
        }
    }
}