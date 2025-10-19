using System;
using System.Numerics;
using DG.Tweening;
using Helpers;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
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
        bool popStarted = false;

        _lifeTimer.Start(
            time,
            onProgress: _ => {
                float remainingPct = _lifeTimer.Remaining / time;
                if (popStarted || !(remainingPct <= 0.25f))
                    return;
                
                popStarted = true;

                transform
                    .DOScale(Vector3.one * 1.25f, 0.075f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad);
            },
            onComplete: () => {
                transform
                    .DOScale(Vector3.zero, 1f)
                    .OnComplete(() => OnExpire?.Invoke(this))
                    .SetEase(Ease.InOutBack);
            }
        );   
    }

    
    private void OnGatherMouseDown() {
        Game.Instance.AudioManager.PlayDzenCollctSFX();
        
        Vector2 screenPos = new Vector2(Screen.width / 2f, Screen.height / 1.15f);
        Vector3 worldPos = Game.Instance.Camera.ScreenToWorldPoint(screenPos);
        
        DOTween.Sequence()
            .Join(transform.DOMove(worldPos, 0.25f))
            .Join(transform.DOScale(Vector3.zero, 0.35f))
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnGather?.Invoke(this));
    }
}
