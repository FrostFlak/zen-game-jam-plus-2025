using Helpers;
using UnityEngine;
using UnityEngine.UI;

public class FeelStateUI : MonoBehaviour {

    [SerializeField] private Slider _distractionSlider;
    [SerializeField] private Slider _dzenSlider;

    private void Awake() {
        _distractionSlider.value = 0.5f;
        _dzenSlider.value = 0.5f;
        
        Game.Instance.AdsManager.ActiveAds.OnUpdate += OnActiveAdsChange;
        Game.Instance.DzenManager.GatheredDzen.OnUpdate += OnGatherDzen;
    }

    private void OnDisable() {
        Game.Instance.AdsManager.ActiveAds.OnUpdate -= OnActiveAdsChange;
        Game.Instance.DzenManager.GatheredDzen.OnUpdate -= OnGatherDzen;
    }
    
    private void SetCurrentTopSlider() {
        if (_distractionSlider.value > _dzenSlider.value)
            _distractionSlider.transform.SetAsLastSibling();
        else
            _dzenSlider.transform.SetAsLastSibling();
    }
    
    private float GetProgressValue(int min, int max) {
        float rawValue = (float)min / max;
        float scaledValue = 0.5f + rawValue * (1f - 0.5f);
        
        return Mathf.Clamp(scaledValue, 0.5f, 1f);
    }

    private void OnActiveAdsChange(int _, int ads) {
        _distractionSlider.Lerp(
            GetProgressValue(ads, AdsManager.MaxAds),
            onComplete: SetCurrentTopSlider
        );
    }
    
    private void OnGatherDzen(int _, int dzenCount) {
        Log.Debug($"OnGatherDzen: {dzenCount}");
        _dzenSlider.Lerp(
            GetProgressValue(dzenCount, 30),
            onComplete: SetCurrentTopSlider
        );
    }
}
