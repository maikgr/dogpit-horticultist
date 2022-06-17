namespace Horticultist.Scripts.Core
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Horticultist.Scripts.Extensions;

    public class TownDialogueParser
    {
        private CultistDialogue[] cultistDialogues;
        private GeneralNpcDialogue[] generalnpcDialogues;
        private const string NPC_PLACEHOLDER = "[NPC]";
        private static readonly string[] SPECIAL_NPCS = new string[] { "ash1" };

        public TownDialogueParser(string generalNpcJson, string cultistJson)
        {
            this.generalnpcDialogues = JsonConvert.DeserializeObject<GeneralNpcDialogue[]>(generalNpcJson);
            this.cultistDialogues = JsonConvert.DeserializeObject<CultistDialogue[]>(cultistJson);
        }

        public NpcDialogueSet GenerateDialogueSet(NpcPersonalityEnum personality)
        {
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

            var list = generalnpcDialogues.Single(g => g.Personality == personalityString)
                .Subsets
                .Where(p => !SPECIAL_NPCS.Contains(p.Subset))
                .ToList();
            UnityEngine.Debug.Log(string.Join(", ", list));
            return list.GetRandom();
        }

        public List<CultistObedienceAction> GenerateCultistActions(string npcName, NpcPersonalityEnum personality)
        {
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
            return cultistDialogues
                .SingleOrDefault(v => v.Personality == personality)
                .Texts
                .Select(textType => {
                    bool isEvent = textType.Type == "event";
                    string text = textType.Text;
                    var actionEnum = CultistObedienceActionEnum.Praise;
                    if (textType.Answer == "scold") actionEnum = CultistObedienceActionEnum.Scold;
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

    public struct GeneralNpcDialogue
    {
        public string Personality { get; set; }
        public NpcDialogueSet[] Subsets { get; set; }
    }

    public struct NpcDialogueSet
    {
        public string Subset { get; set; }
        public string[] Visitor { get; set; }
        public PersonalityTherapyDialogue Therapy { get; set; }
        public string[] Happy_person { get; set; }
        public string[] Angry_person { get; set; }
    }

    public struct PersonalityTherapyDialogue
    {
        public string[] Intro { get; set; }
        public string[] Moodup { get; set; }
        public string[] Mooddown { get; set; }
        public string[] Success { get; set; }
        public string[] Unrecruited { get; set; }
        public string[] Failure { get; set; }
    }

    public struct CultistTextType
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
    }

    public struct CultistDialogue
    {
        public string Personality { get; set; }
        public CultistTextType[] Texts { get; set; }
    }

    public struct CultistObedienceAction
    {
        public bool IsEvent { get; }
        public string Text { get; }
        public CultistObedienceActionEnum Action { get; }

        public CultistObedienceAction(bool isEvent, string text, CultistObedienceActionEnum action)
        {
            this.IsEvent = isEvent;
            this.Text = text;
            this.Action = action;
        }
    }
}