using System;
using System.Numerics;
using DG.Tweening;
using Helpers;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Dzen : MonoBehaviour {
    
    [SerializeField] private Interactable2D _gatherInteraction;

    private Vector3 _initialScale;
    private Timer _lifeTimer;
    
    public event Action<Dzen> OnGather;
    public event Action<Dzen> OnExpire;

    private void Awake() {
        _lifeTimer = new Timer(this);
        _initialScale = transform.localScale;
    }

    private void OnEnable() {
        _gatherInteraction.OnClick += OnGatherMouseDown;
        transform.DOScale(_initialScale * 1.2f, 0.15f).SetEase(Ease.OutBack);
    }

    private void OnDisable() {
        transform.DOKill();
        transform.localScale = _initialScale;
        _lifeTimer.Stop();
        
        _gatherInteraction.OnClick -= OnGatherMouseDown;
    }

    public void SetLifetime(float time) {
        _lifeTimer.Start(
            time,
            // onProgress: (p) => // Pulse
            onComplete: () => {
                transform
                    .DOScale(Vector3.zero, 1f)
                    .OnComplete(() => OnExpire?.Invoke(this))
                    .SetEase(Ease.InOutBack);
            });   
    }
    
    private void OnGatherMouseDown() {
        Vector3 screenPos = new Vector3(Screen.width / 2f, Screen.height, 0);
        Vector3 worldPos = Game.Instance.Camera.ScreenToWorldPoint(screenPos);
        
        DOTween.Sequence()
            .Join(transform.DOMove(worldPos, 0.25f))
            .Join(transform.DOScale(Vector3.zero, 0.35f))
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnGather?.Invoke(this));
    }
}
