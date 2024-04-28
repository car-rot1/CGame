using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization
{
    [AddComponentMenu("Localization/LocalizationText")]
    public class LocalizationText : Text
    {
        private readonly UIVertex[] _tempVerts = new UIVertex[4];

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            if (!Application.isPlaying && supportRichText)
            {
                UpdateIdDic(text);
                cachedTextGenerator.PopulateWithErrors(TextToLocal(text), settings, gameObject);
            }
            else
                cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            int vertCount = verts.Count;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    _tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    _tempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(_tempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(_tempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
        }

        [SerializeField] private string id;
        
        public string Id
        {
            get
            {
                var value = id;
                if (string.IsNullOrWhiteSpace(value))
                    value = CurrentText;
                if (string.IsNullOrWhiteSpace(value))
                    value = text;
                if (string.IsNullOrWhiteSpace(value))
                    value = gameObject.name;
                return value;
            }
            set
            {
                if (value.Equals(id))
                    return;
                id = value;
                LanguageChange(Local.Language);
            }
        }

        [Serializable]
        private struct FontInfo
        {
            public string language;
            public Font font;
        }

        [SerializeField] private List<FontInfo> customFonts = new();
        private Dictionary<string, Font> _fontInfoDic;
        private Dictionary<string, Font> FontInfoDic => _fontInfoDic ??= customFonts.ToDictionary(info => info.language, info => info.font);
        
        private Font _defaultFont;
        private Font DefaultFont
        {
            get
            {
                if (_defaultFont == null)
                    _defaultFont = font;
                return _defaultFont;
            }
        }

        [Serializable]
        private struct TextInfo
        {
            public string language;
            public string text;
        }

        [SerializeField] private List<TextInfo> customTexts = new();
        private Dictionary<string, string> _textInfoDic;
        private Dictionary<string, string> TextInfoDic => _textInfoDic ??= customTexts.ToDictionary(info => info.language, info => info.text);
        
        private string _currentText;
        private string CurrentText
        {
            get
            {
                if (_currentText != null)
                    return _currentText;
                
                _currentText = text ?? "";
                UpdateIdDic(_currentText);
                return _currentText;
            }
        }

        private LocalizationSystem _local;
        private LocalizationSystem Local => _local ??= LocalizationSystem.Instance;

        protected override void Awake()
        {
            // foreach (var info in customFonts)
            //     _fontInfoDic[info.language] = info.font;
            // _defaultFont = font;
            //
            // foreach (var info in customTexts)
            //     _textInfoDic[info.language] = info.text;
            // _currentText = text ?? "";
            //
            // UpdateIdDic(_currentText);
            LanguageChange(Local.Language);
            Local.OnLanguageChange += LanguageChange;
        }

        private void LanguageChange(string language)
        {
            if (!Application.isPlaying)
                return;

            if (FontInfoDic.TryGetValue(language, out var customFont) && customFont != null)
                font = customFont;
            else
                font = DefaultFont;

            text = TextToLocal(CurrentText);
        }
        
        private readonly Dictionary<string, List<string>> _allLocalText = new();

        private void UpdateIdDic(string content)
        {
            if (!supportRichText)
                return;

            _allLocalText.Clear();
            foreach (Match match in Regex.Matches(content, "(<local(=(.*?))?>)([^\"]*)(</local>)"))
            {
                string currentId;
                var s = match.Value;
                if (s[6] == '=')
                {
                    if (s[7] == '>')
                        currentId = s.Substring(8, s.Length - 16);
                    else
                    {
                        var result = Regex.Match(s, @"<local=(.*?)>").Value;
                        currentId = result.Substring(7, result.Length - 8);
                    }
                }
                else
                    currentId = s.Substring(7, s.Length - 15);

                _allLocalText.TryAdd(currentId, new List<string>());
                _allLocalText[currentId].Add(s);
            }
        }

        private string TextToLocal(string content)
        {
            if (!supportRichText)
                return TextInfoDic.TryGetValue(Local.Language, out var customText) ? customText : Local.GetText(Id);
            
            if (_allLocalText.Count <= 0)
                return Local.GetText(content);
                
            foreach (var currentId in _allLocalText.Keys)
            {
                foreach (var s in _allLocalText[currentId])
                    content = content.Replace(s, Local.GetText(currentId));
            }
            return content;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Local.OnLanguageChange -= LanguageChange;
        }
    }
}
