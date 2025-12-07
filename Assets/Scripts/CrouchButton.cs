using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrouchButton : MonoBehaviour
{
    public bool toggled;

    public void OnPointerDown(PointerEventData eventData)
    {
        toggled = !toggled;
    }
}