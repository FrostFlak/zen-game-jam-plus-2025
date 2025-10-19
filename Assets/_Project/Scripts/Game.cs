using Helpers;
using UnityEngine;

public class Game : SingletonMonoBehaviour<Game> {

    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public PrefabsStorage PrefabsStorage { get; private set; }
    [field: SerializeField] public Transform DzenParent { get; private set; }
    [field: SerializeField] public Transform AdsParent { get; private set; }
    [field: SerializeField] public DzenPhraseDisplay DzenPhraseDisplay { get; private set; }

    public StateManager StateManager { get; private set; }
    public AdsManager AdsManager { get; private set; }
    public DzenManager DzenManager { get; private set; }
    public StreakController StreakController { get; private set; }
    

    protected override void Awake() {
        base.Awake();

        StateManager = new StateManager();
        AdsManager = new AdsManager();
        DzenManager = new DzenManager();
        DzenPhraseDisplay.Init();
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
        float time = StateManager.ElapsedTimeFromStart;

        // Normalize dzen progress (0 → 1)
        float dzenProgress = Mathf.InverseLerp(0f, 100f, dzen);
        dzenProgress = Mathf.Pow(dzenProgress, 1.6f); // steeper curve = faster difficulty ramp

        // Time influence (slightly harder over time)
        float timeProgress = Mathf.Clamp01(Mathf.Log10(time + 10f) / 2.5f); // grows a bit faster

        // Combined difficulty
        float difficulty = Mathf.Clamp01((dzenProgress * 0.7f) + (timeProgress * 0.3f));

        // Base interval (shorter = harder)
        float baseInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval * 0.7f, difficulty);

        // Rhythmic wave spikes (sin pattern) – more intense and faster
        float waveFreq = 0.25f + difficulty * 0.6f; // higher freq = more spikes
        float waveAmp = Mathf.Lerp(0.25f, 0.75f, difficulty); // bigger spikes
        float wave = Mathf.Sin(time * Mathf.PI * waveFreq * 2f) * waveAmp; // extra factor to increase wave speed

        // Apply wave to interval and clamp
        float finalInterval = Mathf.Clamp(baseInterval * (1f + wave), minSpawnInterval * 0.5f, maxSpawnInterval);

        // --- Debug info ---
        string phase = difficulty switch {
            < 0.2f => "🪷 Calm start",
            < 0.5f => "🌊 Rising distractions",
            < 0.8f => "🔥 Intense focus test",
            _ => "☯️ Chaotic enlightenment"
        };

        Log.Debug($"[{phase}] Dzen={dzen:F1}, Time={time:F1}s, Interval={finalInterval:F2}s");
        return finalInterval;
    }
}