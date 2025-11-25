using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public static class Utils {
    public static async UniTask WaitForClick(CancellationToken token) {
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
}