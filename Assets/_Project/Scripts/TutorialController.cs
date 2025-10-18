using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialController : MonoBehaviour {
    [Header("Steps")]
    public List<GameObject> Steps = new List<GameObject>();

    [Header("UI")]
    public GameObject TutorialPanel;

    [Header("Settings")]
    public bool StartOnAwake = false;

    private int _currentStepIndex = -1;
    private bool _isRunning;
    private Coroutine _typingRoutine;

    private GameObject CurrentStep => (_currentStepIndex >= 0 && _currentStepIndex < Steps.Count) ? Steps[_currentStepIndex] : null;

    private void Awake() {
        if (StartOnAwake)
            StartTutorial();
        else
            StopAllTutorialUI();
    }

    private void Update() {
        if (!_isRunning) 
            return;

        if (Input.GetMouseButtonDown(0))
            NextStep();
    }

    public void StartTutorial() {
        if (_isRunning || Steps.Count == 0)
            return;

        _isRunning = true;
        _currentStepIndex = -1;

        if (TutorialPanel != null)
            TutorialPanel.SetActive(true);

        NextStep();
    }

    public void NextStep() {
        HideCurrentStep();

        _currentStepIndex++;
        if (_currentStepIndex >= Steps.Count) {
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

        Game.Instance.StateManager.IsGameStarted.Set(true);
        
        if (TutorialPanel != null)
            TutorialPanel.SetActive(false);
    }

    private void ShowCurrentStep() {
        var step = CurrentStep;
        if (step == null) 
            return;

        step.SetActive(true);
        _typingRoutine = StartCoroutine(TypeText(CurrentStep.GetComponentInChildren<TMP_Text>()));

    }
    
    private IEnumerator TypeText(TMP_Text label) {
        var cache = label.text;
        label.text = "";
        foreach (char c in cache) {
            label.text += c;
            yield return new WaitForSecondsRealtime(.005f);
        }
    }

    private void HideCurrentStep() {
        var step = CurrentStep;
        if (step == null)
            return;

        step.SetActive(false);
    }

    private void StopAllTutorialUI() {
        if (TutorialPanel != null)
            TutorialPanel.SetActive(false);
    }
}
