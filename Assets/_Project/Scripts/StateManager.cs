using System;
using DG.Tweening;
using Helpers;
using UnityEngine;

public class StateManager : IManualUpdate{

    public const int LoseDzenCount = 0;
    public const int InitialDzenAmount = 50;
    public const int WinDzenCount = 100;

    public Observable<bool> IsPaused = new(false);
    public Observable<bool> IsGameStarted = new(false);
    public float ElapsedTimeFromStart { get; set; }

    private bool _gameHasEnded;
    // Now fake the start, later add tutorial and then start the game
    public StateManager() {
        IsGameStarted.OnUpdate += OnGameStarted;        
    }

    private void OnGameStarted(bool arg1, bool arg2) {
        Game.Instance.DzenManager.DzenAmount.OnUpdate += OnDzenAmountChange;
    }

    public void ManualUpdate() {
        if (IsGameStarted.Value() && !IsPaused.Value())
            ElapsedTimeFromStart += Time.deltaTime;
    }

    public void Deinitialize() {
        IsGameStarted.OnUpdate -= OnGameStarted;
    }

    private void CheckState() {
        if (_gameHasEnded)
            return;
        
        if (Game.Instance.DzenManager.DzenAmount.Value() <= LoseDzenCount) {
            _gameHasEnded = true;
            IsGameStarted.Set(false);
            Log.Debug("Lose");
            
            ZoomFace(() => {
                Game.Instance.EndUI.gameObject.SetActive(true);
                Game.Instance.EndUI.SetState(false);
            });
        }
        else if (Game.Instance.DzenManager.DzenAmount.Value() >= WinDzenCount) {
            _gameHasEnded = true;
            IsGameStarted.Set(false);
            Log.Debug("Win");

            ZoomFace(() => {
                Game.Instance.EndUI.gameObject.SetActive(true);
                Game.Instance.EndUI.SetState(true);
            });
        }
    }

    private void ZoomFace(Action onComplete) {
        Game.Instance.Camera.transform
            .DOMove(
                new Vector3(
                    Game.Instance.FaceChangerUI.RealFaceTransform.position.x,
                    Game.Instance.FaceChangerUI.RealFaceTransform.position.y,
                    Game.Instance.Camera.transform.position.z), 
                2.25f
            )
            .SetEase(Ease.InOutSine);
        
        Game.Instance.Camera.DOOrthoSize(3f, 4f).SetEase(Ease.InOutSine).OnComplete(() => onComplete?.Invoke());
    }
    
    private void OnDzenAmountChange(int arg1, int arg2) => CheckState();
}
