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
        private GameStateController gameState;
        private bool isTyping;
        private int currentIndex;
        private string nextSceneName;
        private int CULT_SIZE_WEEK_1 = 6;
        private int CULT_SIZE_WEEK_2 = 13;
        private int HIGH_RANK_SIZE_WEEK_2 = 1;
        private float TREE_HEIGHT_WEEK_3 = 550f;
        private int SACRIFICE_INCREMENT_COUNT_WEEK_4 = 2;
        private int sacrificeRequirementCount = 2;
        private List<DialogueSceneText> dialogues = new List<DialogueSceneText>();
        private bool isBlockAction;

        Coroutine c;
        bool coroutineHasStartedAtLeastOnce = false;


        private void OnEnable()
        {
            nameParent.gameObject.SetActive(false);
            dialogueParent.gameObject.SetActive(false);
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

        public void NextDialogue()
        {
            if (isBlockAction) return;
            if (!coroutineHasStartedAtLeastOnce) return;
            if (!isTyping)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex < dialogues.Count)
                {
                    c = StartCoroutine(ShowDialogue(nextIndex));
                    SfxController.Instance.PlaySfx(dialogues[nextIndex].sfxName);
                    currentIndex = nextIndex;
                }
                else
                {
                    NextScene();
                }
            }
            else
            {
                isTyping = false;
                StopCoroutine(c);
                dialogueText.text = ParseDialogueFormatting(dialogues[currentIndex].text);
            }
        }
        
        public void PrevDialogue()
        {
            if (isBlockAction) return;
            if (!coroutineHasStartedAtLeastOnce) return;
            if (!isTyping)
            {
                var prevIndex = currentIndex - 1;
                if (prevIndex >= 0)
                {
                    c = StartCoroutine(ShowDialogue(prevIndex));
                    currentIndex = prevIndex;
                }
            }
            else
            {
                isTyping = false;
                StopCoroutine(c);
                dialogueText.text = ParseDialogueFormatting(dialogues[currentIndex].text);
            }
        }

        public void NextScene()
        {
            isBlockAction = true;
            fadeUIController.FadeOutScreen(() =>
            {
                gameState.AddDay();
                SceneManager.LoadScene(nextSceneName);
            });
        }

        private string ParseDialogueFormatting(string text)
        {
            return text.Replace("*", "<color=\"red\">")
                .Replace("]", "</color>")
                .Replace("[", "<color=\"blue\">")
                .Replace("@", "<color=\"green\">");
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
            if (gameState.CultMembers.Count < CULT_SIZE_WEEK_1)
            {
                // Warn scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I'm disappointed. You've only recruited *{gameState.CultMembers.Count} people] into our family...",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I will let you off for now, but make sure you recruit [{CULT_SIZE_WEEK_2} people and have at least {CULT_SIZE_WEEK_2} high ranking cult members] by Day {gameState.DaysPerAssessment * 2 + 1}, or you will *face the consequences]. Do not test My patience.",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!",
                        sfxName = SfxEnum.VLGetBackToWork
                    },
                };
            }
            else
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                     new DialogueSceneText {
                        // [PLAYER]
                        name = "Tomathotep",
                        text = $"Good job. Our family is now @{gameState.CultMembers.Count}] members strong. It seems I have chosen a good leader.",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"But our task is not yet complete… you must have a total of at least [{CULT_SIZE_WEEK_2} people recruited and have at least {HIGH_RANK_SIZE_WEEK_2} high ranking cult members] by Day {gameState.DaysPerAssessment * 2 + 1} to further fuel My vessel’s growth.",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!",
                        sfxName = SfxEnum.VLGetBackToWork
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

            if (gameState.CultMembers.Count < CULT_SIZE_WEEK_2 && highRankCount < HIGH_RANK_SIZE_WEEK_2)
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        // [PLAYER]
                        text = $"Only *{gameState.CultMembers.Count} people] and *{highRankCount} high ranking members]? I’m sorely disappointed. I thought I told you not to test My patience.",
                        sfxName = SfxEnum.None,

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now… you will have to *suffer the consequences!]",
                        sfxName = SfxEnum.None,

                    },
                };
                gameState.EndingType = EndingTypeEnum.FailedLeaderEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
            else if (highRankCount < HIGH_RANK_SIZE_WEEK_2)
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        // [PLAYER]
                        text = $"Only @{gameState.CultMembers.Count} people] and *{highRankCount} high ranking members]? I’m sorely disappointed. I thought I told you not to test My patience.",
                        sfxName = SfxEnum.None,

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now… you will have to *suffer the consequences!]",
                        sfxName = SfxEnum.None,

                    },
                };
                gameState.EndingType = EndingTypeEnum.FailedLeaderEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
            else if (gameState.CultMembers.Count < CULT_SIZE_WEEK_2)
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        // [PLAYER]
                        text = $"Only *{gameState.CultMembers.Count} people] and @{highRankCount} high ranking members]? I’m sorely disappointed. I thought I told you not to test My patience.",
                        sfxName = SfxEnum.None,

                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now… you will have to *suffer the consequences!]",
                        sfxName = SfxEnum.None,

                    },
                };
                gameState.EndingType = EndingTypeEnum.FailedLeaderEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
            else
            {
                // Praise scene
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        // [PLAYER]
                        text = $"Good job. You have recruited @{gameState.CultMembers.Count} people] and there are @{highRankCount} high ranking members in our family].",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"But do not forget our true goal. I want you to grow our dear vessel further!",
                        sfxName = SfxEnum.VlMyVessel
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I expect the vessel to have [reached at least {TREE_HEIGHT_WEEK_3/10 }m when I check in again on Day {gameState.DaysPerAssessment * 3 + 1}",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Now get back to work!",
                        sfxName = SfxEnum.VLGetBackToWork
                    },
                };
                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
        }

        private void Week3Assesment(int day)
        {
            if (gameState.TreeHeight < TREE_HEIGHT_WEEK_3)
            {
                // Failed leader ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        // [PLAYER]
                        text = $"Oh... You have failed Me. Our poor vessel has only grown to *{(gameState.TreeHeight / 10).ToString("F2")}m].",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Well… I suppose I can do it Myself. You, on the other hand… will have to *face the consequences of your negligence!].",
                        sfxName = SfxEnum.None,
                    },
                };
                gameState.EndingType = EndingTypeEnum.FailedLeaderEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
            else
            {
                // Praise scene
                sacrificeRequirementCount = gameState.SacrificedMembers.Count + SACRIFICE_INCREMENT_COUNT_WEEK_4;
                
                if (gameState.SacrificedMembers.Count == 0) {

                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Good job. You have grown our vessel to @{(gameState.TreeHeight / 10).ToString("F2")}m]. Yes, it is good enough, and the time is almost right!",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        // [PLAYER]
                        text = $"Now for one final step. I can feel the vessel’s thirst. It desires the blood of the faithful.",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"So far, you've sacrified {gameState.SacrificedMembers.Count} people...",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Impressive that you have come this far with no sacrifices. However, it is time to truly commit to Me.",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"To sate the vessel's appetite you'll have to sacrifice at least[{sacrificeRequirementCount} people].",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"I will be checking in on Day {gameState.DaysPerAssessment * 4 + 1}",
                        sfxName = SfxEnum.VLDoNotDisappointMe
                    },
                };
                } else {

                    dialogues = new List<DialogueSceneText>
                    {
                        new DialogueSceneText {
                            name = "Tomathotep",
                            text = $"Good job. You have grown our vessel to @{(gameState.TreeHeight / 10).ToString("F2")}m]. Yes, it is good enough, and the time is almost right!"
                        },
                        new DialogueSceneText {
                            name = "Tomathotep",
                            // [PLAYER]
                            text = $"Now for one final step. I can feel the vessel’s thirst. It desires the blood of the faithful.",
                            sfxName = SfxEnum.VlMyVessel
                        },
                        new DialogueSceneText {
                            name = "Tomathotep",
                            text = $"So far, you've sacrified {gameState.SacrificedMembers.Count} people..."
                        },
                        new DialogueSceneText {
                            name = "Tomathotep",
                            text = $"To sate its appetite you'll have to sacrifice at least [{sacrificeRequirementCount} people]."
                        },
                        new DialogueSceneText {
                            name = "Tomathotep",
                            text = "Since you've already done this before, this should be fairly easy to accomplish."
                        },
                        new DialogueSceneText {
                            name = "Tomathotep",
                            text = $"I will be checking in on Day {gameState.DaysPerAssessment * 4 + 1}",
                            sfxName = SfxEnum.VLDoNotDisappointMe
                        },
                    };
                }
                // Praise scene

                nextSceneName = SceneNameConstant.TOWN_PLAZA;
            }
        }

        private void Week4Assesment(int day)
        {
            if (gameState.SacrificedMembers.Count < 1)
            {
                // Pacifist ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Our vessel is starving. It needs [{sacrificeRequirementCount} sacrifices], and you've only fed it…",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "W-What is this? You haven't fed it at all?",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "You… You didn't sacrifice anybody…?",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "How…? And more importantly, why? You are not betraying Me, but yet you refuse to kill your brethren in My name…",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "...",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "… I suppose this is not completely unacceptable.",
                        sfxName = SfxEnum.None,
                    },
                };
                gameState.EndingType = EndingTypeEnum.PacifistEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
            else if (gameState.SacrificedMembers.Count < sacrificeRequirementCount)
            {
                // Pacifist slight difference in text
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Our vessel is starving. It needs [{sacrificeRequirementCount} sacrifices], and you've only fed it *{gameState.SacrificedMembers.Count}]",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "...",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Why?",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "Why? You are not betraying Me, but yet you refuse to kill more of your brethren in My name…",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "...",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = "… I suppose this is not completely unacceptable.",
                        sfxName = SfxEnum.None,
                    },
                };
                gameState.EndingType = EndingTypeEnum.PacifistEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
            else
            {
                // Ulltimate cult ending
                dialogues = new List<DialogueSceneText>
                {
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Well done. You've sacrificed @{gameState.SacrificedMembers.Count} people] to sate our vessel’s appetite. Our efforts will soon bear fruit!",
                        sfxName = SfxEnum.None,
                    },
                    new DialogueSceneText {
                        name = "Tomathotep",
                        text = $"Let us usher in our era of paradise, the age of Tomathotep!",
                        sfxName = SfxEnum.VLExcellent
                    }
                };
                gameState.EndingType = EndingTypeEnum.UltimateCultEnding;
                nextSceneName = SceneNameConstant.ENDING;
            }
        }
    }
}