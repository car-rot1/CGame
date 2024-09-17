#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CGame
{
    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class InventorySlotMovementManagerBase<T> : Graphic, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler where T : IInventoryItem
    {
        [SerializeField] private InventoryViewBase<T> inventoryView;
        
#if UNITY_EDITOR
        protected new void Reset()
        {
            base.Reset();
            RefreshComponentValue();
        }

#if ODIN_INSPECTOR_3
        [Button]
#endif
        private void RefreshComponentValue()
        {
            inventoryView = GetComponent<InventoryViewBase<T>>();
        }
#endif

        protected InventoryBase<T> CurrentInventory => inventoryView.CurrentInventory;
        protected InventorySlotBase<T>[] Slots => inventoryView.slots;
        
        public InventorySlotMovementBase<T> slotMovement;

        private enum MovementType
        {
            Click,
            Drag
        }

        [SerializeField] private MovementType movementType;

        private bool _startMovement;

        public void OnPointerDown(PointerEventData eventData)
        {
            var index = -1;
            for (var i = 0; i < CurrentInventory.Capacity; i++)
            {
                var slot = Slots[i];
                if (!slot.canMovement || !RectTransformUtility.RectangleContainsScreenPoint(slot.SelfTransform, eventData.position))
                    continue;
                index = i;
                break;
            }

            if (!_startMovement)
            {
                if (BeginControllerSlot(index, slotMovement, eventData))
                {
                    _startMovement = true;
                    slotMovement.RectTransform.position = eventData.position;
                }
            }
            else if (_startMovement)
            {
                if (EndControllerSlot(index, slotMovement, eventData))
                {
                    _startMovement = false;
                }
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_startMovement)
                slotMovement.RectTransform.position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (movementType is MovementType.Click)
                return;

            if (slotMovement.Item == null)
                return;
            
            var index = -1;
            for (var i = 0; i < CurrentInventory.Capacity; i++)
            {
                var slot = Slots[i];
                if (!slot.canMovement || !RectTransformUtility.RectangleContainsScreenPoint(slot.SelfTransform, eventData.position))
                    continue;
                index = i;
                break;
            }

            if (EndControllerSlot(index, slotMovement, eventData))
            {
                _startMovement = false;
            }
        }

        protected virtual bool BeginControllerSlot(int index, InventorySlotMovementBase<T> slotMovement, PointerEventData eventData)
        {
            if (index == -1 || !CurrentInventory.HasItem(index))
                return false;
            
            var info = CurrentInventory.GetInventorySlotInfo(index);
            slotMovement.Item = info.item;
            slotMovement.Num = info.num;
            CurrentInventory.RemoveItem(info.item, index, info.num);
            return true;
        }

        protected virtual bool EndControllerSlot(int index, InventorySlotMovementBase<T> slotMovement, PointerEventData eventData)
        {
            if (index == -1)
                return false;
            
            var num = CurrentInventory.AddItem(slotMovement.Item, index, slotMovement.Num);
            if (num <= 0)
            {
                slotMovement.Clear();
                return true;
            }

            slotMovement.Num = num;
            return false;
        }
    }
}

