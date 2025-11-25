using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private CancellationTokenSource _cts;

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
        _playerAvatar.gameObject.SetActive(true);
        _grandmaAvatar.gameObject.SetActive(false);
        _speechPlayer.gameObject.SetActive(true);
        _speechGrandma.gameObject.SetActive(false);
        _replyContainer.SetActive(false);
        _dialogOptionsContainer.gameObject.SetActive(true);
    }

    private async UniTask Reply(List<string> answers) {
        _replyContainer.SetActive(true);
        _speechPlayer.gameObject.SetActive(false);
        _speechGrandma.gameObject.SetActive(true);

        _playerAvatar.gameObject.SetActive(false);
        _grandmaAvatar.gameObject.SetActive(true);

        foreach (string line in answers) {
            await Print(line);
            await WaitForClick(_cts.Token);
        }
    }

    private async UniTask WaitForClick(CancellationToken token) {
        while (true) {
            if (token.IsCancellationRequested) {
                return;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
                return;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private async UniTask Print(string text) {
        _replyText.text = "";
        CancellationToken token = _cts.Token;

        foreach (char c in text) {
            if (token.IsCancellationRequested) {
                return;
            }

            _replyText.text += c;
            await UniTask.Delay(25, cancellationToken: token);
        }
    }
}