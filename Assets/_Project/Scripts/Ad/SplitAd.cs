public class SplitAd : BaseAd {
    
    private const int SplitParts = 3;
    
    protected override void OnEnable() {
        base.OnEnable();
        
        AdType = AdType.Split;
        Weight = AdWeight.Weights[AdType];
    }
    
    protected override void Update() {
        
    }

    protected override void OnExpire() {
        base.OnExpire();
        
        for (int i = 0; i < SplitParts; i++)
            Game.Instance.AdsManager.Spawn(AdType.Static);
    }

    protected override void OnCloseMouseDown() {
        base.OnCloseMouseDown();

        for (int i = 0; i < SplitParts; i++)
            Game.Instance.AdsManager.Spawn(AdType.Static);
    }
}
