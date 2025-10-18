using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Helpers {
    [RequireComponent(typeof(Toggle))]
    public class Switch : MonoBehaviour {

        #region SerializedFields
        [Header("Components")]
        [SerializeField] private Toggle _toggle;
        [Header("Transforms")]
        [SerializeField] private Transform _onTransform;
        [SerializeField] private Transform _offTransform;
        [Header("Images")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _handleImage;
        [Header("Properties")]
        [SerializeField] private Color _backgroundOnColor;
        [SerializeField] private Color _backgroundOffColor;
        
        private TweenerCore<float, float, FloatOptions> _currentTween;
            
        #endregion

        #region PrivateFields
        private const float LerpDuration = .15f;
        #endregion

        #region MonoBehaviour
        private void OnEnable() {
            AddListener(isOn => SetUI(isOn));

            SetUI(_toggle.isOn, false);
        }

        private void OnDisable() => RemoveAllListeners();
        #endregion

        #region Listeners
        public void AddListener(UnityAction<bool> action) => _toggle.onValueChanged.AddListener(action);

        public void RemoveAllListeners() => _toggle.onValueChanged.RemoveAllListeners();
        #endregion

        #region StateHandling
        public void SetActive(bool isOn) => _toggle.isOn = isOn;
        #endregion

        #region UI
        private void SetUI(bool active, bool lerp = true) {
            // Kill any existing tween to prevent overlap
            _currentTween?.Kill();

            // Disable toggle interaction while animating
            _toggle.interactable = false;

            // Start the tween
            _currentTween = DOTween.To(
                    () => 0f,
                    (progress) =>
                    {
                        _handleImage.transform.localPosition = active
                            ? Vector2.Lerp(_offTransform.localPosition, _onTransform.localPosition, progress)
                            : Vector2.Lerp(_onTransform.localPosition, _offTransform.localPosition, progress);
                    },
                    1f,
                    lerp ? LerpDuration : 0f
            )
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _backgroundImage.color = active ? _backgroundOnColor : _backgroundOffColor;
                    _toggle.interactable = true;
                })
                .Play();
        }
        #endregion
    }
}