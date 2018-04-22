using UnityEngine;
using UnityEngine.UI;

public class SpielwieseController : MonoBehaviour
{
    public GameObject trainPrefab;
    Train train;

    GameObject trainGO;
    GameObject track;

    public Button btnSpawnTrain;
    public Slider slider;

    bool b_ObjectInstantiated = false;
    bool b_ClickedOnce = false;

    /// <summary>
    /// The Unity Start() method.
    /// </summary>
    private void Start()
    {
        track = GameObject.Find("Track(Clone)");

        if (track != null)
        {
            track.SetActive(false);
        }

        btnSpawnTrain.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        if (Input.GetMouseButton(0) && b_ObjectInstantiated == false)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            track.SetActive(true);
            track.transform.position = mousePos;
            track.transform.position += new Vector3(0f, 0f, 10f);

            b_ObjectInstantiated = true;
            btnSpawnTrain.gameObject.SetActive(true);
        }

        if (slider.IsActive() == true)
        {
            train.Drive(slider.value);
        }
    }
    
    // spawn train
    public void BtnSpawnTrain()
    {
        if (!b_ClickedOnce)
        {
            trainGO = Instantiate(trainPrefab, track.transform.GetChild(0).position + new Vector3(0f, 0f, -0.05f), track.transform.GetChild(0).rotation);
            trainGO.transform.Rotate(0f, 180f, 0f);
            train = trainGO.GetComponent<Train>();
            train.Setup(track.transform.GetChild(0).GetComponent<Rail>().direction);

            slider.gameObject.SetActive(true);
            b_ClickedOnce = true;
        }
        else
        {
            Destroy(trainGO);

            trainGO = Instantiate(trainPrefab, track.transform.GetChild(0).position + new Vector3(0f, 0f, -0.05f), track.transform.GetChild(0).rotation);
            trainGO.transform.Rotate(0f, 180f, 0f);
            train = trainGO.GetComponent<Train>();
            train.Setup(track.transform.GetChild(0).GetComponent<Rail>().direction);
        }

    }
}