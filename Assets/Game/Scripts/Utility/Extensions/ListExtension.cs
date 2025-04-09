using System.Collections.Generic;
using UnityEngine;

namespace Garawell.Extensions
{
    public static class ListExtension
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1); //maxValue (i + 1) is EXCLUSIVE
                list.Swap(i, randomIndex);
            }
            return list;
        }

        public static void Swap<T>(this List<T> list, int indexA, int indexB)
        {
            var temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }

        public static T EC_Remove<T>(this List<T> list, T objToRemove)
        {
            list.Remove(objToRemove);
            return objToRemove;
        }
    }
}

