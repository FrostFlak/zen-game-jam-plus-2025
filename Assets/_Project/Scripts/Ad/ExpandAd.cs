using DG.Tweening;
using UnityEngine;

public class ExpandAd : BaseAd {
    
    private Vector3 _initialScale;
    
    public override void Init() {
        base.Init();
        
        AdType = AdType.ExpandOverTime;
        Weight = AdWeight.Weights[AdType].Weight;

        _initialScale = transform.localScale; 
        transform
            .DOScale(new Vector3(1.5f, 1.5f, 1.5f), Game.Instance.AdsManager.GetLifetime())
            .SetEase(Ease.OutBack);
    }

    protected override void Update() { }

    public override void Deinit() {
        base.Deinit();

        transform.DOKill();
        transform.localScale = _initialScale;
    }
}
