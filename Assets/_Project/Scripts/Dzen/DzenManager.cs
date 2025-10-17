using System.Collections;
using Helpers;
using UnityEngine;

public class DzenManager {
    
    private readonly ObjectPool<Dzen> _dzenPool;
    
    private const float MinSpawnInterval = .5f;
    private const float MaxSpawnInterval = 3f;
    private const int PoolDzen = 30;

    private Coroutine _spawnCoroutine;

    public Observable<int> GatheredDzen { get; private set; } = new(0);

    public DzenManager() {
        _dzenPool = new ObjectPool<Dzen>(Game.Instance.PrefabsStorage.DzenPrefab, PoolDzen, Game.Instance.DzenParent);
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
            float interval = Mathf.Lerp(MaxSpawnInterval, MinSpawnInterval, Game.Instance.GetDifficulty());

            // Slows the spawn interval if a lot of ads are active            
            float adFactor = Mathf.Lerp(1f, 2f, (float)Game.Instance.AdsManager.ActiveAds.Value() / AdsManager.MaxAds);
            interval *= adFactor;

            yield return new WaitForSeconds(interval);
            SpawnDzen();
        }
    }
    
    private void SpawnDzen() {
        _dzenPool.Get().transform.position = ScreenHelper.GetRandomScreenPosition(0.15f);
    }

    
    private void OnDzenGather(Dzen dzen) {
        _dzenPool.Release(dzen);
        GatheredDzen.Set(GatheredDzen.Value() + 1);   
    }
    
    private void OnDzenDied(Dzen dzen) {
        _dzenPool.Release(dzen);
    }
}
