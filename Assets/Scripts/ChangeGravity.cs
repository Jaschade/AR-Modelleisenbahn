using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravity : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        Physics.gravity = new Vector3(0f, 0f, 0f);
#else
       Physics.gravity = new Vector3(0f, -0f, 0f);
#endif
    }
}
