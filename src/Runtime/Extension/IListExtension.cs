using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public static class IListExtension
    {
        public static T RandomItem<T>(this IList<T> self) => self.Count <= 0 ? default : self[Random.Range(0, self.Count)];
        public static T RandomItem<T>(this IList<T> self, out int randomIndex)
        {
            if (self.Count <= 0)
            {
                randomIndex = -1;
                return default;
            }

            randomIndex = Random.Range(0, self.Count);
            return self[randomIndex];
        }

        public static bool TryGetValue<T>(this IList<T> self, int index, out T value)
        {
            value = default;
            if (index >= self.Count)
                return false;
            value = self[index];
            return true;
        }
    }
}