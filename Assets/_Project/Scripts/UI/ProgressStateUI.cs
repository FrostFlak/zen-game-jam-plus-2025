using System;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressStateUI : MonoBehaviour {

    [SerializeField] private Slider _distractionSlider;
    [SerializeField] private Slider _dzenSlider;
    [SerializeField] private TMP_Text _dzenAmountLabel;

    private void Awake() {
        _distractionSlider.value = 0.5f;
        _dzenSlider.value = 0.5f;
        
        Game.Instance.DzenManager.DzenAmount.OnUpdate += OnGatherDzenAmount;
        OnGatherDzenAmount(0, Game.Instance.DzenManager.DzenAmount.Value());
    }

    private void OnDisable() {
        Game.Instance.DzenManager.DzenAmount.OnUpdate -= OnGatherDzenAmount;
    }
    
    private void SetCurrentTopSlider() {
        if (_distractionSlider.value > _dzenSlider.value)
            _distractionSlider.transform.SetAsLastSibling();
        else
            _dzenSlider.transform.SetAsLastSibling();
    }
    
    private float GetProgressValue(int current, int target, int initial) {
        float normalized = Mathf.InverseLerp(initial, target, current);
        return Mathf.Lerp(0.5f, 1f, normalized);
    }

    private void OnGatherDzenAmount(int _, int dzenCount) {
        _dzenAmountLabel.SetText(dzenCount.ToString());

        float loseProgress = GetProgressValue(dzenCount, StateManager.LoseDzenCount, StateManager.InitialDzenAmount);
        float winProgress = GetProgressValue(dzenCount, StateManager.WinDzenCount, StateManager.InitialDzenAmount);

        int completed = 0;
        Action onLerpComplete = () => {
            completed++;
            if (completed >= 2)
                SetCurrentTopSlider();
        };

        _distractionSlider.Lerp(loseProgress, onComplete: onLerpComplete);
        _dzenSlider.Lerp(winProgress, onComplete: onLerpComplete);
    }
}
