using System.Collections.Generic;
using EasyTransition;
using Helpers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour {

    [Header("MenuBtns")]
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _creditsBtn;
    [Header("Settings")]
    [SerializeField] private RectTransform _settingsPanel;
    [SerializeField] private List<Button> _closeSettingsBtn;
    [SerializeField] private Switch _sfxSwitch;
    [SerializeField] private Switch _musicSwitch;
    [Header("Credits")]
    [SerializeField] private RectTransform _creditsPanel;
    [SerializeField] private List<Button> _closeCreditsBtn;
    [Header("Audio")]
    [SerializeField] private AudioSource _clickSource;
    [SerializeField] private AudioSource _switchSource;
    [Header("Components")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private TransitionSettings _transitionSettings;
    
    
    private readonly string _sfxParameter = "SFX";
    private readonly string _musicParameter = "Music";

    private void Awake() {
        _playBtn.onClick.AddListener(LoadGameScene);
        
        _musicSwitch.AddListener(ToggleMusic);
        _sfxSwitch.AddListener(ToggleSFX);
        
        _settingsBtn.onClick.AddListener(() => {
            _clickSource.PlayOneShot(_clickSource.clip);
            _settingsPanel.gameObject.SetActive(true);
            _settingsPanel.Pop(true);
        });
        _closeSettingsBtn.ForEach(b => b.onClick.AddListener(() => {
            _clickSource.PlayOneShot(_clickSource.clip);
            _settingsPanel.Pop(false, onComplete: () => _settingsPanel.gameObject.SetActive(false));
        }));
        
        _creditsBtn.onClick.AddListener(() => {
            _clickSource.PlayOneShot(_clickSource.clip);
            _creditsPanel.gameObject.SetActive(true);
            _creditsPanel.Pop(true);
        });
        
        _closeCreditsBtn.ForEach(b => b.onClick.AddListener(() => {
            _clickSource.PlayOneShot(_clickSource.clip);
            _creditsPanel.Pop(false, onComplete: () => _creditsPanel.gameObject.SetActive(false));
        }));
    }

    private void LoadGameScene() {
        _clickSource.PlayOneShot(_clickSource.clip);
        TransitionManager.Instance().Transition("Game", _transitionSettings, 0f);
    }

    private void OnDisable() {
        _playBtn.onClick.RemoveAllListeners();
        _musicSwitch.RemoveAllListeners();
        _sfxSwitch.RemoveAllListeners();
        
        _settingsBtn.onClick.RemoveAllListeners();
        _creditsBtn.onClick.RemoveAllListeners();
        _closeSettingsBtn.ForEach(b => b.onClick.RemoveAllListeners());
    }

    private void ToggleMusic(bool state) {
        _switchSource.PlayOneShot(_switchSource.clip);
        _mixer.SetFloat(_musicParameter, state ? 0 : -80);
    }

    private void ToggleSFX(bool state) {
        _switchSource.PlayOneShot(_switchSource.clip);
        _mixer.SetFloat(_sfxParameter, state ? 0 : -80);
    }
}
