using UnityEngine;

public class Train : MonoBehaviour {

    //float rotationSpeed = 1f;
    public int currentDirection = 0;
    public int newDirection = 0;

    Vector3 moveDirection;
    float speed = 1f;

    bool b_rotate = false;

    public int signX = 0;
    public int signY = 0;

    // rotIdentity = initial rotation of the rail prefabs
    // rotMinimum = rotation of 45 degrees to add to the initial rotation depending on the direction (0-7) of a rail
    private Vector3 rotIdentity = new Vector3(0f, 0f, 0f);
    private Vector3 rotMinimum = new Vector3(0f, 45f, 0f);

    public GameObject m_Smoke;


    public void Setup(int direction)
    {
        currentDirection = direction;
        newDirection = direction;

        CheckDirection();        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name != "Cube")
        {
            //Debug.Log("Es kollidiert Enter");
            newDirection = col.gameObject.GetComponent<Rail>().direction;
        }
    }

    public void Drive(float slider)
    {
        CheckDirection();
        CheckRotation();

        Vector3 moveDirection = -transform.right;

        transform.position = transform.position + (moveDirection * speed * slider * Time.deltaTime);

        Rotate();

        if (slider > 0)
        {
            m_Smoke.SetActive(true);
        }
        else
        {
            m_Smoke.SetActive(false);
        }
    }

    public void Rotate()
    {
        if (b_rotate == true)
        {
            transform.Rotate(rotIdentity + rotMinimum * (newDirection - currentDirection));
            currentDirection = newDirection;
        }
    }

    private void CheckDirection()
    {
        switch (newDirection)
        {
            case (0):
                signX = 0;
                signY = 1;
                break;
            case (1):
                signX = 1;
                signY = 1;
                break;
            case (2):
                signX = 1;
                signY = 0;
                break;
            case (3):
                signX = 1;
                signY = -1;
                break;
            case (4):
                signX = 0;
                signY = -1;
                break;
            case (5):
                signX = -1;
                signY = -1;
                break;
            case (6):
                signX = -1;
                signY = 0;
                break;
            case (7):
                signX = -1;
                signY = 1;
                break;
        }
    }

    public void CheckRotation()
    {
        if (newDirection != currentDirection)
        {
            b_rotate = true;
        }
        else
        {
            b_rotate = false;
        }
    }
}
