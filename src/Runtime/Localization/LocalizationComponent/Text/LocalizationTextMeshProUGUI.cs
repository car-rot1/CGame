#if UNITY_TEXTMESHPRO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace CGame.Localization
{
    [AddComponentMenu("Localization/LocalizationTextMeshProUGUI")]
    public class LocalizationTextMeshProUGUI : TextMeshProUGUI
    {
        private class LocalizationTextPreprocessor : ITextPreprocessor
        {
            private readonly LocalizationTextMeshProUGUI _target;
            
            public LocalizationTextPreprocessor(LocalizationTextMeshProUGUI target)
            {
                _target = target;
            }
            
            public string PreprocessText(string text)
            {
                if (Application.isPlaying || !_target.richText)
                    return text;
                
                _target.UpdateIdDic(text);
                return _target.TextToLocal(text);
            }
        }

        public LocalizationTextMeshProUGUI()
        {
            textPreprocessor = new LocalizationTextPreprocessor(this);
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
            public TMP_FontAsset font;
        }

        [SerializeField] private List<FontInfo> customFonts = new();
        private Dictionary<string, TMP_FontAsset> _fontInfoDic;
        private Dictionary<string, TMP_FontAsset> FontInfoDic => _fontInfoDic ??= customFonts.ToDictionary(info => info.language, info => info.font);
        
        private TMP_FontAsset _defaultFont;
        private TMP_FontAsset DefaultFont
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
            base.Awake();
            
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
            if (!richText)
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
            if (!richText)
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
    }
}
#endif