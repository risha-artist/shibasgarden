using UnityEngine;

public class InventorySlot : MonoBehaviour {
    [SerializeField] private InventoryItem _currentItem;

    public bool HasItem() {
        return _currentItem != null;
    }

    public InventoryItem GetItem() {
        return _currentItem;
    }

    public void SetItem(InventoryItem item) {
        _currentItem = item;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
    }

    public void Clear() {
        _currentItem = null;
    }

    public InventoryItem Swap(InventoryItem newItem) {
        InventoryItem previous = _currentItem;
        SetItem(newItem);
        return previous;
    }
}