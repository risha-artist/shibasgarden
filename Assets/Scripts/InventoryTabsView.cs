using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabsView : MonoBehaviour {
    [SerializeField]
    private List<Image> _tabIcons;

    [SerializeField]
    private List<Toggle> _tabToggles;

    [SerializeField]
    private Color _transparentColor = new Color(1f, 1f, 1f, 0.5f);

    private void Awake() {
        OnTabToggle(false);
    }

    public void OnTabToggle(bool isOn) {
        for (int index = 0; index < _tabToggles.Count; index++) {
            Toggle toggle = _tabToggles[index];
            _tabIcons[index].color = toggle.isOn ? Color.white : _transparentColor;
        }
    }
}

