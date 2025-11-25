using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DialogsView : MonoBehaviour {
    private List<string> _found = new List<string>();

    [SerializeField]
    private int _xpTpLvlUp = 3;

    private bool _isGain;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _show, _grow, _achivement, _hide;

    [SerializeField]
    private GameObject _questView, _badgeView;

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Animation _reactAnimation;

    [SerializeField]
    private AnimationClip _questPop;

    private CancellationTokenSource _cts = new CancellationTokenSource();

    public void Increment(string discovery) {
        if (_found.Contains(discovery)) {
            return;
        }

        _found.Add(discovery);

        _slider.value = _found.Count / (_xpTpLvlUp + 0f);

        if (_isGain) {
            return;
        }

        _reactAnimation.Play(_questPop.name);
        
        if (_found.Count >= _xpTpLvlUp) {
            _isGain = true;
            ShowLvlUp().Forget();
        }
    }

    private async UniTask ShowLvlUp() {
        _animation.Play(_show.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        _animation.Play(_grow.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        await Utils.WaitForClick(_cts.Token);
        _animation.Play(_achivement.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        await Utils.WaitForClick(_cts.Token);
        _animation.Play(_hide.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        _questView.gameObject.SetActive(false);
        _badgeView.gameObject.SetActive(true);
    }
}