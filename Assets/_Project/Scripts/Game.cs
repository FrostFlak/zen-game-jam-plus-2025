using System;
using Helpers;
using UnityEngine;

public class Game : SingletonMonoBehaviour<Game> {

    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public PrefabsStorage PrefabsStorage { get; private set; }
    [field: SerializeField] public Transform DzenParent { get; private set; }
    [field: SerializeField] public Transform AdsParent { get; private set; }

    public AdsManager AdsManager { get; private set; }
    public DzenManager DzenManager { get; private set; }

    private const float DifficultyRampTime = 300f;
    private float _elapsedTime;

    protected override void Awake() {
        base.Awake();

        AdsManager = new AdsManager();
        DzenManager = new DzenManager();
    }

    private void Update() {
        _elapsedTime += Time.deltaTime;
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        AdsManager.Deinitialize();
        DzenManager.Deinitialize();
    }

    public float GetDifficulty() => Mathf.Clamp01(_elapsedTime / DifficultyRampTime);
}
