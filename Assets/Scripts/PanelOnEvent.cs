using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelOnEvent : MonoBehaviour
     , IPointerClickHandler
     , IPointerEnterHandler
     , IPointerExitHandler
     , IPointerDownHandler
     , IPointerUpHandler
{
    private string eventStr = "";

    public string GetEvent()
    {
        return eventStr;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventStr = "Click";
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        eventStr = "Up";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        eventStr = "Down";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventStr = "Enter";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventStr = "Exit";
    }

}
