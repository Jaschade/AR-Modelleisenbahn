using System.Collections.Generic;
using UnityEngine;
using System;

public class Track : MonoBehaviour {

    // Prefabs for the specific rails
    public GameObject railStraightPrefab;
    public GameObject railLeftPrefab;
    public GameObject railRightPrefab;
    GameObject railGO; // GameObject to instantiate single rail

    float meshSize = 0.10000008f; //railGO.GetComponent<MeshRenderer>().bounds.size.x
    float curveOffsetPrecision = 0.022f;

    // rotIdentity = initial rotation of the rail prefabs
    // rotMinimum = rotation of 45 degrees to add to the initial rotation depending on the direction (0-7) of a rail
    private Vector3 rotIdentity = new Vector3(0f, 0f, 0f);
    private Vector3 rotMinimum = new Vector3(0f, 45f, 0f);

    // When is a curve a curve?
    float trackPrecision = 0.05f;

    // Store information like xPos, yPos and direction for each point to then determine the endpoint and railPrefab of a point.
    // Then set the information of each rail.
    struct PointProps
    {
        public int direction; // Up=0, UpRight=1, ... , UpLeft=7, Endpoint=8
        public string prefab;
        public Vector2 endPoint;

        public PointProps(int a, string b, Vector2 c)
        {
            direction = a;
            prefab = b;
            endPoint = c;
        }
    }
    PointProps[] rails;
    float startingPointX = 0;
    float startingPointY = 0;
    string railPrefab = "";

    // Stay alive in other scenes
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Setup the track
    public void Setup(List<Vector2> line)
    {
        // check the passed line
        if (line == null)
        {
            return;
        }
        else
        {
            Init(line);
            Build();
            CompleteTrack();
        }
    }

    // Set the directions of the rails
    private void Init(List<Vector2> line)
    {
        // Set the length equal to rails in line
        rails = new PointProps[line.Count];

        // Set the starting point values
        startingPointX = line[0].x;
        startingPointY = line[0].y;

        // check the approximate direction of a point with regard to the next point
        for (int j = 0; j < rails.Length; j++)
        {
            // last point? set direction according to the starting point
            if (j == rails.Length-1)
            {
                rails[j].direction = rails[0].direction; // (TODO)
                return;
            }
            // if the x value doesn't really change and the y value of the next point is bigger than the y value of the current point,
            // then it's going upwards
            if (line[j].x - line[j+1].x > -trackPrecision && line[j].x - line[j+1].x < trackPrecision &&
                line[j].y - line[j+1].y < 0)
            {
                rails[j].direction = 0;
            }
            // if the x and y values of the next point are bigger than the x and y values of the current point,
            // then it's going up and right
            if (line[j].x - line[j+1].x < -trackPrecision && line[j].y - line[j+1].y < -trackPrecision)
            {
                rails[j].direction = 1;
            }
            // if the y value doesn't really change and the x value of the next point is bigger than the x value of the current point,
            // then it's going right
            if (line[j].x - line[j+1].x < 0 && line[j].y - line[j+1].y > -trackPrecision
                && line[j].y - line[j+1].y < trackPrecision)
            {
                rails[j].direction = 2;
            }
            // if the x value of the next point is bigger than the x value of the current point and the y value of the next point is smaller than the y value of the current point,
            // then it's going down and right
            if (line[j].x - line[j+1].x < 0 && line[j].y - line[j+1].y > trackPrecision)
            {
                rails[j].direction = 3;
            }
            // if the x value doesn't really change and the y value of the next point is smaller than the y value of the current point,
            // then it's going downwards
            if (line[j].x - line[j+1].x > -trackPrecision && line[j].x - line[j+1].x < trackPrecision &&
                line[j].y - line[j+1].y > 0)
            {
                rails[j].direction = 4;
            }
            // if the x and y values of the next point are smaller than the x and y values of the current point,
            // then it's going down and left
            if (line[j].x - line[j+1].x > trackPrecision && line[j].y - line[j+1].y > trackPrecision)
            {
                rails[j].direction = 5;
            }
            // if the y value doesn't really change and the x value of the next point is smaller than the x value of the current point,
            // then it's going left
            if (line[j].x - line[j+1].x > 0 && line[j].y - line[j+1].y > -trackPrecision
                    && line[j].y - line[j+1].y < trackPrecision)
            {
                rails[j].direction = 6;
            }
            // if the x value of the next point is smaller than the x value of the current point and the y value of the next point is bigger than the y value of the current point,
            // then it's going up and left
            if (line[j].x - line[j+1].x > trackPrecision && line[j].y - line[j+1].y < -trackPrecision)
            {
                rails[j].direction = 7;
            }
        }
    }

