using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CGame.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CGame.Localization.Editor
{
    public class GenerateLanguageImageSOEditorWindow : EditorWindow
    {
        private struct ImageInfo
        {
            public string id;
            public Sprite value;
            public bool isSelect;
        }
        
        private static readonly Dictionary<int, ImageInfo> ImageInfos = new();
        private static readonly List<int> ImageHashCodes = new();

        private static void RefreshImageInfo()
        {
            var tempIsSelectInfos = new Dictionary<int, bool>();
            foreach (var (hashCode, imageInfo) in ImageInfos)
                tempIsSelectInfos[hashCode] = imageInfo.isSelect;
            
            ImageInfos.Clear();
            ImageHashCodes.Clear();
            var imageList = LocalizationEditorUtility.GetCurrentAllImage();
            foreach (var image in imageList)
            {
                var hashCode = image.GetHashCode();

                var imageInfo = new ImageInfo { isSelect = image.isActiveAndEnabled };
                if (image is LocalizationImage localizationImage)
                {
                    imageInfo.id = localizationImage.Id;
                    imageInfo.value = localizationImage.sprite;
                }
                else if (image is LocalizationRawImage localizationRawImage)
                {
                    imageInfo.id = localizationRawImage.Id;
                    imageInfo.value = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(localizationRawImage.texture));
                }
                else
                {
                    imageInfo.id = image.gameObject.name;
                    imageInfo.value = image switch
                    {
                        Image _image => _image.sprite,
                        RawImage _rawImage => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(_rawImage.texture)),
                        _ => imageInfo.value
                    };
                }
                
                if (tempIsSelectInfos.TryGetValue(hashCode, out var isSelect))
                    imageInfo.isSelect = isSelect;

                ImageInfos[hashCode] = imageInfo;
            }

            var removeHashCodeList = ImageInfos.Keys.Where(hashCode => Path.GetExtension(AssetDatabase.GetAssetPath(ImageInfos[hashCode].value)) is not (".jpg" or ".png" or ".bmp")).ToList();
            foreach (var hashCode in removeHashCodeList)
            {
                ImageInfos.Remove(hashCode);
            }
            ImageHashCodes.AddRange(ImageInfos.Keys);
        }

        private LocalizationConfig _config;
        
        public static void OpenWindow()
        {
            RefreshImageInfo();
            var window = GetWindow<GenerateLanguageImageSOEditorWindow>();
            window._config = LocalizationConfig.Instance;
            window.titleContent = new GUIContent(nameof(GenerateLanguageImageSOEditorWindow));
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
                foreach (var hashCode in ImageHashCodes)
                {
                    var imageInfo = ImageInfos[hashCode];
                    imageInfo.isSelect = true;
                    ImageInfos[hashCode] = imageInfo;
                }
            }
            if (GUI.Button(rects[2], "None"))
            {
                foreach (var hashCode in ImageHashCodes)
                {
                    var imageInfo = ImageInfos[hashCode];
                    imageInfo.isSelect = false;
                    ImageInfos[hashCode] = imageInfo;
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
            foreach (var hashCode in ImageHashCodes)
            {
                var imageInfo = ImageInfos[hashCode];
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(imageInfo.id), idValueWidth - 2));
                contentRect.height += idValueHeight + VerticalMargin;
            }
            
            var itemRect = new Rect(contentRect);
            var isSelect = false;
            var rectToValue = new Dictionary<Rect, Object>();
            _scrollPos = GUI.BeginScrollView(verticalRects[2], _scrollPos, contentRect);
            foreach (var hashCode in ImageHashCodes)
            {
                var imageInfo = ImageInfos[hashCode];
                
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(imageInfo.id), idValueWidth - 2));
                
                itemRect.height = idValueHeight;
                rects = itemRect.HorizontalSplit(TriggerWidth, idWidth, 10, valueWidth);
                itemRect.y += itemRect.height + VerticalMargin;

                EditorGUI.BeginChangeCheck();

                rects[0].height = 18;
                imageInfo.isSelect = EditorGUI.ToggleLeft(rects[0], "", imageInfo.isSelect);

                rects[1].height = idValueHeight;
                // EditorGUIUtility.labelWidth = idLabelWidth;
                // var idValue = EditorGUI.TextField(rects[1], "Id :", imageInfo.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                var idRect = rects[1].HorizontalSplit(idLabelWidth, idValueWidth);
                EditorGUI.LabelField(idRect[0], "Id :", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
                var idValue = EditorGUI.TextArea(idRect[1], imageInfo.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                
                rects[3].height = 18;
                var valueRects = rects[3].HorizontalSplit(valueLabelWidth, -1);
                EditorGUI.LabelField(valueRects[0], "Value :");
                var valueValue = (Sprite)EditorGUI.ObjectField(valueRects[1], imageInfo.value, typeof(Sprite), true);
                rectToValue[valueRects[1]] = imageInfo.value;
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrWhiteSpace(idValue) && ImageInfos.Values.ToList().FindIndex(i => i.id == idValue) == -1)
                        imageInfo.id = idValue;
                    if (valueValue != null)
                        imageInfo.value = valueValue;
                }

                if (imageInfo.isSelect)
                    isSelect = true;

                ImageInfos[hashCode] = imageInfo;
            }
            GUI.EndScrollView();
            
            rects = verticalRects[4].HorizontalSplit(-1, RefreshButtonWidth, 20, GenerateButtonWidth, 10);
            if (GUI.Button(rects[1], "Refresh"))
            {
                RefreshImageInfo();
            }

            EditorGUI.BeginDisabledGroup(!isSelect);
            if (GUI.Button(rects[3], "Generate"))
            {
                GenerateImageFile();
            }
            EditorGUI.EndDisabledGroup();
            
            foreach (var (valueRect, value) in rectToValue)
            {
                if (valueRect.Contains(Event.current.mousePosition))
                {
                    if (value == null)
                        return;
                    
                    var imageRect = new Rect(Event.current.mousePosition, new Vector2(100, 100));
                    switch (value)
                    {
                        case Sprite sprite:
                        {
                            EditorGUI.DrawTextureTransparent(imageRect, sprite.GetPartTexture(), ScaleMode.ScaleToFit);
                            // GUI.DrawTexture(imageRect, sprite.texture, ScaleMode.ScaleToFit, true);
                            break;
                        }
                        case Texture2D texture2D:
                        {
                            EditorGUI.DrawTextureTransparent(imageRect, texture2D, ScaleMode.ScaleToFit);
                            // GUI.DrawTexture(imageRect, texture2D, ScaleMode.ScaleToFit, true);
                            break;
                        }
                    }
                }
            }
        }

        private void GenerateImageFile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create LanguageImageSO", "", "asset", "");
            if (string.IsNullOrWhiteSpace(path))
                return;

            var so = CreateInstance<LanguageAssetSO>();
            foreach (var imageInfo in ImageInfos.Values.Where(imageInfo => imageInfo.isSelect))
            {
                so.languageAssetInfos.Add(new LanguageAssetInfo { id = imageInfo.id, asset = imageInfo.value });
            }
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}