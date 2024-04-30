using System;
using System.Collections.Generic;
using System.Linq;

namespace CGame
{
    public static class EnumExtension
    {
        public static int ToInt(this Enum self) => self.GetHashCode();

        public static bool ContainsAny(this Enum self, params Enum[] values)
        {
            var selfInt = ToInt(self);
            return values.Select(ToInt).Any(valueInt => (selfInt & valueInt) != 0);
        }
        
        public static bool ContainsAll(this Enum self, params Enum[] values)
        {
            var selfInt = ToInt(self);
            return values.Select(ToInt).All(valueInt => (selfInt & valueInt) != 0);
        }

        public static IEnumerator<Enum> GetEnumerator(this Enum self)
        {
            foreach (Enum value in Enum.GetValues(self.GetType()))
            {
                var valueInt = value.ToInt();
                if (valueInt != 0 && (valueInt == 1 || valueInt % 2 == 0) && self.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}