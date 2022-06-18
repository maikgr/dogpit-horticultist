namespace Horticultist.Scripts.Core
{
    public enum CultistObedienceLevelEnum
    {
        VeryRebellious = -2,
        Rebellious = -1,
        Neutral = 0,
        Obedient = 1,
        VeryObedient = 2
    }

    public static class CultistObedienceLevelEnumExtension
    {
        public static string DisplayString(this CultistObedienceLevelEnum level)
        {
            switch(level)
            {
                case CultistObedienceLevelEnum.Neutral:
                default:
                    return "Neutral";
                case CultistObedienceLevelEnum.VeryRebellious:
                    return "Very Rebellious";
                case CultistObedienceLevelEnum.Rebellious:
                    return "Rebellious";
                case CultistObedienceLevelEnum.Obedient:
                    return "Obedient";
                case CultistObedienceLevelEnum.VeryObedient:
                    return "Very Obedient";
            }
        }
    }
}