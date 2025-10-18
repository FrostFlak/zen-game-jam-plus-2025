public class StaticAd : BaseAd {
    public override void Init() {
        base.Init();
        
        AdType = AdType.Static;
        Weight = AdWeight.Weights[AdType].Weight;
    }

    protected override void Update() { }
}
