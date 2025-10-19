using System.Collections;
using Helpers;
using UnityEngine;

public class DzenManager {
    
    private readonly ObjectPool<Dzen> _dzenPool;
    
    private const float MinSpawnInterval = 1f;
    private const float MaxSpawnInterval = 3f;
    private const float MaxLifetime = 2f;
    private const float MinLifetime = 1f;
    private const int MaxDzenOnScreen = 20;

    private Coroutine _spawnCoroutine;

    public Observable<int> DzenAmount { get; } = new(StateManager.InitialDzenAmount);

    public DzenManager() {
        _dzenPool = new ObjectPool<Dzen>(Game.Instance.PrefabsStorage.DzenPrefab, MaxDzenOnScreen, Game.Instance.DzenParent);
        _spawnCoroutine = Game.Instance.StartCoroutine(SpawnRoutine());
        
        Game.Instance.StateManager.IsGameStarted.OnUpdate += OnGameStateChange;

        foreach (var dz in _dzenPool.All) {
            dz.OnGather += OnDzenGather;
            dz.OnExpire += OnDzenExpired;
        }
    }

    public void Deinitialize() {
        if (_spawnCoroutine == null) 
            return;
        
        Game.Instance.StateManager.IsGameStarted.OnUpdate -= OnGameStateChange;
        
        foreach (var dz in _dzenPool.All) {
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
    
    private void OnGameStateChange(bool arg1, bool started) {
        if (started)
            return;
        
        Game.Instance.StopCoroutine(_spawnCoroutine);

        foreach (var dzen in _dzenPool.All)
            dzen.gameObject.SetActive(false);
    }
    
    private void OnDzenGather(Dzen dzen) {
        _dzenPool.Release(dzen);
        DzenAmount.Set(DzenAmount.Value() + 1);
    }
    
    private void OnDzenExpired(Dzen dzen) {
        _dzenPool.Release(dzen);
    }
}
