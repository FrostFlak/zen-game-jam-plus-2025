using DG.Tweening;
using Helpers;
using UnityEngine;

public class RotationAd : BaseAd {

    private Timer _timer;
    
    public override void Init() {
        base.Init();

        _timer = new Timer(this, true);
        
        AdType = AdType.RotationAd;
        Weight = AdWeight.Weights[AdType].Weight;
        DzenPrice = 2;

        _timer.Start(1f, onStart: PlayRotation);
    }

    private void PlayRotation() => transform.DORotate(new Vector3(0, 0, 360f), .45f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);

    protected override void Update() { }

    public override void Deinit() {
        base.Deinit();
        
        _timer?.Stop();
        transform.DOKill();
    }
}
