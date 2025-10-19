using UnityEngine;

public class MovingAd : BaseAd {
    
    private const float Speed = 3f;
    private Vector2 _targetPos;
    
    public override void Init() {
        base.Init();
        
        AdType = AdType.RandomMove;
        Weight = AdWeight.Weights[AdType].Weight;
        DzenPrice = 1;
    }

    protected override void Update() {
        transform.position = Vector2.MoveTowards(transform.position, _targetPos, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _targetPos) < 0.15f)
            SetNewTargetPosition();
    }
    
    private void SetNewTargetPosition() {
        var cam = Game.Instance.Camera;
        Vector2 min = cam.ScreenToWorldPoint(new Vector2(50, 50));
        Vector2 max = cam.ScreenToWorldPoint(new Vector2(Screen.width - 300, Screen.height - 300));

        _targetPos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }
}
