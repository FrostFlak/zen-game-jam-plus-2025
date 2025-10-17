using System;
using System.Collections;
using Helpers;
using UnityEngine;

public class Dzen : MonoBehaviour {
    
    [SerializeField] private Interactable _gatherInteraction;

    private Timer _lifeTimer;
    
    private const float InitialLifetime = 5f;
    private const float MinLifetime = 2f;
    
    public event Action<Dzen> OnGather;
    public event Action<Dzen> OnDie;
    
    private void OnEnable() {
        _lifeTimer = new Timer(this);
        _gatherInteraction.OnMouseDownEvent += OnGatherMouseDown;
        
        var lifetime = Mathf.Lerp(InitialLifetime, MinLifetime, Game.Instance.GetDifficulty());
        _lifeTimer.Start(
            lifetime,
            // onProgress: (p) => // Pulse
            onComplete: () => OnDie?.Invoke(this)
        );   
    }

    private void OnDisable() {
        _lifeTimer.Stop();
        _gatherInteraction.OnMouseDownEvent -= OnGatherMouseDown;
    }

    private void OnGatherMouseDown() => OnGather?.Invoke(this);
}
