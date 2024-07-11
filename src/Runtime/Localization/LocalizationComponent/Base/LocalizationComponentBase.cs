using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    public abstract class LocalizationComponentBase : MonoBehaviour
    {
        protected LocalizationSystem localizationSystem;

        protected virtual void Awake()
        {
            Init();
            LanguageChange(localizationSystem.Language);
            localizationSystem.OnLanguageChange += LanguageChange;
        }

        protected virtual void Init()
        {
            localizationSystem = LocalizationSystem.Instance;
        }

        protected abstract void LanguageChange(string language);

        protected virtual void OnDestroy()
        {
            localizationSystem.OnLanguageChange -= LanguageChange;
        }
    }
}