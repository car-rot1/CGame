using System.Collections.Generic;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CGame
{
    public abstract class InventorySlotMovementBase<T> : MonoBehaviour where T : IInventoryItem
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        
        public Image spriteImage;
#if UNITY_TEXTMESHPRO
        public TextMeshProUGUI numText;
#else
        public Text numText;
#endif
        private T _item;
        public T Item
        {
            get => _item;
            set
            {
                _item = value;
                RefreshItemUI(_item);
            }
        }

        private void RefreshItemUI(T item)
        {
            if (spriteImage == null)
                return;

            if (EqualityComparer<T>.Default.Equals(item, default))
            {
                spriteImage.sprite = null;
                spriteImage.color = Color.clear;
                gameObject.SetActive(false);
            }
            else
            {
                spriteImage.sprite = Item.Sprite;
                spriteImage.color = Color.white;
                gameObject.SetActive(true);
            }
        }
        
        private int _num;
        public int Num
        {
            get => _num;
            set
            {
                _num = value;
                RefreshNumUI(_num);
            }
        }

        private void RefreshNumUI(int num)
        {
            if (numText == null)
                return;

            if (num <= 0)
            {
                numText.text = "";
                gameObject.SetActive(false);
            }
            else
            {
                numText.text = Item?.MaxNum > 1 ? num.ToString() : "";
                gameObject.SetActive(true);
            }
        }
        
#if UNITY_EDITOR

        private void Reset()
        {
            RefreshComponentValue();
        }

#if ODIN_INSPECTOR_3
        [Button]
#endif
        private void RefreshComponentValue()
        {
            RectTransform = GetComponent<RectTransform>();
        }

#endif
        
        public void Clear()
        {
            Item = default;
            Num = 0;
        }
    }
}
