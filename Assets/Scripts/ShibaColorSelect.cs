using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShibaColorSelect : MonoBehaviour {
    [SerializeField]
    private List<Sprite> _breads, _shibas, _shibasDialogs1, _shibasDialogs2, _shibasAvatars;

    [SerializeField]
    private List<string> _colorNames;

    [SerializeField]
    private Image _shibaImage, _breadImage, _shibaDialog1Image, _shibaDialog2Image, _shibaAvatarImage;

    [SerializeField]
    private TextMeshProUGUI _colorNameText;

    public void OnValueChanged(float changed) {
        int value = (int)changed;
        _colorNameText.text = _colorNames[value];
        _shibaImage.sprite = _shibas[value];
        _breadImage.sprite = _breads[value];
        _shibaDialog1Image.sprite = _shibasDialogs1[value];
        _shibaDialog2Image.sprite = _shibasDialogs2[value];
        _shibaAvatarImage.sprite = _shibasAvatars[value];
    }
}