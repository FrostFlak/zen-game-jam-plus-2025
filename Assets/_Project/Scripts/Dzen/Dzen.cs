using System;
using DG.Tweening;
using Helpers;
using UnityEngine;

public class Dzen : MonoBehaviour {
    
    [SerializeField] private Interactable2D _gatherInteraction;

    private Vector3 _initialSize;
    private Timer _lifeTimer;
    
    public event Action<Dzen> OnGather;
    public event Action<Dzen> OnExpire;

    private void Awake() {
        _lifeTimer = new Timer(this);
        _initialSize = transform.localScale;
    }

    private void OnEnable() {
        _gatherInteraction.OnClick += OnGatherMouseDown;
    }

    private void OnDisable() {
        transform.DOKill();
        transform.localScale = _initialSize;
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
        transform
            .DOMove(worldPos, .25f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnGather?.Invoke(this));
    }
}
