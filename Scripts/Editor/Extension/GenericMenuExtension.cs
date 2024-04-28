using System.Collections;
using System.Reflection;
using UnityEditor;

namespace CGame
{
    public static class GenericMenuExtension
    {
        private static readonly FieldInfo _menuItem;

        static GenericMenuExtension()
        {
            _menuItem = typeof(GenericMenu).GetField("m_MenuItems", BindingFlags.Instance | BindingFlags.NonPublic)!;
        }
        
        public static void ClearItem(this GenericMenu self)
        {
            var list = (IList)_menuItem.GetValue(self);
            list.Clear();
        }
    }
}