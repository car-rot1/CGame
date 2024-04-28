using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public class SingletonMonoBehaviourItemGUIEditor
    {
        private static readonly Color _backgroundColor;
        private static bool _isLogError;

        static SingletonMonoBehaviourItemGUIEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemCallback;

            _backgroundColor = Color.cyan;
            _backgroundColor.a = 0.2f;
        }

        private static void HierarchyWindowItemCallback(int instanceID, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject gameObject)
                return;

            if (!gameObject.TryGetComponent<SingletonMonoBehaviour>(out _))
                return;

            DrawTitle(selectionRect, 50, "单例");
        }

        private static void ProjectWindowItemCallback(string guid, Rect selectionRect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (gameObject != null && gameObject.TryGetComponent<SingletonMonoBehaviour>(out _))
            {
                DrawTitle(selectionRect, 50, "单例");
                return;
            }

            var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (monoScript != null && monoScript.GetClass() != null &&
                monoScript.GetClass().IsSubclassOf(typeof(SingletonMonoBehaviour)))
            {
                DrawTitle(selectionRect, 50, "单例");
                return;
            }
        }

        private static void DrawTitle(Rect totalRect, float width, string label)
        {
            var backgroundRect = new Rect(totalRect);
            backgroundRect.xMin += 18;
            EditorGUI.DrawRect(backgroundRect, _backgroundColor);

            var labelRect = new Rect(totalRect);
            labelRect.x += labelRect.width;
            labelRect.x -= width;
            labelRect.width = width;
            var style = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.white,
                    background = Texture2D.linearGrayTexture
                }
            };
            GUI.Label(labelRect, label, style);
        }
    }
}
