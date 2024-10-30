using System.Collections.Generic;
using System.Linq;
using CGame.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization.Editor
{
    public class ReplaceTextComponentEditorWindow : EditorWindow
    {
        private struct TextInfo
        {
            public MonoBehaviour text;
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
            var textList = LocalizationEditorUtility.GetCurrentAllText()
                .Where(text => text is not LocalizationText && text is not LocalizationTextMeshPro && text is not LocalizationTextMeshProUGUI);

            foreach (var text in textList)
            {
                var hashCode = text.GetHashCode();
                
                var textInfo = new TextInfo { text = text, isSelect = text.isActiveAndEnabled };
                
                textInfo.value = text switch
                {
                    Text _text => _text.text,
                    TextMeshPro _text => _text.text,
                    TextMeshProUGUI _text => _text.text,
                    _ => textInfo.value
                };
                
                if (tempIsSelectInfos.TryGetValue(hashCode, out var isSelect))
                    textInfo.isSelect = isSelect;

                TextInfos[hashCode] = textInfo;
            }
            TextHashCodes.AddRange(TextInfos.Keys);
        }
        
        public static void OpenWindow()
        {
            RefreshTextInfo();
            var window = GetWindow<ReplaceTextComponentEditorWindow>();
            window.titleContent = new GUIContent(nameof(ReplaceTextComponentEditorWindow));
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
            
            var idLabelWidth = 0;
            var valueLabelWidth = 0;

            var idValueWidth = idWidth - idLabelWidth;
            var valueValueWidth = valueWidth - valueLabelWidth;
            
            contentRect.height = -VerticalMargin;
            foreach (var hashCode in TextHashCodes)
            {
                var textInfo = TextInfos[hashCode];
                
                var valueValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(textInfo.value), valueValueWidth - 2));

                contentRect.height += valueValueHeight + VerticalMargin;
            }
            
            var itemRect = new Rect(contentRect);
            var isSelect = false;
            _scrollPos = GUI.BeginScrollView(verticalRects[2], _scrollPos, contentRect);
            foreach (var hashCode in TextHashCodes)
            {
                var textInfo = TextInfos[hashCode];
                
                var valueValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(textInfo.value), valueValueWidth - 2));
                
                itemRect.height = valueValueHeight;
                rects = itemRect.HorizontalSplit(TriggerWidth, idWidth, 10, valueWidth);
                itemRect.y += itemRect.height + VerticalMargin;
                
                EditorGUI.BeginChangeCheck();
                rects[0].height = 18;
                textInfo.isSelect = EditorGUI.ToggleLeft(rects[0], "", textInfo.isSelect);
                EditorGUI.EndChangeCheck();

                EditorGUI.BeginDisabledGroup(true);
                rects[1].height = 18;
                EditorGUI.ObjectField(rects[1], textInfo.text, textInfo.text.GetType(), true);
                EditorGUI.EndDisabledGroup();
                
                rects[3].height = valueValueHeight;
                EditorGUI.SelectableLabel(rects[3], textInfo.value, new GUIStyle(GUI.skin.textField) { wordWrap = true });

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
            if (GUI.Button(rects[3], "Replace"))
            {
                if (EditorUtility.DisplayDialog("替换确认","确定将选中的Text组件替换为LocalText组件？","替换","取消"))
                    ReplaceAllComponent();
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void ReplaceAllComponent()
        {
            var texts = TextInfos.Values.Where(textInfo => textInfo.isSelect).Select(textInfo => textInfo.text).ToList();
            
            for (var i = 0; i < texts.Count; i++)
            {
                var maskableGraphic = texts[i];
                switch (maskableGraphic)
                {
                    case Text text:
                        ComponentUtility.ReplaceComponent<Text, LocalizationText>(text);
                        break;
                    case TextMeshPro text:
                        ComponentUtility.ReplaceComponent<TextMeshPro, LocalizationTextMeshPro>(text);
                        break;
                    case TextMeshProUGUI text:
                        ComponentUtility.ReplaceComponent<TextMeshProUGUI, LocalizationTextMeshProUGUI>(text);
                        break;
                }
                if (EditorUtility.DisplayCancelableProgressBar("Replace Text Component", "Replacing...", (i + 1.0f) / texts.Count))
                    break;
            }
            EditorUtility.ClearProgressBar();
            RefreshTextInfo();
        }
    }
}