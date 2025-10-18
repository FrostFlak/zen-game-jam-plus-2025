using Helpers;
using UnityEngine;

public class Game : SingletonMonoBehaviour<Game> {

    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public PrefabsStorage PrefabsStorage { get; private set; }
    [field: SerializeField] public Transform DzenParent { get; private set; }
    [field: SerializeField] public Transform AdsParent { get; private set; }

    public StateManager StateManager { get; private set; }
    public AdsManager AdsManager { get; private set; }
    public DzenManager DzenManager { get; private set; }
    public StreakController StreakController { get; private set; }

    protected override void Awake() {
        base.Awake();

        StateManager = new StateManager();
        AdsManager = new AdsManager();
        DzenManager = new DzenManager();
        // StreakController = new StreakController();
    }

    private void Update() {
        StateManager.ManualUpdate();
    }

    protected override void OnDisable() {
        base.OnDisable();

        StateManager.Deinitialize();
        AdsManager.Deinitialize();
        DzenManager.Deinitialize();
        // StreakController.Deinitialize();
    }

    public float GetWaveInterval(float maxSpawnInterval, float minSpawnInterval) {
        float dzen = DzenManager.DzenAmount.Value();
        float progress;

        // --- Calculate progress based on dzens ---
        if (dzen >= StateManager.InitialDzenAmount)
            progress = Mathf.InverseLerp(StateManager.InitialDzenAmount, StateManager.WinDzenCount, dzen);
        else
            progress = -Mathf.InverseLerp(StateManager.LoseDzenCount, StateManager.InitialDzenAmount, dzen);

        progress = Mathf.Clamp01(progress * 0.5f + 0.5f);

        float elapsed = StateManager.ElapsedTimeFromStart; // your timer
        float easyPhase = 60f;
        float timeProgress = elapsed < easyPhase ? elapsed / easyPhase * 0.2f : 0.2f + (elapsed - easyPhase) / 120f;
        timeProgress = Mathf.Clamp01(timeProgress);

        float overallProgress = Mathf.Clamp01(progress + timeProgress);

        float baseInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, overallProgress);

        // --- Wave spikes (alternating easy/hard waves) ---
        float waveFrequency = 3f + overallProgress * 2.5f;   // more frequent spikes as progress increases
        float waveAmplitude = 0.75f;                        // intensity of spikes
        float spikeSharpness = 4f;                         // makes spikes pointier

        // alternating wave: sin goes from 0→1→0, ^ spikeSharpness makes it spikier
        float wave = Mathf.Pow(Mathf.Sin(Time.time * Mathf.PI * waveFrequency), spikeSharpness) * waveAmplitude;

        // --- Final interval ---
        float intervalWithWave = Mathf.Clamp(baseInterval * (1f + wave), minSpawnInterval, maxSpawnInterval);
        return intervalWithWave;
    }
}
