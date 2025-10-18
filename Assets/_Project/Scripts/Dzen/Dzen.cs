using System;
using Helpers;
using UnityEngine;

public class Dzen : MonoBehaviour {
    
    [SerializeField] private Interactable2D _gatherInteraction;

    private Timer _lifeTimer;
    
    public event Action<Dzen> OnGather;
    public event Action<Dzen> OnDie;

    private void Awake() {
        _lifeTimer = new Timer(this);
    }

    private void OnEnable() {
        _gatherInteraction.OnMouseDownEvent += OnGatherMouseDown;
    }

    private void OnDisable() {
        _lifeTimer.Stop();
        _gatherInteraction.OnMouseDownEvent -= OnGatherMouseDown;
    }

    public void SetLifetime(float time) {
        _lifeTimer.Start(
            time,
            // onProgress: (p) => // Pulse
            onComplete: () => OnDie?.Invoke(this)
        );   
    }
    
    private void OnGatherMouseDown() => OnGather?.Invoke(this);
}
