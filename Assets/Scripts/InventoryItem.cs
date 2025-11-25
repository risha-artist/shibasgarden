using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Canvas _canvas;
    private RectTransform _rect;
    private CanvasGroup _group;
    private Transform _originalParent;
    private GraphicRaycaster _raycaster;

    private void Awake() {
        _rect = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
        _raycaster = _canvas.GetComponent<GraphicRaycaster>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        _originalParent = transform.parent;
        transform.SetParent(_canvas.transform);
        _group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        _group.blocksRaycasts = true;

        InventorySlot targetSlot = RaycastSlot(eventData);
        InventorySlot originalSlot = _originalParent.GetComponent<InventorySlot>();

        if (targetSlot == null) {
            Return();
            return;
        }

        if (targetSlot.HasItem()) {
            InventoryItem swapped = targetSlot.Swap(this);
            originalSlot.SetItem(swapped);
        } else {
            targetSlot.SetItem(this);
            originalSlot.Clear();
        }
    }

    private InventorySlot RaycastSlot(PointerEventData eventData) {
        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(eventData, results);

        foreach (var r in results) {
            InventorySlot slot = r.gameObject.GetComponent<InventorySlot>();
            if (slot != null) {
                return slot;
            }
        }

        return null;
    }

    private void Return() {
        transform.SetParent(_originalParent);
        _rect.localPosition = Vector3.zero;
    }
}
