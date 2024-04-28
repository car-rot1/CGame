using System.Collections.Generic;
using System.Linq;
using CGame.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CGame.Localization.Editor
{
    public class ReplaceImageComponentEditorWindow : EditorWindow
    {
        private struct ImageInfo
        {
            public MonoBehaviour image;
            public Object value;
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
            
            var imageList = LocalizationEditorUtility.GetCurrentAllImage()
                .Where(image => image is not LocalizationImage && image is not LocalizationRawImage);
            
            foreach (var image in imageList)
            {
                var hashCode = image.GetHashCode();
                
                var imageInfo = new ImageInfo { image = image, isSelect = image.isActiveAndEnabled };
                
                imageInfo.value = image switch
                {
                    Image _image => _image.sprite,
                    RawImage _rawImage => _rawImage.texture,
                    _ => imageInfo.value
                };
                
                if (tempIsSelectInfos.TryGetValue(hashCode, out var isSelect))
                    imageInfo.isSelect = isSelect;

                ImageInfos[hashCode] = imageInfo;
            }
            
            ImageHashCodes.AddRange(ImageInfos.Keys);
        }
        
        public static void OpenWindow()
        {
            RefreshImageInfo();
            var window = GetWindow<ReplaceImageComponentEditorWindow>();
            window.titleContent = new GUIContent(nameof(ReplaceImageComponentEditorWindow));
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

        private const float TriggerHeight = 18;
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

            var idWidth = (contentRect.width - TriggerWidth - 10) / 3f * 1f;
            var valueWidth = (contentRect.width - TriggerWidth - 10) / 3f * 2f;
            
            var idLabelWidth = 0f;
            var valueLabelWidth = 0f;

            var idValueWidth = idWidth - idLabelWidth;
            var valueValueWidth = valueWidth - valueLabelWidth;
            
            contentRect.height = -VerticalMargin;
            foreach (var hashCode in ImageHashCodes)
            {
                var imageInfo = ImageInfos[hashCode];
                contentRect.height += TriggerHeight + VerticalMargin;
            }
            
            var itemRect = new Rect(contentRect);
            var isSelect = false;
            var rectToValue = new Dictionary<Rect, Object>();
            _scrollPos = GUI.BeginScrollView(verticalRects[2], _scrollPos, contentRect);
            foreach (var hashCode in ImageHashCodes)
            {
                var imageInfo = ImageInfos[hashCode];
                
                itemRect.height = TriggerHeight;
                rects = itemRect.HorizontalSplit(TriggerWidth, idWidth, 10, valueWidth);
                itemRect.y += itemRect.height + VerticalMargin;

                EditorGUI.BeginChangeCheck();
                imageInfo.isSelect = EditorGUI.ToggleLeft(rects[0], "", imageInfo.isSelect);
                EditorGUI.EndChangeCheck();
                
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(rects[1], imageInfo.image, imageInfo.image.GetType(), true);
                EditorGUI.ObjectField(rects[3], imageInfo.value, imageInfo.value.GetType(), true);
                EditorGUI.EndDisabledGroup();
                rectToValue[rects[3]] = imageInfo.value;

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
            if (GUI.Button(rects[3], "Replace"))
            {
                if (EditorUtility.DisplayDialog("替换确认","确定将选中的Image组件替换为LocalImage组件？","替换","取消"))
                    ReplaceAllComponent();
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

        private static void ReplaceAllComponent()
        {
            var images = ImageInfos.Values.Where(imageInfo => imageInfo.isSelect).Select(imageInfo => imageInfo.image).ToList();
            
            for (var i = 0; i < images.Count; i++)
            {
                var maskableGraphic = images[i];
                switch (maskableGraphic)
                {
                    case Image image:
                        ComponentUtility.ReplaceComponent<Image, LocalizationImage>(image);
                        break;
                    case RawImage image:
                        ComponentUtility.ReplaceComponent<RawImage, LocalizationRawImage>(image);
                        break;
                }
                if (EditorUtility.DisplayCancelableProgressBar("Replace Image Component", "Replacing...", (i + 1.0f) / images.Count))
                    break;
            }
            EditorUtility.ClearProgressBar();
            RefreshImageInfo();
        }
    }
}