    // Get the drawn line as railway track
    public void Build()
    {
        // Initialize the next direction of the rails
        for (int j = 0; j < rails.Length; j++)
        {
            if (j == 0)
            {
                SetFirstRail(j, rails[j].direction);
            }
            else
            {
                CheckNextRail(j, rails[j].direction, rails[j - 1].direction);
            }
        }
    }

    // Set the first rail ot the track
    public void SetFirstRail(int index, int direction)
    {
        // First rail is always straight
        railGO = Instantiate(railStraightPrefab);
        railPrefab = "railStraight";

        // Variables to determine the endpoint of the current prefab depending on the direction 
        // signX and signY: (direction 2 / right -> +x, 0y), (3 / downright -> +x, -y), (5 / downleft -> -x, -y).
        int signX = 0;
        int signY = 0;
        float initialPrefabOffset = 0f;
        float prefabOffset = 0f;


        // Determine the initial offset of a prefab that is instantiated in it's center depending on the direction.
        if (direction % 2 == 0)
            initialPrefabOffset = railGO.GetComponent<MeshRenderer>().bounds.size.x;
        else
            initialPrefabOffset = railGO.GetComponent<MeshRenderer>().bounds.size.x * 2 / 3;

        // Determines if prefabOffset has to be added, done nothing or subtracted to the rail prefab in x coordinate or y coordinate depending on the direction.
        // PrefabOffset determines the amount that has to be added, if so.
        switch (direction)
        {
            case (0):
                signX = 0;
                signY = 1;
                prefabOffset = meshSize / 2;
                break;
            case (1):
                signX = 1;
                signY = 1;
                prefabOffset = meshSize / 3;
                break;
            case (2):
                signX = 1;
                signY = 0;
                prefabOffset = meshSize / 2;
                break;
            case (3):
                signX = 1;
                signY = -1;
                prefabOffset = meshSize / 3;
                break;
            case (4):
                signX = 0;
                signY = -1;
                prefabOffset = meshSize / 2;
                break;
            case (5):
                signX = -1;
                signY = -1;
                prefabOffset = meshSize / 3;
                break;
            case (6):
                signX = -1;
                signY = 0;
                prefabOffset = meshSize / 2;
                break;
            case (7):
                signX = -1;
                signY = 1;
                prefabOffset = meshSize / 3;
                break;
        }

        // Set the position and rotation of the rail and add it to the game object for the complete track.
        railGO.transform.position = new Vector2(startingPointX + (float)signX * prefabOffset,
                                                startingPointY + (float)signY * prefabOffset);
        railGO.transform.Rotate(rotIdentity + rotMinimum * direction);
        railGO.transform.parent = GetComponentInParent<Track>().transform;

        // Set the end position of the prefab of the current rail,
        // that is also the starting position of the next rail.
        rails[index].endPoint = new Vector2(startingPointX + signX * prefabOffset + signX * initialPrefabOffset,
                                             startingPointY + signY * prefabOffset + signY * initialPrefabOffset);

        // Set the info of the instantiated rail
        SetRailInfo(rails[index].direction, railPrefab, rails[index].endPoint);
    }

