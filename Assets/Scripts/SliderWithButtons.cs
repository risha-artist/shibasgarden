using UnityEngine;
using UnityEngine.UI;

public class SliderWithButtons : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Button _plusButton, _minusButton;

    [SerializeField]
    private float _stepSize = 0.1f;

    private void Awake() {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateButtonsState();
    }

    private void OnDestroy() {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    public void Plus() {
        float newValue = _slider.value + _stepSize;
        _slider.value = Mathf.Min(newValue, _slider.maxValue);
        UpdateButtonsState();
    }

    public void Minus() {
        float newValue = _slider.value - _stepSize;
        _slider.value = Mathf.Max(newValue, _slider.minValue);
        UpdateButtonsState();
    }

    private void OnSliderValueChanged(float value) {
        UpdateButtonsState();
    }

    private void UpdateButtonsState() {
        _minusButton.interactable = _slider.value > _slider.minValue;
        _plusButton.interactable = _slider.value < _slider.maxValue;
    }
}