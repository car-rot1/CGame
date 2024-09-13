using System.Collections.Generic;
using System.Linq;
using CGame.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization.Editor
{
    public class GenerateLanguageTextSOEditorWindow : EditorWindow
    {
        private struct TextInfo
        {
            public string id;
            public string value;
            public bool isSelect;
        }
        
        private static readonly Dictionary<int, TextInfo> TextInfos = new();
        private static readonly List<int> TextHashCodes = new();

        private static void RefreshTextInfo()
        {
            var tempIsSelectInfos = new Dictionary<int, bool>();
            foreach (var (hashCode, textInfo) in TextInfos)
                tempIsSelectInfos[hashCode] = textInfo.isSelect;
            
            TextInfos.Clear();
            TextHashCodes.Clear();
            var textList = LocalizationEditorUtility.GetCurrentAllText();
            foreach (var text in textList)
            {
                var hashCode = text.GetHashCode();
                
                var textInfo = new TextInfo { isSelect = text.isActiveAndEnabled };
                if (text is LocalizationText localizationText)
                {
                    textInfo.id = localizationText.Id;
                    textInfo.value = localizationText.text;
                }
                else if (text is LocalizationTextMeshPro localizationTextMeshPro)
                {
                    textInfo.id = localizationTextMeshPro.Id;
                    textInfo.value = localizationTextMeshPro.text;
                }
                else if (text is LocalizationTextMeshProUGUI localizationTextMeshProUGUI)
                {
                    textInfo.id = localizationTextMeshProUGUI.Id;
                    textInfo.value = localizationTextMeshProUGUI.text;
                }
                else
                {
                    textInfo.id = textInfo.value = text switch
                    {
                        Text _text => _text.text,
#if UNITY_TEXTMESHPRO
                        TextMeshProUGUI _textMeshPro => _textMeshPro.text,
#endif
                        _ => textInfo.value
                    };
                    if (string.IsNullOrWhiteSpace(textInfo.id))
                        textInfo.id = text.gameObject.name;
                }
                
                if (tempIsSelectInfos.TryGetValue(hashCode, out var isSelect))
                    textInfo.isSelect = isSelect;

                TextInfos[hashCode] = textInfo;
            }
            TextHashCodes.AddRange(TextInfos.Keys);
        }
        
        public static void OpenWindow()
        {
            RefreshTextInfo();
            var window = GetWindow<GenerateLanguageTextSOEditorWindow>();
            window.titleContent = new GUIContent(nameof(GenerateLanguageTextSOEditorWindow));
            window.Show();
        }

        private const float TopPadding = 2;
        private const float RightPadding = 3;
        private const float BottomPadding = 2;
        private const float LeftPadding = 3;

        private const float VerticalMargin = 2;
        
        private const float AllButtonHeight = 18;
        private const float AllButtonWidth = 50;
        private const float NoneButtonWidth = 50;

        private const float TriggerWidth = 30;
        
        private const float RefreshButtonHeight = 30;
        private const float RefreshButtonWidth = 80;
        private const float GenerateButtonWidth = 80;
        
        private Vector2 _scrollPos;

        private void OnGUI()
        {
            var iconRect = new Rect(Vector2.zero, position.size).HorizontalEquallySplitForNum(5)[4].VerticalEquallySplitForNum(5)[4];
            if (CGameIcon.CGameFileIcon != null)
                GUI.DrawTexture(iconRect, CGameIcon.CGameFileIcon, ScaleMode.ScaleToFit);
            
            var rect = new Rect(new Vector2(RightPadding, TopPadding),
                new Vector2(position.width - RightPadding - LeftPadding, position.height - TopPadding - BottomPadding));

            var verticalRects = rect.VerticalSplit(AllButtonHeight, VerticalMargin, -1, VerticalMargin, RefreshButtonHeight);
            
            var rects = verticalRects[0].HorizontalSplit(AllButtonWidth, 10, NoneButtonWidth);
            if (GUI.Button(rects[0], "All"))
            {
                foreach (var hashCode in TextHashCodes)
                {
                    var textInfo = TextInfos[hashCode];
                    textInfo.isSelect = true;
                    TextInfos[hashCode] = textInfo;
                }
            }
            if (GUI.Button(rects[2], "None"))
            {
                foreach (var hashCode in TextHashCodes)
                {
                    var textInfo = TextInfos[hashCode];
                    textInfo.isSelect = false;
                    TextInfos[hashCode] = textInfo;
                }
            }
            
            var contentRect = new Rect(verticalRects[2]);
            contentRect.width -= GUI.skin.verticalScrollbar.fixedWidth;

            var idWidth = (contentRect.width - TriggerWidth - 10) / 2f;
            var valueWidth = (contentRect.width - TriggerWidth - 10) / 2f;
            
            var idLabelWidth = GUI.skin.label.CalcSize(new GUIContent("Id :")).x + 2;
            var valueLabelWidth = GUI.skin.label.CalcSize(new GUIContent("Value :")).x + 2;

            var idValueWidth = idWidth - idLabelWidth;
            var valueValueWidth = valueWidth - valueLabelWidth;
            
            contentRect.height = -VerticalMargin;
            foreach (var hashCode in TextHashCodes)
            {
                var textInfo = TextInfos[hashCode];
                
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(textInfo.id), idValueWidth - 2));
                var valueValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(textInfo.value), valueValueWidth - 2));

                contentRect.height += Mathf.Max(idValueHeight, valueValueHeight) + VerticalMargin;
            }
            
            var itemRect = new Rect(contentRect);
            var isSelect = false;
            _scrollPos = GUI.BeginScrollView(verticalRects[2], _scrollPos, contentRect);
            foreach (var hashCode in TextHashCodes)
            {
                var textInfo = TextInfos[hashCode];
                
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(textInfo.id), idValueWidth - 2));
                var valueValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(textInfo.value), valueValueWidth - 2));
                
                itemRect.height = Mathf.Max(idValueHeight, valueValueHeight);
                rects = itemRect.HorizontalSplit(TriggerWidth, idWidth, 10, valueWidth);
                itemRect.y += itemRect.height + VerticalMargin;
                
                EditorGUI.BeginChangeCheck();

                rects[0].height = 18;
                textInfo.isSelect = EditorGUI.ToggleLeft(rects[0], "", textInfo.isSelect);

                rects[1].height = idValueHeight;
                // EditorGUIUtility.labelWidth = idLabelWidth;
                // var idValue = EditorGUI.TextField(rects[1], "Id :", textInfo.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                var idRect = rects[1].HorizontalSplit(idLabelWidth, idValueWidth);
                EditorGUI.LabelField(idRect[0], "Id :", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
                var idValue = EditorGUI.TextArea(idRect[1], textInfo.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });

                rects[3].height = valueValueHeight;
                // EditorGUIUtility.labelWidth = valueLabelWidth;
                // var valueValue = EditorGUI.TextField(rects[3], "Value :", textInfo.value, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                var valueRect = rects[3].HorizontalSplit(valueLabelWidth, valueValueWidth);
                EditorGUI.LabelField(valueRect[0], "Value :", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
                var valueValue = EditorGUI.TextArea(valueRect[1], textInfo.value, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrWhiteSpace(idValue) && TextInfos.Values.ToList().FindIndex(t => t.id == idValue) == -1)
                        textInfo.id = idValue;
                    if (!string.IsNullOrWhiteSpace(valueValue))
                        textInfo.value = valueValue;
                }

                if (textInfo.isSelect)
                    isSelect = true;

                TextInfos[hashCode] = textInfo;
            }
            GUI.EndScrollView();
            
            rects = verticalRects[4].HorizontalSplit(-1, RefreshButtonWidth, 20, GenerateButtonWidth, 10);
            if (GUI.Button(rects[1], "Refresh"))
            {
                RefreshTextInfo();
            }

            EditorGUI.BeginDisabledGroup(!isSelect);
            if (GUI.Button(rects[3], "Generate"))
            {
                GenerateCsvFile();
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void GenerateCsvFile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create LanguageTextSO", "", "asset", "");
            if (string.IsNullOrWhiteSpace(path))
                return;

            var so = CreateInstance<LocalizationStringSO>();
            foreach (var textInfo in TextInfos.Values.Where(imageInfo => imageInfo.isSelect))
            {
                so.localizationTextInfos.Add(new LocalizationTextInfo { id = textInfo.id, text = textInfo.value });
            }
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}