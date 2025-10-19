using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {
 
    [Header("Steps")]
    [SerializeField] private List<GameObject> _steps = new();
    [Header("UI")]
    [SerializeField] private Image _panel;
    [Header("Settings")]
    [SerializeField] private bool _startOnAwake = true;

    private int _currentStepIndex = -1;
    private bool _isRunning;
    private Coroutine _typingRoutine;
    private bool _isLockedForClicks;

    private GameObject CurrentStep => (_currentStepIndex >= 0 && _currentStepIndex < _steps.Count) ? _steps[_currentStepIndex] : null;

    private void Awake() {
        if (_startOnAwake)
            StartTutorial();
        else
            _panel.gameObject.SetActive(false);
    }

    private void Update() {
        if (!_isRunning) 
            return;

        if (Input.GetMouseButtonDown(0) && !_isLockedForClicks)
            NextStep();
    }

    private void OnDisable() {
        if (_typingRoutine != null)
            StopCoroutine(_typingRoutine);
    }

    public void StartTutorial() {
        if (_isRunning || _steps.Count == 0)
            return;

        _isRunning = true;
        _currentStepIndex = -1;

        if (_panel != null)
            _panel.gameObject.SetActive(true);

        NextStep();
    }

    public void NextStep() {
        _isLockedForClicks = true;
        HideCurrentStep();

        _currentStepIndex++;
        if (_currentStepIndex >= _steps.Count) {
            EndTutorial();
            return;
        }

        ShowCurrentStep();
    }

    public void EndTutorial() {
        if (!_isRunning) 
            return;

        HideCurrentStep();
        _isRunning = false;
        _currentStepIndex = -1;

        Game.Instance.AudioManager.SetKeyboardSFX(false);
        Game.Instance.StateManager.IsGameStarted.Set(true);
        
        _panel.DOFade(0, 1f).OnComplete(() => _panel.gameObject.SetActive(false));
    }

    private void ShowCurrentStep() {
        var step = CurrentStep;
        if (step == null) 
            return;

        step.SetActive(true);
        Game.Instance.AudioManager.SetKeyboardSFX(true);
        _typingRoutine = StartCoroutine(TypeText(CurrentStep.GetComponentInChildren<TMP_Text>()));

    }
    
    private IEnumerator TypeText(TMP_Text label) {
        var cache = label.text;
        label.text = "";
        foreach (char c in cache) {
            label.text += c;
            yield return new WaitForSecondsRealtime(.005f);
        }
        
        Game.Instance.AudioManager.SetKeyboardSFX(false);
        _isLockedForClicks = false;
    }

    private void HideCurrentStep() {
        var step = CurrentStep;
        if (step == null)
            return;

        step.SetActive(false);
    }
}
