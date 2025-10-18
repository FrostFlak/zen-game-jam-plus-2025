public class StaticAd : BaseAd {
    protected override void OnEnable() {
        base.OnEnable();
        
        AdType = AdType.Static;
        Weight = AdWeight.Weights[AdType];
    }

    protected override void Update() {
    }
}
