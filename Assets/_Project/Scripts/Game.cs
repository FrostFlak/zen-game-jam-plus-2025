using Helpers;
using UnityEngine;

public class Game : SingletonMonoBehaviour<Game> {

    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public TutorialController TutorialController { get; private set; }
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

        // --- Normalize Dzen progress from 0 → 100 ---
        float dzenProgress = Mathf.InverseLerp(0f, 100f, dzen);
    
        // Make early dzens easier (first ~10) and later harder
        dzenProgress = Mathf.Pow(dzenProgress, 1.5f); // exponent >1 slows down early progress

        // --- Time-based progress (optional: makes waves slightly faster over time) ---
        float timeProgress = Mathf.Clamp01(
            StateManager.ElapsedTimeFromStart < 60f 
                ? StateManager.ElapsedTimeFromStart / 60f * 0.2f
                : 0.2f + (StateManager.ElapsedTimeFromStart - 60f) / 120f
        );

        float overallProgress = Mathf.Clamp01(dzenProgress + timeProgress);

        // --- Base interval ---
        float baseInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, overallProgress);

        // --- Sin wave spikes ---
        float waveFreq = 1.5f + overallProgress * 2.5f;   // more frequent spikes as progress increases
        float wave = Mathf.Pow(Mathf.Sin(Time.time * Mathf.PI * waveFreq), 3f) * 0.5f;

        return Mathf.Clamp(baseInterval * (1f + wave), minSpawnInterval, maxSpawnInterval);
    }

}