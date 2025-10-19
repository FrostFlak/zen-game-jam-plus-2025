using UnityEngine;
using UnityEngine.UI;

public class FaceChangerUI : MonoBehaviour {
    
    [Header("UI")]
    [SerializeField] private Image _faceImage;
    [field: SerializeField] public Transform RealFaceTransform { get; private set; }
    [Header("Faces")]
    [SerializeField] private Sprite _calmFace;
    [SerializeField] private Sprite _calmFaceOpen;
    [SerializeField] private Sprite _annoyed;
    [SerializeField] private Sprite _angryFace;
    [SerializeField] private Sprite _superAngry;

    public void Init() {
        Game.Instance.AdsManager.ActiveAds.OnUpdate += OnAdsChange;
    }

    private void OnDisable() {
        Game.Instance.AdsManager.ActiveAds.OnUpdate -= OnAdsChange;
    }

    private void UpdateFace() {
        float currentDzen = Game.Instance.DzenManager.DzenAmount.Value();
        int adsCount = Game.Instance.AdsManager.ActiveAds.Value();

        float stress = adsCount * 10 + (100 - currentDzen) * 0.65f;

        _faceImage.sprite = stress switch {
            >= 90 => _superAngry,
            >= 70 => _angryFace,
            >= 50 => _annoyed,
            >= 30 => _calmFaceOpen,
            _ => _calmFace
        };

    }
    
    private void OnAdsChange(int arg1, int ads) => UpdateFace();
}