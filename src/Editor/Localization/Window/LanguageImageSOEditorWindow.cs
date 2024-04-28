using System.Collections.Generic;
using System.IO;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    public class LanguageImageSOEditorWindow : EditorWindow
    {
        private LanguageImageSO _target;
        private LanguageImageSO Target
        {
            get => _target;
            set
            {
                if (_target == value)
                    return;
                _target = value;
                _selects.Clear();
                for (var i = 0; i < _target.languageImageInfos.Count; i++)
                {
                    _selects.Add(false);
                }
            }
        }
        
        [SerializeField] private List<bool> _selects = new();

        public static void Open(LanguageImageSO target = null)
        {
            var window = GetWindow<LanguageImageSOEditorWindow>();

            window.Target = target;
            window.titleContent = new GUIContent(target == null ? "" : target.name);
            window.Show();
        }
        
        private const float TopPadding = 5;
        private const float RightPadding = 3;
        private const float BottomPadding = 20;
        private const float LeftPadding = 3;

        private const float VerticalMargin = 5;

        private const float FileObjectFieldHeight = 18;
        private const float ImportButtonWidth = 50;
        private const float ExportButtonWidth = 50;
        
        private const float AllButtonHeight = 18;
        private const float AllButtonWidth = 50;
        private const float NoneButtonWidth = 50;

        private const float TriggerWidth = 30;

        private const float RemoveButtonHeight = 18;
        private const float RemoveButtonWidth = 60;
        private const float AddButtonWidth = 60;

        private Vector2 _scrollPos;
        private readonly GenericMenu _genericMenu = new();
        
        private void OnGUI()
        {
            var iconRect = new Rect(Vector2.zero, position.size).HorizontalEquallySplitForNum(5)[4].VerticalEquallySplitForNum(5)[4];
            if (CGameIcon.CGameFileIcon != null)
                GUI.DrawTexture(iconRect, CGameIcon.CGameFileIcon, ScaleMode.ScaleToFit);
            
            var rect = new Rect(new Vector2(RightPadding, TopPadding),
                new Vector2(position.width - RightPadding - LeftPadding, position.height - TopPadding - BottomPadding));

            var verticalRects = rect.VerticalSplit(
                FileObjectFieldHeight,
                VerticalMargin,
                AllButtonHeight,
                VerticalMargin,
                -1,
                VerticalMargin,
                RemoveButtonHeight);
            
            var rects = verticalRects[0].HorizontalSplit(-1, 10, ImportButtonWidth, 10, ExportButtonWidth);
            var obj = (LanguageImageSO)EditorGUI.ObjectField(rects[0], Target, typeof(LanguageImageSO), true);
            if (obj != null)
                Target = obj;
            if (GUI.Button(rects[2], "Import"))
            {
                _genericMenu.ClearItem();
                _genericMenu.AddItem(new GUIContent("Import ImageFile"), false, ImportImageFile);
                _genericMenu.ShowAsContext();
            }
            if (GUI.Button(rects[4], "Export"))
            {
                _genericMenu.ClearItem();
                _genericMenu.AddItem(new GUIContent("Export ImageFile"), false, ExportImageFile);
                _genericMenu.ShowAsContext();
            }
            
            rects = verticalRects[2].HorizontalSplit(AllButtonWidth, 10, NoneButtonWidth);
            if (GUI.Button(rects[0], "All"))
            {
                for (var i = 0; i < _selects.Count; i++)
                {
                    _selects[i] = true;
                }
            }
            if (GUI.Button(rects[2], "None"))
            {
                for (var i = 0; i < _selects.Count; i++)
                {
                    _selects[i] = false;
                }
            }

            var contentRect = new Rect(verticalRects[4]);
            contentRect.width -= GUI.skin.verticalScrollbar.fixedWidth;

            var idWidth = (contentRect.width - TriggerWidth - 10) / 2f;
            var valueWidth = (contentRect.width - TriggerWidth - 10) / 2f;
            
            var idLabelWidth = GUI.skin.label.CalcSize(new GUIContent("Id :")).x + 2;
            var valueLabelWidth = GUI.skin.label.CalcSize(new GUIContent("Value :")).x + 2;

            var idValueWidth = idWidth - idLabelWidth;
            var valueValueWidth = valueWidth - valueLabelWidth;
            
            contentRect.height = -VerticalMargin;
            foreach (var rowInfo in Target.languageImageInfos)
            {
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(rowInfo.id), idValueWidth - 2));
                contentRect.height += idValueHeight + VerticalMargin;
            }
            
            var itemRect = new Rect(contentRect);
            var isSelect = false;
            var rectToValue = new Dictionary<Rect, Sprite>();
            _scrollPos = GUI.BeginScrollView(verticalRects[4], _scrollPos, contentRect);
            for (var i = 0; i < Target.languageImageInfos.Count; i++)
            {
                var temp = Target.languageImageInfos[i];
                
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(temp.id), idValueWidth - 2));
                
                itemRect.height = idValueHeight;
                rects = itemRect.HorizontalSplit(TriggerWidth, idWidth, 10, valueWidth);
                itemRect.y += itemRect.height + VerticalMargin;
                
                EditorGUI.BeginChangeCheck();

                rects[0].height = 18;
                _selects[i] = EditorGUI.ToggleLeft(rects[0], "", _selects[i]);

                rects[1].height = idValueHeight;
                // EditorGUIUtility.labelWidth = idLabelWidth;
                // var idValue = EditorGUI.TextField(rects[1], "Id :", rowInfo.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                var idRect = rects[1].HorizontalSplit(idLabelWidth, idValueWidth);
                EditorGUI.LabelField(idRect[0], "Id :", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
                var idValue = EditorGUI.TextArea(idRect[1], temp.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                
                rects[3].height = 18;
                var valueRects = rects[3].HorizontalSplit(valueLabelWidth, -1);
                EditorGUI.LabelField(valueRects[0], "Value :");
                var valueValue = (Sprite)EditorGUI.ObjectField(valueRects[1], temp.sprite, typeof(Sprite), true);
                rectToValue[valueRects[1]] = temp.sprite;
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrWhiteSpace(idValue) && Target.languageImageInfos.FindIndex(r => r.id == idValue) == -1)
                        temp.id = idValue;
                    if (valueValue != null)
                        temp.sprite = valueValue;
                }

                if (_selects[i])
                    isSelect = true;

                Target.languageImageInfos[i] = temp;
            }
            GUI.EndScrollView();
            
            rects = verticalRects[6].HorizontalSplit(-1, RemoveButtonWidth, 10, AddButtonWidth);
            if (GUI.Button(rects[1], "Remove"))
            {
                if (!isSelect && Target.languageImageInfos.Count > 0)
                {
                    var index = Target.languageImageInfos.Count - 1;
                    Target.languageImageInfos.RemoveAt(index);
                    _selects.RemoveAt(index);
                }
                else
                    for (var i = Target.languageImageInfos.Count - 1; i >= 0; i--)
                    {
                        if (_selects[i])
                        {
                            Target.languageImageInfos.RemoveAt(i);
                            _selects.RemoveAt(i);
                        }
                    }
            }
            if (GUI.Button(rects[3], "Add"))
            {
                Target.languageImageInfos.Add(new LanguageImageInfo());
                _selects.Add(false);
            }
            
            foreach (var (valueRect, value) in rectToValue)
            {
                if (valueRect.Contains(Event.current.mousePosition))
                {
                    if (value == null)
                        return;
                    
                    var imageRect = new Rect(Event.current.mousePosition, new Vector2(100, 100));
                    EditorGUI.DrawTextureTransparent(imageRect, value.GetPartTexture(), ScaleMode.ScaleToFit);
                }
            }
        }

        private void ImportImageFile()
        {
            
        }
        
        private void ExportImageFile()
        {
            var path = EditorUtility.SaveFolderPanel(nameof(ExportImageFile), Application.dataPath, "");
            if (string.IsNullOrWhiteSpace(path))
                return;

            path = path + '/' + _target.name + '/';
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            foreach (var languageImageInfo in _target.languageImageInfos)
            {
                var texture = languageImageInfo.sprite.GetPartTexture();
                if (texture.alphaIsTransparency)
                    File.WriteAllBytes(path + languageImageInfo.id + ".png", texture.EncodeToPNG());
                else
                    File.WriteAllBytes(path + languageImageInfo.id + ".jpg", texture.EncodeToJPG());
            }
            AssetDatabase.Refresh();
        }
    }
}