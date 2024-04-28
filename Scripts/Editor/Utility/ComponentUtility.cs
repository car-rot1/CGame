using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    public static class ComponentUtility
    {
        public static void ReplaceComponent<TSource, TDest>(TSource source) where TSource : MonoBehaviour where TDest : TSource
        {
            var so = new SerializedObject(source);
            so.Update();

            var oldEnable = source.enabled;
            source.enabled = false;
            
            foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
            {
                if (script.GetClass() != typeof(TDest))
                    continue;
                so.FindProperty("m_Script").objectReferenceValue = script;
                so.ApplyModifiedProperties();
                break;
            }
            
            ((MonoBehaviour)so.targetObject).enabled = oldEnable;
        }
    }
}