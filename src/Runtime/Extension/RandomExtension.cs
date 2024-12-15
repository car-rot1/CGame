using System;
using System.Collections.Generic;
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

            var length = max - min;
            if (length < num)
            {
                throw new Exception("提供的min max范围过小，无法满足返回num数量的不重复随机数。");
            }
            
            var randomValues = new int[max - min];
            var randomEndIndex = 0;
            for (var i = min; i < max; i++)
                randomValues[randomEndIndex++] = i;
            for (var i = 0; i < num; i++)
            {
                var index = Random.Range(0, randomEndIndex);
                (randomValues[index], randomValues[randomEndIndex - 1]) = (randomValues[randomEndIndex - 1], randomValues[index]);
                randomEndIndex--;
            }

            return max - min == num ? randomValues : randomValues[(randomEndIndex - 1)..^1];
        }

        public static T RangeForList<T>(IList<T> list) => list is not { Count: > 0 } ? default : list[Random.Range(0, list.Count)];
    }
}