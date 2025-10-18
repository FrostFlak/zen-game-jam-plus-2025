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
    private const int MaxAdsOnScreen = 10;

    private Coroutine _spawnCoroutine;

    public Observable<int> ActiveAds { get; private set; } = new(0);

    public AdsManager() {
        var staticAd = Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(k => k.Key == AdType.Static).Value;
        var moveAd = Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(k => k.Key == AdType.Moveable).Value;
        var splitAd = Game.Instance.PrefabsStorage.AdPrefabs.FirstOrDefault(k => k.Key == AdType.Split).Value;

        _adPool = new ObjectPool<BaseAd>(new[] { splitAd, moveAd, staticAd }, MaxAdsOnScreen, Game.Instance.AdsParent);
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

            var interval = GetSpawnInterval();
            yield return new WaitForSeconds(interval);

            Log.Debug($"Ad interval: {interval}");
            Spawn();
        }
    }

    private float GetSpawnInterval() {
        float progress;

        var dzen = Game.Instance.DzenManager.DzenAmount.Value();
        if (dzen >= StateManager.InitialDzenAmount)
            progress = Mathf.InverseLerp(StateManager.InitialDzenAmount, StateManager.WinDzenCount, dzen);
        else
            progress = -Mathf.InverseLerp(StateManager.LoseDzenCount, StateManager.InitialDzenAmount, dzen);

        // Ads should spawn faster as player gets closer to winning
        float baseInterval = Mathf.Lerp(MaxSpawnInterval, MinSpawnInterval, Mathf.Clamp01(progress * 0.5f + 0.5f));

        // // Modify interval based on number of active ads — more ads → faster spawn
        // float adFactor = Mathf.Lerp(1f, 0.4f, Mathf.Clamp01((float)ActiveAds.Value() / MaxAdsOnScreen));

        return baseInterval;
    }

    public void Spawn(AdType requiredType = AdType.None) {
        AdType spawnType = requiredType;

        if (requiredType == AdType.None)
            spawnType = GetWeightedType();

        var ad = _adPool.Get(x => x.AdType == spawnType);
        ad.SetLifetime(3);
        ad.transform.position = ScreenHelper.GetRandomScreenPosition(0.15f);
        
        ActiveAds.Set(ActiveAds.Value() + 1);
    }

    private AdType GetWeightedType() {
        float totalWeight = 0f;
        foreach (var kv in AdWeight.Weights)
            totalWeight += kv.Value;

        float rnd = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var kv in AdWeight.Weights) {
            cumulative += kv.Value;
            if (rnd <= cumulative)
                return kv.Key;
        }

        return AdType.Static;
    }

    private void OnAdClose(BaseAd ad) {
        if (!_adPool.TryRelease(ad))
            return;
        
        ActiveAds.Set(ActiveAds.Value() - 1);        
    }
    
    private void OnAdExpired(BaseAd ad) {
        if (!_adPool.TryRelease(ad))
            return;
        
        ActiveAds.Set(ActiveAds.Value() - 1);
        Game.Instance.DzenManager.DzenAmount.Set(
            Math.Max(0, Game.Instance.DzenManager.DzenAmount.Value() - 1)
        );
    }
}