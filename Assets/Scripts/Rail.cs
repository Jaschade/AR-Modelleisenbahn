using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour {

    public int direction;       // Up=0, UpRight=1, ... , UpLeft=7
    public string prefab;       // railStraight, railLeft, railRight
    public Vector2 endpoint;
}
