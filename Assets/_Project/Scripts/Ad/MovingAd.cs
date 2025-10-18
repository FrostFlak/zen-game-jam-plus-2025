using UnityEngine;

public class MovingAd : BaseAd {
    
    private const float Speed = 3f;
    private Vector2 _targetPos;
    
    protected override void OnEnable() {
        base.OnEnable();
        
        AdType = AdType.Moveable;
        Weight = AdWeight.Weights[AdType];
    }
    protected override void Update() {
        transform.position = Vector2.MoveTowards(transform.position, _targetPos, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _targetPos) < 0.15f)
            SetNewTargetPosition();
    }
    
    private void SetNewTargetPosition() {
        Vector2 min = Game.Instance.Camera.ScreenToWorldPoint(new Vector2(50, 50));
        Vector2 max = Game.Instance.Camera.ScreenToWorldPoint(new Vector2(Screen.width - 50, Screen.height - 50));
        _targetPos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }
}
