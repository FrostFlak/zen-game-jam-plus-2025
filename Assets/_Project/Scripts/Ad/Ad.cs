using System;
using UnityEngine;

public class Ad : MonoBehaviour {

    [SerializeField] private SpriteRenderer _adSprite;
    [SerializeField] private SpriteRenderer _closeBtnSprite;
    [SerializeField] private Interactable _closeBtn;

    public event Action<Ad> OnClose;

    private void OnEnable() {
        _closeBtn.OnMouseDownEvent += OnCloseMouseDown;
    }

    private void OnDisable() {
        _closeBtn.OnMouseDownEvent -= OnCloseMouseDown;
    }

    private void OnCloseMouseDown() => OnClose?.Invoke(this);
}
