using System;
using System.Collections;
using Helpers;
using UnityEngine;

public class DzenManager {
    
    private readonly ObjectPool<Dzen> _dzenPool;
    
    private const float MinSpawnInterval = .35f;
    private const float MaxSpawnInterval = 1.85f;
    private const float MaxLifetime = 3f;
    private const float MinLifetime = 1.5f;
    private const int MaxDzenOnScreen = 20;

    private Coroutine _spawnCoroutine;

    public Observable<int> DzenAmount { get; } = new(StateManager.InitialDzenAmount);

    public DzenManager() {
        _dzenPool = new ObjectPool<Dzen>(Game.Instance.PrefabsStorage.DzenPrefab, MaxDzenOnScreen, Game.Instance.DzenParent);
        _spawnCoroutine = Game.Instance.StartCoroutine(SpawnRoutine());

        foreach (var dz in _dzenPool.Pool) {
            dz.OnGather += OnDzenGather;
            dz.OnExpire += OnDzenExpired;
        }
    }

    public void Deinitialize() {
        if (_spawnCoroutine == null) 
            return;
        
        foreach (var dz in _dzenPool.Pool) {
            dz.OnGather -= OnDzenGather;
            dz.OnExpire -= OnDzenExpired;
        }
        
        Game.Instance.StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitUntil(() => Game.Instance.StateManager.IsGameStarted.Value());
            yield return new WaitUntil(() => !Game.Instance.StateManager.IsPaused.Value());
            
            var interval = Game.Instance.GetWaveInterval(MaxSpawnInterval, MinSpawnInterval);
            yield return new WaitForSeconds(interval);
            SpawnDzen();
        }
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
        _dzenPool.Release(dzen);
        // var amount = Mathf.RoundToInt(Game.Instance.StreakController.DzenMultiplier * 1);
        DzenAmount.Set(DzenAmount.Value() + 1);
    }
    
    private void OnDzenExpired(Dzen dzen) {
        _dzenPool.Release(dzen);
    }
}
