using System;
using System.Collections.Generic;
using System.Linq;

namespace CGame
{
    public static class EnumExtension
    {
        public static int ToInt(this Enum self) => self.GetHashCode();

        public static bool ContainsAny(this Enum self, Enum value)
        {
            var selfInt = ToInt(self);
            var valueInt = ToInt(value);
            return (selfInt & valueInt) != 0;
        }
        
        public static bool ContainsAll(this Enum self, Enum value)
        {
            return self.HasFlag(value);
        }

        public static IEnumerator<Enum> GetEnumerator(this Enum self)
        {
            foreach (Enum value in Enum.GetValues(self.GetType()))
            {
                var valueInt = value.ToInt();
                
                if (valueInt != 0 && (valueInt == 1 || (valueInt & 1) == 0) && self.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}