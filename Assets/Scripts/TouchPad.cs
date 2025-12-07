using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public float sensitivity = 0.2f;

    private bool isDragging = false;
    private Vector2 dragDelta;

    public Vector2 GetDelta()
    {
        return dragDelta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        dragDelta = Vector2.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        dragDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        dragDelta = eventData.delta * sensitivity;
    }
}