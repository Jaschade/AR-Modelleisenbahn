using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GraphicRaycast : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    string s_panelHits = "";
    public bool b_hitDrawPanel = false;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
#if UNITY_EDITOR
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            s_panelHits = s_panelHits + result.gameObject.name;           
        }

        if (s_panelHits.Contains("DrawPanel"))
        {
            b_hitDrawPanel = true;
        }
        else
        {
            b_hitDrawPanel = false;
        }

        s_panelHits = "";
#else
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.GetTouch(0).position;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            s_panelHits = s_panelHits + result.gameObject.name;
        }

        if (s_panelHits.Contains("DrawPanel"))
        {
            b_hitDrawPanel = true;
        }
        else
        {
            b_hitDrawPanel = false;
        }

        s_panelHits = "";
#endif
    }
}
