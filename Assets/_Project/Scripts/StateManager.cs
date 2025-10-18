using Helpers;
using UnityEngine;

public class StateManager : IManualUpdate{

    public const int LoseDzenCount = 0;
    public const int InitialDzenAmount = 50;
    public const int WinDzenCount = 100;

    public Observable<bool> IsPaused = new(false);
    public Observable<bool> IsGameStarted = new(false);
    public float ElapsedTimeFromStart { get; set; }
    
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
        if (Game.Instance.DzenManager.DzenAmount.Value() <= LoseDzenCount) {
            Log.Debug("Lose");
            IsGameStarted.Set(false);
        }
        else if (Game.Instance.DzenManager.DzenAmount.Value() >= WinDzenCount) {
            Log.Debug("Win");
            IsGameStarted.Set(false);
        }
    }

    private void OnDzenAmountChange(int arg1, int arg2) => CheckState();
}
