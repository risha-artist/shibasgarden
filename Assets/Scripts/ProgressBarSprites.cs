using UnityEngine;
using UnityEngine.UI;

public class ProgressBarSprites : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Image _fill;

    [SerializeField]
    private Image _secondSprite;

    [SerializeField]
    private Sprite _greenSprite;

    [SerializeField]
    private Sprite _yellowSprite;

    [SerializeField]
    private Sprite _redSprite;

    [SerializeField]
    private float _yellowPercent = 0.5f, _redPercent = 0.3f;

    [SerializeField]
    private Animation _animation;

    public void OnClicked() {
        if (_slider.value >= 1f) {
            _slider.value = 0f;
        } else {
            _slider.value += 0.1f;
        }

        _animation.Stop();
        _animation.Play();
    }

    private void Awake() {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateSprites();
    }

    private void OnDestroy() {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value) {
        UpdateSprites();
    }

    private void UpdateSprites() {
        float percent = (_slider.value - _slider.minValue) / (_slider.maxValue - _slider.minValue);
        Sprite currentSprite;

        if (percent > _yellowPercent) {
            currentSprite = _greenSprite;
        } else if (percent > _redPercent) {
            currentSprite = _yellowSprite;
        } else {
            currentSprite = _redSprite;
        }

        if (_fill != null) {
            _fill.sprite = currentSprite;
        }

        if (_secondSprite != null) {
            _secondSprite.sprite = currentSprite;
        }
    }
}