using System;
using UnityEngine;

namespace Helpers {
    [RequireComponent(typeof(Collider2D))]
    public class Interactable2D : MonoBehaviour {

        public event Action OnMouseDownEvent;
        public event Action OnMouseUpEvent;
        public event Action OnMouseEnterEvent;
        public event Action OnMouseExitEvent;
        public event Action OnMouseOverEvent;

        private void OnMouseDown() => OnMouseDownEvent?.Invoke();
        private void OnMouseUp() => OnMouseUpEvent?.Invoke();
        private void OnMouseEnter() => OnMouseEnterEvent?.Invoke();
        private void OnMouseExit() => OnMouseExitEvent?.Invoke();
        private void OnMouseOver() => OnMouseOverEvent?.Invoke();
    }
}