using Helpers;

public class StateManager {

    public const int LoseDzenCount = 0;
    public const int InitialDzenAmount = 50;
    public const int WinDzenCount = 100;

    public Observable<bool> IsPaused = new(false);
    public Observable<bool> IsGameStarted = new(false);
    
    // Now fake the start, later add tutorial and then start the game
    public StateManager() {
        new Timer(Game.Instance).Start(3,
            onComplete: () => {
                IsGameStarted.Set(true);

                Game.Instance.DzenManager.DzenAmount.OnUpdate += OnDzenAmountChange;
            });
    }

    public void Deinitialize() {
        
    }


    private void CheckState() {
        if (Game.Instance.DzenManager.DzenAmount.Value() <= LoseDzenCount) {
            Log.Debug("Lose");
        }
        else if (Game.Instance.DzenManager.DzenAmount.Value() >= WinDzenCount) {
            Log.Debug("Win");
        }
    }

    private void OnDzenAmountChange(int arg1, int arg2) => CheckState();
}
