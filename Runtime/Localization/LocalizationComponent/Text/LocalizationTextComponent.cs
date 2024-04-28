using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization
{
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Localization/LocalizationTextComponent")]
    public class LocalizationTextComponent : LocalizationComponentBase
    {
        [SerializeField] private string id;
        
        public string Id
        {
            get
            {
                var value = id;
                if (string.IsNullOrWhiteSpace(value))
                    value = CurrentText;
                if (string.IsNullOrWhiteSpace(value))
                    value = TargetText.text;
                if (string.IsNullOrWhiteSpace(value))
                    value = gameObject.name;
                return value;
            }
            set
            {
                if (value.Equals(id))
                    return;
                id = value;
                LanguageChange(localizationSystem.Language);
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
                    _defaultFont = TargetText.font;
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
                
                _currentText = TargetText.text ?? "";
                UpdateIdDic(_currentText);
                return _currentText;
            }
        }

        private Text _targetText;
        public Text TargetText
        {
            get
            {
                if (_targetText == null)
                    _targetText = GetComponent<Text>();
                return _targetText;
            }
        }

        protected override void LanguageChange(string language)
        {
            if (FontInfoDic.TryGetValue(language, out var customFont) && customFont != null)
                TargetText.font = customFont;
            else
                TargetText.font = DefaultFont;

            TargetText.text = TextToLocal(CurrentText);
        }

        private readonly Dictionary<string, List<string>> _allLocalText = new();

        private void UpdateIdDic(string content)
        {
            if (!TargetText.supportRichText)
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
            if (!TargetText.supportRichText)
                return TextInfoDic.TryGetValue(localizationSystem.Language, out var customText) ? customText : localizationSystem.GetText(Id);
            
            if (_allLocalText.Count <= 0)
                return localizationSystem.GetText(content);
                
            foreach (var currentId in _allLocalText.Keys)
            {
                foreach (var s in _allLocalText[currentId])
                    content = content.Replace(s, localizationSystem.GetText(currentId));
            }
            return content;
        }
    }
}
