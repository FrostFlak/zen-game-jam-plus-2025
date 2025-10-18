using Helpers;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour {

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Switch _sfxSwitch;
    [SerializeField] private Switch _musicSwitch;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    private readonly string _sfxParameter = "SFX";
    private readonly string _musicParameter = "Music";

    private void Awake() {
        _musicSwitch.AddListener(ToggleMusic);
        _sfxSwitch.AddListener(ToggleSFX);
    }

    private void OnDisable() {
        _musicSwitch.RemoveAllListeners();
        _sfxSwitch.RemoveAllListeners();
    }

    private void ToggleMusic(bool state) => _mixer.SetFloat(_musicParameter, state ? 0 : -80);
    
    private void ToggleSFX(bool state) => _mixer.SetFloat(_sfxParameter, state ? 0 : -80);

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void TogglePause() {
        Game.Instance.StateManager.IsPaused.Set(!Game.Instance.StateManager.IsPaused.Value());
        _canvasGroup.SetState(Game.Instance.StateManager.IsPaused.Value());
        
        Log.Debug(Game.Instance.StateManager.IsPaused.Value() ? "Game Paused" : "Game Resumed");
    }
}
