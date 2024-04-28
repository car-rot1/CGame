using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CGame
{
    public abstract class ItemSlotBase<T> : MonoBehaviour where T : IInventoryItem
    {
        public bool canMovement = true;
        public RectTransform movementTransform;
        public RectTransform SelfTransform { get; private set; }
        
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
            private set
            {
                _item = value;
                RefreshItemUI();
            }
        }

        protected virtual void RefreshItemUI()
        {
            if (spriteImage == null)
                return;

            if (HasItem())
            {
                spriteImage.sprite = null;
                spriteImage.color = Color.clear;
            }
            else
            {
                spriteImage.sprite = Item.Sprite;
                spriteImage.color = Color.white;
            }
        }
        
        private int _num;
        public int Num
        {
            get => _num;
            private set
            {
                _num = value;
                RefreshNumUI();
            }
        }

        protected virtual void RefreshNumUI()
        {
            if (numText == null)
                return;
            numText.text = Item?.MaxNum > 1 ? _num.ToString() : "";
        }

        private void Awake()
        {
            SelfTransform = GetComponent<RectTransform>();
        }

        public int SetItem(T inventoryItem, int num = 1)
        {
            Item = inventoryItem;
            var addNum = Mathf.Min(Item.MaxNum, num);
            Num = addNum;
            return num - addNum;
        }

        public int AddNum(int num)
        {
            var addNum = Mathf.Min(Item.MaxNum - Num, num);
            Num += addNum;
            return num - addNum;
        }
        
        public int RemoveNum(int num)
        {
            var resultNum = 0;
            if (Num <= num)
            {
                resultNum = num - Num;
                Clear();
            }
            else
                Num -= num;
            return resultNum;
        }

        public void Clear()
        {
            Item = default;
            Num = 0;
        }

        public bool HasItem() => !EqualityComparer<T>.Default.Equals(Item, default);
    }
}