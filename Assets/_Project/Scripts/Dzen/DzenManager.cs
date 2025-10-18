using System.Collections;
using Helpers;
using UnityEngine;

public class DzenManager {
    
    private readonly ObjectPool<Dzen> _dzenPool;
    
    private const float MinSpawnInterval = .5f;
    private const float MaxSpawnInterval = 1.85f;
    private const float MaxLifetime = 3f;
    private const float MinLifetime = 1.5f;
    private const int MaxDzenOnScreen = 40;

    private Coroutine _spawnCoroutine;

    public Observable<int> DzenAmount { get; } = new(StateManager.InitialDzenAmount);

    public DzenManager() {
        _dzenPool = new ObjectPool<Dzen>(Game.Instance.PrefabsStorage.DzenPrefab, MaxDzenOnScreen, Game.Instance.DzenParent);
        _spawnCoroutine = Game.Instance.StartCoroutine(SpawnRoutine());

        foreach (var dz in _dzenPool.Pool) {
            dz.OnGather += OnDzenGather;
            dz.OnDie += OnDzenDied;
        }
    }

    public void Deinitialize() {
        if (_spawnCoroutine == null) 
            return;
        
        foreach (var dz in _dzenPool.Pool) {
            dz.OnGather -= OnDzenGather;
            dz.OnDie -= OnDzenDied;
        }
        
        Game.Instance.StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitUntil(() => Game.Instance.StateManager.IsGameStarted.Value());
            yield return new WaitUntil(() => !Game.Instance.StateManager.IsPaused.Value());

            var interval = GetSpawnInterval();
            yield return new WaitForSeconds(interval);
            Log.Debug($"Dzen interval: {interval}");
            SpawnDzen();
        }
    }

    private float GetSpawnInterval() {
        float progress;
        if (DzenAmount.Value() >= StateManager.InitialDzenAmount)
            progress = Mathf.InverseLerp(StateManager.InitialDzenAmount, StateManager.WinDzenCount, DzenAmount.Value());
        else
            progress = -Mathf.InverseLerp(StateManager.LoseDzenCount, StateManager.InitialDzenAmount, DzenAmount.Value());

        // Invert progress because dzens should spawn *faster* when player has less
        float inverted = 1f - (progress * 0.5f + 0.5f);
        float interval = Mathf.Lerp(MinSpawnInterval, MaxSpawnInterval, inverted);

        return interval;
    }

    
    private void SpawnDzen() {
        var dzen = _dzenPool.Get();
        dzen.SetLifetime(GetLifetime());
        dzen.transform.position = ScreenHelper.GetRandomScreenPosition(0.15f);
    }

    private float GetLifetime() {
        float progress = Mathf.Clamp01((float)DzenAmount.Value() / StateManager.WinDzenCount);
        return Mathf.Lerp(MaxLifetime, MinLifetime, progress);
    }
    
    private void OnDzenGather(Dzen dzen) {
        _dzenPool.TryRelease(dzen);
        DzenAmount.Set(DzenAmount.Value() + 1);
        Log.Debug($"Gathered: {DzenAmount.Value()}");
    }
    
    private void OnDzenDied(Dzen dzen) {
        _dzenPool.TryRelease(dzen);
    }
}
