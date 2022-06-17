namespace Horticultist.Scripts.Extensions
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public static class ListExtensions 
    {
        public static T GetRandom<T>(this List<T> list)
        {
            var index = Random.Range(0, list.Count);
            return list[index];
        }

        public static T GetRandom<T>(this T[] list)
        {
            var index = Random.Range(0, list.Length);
            return list[index];
        }
    }
}