using System;
using Helpers;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

public class AdsManager {

    private readonly ObjectPool<BaseAd> _adPool;

    private const float MinSpawnInterval = .5f;
    private const float MaxSpawnInterval = 3f;
    private const int MaxAdsOnScreen = 5; // of each type

    private Coroutine _spawnCoroutine;

    public Observable<int> ActiveAds { get; private set; } = new(0);
    public event Action OnAdLifetimeExpired;

    public AdsManager() {
        _adPool = new ObjectPool<BaseAd>(
            new[] {
                Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(kv => kv.Key == AdType.Static).Value,
                Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(kv => kv.Key == AdType.RandomMove).Value,
                Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(kv => kv.Key == AdType.Split).Value,
                Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(kv => kv.Key == AdType.CursorFollower).Value,
                Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(kv => kv.Key == AdType.ShrinkOverTime).Value,
                Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(kv => kv.Key == AdType.ExpandOverTime).Value
            },
            MaxAdsOnScreen,
            Game.Instance.AdsParent
        );
        
        foreach (var ad in _adPool.Pool) {
            ad.OnClose += OnAdClose;
            ad.OnLifetimeExpired += OnAdExpired;
        }
        
        _spawnCoroutine = Game.Instance.StartCoroutine(SpawnRoutine());
    }

    public void Deinitialize() {
        foreach (var ad in _adPool.Pool) {
            ad.OnClose -= OnAdClose;
            ad.OnLifetimeExpired -= OnAdExpired;
        }
        
        if (_spawnCoroutine == null)
            return;

        Game.Instance.StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitUntil(() => Game.Instance.StateManager.IsGameStarted.Value());
            yield return new WaitUntil(() => !Game.Instance.StateManager.IsPaused.Value());

            var interval = Game.Instance.GetWaveInterval(MaxSpawnInterval, MinSpawnInterval);
            yield return new WaitForSeconds(interval);
            Spawn();
        }
    }

    public void Spawn(AdType requiredType = AdType.None) {
        AdType spawnType = requiredType;
    
        if (requiredType == AdType.None)
            spawnType = GetWeightedType();
        
        if (!_adPool.Pool.Exists(a => a.AdType == spawnType))
            spawnType = _adPool.Pool.FirstOrDefault().AdType;

        var ad = _adPool.Get(x => x.AdType == spawnType);
        ad.Init();
        ad.transform.position = ScreenHelper.GetRandomScreenPosition(0.25f);
        
        ActiveAds.Set(ActiveAds.Value() + 1);
    }

    public float GetLifetime() {
        float progress = Mathf.Clamp01((float)Game.Instance.DzenManager.DzenAmount.Value() / StateManager.WinDzenCount);
        return Mathf.Lerp(4f, 2.5f, progress);
    }
    
    private AdType GetWeightedType() {
        float totalWeight = 0f;
        foreach (var kv in AdWeight.Weights)
            totalWeight += kv.Value.Weight;

        float rnd = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var kv in AdWeight.Weights) {
            cumulative += kv.Value.Weight;
            if (rnd <= cumulative)
                return kv.Key;
        }

        return AdType.Static;
    }

    private void OnAdClose(BaseAd ad) {
        Game.Instance.AudioManager.PlayCloseAdSFX();
        
        ad.Deinit();
        _adPool.Release(ad);
        ActiveAds.Set(ActiveAds.Value() - 1);
    }
    
    private void OnAdExpired(BaseAd ad) {
        OnAdLifetimeExpired?.Invoke();
        
        ad.Deinit();
        _adPool.Release(ad);
        ActiveAds.Set(ActiveAds.Value() - 1);
        
        Game.Instance.DzenManager.DzenAmount.Set(
            Math.Max(0, Game.Instance.DzenManager.DzenAmount.Value() - 1)
        );
    }
}