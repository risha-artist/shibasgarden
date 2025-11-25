using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RotatingCardView : MonoBehaviour {
    [Header("Card view")]
    [SerializeField]
    private Image _image;

    [SerializeField]
    private TextMeshProUGUI _header, _explain, _effect, _energy;

    [SerializeField]
    private List<GameObject> _stars;

    [Header("Other")]
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _flipStart, _flipEnd;

    [SerializeField]
    private List<CardData> _sprites;

    private Queue<CardData> _datasQueue;

    private bool _isAnimating;

    private void Awake() {
        _datasQueue = new Queue<CardData>(_sprites.OrderBy(v => Random.Range(0, 1f)));
        ShowNextCard();
    }

    public void Click() {
        if (_isAnimating) {
            return;
        }

        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation() {
        _isAnimating = true;
        _animation.Play(_flipStart.name);
        yield return new WaitWhile(() => _animation.isPlaying);
        ShowNextCard();
        _animation.Play(_flipEnd.name);
        yield return new WaitWhile(() => _animation.isPlaying);
        _isAnimating = false;
    }

    private void ShowNextCard() {
        var data = _datasQueue.Dequeue();
        _image.sprite = data.Icon;
        _header.text = data.Header;
        _explain.text = data.Explain;
        _effect.text = data.Effect;
        _energy.text = data.Energy;
        for (int i = 0; i < _stars.Count; i++) {
            _stars[i].SetActive(i < data.Stars);
        }

        _datasQueue.Enqueue(data);
    }
}

[Serializable]
public class CardData {
    public string Header;
    public string Explain;
    public string Effect;
    public string Energy;

    [Min(1)]
    public int Stars;

    public Sprite Icon;
}