using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TouchController : MonoBehaviour {

    public GameObject linePrefab;
    public GameObject trackPrefab;

    public Button btnCheck;
    public Sprite checkBlack;
    public Sprite checkGreen;
    public GameObject canvas;

    Line activeLine;
    Track track;

    bool b_insidePanel = false;
    bool b_CheckedOnce = false;

    // Use this for initialization
    void Start ()
    {
        GameObject lineGO = Instantiate(linePrefab);
        activeLine = lineGO.GetComponent<Line>();
        activeLine.gameObject.SetActive(false);

        GameObject trackGO = Instantiate(trackPrefab);
        track = trackGO.GetComponent<Track>();        
    }

    // Update is called once per frame
    void Update()
    {        
        #region Standalone Inputs

        if (canvas.GetComponent<GraphicRaycast>().b_hitDrawPanel == true)
        {
            b_insidePanel = true;
        }
        else
            b_insidePanel = false;


        if (activeLine != null)
        { 
            if (Input.GetMouseButton(0) && b_insidePanel == true)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                activeLine.gameObject.SetActive(true);
                activeLine.UpdateLine(mousePos);
            }           
        }

        #endregion

        #region Mobile Inputs

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            activeLine.gameObject.SetActive(true);
            activeLine.UpdateLine(mousePos);
        }

        #endregion
    }

    // submit line/track
    public void BtnCheckLine()
    {
        if (b_CheckedOnce == false )
        {
            if (activeLine.GetComponent<Line>().lineRenderer.positionCount > 2)
            {
                track.Setup(activeLine.GetLine());
                btnCheck.image.sprite = checkGreen;
                ResetLine();

                b_CheckedOnce = true;
            }
        }
        else
        {
#if UNITY_EDITOR
            SceneManager.LoadScene("Spielwiese");
#else
            SceneManager.LoadScene("ToytrainAR");
#endif
        }
    }

    // reset line to draw a new one
    public void BtnClearLine()
    {
        if (b_CheckedOnce)
        {
            track.Reset();
            btnCheck.image.sprite = checkBlack;

            b_CheckedOnce = false;            
        }

        ResetLine();
    }

    private void ResetLine()
    {
        Destroy(activeLine.gameObject);

        // instantiate new line
        GameObject lineGO = Instantiate(linePrefab);
        activeLine = lineGO.GetComponent<Line>();
        activeLine.gameObject.SetActive(false);
    }
}
