using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CGame
{
    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class ItemSlotMovementBase<T> : Graphic, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
        where T : IInventoryItem
    {
        public ItemSlotBase<T>[] itemSlots;

        private enum MovementType
        {
            Click,
            Drag
        }

        [SerializeField] private MovementType slotMovementType;

        private ItemSlotBase<T> _sourceSlotBase;
        private Vector3 _sourceLocalPosition;
        private int _sourceSiblingIndex;
        private Transform _sourceParent;

        private RectTransform _transform;

        protected override void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var downSlot = itemSlots
                .Where(slot =>
                    slot.canMovement &&
                    RectTransformUtility.RectangleContainsScreenPoint(slot.SelfTransform, eventData.position))
                .FirstOrDefault(slot => slot != null);
            if (downSlot == null)
                return;

            if (_sourceSlotBase == null && downSlot.Item != null)
                BeginControllerSlot(downSlot, eventData.position);
            else if (_sourceSlotBase != null)
                EndControllerSlot(downSlot);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_sourceSlotBase != null)
                _sourceSlotBase.movementTransform.position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (slotMovementType is MovementType.Click)
                return;

            var upSlot = itemSlots
                .Where(slot =>
                    slot.canMovement &&
                    RectTransformUtility.RectangleContainsScreenPoint(slot.SelfTransform, eventData.position))
                .FirstOrDefault(slot => slot != null);
            if (upSlot == null)
                return;

            EndControllerSlot(upSlot);
        }

        private void BeginControllerSlot(ItemSlotBase<T> slotBase, Vector3 StartPos)
        {
            _sourceSlotBase = slotBase;

            _sourceLocalPosition = _sourceSlotBase.movementTransform.localPosition;
            _sourceSiblingIndex = _sourceSlotBase.movementTransform.GetSiblingIndex();
            _sourceParent = _sourceSlotBase.movementTransform.parent;

            _sourceSlotBase.movementTransform.SetParent(_transform, false);
            _sourceSlotBase.movementTransform.position = StartPos;
        }

        private void EndControllerSlot(ItemSlotBase<T> targetSlotBase)
        {
            _sourceSlotBase.movementTransform.SetParent(_sourceParent, false);
            _sourceSlotBase.movementTransform.SetSiblingIndex(_sourceSiblingIndex);
            _sourceSlotBase.movementTransform.localPosition = _sourceLocalPosition;

            if (_sourceSlotBase != targetSlotBase)
            {
                SlotCombine(_sourceSlotBase, targetSlotBase);
            }

            _sourceSlotBase = null;
        }

        protected virtual void SlotCombine(ItemSlotBase<T> sourceSlot, ItemSlotBase<T> targetSlot)
        {
        }
    }
}
