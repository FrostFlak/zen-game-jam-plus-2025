public class SplitAd : BaseAd {
    
    private const int SplitParts = 2;
    
    public override void Init() {
        base.Init();
        
        AdType = AdType.Split;
        Weight = AdWeight.Weights[AdType].Weight;
    }
    
    protected override void Update() { }
    

    protected override void OnExpire() {
        base.OnExpire();
        
        for (int i = 0; i < SplitParts; i++)
            Game.Instance.AdsManager.Spawn(AdType.Static);
    }
}
