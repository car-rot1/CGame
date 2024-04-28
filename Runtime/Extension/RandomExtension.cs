using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CGame
{
    public static class RandomExtension
    {
        public static int[] MultiRange(int min, int max, int num, bool isRepeat = false)
        {
            if (num <= 0)
                return Array.Empty<int>();
            
            if (isRepeat)
            {
                var result = new int[num];
                for (var i = 0; i < num; i++)
                    result[i] = Random.Range(min, max);
                return result;
            }

            if (max - min <= num + num)
            {
                var randomValues = new List<int>(max - min);
                for (var i = min; i < max; i++)
                    randomValues.Add(i);

                var result = new int[num];
                for (var i = 0; i < num; i++)
                {
                    var index = Random.Range(0, randomValues.Count);
                    result[i] = randomValues[index];
                    randomValues.RemoveAt(index);
                }
                return result;
            }
            
            var hashSet = new HashSet<int>();
            var differentNum = Mathf.Min(num, max - min);
            var length = 0;
            while (length < differentNum)
            {
                var value = Random.Range(min, max);
                if (hashSet.Contains(value))
                    continue;
                hashSet.Add(value);
                length++;
            }
            return hashSet.ToArray();
        }

        public static T RangeForList<T>(IList<T> list) => list.Count <= 0 ? default : list[Random.Range(0, list.Count)];
    }
}