    // Check the next rail
    public void CheckNextRail(int index, int directionCurrentIndex, int directionPreviousIndex)
    {
        int degree = 0;

        // Check the difference in degree of the current direction and the previous direction to determine the next rail
        //
        // Check exception clockwise
        if (directionCurrentIndex == 0 && directionPreviousIndex == 7)
            degree = 45;
        // Check exception clockwise
        else if (directionCurrentIndex == 0 && directionPreviousIndex == 6)
            degree = 90;
        // Check exception clockwise
        else if (directionCurrentIndex == 0 && directionPreviousIndex == 5)
            degree = 135;
        // Check exception counter clockwise
        else if (directionCurrentIndex == 7 && directionPreviousIndex == 0)
            degree = -45;
        // Check exception counter clockwise
        else if (directionCurrentIndex == 6 && directionPreviousIndex == 0)
            degree = -90;
        // Check exception counter clockwise
        else if (directionCurrentIndex == 5 && directionPreviousIndex == 0)
            degree = -135;

        // Everything else
        else
            degree = directionCurrentIndex * 45 - directionPreviousIndex * 45;

        //
        if (degree > 45 || degree < -45)
        {           

            if (degree > 0)
            {
                directionCurrentIndex = directionPreviousIndex + 1;
                AddItemToArray(index, directionCurrentIndex);

                SetNextRail(index, directionCurrentIndex, degree);
            }
            else
            {
                directionCurrentIndex = directionPreviousIndex - 1;
                AddItemToArray(index, directionCurrentIndex);

                SetNextRail(index, directionCurrentIndex, degree);
            }
        }
        else
            SetNextRail(index, directionCurrentIndex, degree);
    }

    // Set the next rail of the track
    public void SetNextRail(int index, int direction, int degree)
    {

        // Determine the next rail
        if (degree < 0)
        {
            railGO = Instantiate(railLeftPrefab);
            railPrefab = "railLeft";
        }
        else if (degree == 0)
        {
            railGO = Instantiate(railStraightPrefab);
            railPrefab = "railStraight";
        }
        else if (degree > 0)
        {
            railGO = Instantiate(railRightPrefab);
            railPrefab = "railRight";
        }

        // Variables to determine the endpoint of the current prefab depending on the direction 
        // signX and signY: (direction 2 / right -> +x, 0y), (3 / downright -> +x, -y), (5 / downleft -> -x, -y).
        int signX = 0;
        int signY = 0;
        float prefabOffset = 0f;
        // following variables needed, because the prefab of a curve needs further adjustments/do not simply harmonize with straight rails
        int signCurveX = 0;
        int signCurveY = 0;
        float curveOffset = 0f;

        // Determines if prefabOffset has to be added, done nothing or subtracted to the rail prefab in x coordinate or y coordinate
        // depending on the direction and the previous rail.
        // PrefabOffset determines the amount that has to be added, if so.
        switch (direction)
        {
            case (0):
                signX = 0;
                signY = 1;
                if (direction == rails[index - 1].direction)
                    prefabOffset = meshSize;
                else
                {
                    prefabOffset = railGO.GetComponent<MeshRenderer>().bounds.size.x * 5 / 6;
                }
                signCurveX = 1;
                signCurveY = 0;
                if (rails[index - 1].direction == 7)
                {
                    curveOffset = curveOffsetPrecision;
                }
                if (rails[index - 1].direction > 0 && rails[index - 1].direction < 7)
                {
                    curveOffset = -curveOffsetPrecision;
                }
                break;
            case (1):
                signX = 1;
                signY = 1;
                prefabOffset = 0.07000004f;
                if (direction < rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = -1;
                    curveOffset = -curveOffsetPrecision;
                }
                if (direction > rails[index - 1].direction)
                {
                    signCurveX = 1;
                    signCurveY = 0;
                    curveOffset = curveOffsetPrecision;
                }
                break;
            case (2):
                signX = 1;
                signY = 0;
                if (direction == rails[index - 1].direction)
                    prefabOffset = meshSize;
                else
                {
                    prefabOffset = railGO.GetComponent<MeshRenderer>().bounds.size.x * 5 / 6;
                }
                if (direction < rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = -1;
                    curveOffset = -curveOffsetPrecision;
                }
                if (direction > rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = -1;
                    curveOffset = curveOffsetPrecision;
                }
                break;
            case (3):
                signX = 1;
                signY = -1;
                prefabOffset = 0.07000004f;
                if (direction < rails[index - 1].direction)
                {
                    signCurveX = -1;
                    signCurveY = 0;
                    curveOffset = -curveOffsetPrecision;
                }
                if (direction > rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = -1;
                    curveOffset = curveOffsetPrecision;
                }
                break;
            case (4):
                signX = 0;
                signY = -1;
                if (direction == rails[index - 1].direction)
                    prefabOffset = meshSize;
                else
                {
                    prefabOffset = railGO.GetComponent<MeshRenderer>().bounds.size.x * 5 / 6;
                }
                if (direction < rails[index - 1].direction)
                {
                    signCurveX = -1;
                    signCurveY = 0;
                    curveOffset = -curveOffsetPrecision;
                }
                if (direction > rails[index - 1].direction)
                {
                    signCurveX = -1;
                    signCurveY = 0;
                    curveOffset = curveOffsetPrecision;
                }
                break;
            case (5):
                signX = -1;
                signY = -1;
                prefabOffset = 0.07000004f;
                if (direction < rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = 1;
                    curveOffset = -curveOffsetPrecision;
                }
                if (direction > rails[index - 1].direction)
                {
                    signCurveX = -1;
                    signCurveY = 0;
                    curveOffset = curveOffsetPrecision;
                }
                break;
            case (6):
                signX = -1;
                signY = 0;
                if (direction == rails[index - 1].direction)
                    prefabOffset = meshSize;
                else
                {
                    prefabOffset = railGO.GetComponent<MeshRenderer>().bounds.size.x * 5 / 6;
                }
                if (direction < rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = 1;
                    curveOffset = -curveOffsetPrecision;
                }
                if (direction > rails[index - 1].direction)
                {
                    signCurveX = 0;
                    signCurveY = 1;
                    curveOffset = curveOffsetPrecision;
                }
                break;
            case (7):
                signX = -1;
                signY = 1;
                prefabOffset = 0.07000004f;
                if (rails[index - 1].direction == 0)
                {
                    signCurveX = 1;
                    signCurveY = 0;
                    curveOffset = -curveOffsetPrecision;
                }
                if (rails[index - 1].direction > 0 && rails[index - 1].direction < 7)
                {
                    signCurveX = 0;
                    signCurveY = 1;
                    curveOffset = curveOffsetPrecision;
                }
                break;
        }

        // Set the position and rotation of the rail and add it to the game object for the complete track.
        railGO.transform.position = rails[index - 1].endPoint + new Vector2(signCurveX * curveOffset, signCurveY * curveOffset);
        railGO.transform.Rotate(rotIdentity + rotMinimum * rails[index - 1].direction);
        railGO.transform.parent = GetComponentInParent<Track>().transform;               

        // Set the end position of the prefab of the current rail,
        // that is also the starting position of the next rail.
        rails[index].endPoint = new Vector2(rails[index - 1].endPoint.x + signX * prefabOffset,
                                             rails[index - 1].endPoint.y + signY * prefabOffset);

        // Set the info of the instantiated rail
        SetRailInfo(rails[index].direction, railPrefab, rails[index].endPoint);
    }

