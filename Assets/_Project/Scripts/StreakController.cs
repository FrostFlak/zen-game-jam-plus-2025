using Helpers;
using UnityEngine;

public class StreakController {

    private int _currentStreak;
    private int _dzenInCurrentStreak;
    private const int DzenPerStreakLevel = 10;
    private const float StreakDuration = 3f;
    private readonly Timer _streakTimer;

    public float DzenMultiplier { get; private set; } = 1f;

    public StreakController() {
        _streakTimer = new Timer(Game.Instance);

        Game.Instance.DzenManager.DzenAmount.OnUpdate += OnDzenAmountUpdate;
        Game.Instance.AdsManager.OnAdLifetimeExpired += OnAdLifetimeExpired;
    }

    public void Deinitialize() {
        Game.Instance.DzenManager.DzenAmount.OnUpdate -= OnDzenAmountUpdate;
        Game.Instance.AdsManager.OnAdLifetimeExpired -= OnAdLifetimeExpired;
        _streakTimer.Stop();
    }

    private void ApplyModifier() {
        _currentStreak++;
        DzenMultiplier = 1f + Mathf.Min(_currentStreak - 1, 4) * 0.5f;
        Log.Debug($"Streak level: {_currentStreak}, DzenMultiplier: {DzenMultiplier}");

        // Restart timer every time a streak level is applied
        _streakTimer.Start(StreakDuration, onComplete: ResetStreak);
    }

    private void ResetStreak() {
        _currentStreak = 0;
        _dzenInCurrentStreak = 0;
        DzenMultiplier = 1f;
        
        _streakTimer.Stop();
        Log.Debug("Streak reset");
    }

    private void OnDzenAmountUpdate(int previousAmount, int newAmount) {
        int delta = newAmount - previousAmount;
        if (delta <= 0) {
            ResetStreak();
            return;
        }

        _dzenInCurrentStreak += delta;

        int streakLevels = _dzenInCurrentStreak / DzenPerStreakLevel;
        if (streakLevels <= 0)
            return;

        _dzenInCurrentStreak -= streakLevels * DzenPerStreakLevel;

        for (int i = 0; i < streakLevels; i++)
            ApplyModifier();
    }

    private void OnAdLifetimeExpired() => ResetStreak();
}