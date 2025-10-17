using System;
using Helpers;
using UnityEngine;
using System.Collections;

public class AdsManager {
    
    private readonly ObjectPool<Ad> _adPool;
    
    private const float MinSpawnInterval = 1f;
    private const float MaxSpawnInterval = 4f;

    private Coroutine _spawnCoroutine;

    public Observable<int> ActiveAds { get; private set; } = new(0);
    public const int MaxAds = 30;

    public AdsManager() {
        _adPool = new ObjectPool<Ad>(Game.Instance.PrefabsStorage.AdPrefab, MaxAds, Game.Instance.AdsParent);
        _spawnCoroutine = Game.Instance.StartCoroutine(SpawnRoutine());
    }

    public void Deinitialize() {
        if (_spawnCoroutine == null) 
            return;
        
        Game.Instance.StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine() {
        while (ActiveAds.Value() < MaxAds) {
            float interval = Mathf.Lerp(MaxSpawnInterval, MinSpawnInterval, Game.Instance.GetDifficulty());
            yield return new WaitForSeconds(interval);

            SpawnAd();
        }
    }
    
    private void SpawnAd() {
        var ad = _adPool.Get();
        ad.transform.position = ScreenHelper.GetRandomScreenPosition(0.15f);
        
        ActiveAds.Set(ActiveAds.Value() + 1);
        Game.Instance.DzenManager.GatheredDzen.Set(Math.Max(0, Game.Instance.DzenManager.GatheredDzen.Value() - 1));

        ad.OnClose -= HandleAdClose;
        ad.OnClose += HandleAdClose;
    }

    private void HandleAdClose(Ad popup) {
        _adPool.Release(popup);
        ActiveAds.Set(ActiveAds.Value() - 1);
    }
}