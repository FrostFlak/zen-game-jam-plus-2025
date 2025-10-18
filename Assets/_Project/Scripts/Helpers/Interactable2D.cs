using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Helpers {
    [RequireComponent(typeof(Collider2D))]
    public class Interactable2D : MonoBehaviour, IPointerDownHandler {
        public event Action OnClick;

        public void OnPointerDown(PointerEventData eventData) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            
            if (hit.collider != null)
                OnClick?.Invoke();
        }
    }
}