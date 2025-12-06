using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    public bool pressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }
}