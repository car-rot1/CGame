#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.UI;

namespace CGame
{
    public class KeySetManager : MonoBehaviour
    {
        [SerializeField] private KeySet[] keySets;
        public Button saveButton;
        public Button resetButton;

        private void Awake()
        {
            foreach (var keySet in keySets)
            {
                keySet.Init();
            }

            saveButton.onClick.AddListener(() =>
            {
                foreach (var keySet in keySets)
                {
                    keySet.SaveKey();
                }
            });
            resetButton.onClick.AddListener(() =>
            {
                foreach (var keySet in keySets)
                {
                    keySet.ResetKey();
                }
            });
        }
    }
}
#endif
