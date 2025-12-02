using TMPro;
using UnityEngine;

public class DropdownFirstRecolor : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _toRecolor;

    [SerializeField]
    private Color _newColor;

    public void Recolor() {
        _toRecolor.color = _newColor;
    }
}