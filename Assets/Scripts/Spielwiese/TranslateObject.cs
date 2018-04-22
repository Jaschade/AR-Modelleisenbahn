using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
               

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the screenDelta value based on these fingers
            var screenDelta = mousePos - transform.position;

            // Perform the translation
            Translate(screenDelta);
        }
    }

    void Translate(Vector3 screenDelta)
    {       
        // Screen position of the transform
        var screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        // Add the deltaPosition
        screenPosition += (Vector3)screenDelta;

        // Convert back to world space
        transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
        //transform.Translate(screenPosition - screenDelta);
    }
}
