using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShibaColorSelect : MonoBehaviour {
    [SerializeField]
    private List<Sprite> _breads, _shibas;

    [SerializeField]
    private List<string> _colorNames;

    [SerializeField]
    private Image _shibaImage, _breadImage;

    [SerializeField]
    private TextMeshProUGUI _colorNameText;

    public void OnValueChanged(float changed) {
        int value = (int)changed;
        _colorNameText.text = _colorNames[value];
        _shibaImage.sprite = _shibas[value];
        _breadImage.sprite = _breads[value];
    }
}