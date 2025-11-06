using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShibaToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler {
    [SerializeField]
    private Image _onCheckmark, _offCheckmark;

    [SerializeField]
    private Sprite _onHover, _offHover;

    private Sprite _onDefault, _offDefault;
    private bool _isHovered;

    private void Awake() {
        _onDefault = _onCheckmark.sprite;
        _offDefault = _offCheckmark.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHovered = true;
        UpdateSprites();
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHovered = false;
        UpdateSprites();
    }

    public void OnSelect(BaseEventData eventData) {
        UpdateSprites();
    }

    private void UpdateSprites() {
        if (_isHovered) {
            _onCheckmark.sprite = _onHover;
            _offCheckmark.sprite = _offHover;
        } else {
            _onCheckmark.sprite = _onDefault;
            _offCheckmark.sprite = _offDefault;
        }
    }

    public void OnToggleChange(bool isOn) {
        _onCheckmark.gameObject.SetActive(isOn);
        _offCheckmark.gameObject.SetActive(!isOn);
    }
}