using DG.Tweening;
using UnityEngine;

public class FallingAd : BaseAd {
    
    private const float BottomOffset = 0.75f;

    public override void Init() {
        base.Init();

        AdType = AdType.FallingAd;
        Weight = AdWeight.Weights[AdType].Weight;
        DzenPrice = 2;

        Fall();
    }

    private void Fall() {
        var bottom = Game.Instance.Camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, transform.position.z - Game.Instance.Camera.transform.position.z)).y + BottomOffset;

        Vector3 targetPos = transform.position;
        targetPos.y = bottom;

        transform.DOMoveY(targetPos.y, .75f).SetEase(Ease.InQuad);
    }

    protected override void Update() { }

    public override void Deinit() {
        base.Deinit();
        transform.DOKill();
    }
}