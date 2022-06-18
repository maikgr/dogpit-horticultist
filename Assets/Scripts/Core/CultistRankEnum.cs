namespace Horticultist.Scripts.Core
{
    public enum CultistRankEnum
    {
        Rank1,
        Rank2,
        Rank3
    }
    
    public static class CultistRankEnumExtension
    {
        public static string DisplayString(this CultistRankEnum rank)
        {
            switch(rank)
            {
                case CultistRankEnum.Rank1:
                default:
                    return "Newbud";
                case CultistRankEnum.Rank2:
                    return "Horticultist";
                case CultistRankEnum.Rank3:
                    return "High Tomatholyte";
            }
        }
    }
}