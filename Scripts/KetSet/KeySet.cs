#if ENABLE_INPUT_SYSTEM
using CGame.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CGame
{
    public class KeySet : MonoBehaviour
    {
        public string overrideJson;
        
        public string customKeyName;
        public InputActionReference inputActionReference;
        public int index;

#if UNITY_TEXTMESHPRO
        private LocalizationTextMeshProUGUI _keyNameText;
#else
        private LocalizationText _keyNameText;
#endif
        private Button _rebindKeyButton;
#if UNITY_TEXTMESHPRO
        private TextMeshProUGUI _keyText;
#else
        private Text _keyText;
#endif

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

        private string _defaultPath;
        private string _rebindPath;

        public void Init()
        {
            if (!string.IsNullOrEmpty(overrideJson))
                inputActionReference.action.LoadBindingOverridesFromJson(overrideJson);
                
#if UNITY_TEXTMESHPRO
            _keyNameText = GetComponentInChildren<LocalizationTextMeshProUGUI>();
#else
            _keyNameText = GetComponentInChildren<LocalizationText>();
#endif
            _rebindKeyButton = GetComponentInChildren<Button>();
#if UNITY_TEXTMESHPRO
            _keyText = _rebindKeyButton.GetComponentInChildren<TextMeshProUGUI>();
#else
            _keyText = _rebindKeyButton.GetComponentInChildren<Text>();
#endif

            _keyNameText.Id = string.IsNullOrEmpty(customKeyName) ? inputActionReference.action.name : customKeyName;
            _rebindKeyButton.onClick.AddListener(RebindKey);
            _keyText.text = inputActionReference.action.GetBindingDisplayString(index);

            _defaultPath = inputActionReference.action.bindings[index].path;
        }

        private void RebindKey()
        {
            inputActionReference.action.Disable();

            _rebindingOperation?.Cancel();
            _rebindingOperation = inputActionReference.action.PerformInteractiveRebinding(index)
                .WithControlsExcluding("Mouse")
                .OnCancel(rebindingOperation =>
                {
                    CleanUp();
                    inputActionReference.action.Enable();
                    _keyText.text = inputActionReference.action.GetBindingDisplayString(index);
                })
                .OnComplete(rebindingOperation =>
                {
                    CleanUp();
                    inputActionReference.action.Enable();
                })
                .OnApplyBinding((rebindingOperation, path) =>
                {
                    _rebindPath = path;
                    _keyText.text = InputControlPath.ToHumanReadableString(path,
                        InputControlPath.HumanReadableStringOptions.OmitDevice |
                        InputControlPath.HumanReadableStringOptions.UseShortNames, rebindingOperation.selectedControl);
                });
            _rebindingOperation.Start();
            _keyText.text = "...";

            return;

            void CleanUp()
            {
                _rebindingOperation?.Dispose();
                _rebindingOperation = null;
            }
        }

        public void SaveKey()
        {
            ApplyKeyPath(_rebindPath);
        }

        public void ResetKey()
        {
            ApplyKeyPath(_defaultPath);
        }

        private void ApplyKeyPath(string path)
        {
            if (string.IsNullOrEmpty(path) || inputActionReference.action.bindings[index].effectivePath.Equals(path))
                return;
            inputActionReference.action.ApplyBindingOverride(index, path);
            overrideJson = inputActionReference.action.SaveBindingOverridesAsJson();
            _keyText.text = inputActionReference.action.GetBindingDisplayString(index);
            GlobalEvent.SendEvent("RebindKey", inputActionReference);
        }
    }
}
#endif
