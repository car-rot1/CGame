#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CGame
{
    public abstract class InventorySlotBase<T> : MonoBehaviour where T : IInventoryItem
    {
        public bool canMovement = true;
        [field: SerializeField] public RectTransform SelfTransform { get; private set; }
        
        public Image spriteImage;
        public TextMeshProUGUI numText;

        private Sprite _sprite;
        public Sprite Sprite
        {
            get => _sprite;
            private set
            {
                _sprite = value;
                RefreshSpriteUI(_sprite);
            }
        }

        private void RefreshSpriteUI(Sprite sprite)
        {
            if (spriteImage == null)
                return;

            spriteImage.sprite = sprite;
            spriteImage.color = sprite == null ? Color.clear : Color.white;
        }

        private bool _canStack;
        private int _num;
        public int Num
        {
            get => _num;
            private set
            {
                _num = value;
                RefreshNumUI(_num);
            }
        }

        private void RefreshNumUI(int num)
        {
            if (numText == null)
                return;
            
            numText.text = _canStack ? num.ToString() : "";
        }
        
#if UNITY_EDITOR

        protected virtual void Reset()
        {
            RefreshComponentValue();
        }

#if ODIN_INSPECTOR_3
        [Button]
#endif
        private void RefreshComponentValue()
        {
            SelfTransform = GetComponent<RectTransform>();
        }

#endif
        
        public void SetItem(T item, int num = 0)
        {
            _canStack = item.MaxNum > 1;
            Sprite = item.Sprite;
            Num = num;
        }

        public void Clear()
        {
            _canStack = false;
            Sprite = null;
            Num = 0;
        }
    }
}