using UnityEngine;
using UnityEngine.Events;

namespace CGame.Localization
{
    public class LocalizationStringUnityEvent : LocalizationStringBase
    {
        [SerializeField] private UnityEvent<string> OnUpdateString;
        
        protected override void UpdateString(string content)
        {
            OnUpdateString?.Invoke(content);
        }
    }
}