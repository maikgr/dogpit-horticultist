namespace Horticultist.Scripts.Core
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using Horticultist.Scripts.Extensions;
    using UnityEngine;

    public class TownDialogueParser
    {
        private CultistDialogueWrapper cultistDialogueWrapper;
        private GeneralNpcWrapper generalNpcWrapper;
        private const string NPC_PLACEHOLDER = "[NPC]";
        private List<string> SPECIAL_NPCS = new List<string> { "ash1", "persica1", "persica2", "uwu_demon", "radishgon", "alocasia1", "alocasia2" };
        private bool isParsed;

        public TownDialogueParser(string generalNpcJson, string cultistJson)
        {
            try
            {
                this.generalNpcWrapper = JsonUtility.FromJson<GeneralNpcWrapper>(generalNpcJson);
                this.cultistDialogueWrapper = JsonUtility.FromJson<CultistDialogueWrapper>(cultistJson);
                isParsed = true;
            }
            catch (Exception e)
            {
                isParsed = false;
                UnityEngine.Debug.LogError("Cannot read dialogues json");
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        public NpcDialogueSet GenerateDialogueSet(NpcPersonalityEnum personality)
        {
            if (!isParsed) return new NpcDialogueSet();
            var personalityString = string.Empty;
            switch(personality)
            {
                case NpcPersonalityEnum.Wealth:
                    personalityString = "wealth";
                    break;
                case NpcPersonalityEnum.Health:
                    personalityString = "health";
                    break;
                case NpcPersonalityEnum.Love:
                default:
                    personalityString = "love";
                    break;
            }

            var list = generalNpcWrapper.data.Single(g => g.personality == personalityString)
                .subsets
                .Where(p => !SPECIAL_NPCS.Contains(p.subset))
                .ToList();
            return list.GetRandom();
        }

        public List<CultistObedienceAction> GenerateCultistActions(string npcName, NpcPersonalityEnum personality)
        {
            if (!isParsed) return new List<CultistObedienceAction>();
            var miscDialogues = ParseCultistDialogue(npcName, "misc");

            switch(personality)
            {
                case NpcPersonalityEnum.Wealth:
                    return ParseCultistDialogue(npcName, "wealth")
                        .Concat(miscDialogues)
                        .OrderBy(_ => UnityEngine.Random.Range(0, 1f))
                        .ToList();
                case NpcPersonalityEnum.Health:
                    return ParseCultistDialogue(npcName, "health")
                        .Concat(miscDialogues)
                        .OrderBy(_ => UnityEngine.Random.Range(0, 1f))
                        .ToList();
                case NpcPersonalityEnum.Love:
                default:
                    return ParseCultistDialogue(npcName, "love")
                        .Concat(miscDialogues)
                        .OrderBy(_ => UnityEngine.Random.Range(0, 1f))
                        .ToList();
            }
        }

        private IEnumerable<CultistObedienceAction> ParseCultistDialogue(string npcName, string personality)
        {
            return cultistDialogueWrapper.data
                .SingleOrDefault(v => v.personality == personality)
                .texts
                .Select(textType => {
                    bool isEvent = textType.type == "event";
                    string text = textType.text;
                    var actionEnum = CultistObedienceActionEnum.Praise;
                    if (textType.answer == "scold") actionEnum = CultistObedienceActionEnum.Scold;
                    if (isEvent) text = ParseEventText(npcName, text);
                    
                    return new CultistObedienceAction(
                        isEvent,
                        text,
                        actionEnum
                    );
                });
        }

        private string ParseEventText(string npcName, string text)
        {
            var sb = new StringBuilder(text);
            sb.Replace(NPC_PLACEHOLDER, npcName);
            return sb.ToString();
        }
    }


    [Serializable]
    public class CultistDialogueWrapper
    {
        public List<CultistDialogue> data;
    }

    [Serializable]
    public class GeneralNpcWrapper
    {
        public List<GeneralNpcDialogue> data;
    }


    [Serializable]
    public struct GeneralNpcDialogue
    {
        public string personality;
        public List<NpcDialogueSet> subsets;
    }

    [Serializable]
    public struct NpcDialogueSet
    {
        public string subset;
        public List<string> visitor;
        public PersonalityTherapyDialogue therapy;
        public List<string> happy_person;
        public List<string> angry_person;
    }

    [Serializable]
    public struct PersonalityTherapyDialogue
    {
        public List<string> intro;
        public List<string> moodup;
        public List<string> moodup_misc;
        public List<string> mooddown;
        public List<string> mooddown_misc;
        public List<string> success;
        public List<string> unrecruited;
        public List<string> failure;
    }

    [Serializable]
    public struct CultistTextType
    {
        public string type;
        public string text;
        public string answer;
    }

    [Serializable]
    public struct CultistDialogue
    {
        public string personality;
        public List<CultistTextType> texts;
    }

    [Serializable]
    public struct CultistObedienceAction
    {
        public bool isEvent;
        public string text;
        public CultistObedienceActionEnum action;

        public CultistObedienceAction(bool isEvent, string text, CultistObedienceActionEnum action)
        {
            this.isEvent = isEvent;
            this.text = text;
            this.action = action;
        }
    }
}