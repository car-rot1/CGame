#if UNITY_2DSPRITE
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public class ChangeSpriteRect : EditorWindow
    {
        private static readonly Type SpriteEditorWindowType;
        private static EditorWindow _spriteEditorWindow;
        private static readonly Box BoxElement;

        private static readonly FieldInfo RectsCacheFieldInfo;
        private static readonly PropertyInfo SelectedSpriteRectPropertyInfo;
        private static readonly MethodInfo SetDataModifiedMethodInfo;

        static ChangeSpriteRect()
        {
            SpriteEditorWindowType =
                typeof(SpriteEditorModuleBase).Assembly.GetType("UnityEditor.U2D.Sprites.SpriteEditorWindow");
            BoxElement = new Box
                { style = { backgroundColor = new StyleColor(new Color(50f / 255f, 50f / 255f, 50f / 255f)) } };
            RectsCacheFieldInfo =
                SpriteEditorWindowType.GetField("m_RectsCache", BindingFlags.Instance | BindingFlags.NonPublic);
            SelectedSpriteRectPropertyInfo =
                SpriteEditorWindowType.GetProperty("selectedSpriteRect", BindingFlags.Instance | BindingFlags.Public);
            SetDataModifiedMethodInfo =
                SpriteEditorWindowType.GetMethod("SetDataModified", BindingFlags.Instance | BindingFlags.Public);

            EditorApplication.update += EditorUpdate;
        }

        private static void EditorUpdate()
        {
            if (_spriteEditorWindow == null)
            {
                var editorWindows = Resources.FindObjectsOfTypeAll(SpriteEditorWindowType);
                if (editorWindows.Length > 0)
                    _spriteEditorWindow = (EditorWindow)editorWindows[0];
            }

            if (_spriteEditorWindow != null)
            {
                var element = _spriteEditorWindow.rootVisualElement.Q("spriteFrameModuleInspector");
                if (!element.Contains(BoxElement))
                {
                    InitBox(element);
                    element.Add(BoxElement);
                }
            }
        }

        private static void InitBox(VisualElement element)
        {
            BoxElement.Clear();
            for (var i = element.childCount - 1; i >= 2; i--)
            {
                BoxElement.Insert(0, element[i]);
            }

            BoxElement.Add(new Button(() =>
            {
                var list = (List<SpriteRect>)RectsCacheFieldInfo.GetValue(_spriteEditorWindow);
                var selectedSpriteRect = (SpriteRect)SelectedSpriteRectPropertyInfo.GetValue(_spriteEditorWindow);

                foreach (var spriteRect in list)
                {
                    spriteRect.border = selectedSpriteRect.border;
                    spriteRect.alignment = selectedSpriteRect.alignment;
                    spriteRect.pivot = selectedSpriteRect.pivot;
                }

                SetDataModifiedMethodInfo.Invoke(_spriteEditorWindow, new object[] { });
            }) { text = "Apply All Sprite" });
        }

        private static readonly Type SpriteEditorUtilityType =
            typeof(SpriteEditorModuleBase).Assembly.GetType("UnityEditor.U2D.Sprites.SpriteEditorUtility");

        private static readonly MethodInfo GetPivotValueMethodInfo =
            SpriteEditorUtilityType.GetMethod("GetPivotValue", BindingFlags.Static | BindingFlags.Public);

        private Texture2D _texture2D;
        private Vector4 _border;
        private SpriteAlignment _alignment;
        private Vector2 _pivot;

        [MenuItem("Tools/Change Sprite Rect")]
        private static void OpenWindow()
        {
            var window = GetWindow<ChangeSpriteRect>();
            window.maxSize = new Vector2(400, 200);
        }

        private void OnEnable()
        {
            _pivot = (Vector2)GetPivotValueMethodInfo.Invoke(null, new object[] { _alignment, _pivot });
        }

        private void OnGUI()
        {
            _texture2D =
                (Texture2D)EditorGUILayout.ObjectField("Target Texture2D", _texture2D, typeof(Texture2D), false);
            _border = EditorGUILayout.Vector4Field("Border", _border);

            EditorGUI.BeginChangeCheck();
            _alignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Alignment", _alignment);
            if (EditorGUI.EndChangeCheck())
            {
                _pivot = (Vector2)GetPivotValueMethodInfo.Invoke(null, new object[] { _alignment, _pivot });
            }

            EditorGUI.BeginDisabledGroup(_alignment is not SpriteAlignment.Custom);
            _pivot = EditorGUILayout.Vector2Field("Pivot", _pivot);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Apply All Sprite"))
            {
                Change(_texture2D, _border, _alignment, _pivot);
            }
        }

        private static void Change(Texture2D texture2D, Vector4 border, SpriteAlignment alignment, Vector2 pivot)
        {
            var path = AssetDatabase.GetAssetPath(texture2D);

            var spriteDataProviderFactories = new SpriteDataProviderFactories();
            spriteDataProviderFactories.Init();

            var spriteEditorDataProvider = spriteDataProviderFactories.GetSpriteEditorDataProviderFromObject(texture2D);
            spriteEditorDataProvider.InitSpriteEditorDataProvider();

            var spriteRects = spriteEditorDataProvider.GetSpriteRects();
            foreach (var spriteRect in spriteRects)
            {
                spriteRect.border = border;
                spriteRect.alignment = alignment;
                spriteRect.pivot = pivot;
            }

            spriteEditorDataProvider.SetSpriteRects(spriteRects);
            spriteEditorDataProvider.Apply();

            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(path);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
#endif
