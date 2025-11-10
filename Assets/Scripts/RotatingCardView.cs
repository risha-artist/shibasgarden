using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RotatingCardView : MonoBehaviour {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _flipStart, _flipEnd;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private List<Sprite> _sprites;

    private Queue<Sprite> _spriteQueue;

    private bool _isAnimating;

    private void Awake() {
        _spriteQueue = new Queue<Sprite>(_sprites.OrderBy(v => Random.Range(0, 1f)));
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
        var sprite = _spriteQueue.Dequeue();
        _image.sprite = sprite;
        _spriteQueue.Enqueue(sprite);
    }
}