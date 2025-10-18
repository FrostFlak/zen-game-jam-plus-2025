using Helpers;
using UnityEngine;

public class Game : SingletonMonoBehaviour<Game> {

    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public PrefabsStorage PrefabsStorage { get; private set; }
    [field: SerializeField] public Transform DzenParent { get; private set; }
    [field: SerializeField] public Transform AdsParent { get; private set; }

    public StateManager StateManager { get; private set; }
    public AdsManager AdsManager { get; private set; }
    public DzenManager DzenManager { get; private set; }

    protected override void Awake() {
        base.Awake();

        StateManager = new StateManager();
        AdsManager = new AdsManager();
        DzenManager = new DzenManager();
    }

    protected override void OnDisable() {
        base.OnDisable();

        StateManager.Deinitialize();
        AdsManager.Deinitialize();
        DzenManager.Deinitialize();
    }
}
