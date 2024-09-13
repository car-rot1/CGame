using UnityEngine;

namespace CGame.Localization
{
    public abstract class LocalizationStringBase : MonoBehaviour
    {
        public string id;
        private string content;
        private string Content
        {
            get => content;
            set
            {
                if (content == value)
                    return;
                content = value;
                UpdateString(content);
            }
        }
        protected LocalizationSystem localizationSystem;

        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            localizationSystem = LocalizationSystem.Instance;
            LanguageChange(localizationSystem.Language);
            localizationSystem.OnLanguageChange += LanguageChange;
        }
        
        private void LanguageChange(string language)
        {
            Content = localizationSystem.GetString(id);
        }

        private void OnDestroy()
        {
            DeInit();
        }
        
        protected virtual void DeInit()
        {
            localizationSystem.OnLanguageChange -= LanguageChange;
        }

        protected abstract void UpdateString(string content);
    }
}