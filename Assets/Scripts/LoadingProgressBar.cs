using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour {
    [SerializeField]
    private List<Image> _plants;

    [SerializeField]
    private List<Sprite> _plantSprites;

    [SerializeField]
    private List<Sprite> _oddPlantSprites;

    [SerializeField]
    private Slider _slider;

    private const int FRAMES_PER_PLANT = 6;

    private void Awake() {
        if (_slider != null) {
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        UpdatePlants();
    }

    private void OnDestroy() {
        if (_slider != null) {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value) {
        UpdatePlants();
    }

    private void UpdatePlants() {
        if (_plants == null || _plantSprites == null || _plants.Count == 0 || _plantSprites.Count < FRAMES_PER_PLANT) {
            return;
        }

        float normalizedProgress = 0f;
        if (_slider != null) {
            float range = _slider.maxValue - _slider.minValue;
            if (range > 0) {
                normalizedProgress = (_slider.value - _slider.minValue) / range;
            }
        }

        normalizedProgress = Mathf.Clamp01(normalizedProgress);

        int totalPlants = _plants.Count;
        float progressPerPlant = 1f / totalPlants;

        for (int i = 0; i < totalPlants; i++) {
            float plantStartProgress = i * progressPerPlant;
            float plantEndProgress = (i + 1) * progressPerPlant;

            Image plantImage = _plants[i];
            if (plantImage == null) {
                continue;
            }

            List<Sprite> spritesList = i % 2 == 0 ? _plantSprites : _oddPlantSprites;
            if (normalizedProgress <= plantStartProgress) {
                plantImage.sprite = spritesList[0];
                plantImage.gameObject.SetActive(false);
            } else if (normalizedProgress >= plantEndProgress) {
                plantImage.sprite = spritesList[FRAMES_PER_PLANT - 1];
                plantImage.gameObject.SetActive(true);
            } else {
                float plantLocalProgress = (normalizedProgress - plantStartProgress) / progressPerPlant;
                int frameIndex = Mathf.FloorToInt(plantLocalProgress * FRAMES_PER_PLANT);
                frameIndex = Mathf.Clamp(frameIndex, 0, FRAMES_PER_PLANT - 1);
                plantImage.sprite = spritesList[frameIndex];
                plantImage.gameObject.SetActive(true);
            }
        }
    }
}