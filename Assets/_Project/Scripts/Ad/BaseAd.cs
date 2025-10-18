using System;
using Helpers;
using UnityEngine;

public abstract class BaseAd : MonoBehaviour {

    [SerializeField] private Interactable2D _closeBtn;
    private Timer _lifeTimer;

    public event Action<BaseAd> OnClose;
    public event Action<BaseAd> OnLifetimeExpired;
    public AdType AdType { get; protected set; } = AdType.None;
    public int Weight { get; protected set; }

    private void Awake() {
        _lifeTimer = new Timer(this);
    }

    protected virtual void OnEnable() {
        _closeBtn.OnMouseDownEvent += OnCloseMouseDown;
    }

    protected abstract void Update();

    protected virtual void OnDisable() {
        _lifeTimer.Stop();
        _closeBtn.OnMouseDownEvent -= OnCloseMouseDown;
    }
    
    public void SetLifetime(float time) {
        _lifeTimer.Start(
            time,
            onComplete: OnExpire
        );
    }
    
    protected virtual void OnCloseMouseDown() => OnClose?.Invoke(this);
    
    protected virtual void OnExpire() => OnLifetimeExpired?.Invoke(this);
}