    // Optimize the current track, it has to be continuous
    // (the last rail needs to connect to the first rail)
    private void CompleteTrack()
    {
        // (TODO)
        // Wenn sich Gleise am Ende überlappen, dann welche rausnehmen und die Richtung und Verbindung überprüfen.
        // Wenn es am Ende Lücken gibt, dann diese auffüllen und ggf. direkt das letzte Gleis ersetzen um besser auffüllen zu können.

    }

    // Set info of a rail
    private void SetRailInfo(int direction, string prefab, Vector2 endpoint)
    {
        railGO.GetComponent<Rail>().direction = direction;
        railGO.GetComponent<Rail>().prefab = prefab;
        railGO.GetComponent<Rail>().endpoint = endpoint;
    }

    // Add item to array if an extra curve has to be added
    public void AddItemToArray(int index, int direction)
    {
        PointProps[] tempArray = new PointProps[rails.Length];
        rails.CopyTo(tempArray, 0);

        Array.Clear(rails, 0, rails.Length);
        Array.Resize(ref rails, rails.Length + 1);

        for (int i = 0; i < rails.Length; i++)
        {
            if (i < index)
            {
                var v = rails[i];
                
                v.direction = tempArray[i].direction;
                v.endPoint = tempArray[i].endPoint;
                v.prefab = tempArray[i].prefab;

                rails[i] = v;
            }
            if (i == index)
            {
                var v = rails[i];

                v.direction = direction;
                v.endPoint = new Vector2(0, 0);
                v.prefab = "";

                rails[i] = v;
            }
            if (i > index)
            {
                var v = rails[i];
                
                v.direction = tempArray[i-1].direction;
                v.endPoint = tempArray[i-1].endPoint;
                v.prefab = tempArray[i-1].prefab;

                rails[i] = v;
            }
        }
    }

    public void Reset()
    {
        int numChilds = transform.childCount;

        for (int i = 0; i < numChilds; i++)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
