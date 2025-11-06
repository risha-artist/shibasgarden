using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Sprite _hoverSprite;

    private Sprite _defaultSprite;

    private void Awake() {
        if (_image == null) {
            _image = GetComponent<Image>();
        }
        _defaultSprite = _image.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_hoverSprite != null) {
            _image.sprite = _hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        _image.sprite = _defaultSprite;
    }
}


