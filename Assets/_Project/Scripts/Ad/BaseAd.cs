using System;
using System.Globalization;
using DG.Tweening;
using Helpers;
using TMPro;
using UnityEngine;

public abstract class BaseAd : MonoBehaviour {

    [SerializeField] private TMP_Text _lifetimeLabel;
    [SerializeField] private Interactable2D _closeBtn;
    
    private Timer _lifeTimer;
    private Vector3 _initialScale;

    public event Action<BaseAd> OnClose;
    public event Action<BaseAd> OnLifetimeExpired;
    public AdType AdType { get; protected set; } = AdType.None;
    public int Weight { get; protected set; }

    private void Awake() {
        _lifeTimer = new Timer(this);
    }

    private void OnEnable() {
        _closeBtn.OnClick += OnCloseMouseDown;

        _initialScale = transform.localScale;
        transform.DOScale(_initialScale * 1.2f, 0.15f).SetEase(Ease.OutBack);
    }

    protected abstract void Update();

    private void OnDisable() {
        transform.localScale = _initialScale;
        transform.DOKill();
        
        _closeBtn.OnClick -= OnCloseMouseDown;
    }

    public virtual void Init() {
        _lifeTimer.Start(
            Game.Instance.AdsManager.GetLifetime(),
            onProgress: UpdateLifetimeProgress,
            onComplete: OnExpire
        );
    }

    public virtual void Deinit() => _lifeTimer.Stop();
    
    private void UpdateLifetimeProgress(float progress) => _lifetimeLabel.SetText(_lifeTimer.Remaining.ToString("F1", CultureInfo.InvariantCulture));

    protected virtual void OnCloseMouseDown() => OnClose?.Invoke(this);

    protected virtual void OnExpire() => OnLifetimeExpired?.Invoke(this);
}
