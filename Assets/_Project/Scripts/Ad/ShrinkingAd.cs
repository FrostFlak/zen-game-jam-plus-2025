using DG.Tweening;
using UnityEngine;

public class ShrinkingAd : BaseAd {

    private Vector3 _initialSize;
    
    public override void Init() {
        base.Init();
        
        AdType = AdType.ShrinkOverTime;
        Weight = AdWeight.Weights[AdType].Weight;
        DzenPrice = 2;

        _initialSize = transform.localScale; 
        transform
            .DOScale(new Vector3(0.75f, 0.75f, 0.75f), Game.Instance.AdsManager.GetLifetime() * 1.35f)
            .SetEase(Ease.OutBack);
    }

    protected override void Update() { }

    public override void Deinit() {
        base.Deinit();

        transform.DOKill();
        transform.localScale = _initialSize;
    }
}
