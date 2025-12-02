using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogView : MonoBehaviour {
    [SerializeField]
    private List<string> _replies0;

    [SerializeField]
    private List<string> _replies1;

    [SerializeField]
    private GameObject _dialogOptionsContainer;

    [SerializeField]
    private GameObject _speechPlayer, _speechGrandma;

    [SerializeField]
    private GameObject _replyContainer, _playerAvatar, _grandmaAvatar;

    [SerializeField]
    private TextMeshProUGUI _replyText;

    [SerializeField]
    private GameObject _backButton;

    [SerializeField]
    private Slider _slider;

    private CancellationTokenSource _cts;

    private bool _isWaitingForContinue;

    [SerializeField]
    private GameObject _continueButton;

    [SerializeField]
    private TextMeshProUGUI _avatarNameText;

    public void Continue() {
        _isWaitingForContinue = false;
    }

    public void SelectVariant(int index) {
        _dialogOptionsContainer.gameObject.SetActive(false);

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        switch (index) {
            case 0: Reply(_replies0).Forget(); break;
            case 1: Reply(_replies1).Forget(); break;
        }
    }

    public void ResetDialog() {
        _slider.value = 0;
        _backButton.gameObject.SetActive(false);
        _playerAvatar.gameObject.SetActive(true);
        _grandmaAvatar.gameObject.SetActive(false);
        _speechPlayer.gameObject.SetActive(true);
        _speechGrandma.gameObject.SetActive(false);
        _replyContainer.SetActive(false);
        _dialogOptionsContainer.gameObject.SetActive(true);
    }

    private async UniTask Reply(List<string> answers) {
        _continueButton.gameObject.SetActive(false);
        _replyContainer.SetActive(true);
        _speechPlayer.gameObject.SetActive(false);
        _speechGrandma.gameObject.SetActive(true);

        _playerAvatar.gameObject.SetActive(false);
        _grandmaAvatar.gameObject.SetActive(true);

        _backButton.gameObject.SetActive(true);

        for (int index = 0; index < answers.Count; index++) {
            string line = answers[index];
            float part = 1f / answers.Count;
            await Print(line, part * index, part * (index + 1));
            if (index != answers.Count - 1) {
                _continueButton.gameObject.SetActive(true);
            }

            _isWaitingForContinue = true;
            await UniTask.WaitWhile(() => _isWaitingForContinue);
            _continueButton.gameObject.SetActive(false);
        }
    }

    private async UniTask Print(string text, float percent, float maxPercent) {
        _replyText.text = "";
        CancellationToken token = _cts.Token;

        for (int index = 0; index < text.Length; index++) {
            char c = text[index];
            if (token.IsCancellationRequested) {
                return;
            }

            _replyText.text += c;
            _slider.value = percent + (maxPercent - percent) * index / text.Length;
            await UniTask.Delay(25, cancellationToken: token);
        }

        _slider.value = maxPercent;
    }

    public void SetName(string playerName) {
        _avatarNameText.text = playerName;
    }
}