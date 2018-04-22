using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool b_Pressed = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        b_Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        b_Pressed = false;
    }
}
