using UnityEngine;

public class CursorFollowerAd : BaseAd
{
    private const float Speed = 5f;
    private Vector2 _targetPos;

    public override void Init()
    {
        base.Init();

        AdType = AdType.CursorFollower;
        Weight = AdWeight.Weights[AdType].Weight;
    }

    protected override void Update() {
        var cam = Game.Instance.Camera;

        Vector3 cursorWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 minWorld = cam.ScreenToWorldPoint(new Vector3(50, 50));
        Vector3 maxWorld = cam.ScreenToWorldPoint(new Vector3(Screen.width - 250, Screen.height - 250));

        _targetPos.x = Mathf.Clamp(cursorWorld.x, minWorld.x, maxWorld.x);
        _targetPos.y = Mathf.Clamp(cursorWorld.y, minWorld.y, maxWorld.y);

        transform.position = Vector2.MoveTowards(transform.position, _targetPos, Speed * Time.deltaTime);
    }
}