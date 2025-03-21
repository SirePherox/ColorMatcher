using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;

public class ColourWheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject colorSegmentPrefab;
    [SerializeField] private Transform colorSegmentsParent;

    [Header("Variables")]
    [SerializeField] private int colorSegmentCount = 3; //number of color segments on wheel
    [SerializeField] private float colourWheelSize = 1.25f;

    [Header("Color Variables")]
    [SerializeField]
    private List<Color32> ColoursList = new List<Color32>();
    [SerializeField]
    private List<Color32> SelectedWheelColors = new List<Color32>();
    private int currentIndex = 0;
    private int colorsUsedIndex = 0;
    [SerializeField]
    private List<Transform> ColorSegmentsTransform = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        GetSessionColorSegmentCount();
        GenerateColours();
        GenerateColoursToWheel(colorSegmentCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ClearPreviousColors();
        }
    }

    private void GetSessionColorSegmentCount()
    {
        colorSegmentCount = GameplayManager.Instance.CalculateNewSessionColorSegmentCount();
    }

    private void GenerateColoursToWheel(int colorCount)
    {
        float anglePerQuadrant = 360f / colorCount;
        float currentAngle = 0f;

        
        for (int i = 0; i < colorCount; i++)
        {
            Vector2 itemPos = GetQuadrantPosition(currentAngle, colourWheelSize);
            GameObject colorSegmentNew = Instantiate(colorSegmentPrefab, itemPos, Quaternion.identity);
            colorSegmentNew.transform.parent = colorSegmentsParent;
            colorSegmentNew.transform.GetComponent<ColorSegmentController>().segmentColor = GetRandomColorFromList();
            ColorSegmentsTransform.Add(colorSegmentNew.transform);
            currentAngle += anglePerQuadrant;
        }

        //once colors have been generated for wheel, save the colors
        for(int i = 0; i < colorCount; i++)
        {
            //since color has been sorted based on used, select the topmost colors from the list
            //the top  most are the ones that were used
            SelectedWheelColors.Add(ColoursList[i]);
        }
    }

    public void GenerateColoursToWheelOnNewSessionLoad()
    {
        //clear previous colors and segments
        ClearPreviousColors();
        //get the coount of segment for this sessioin
        GetSessionColorSegmentCount();
        //create the segments
        GenerateColoursToWheel(colorSegmentCount);
    }

    private void ClearPreviousColors()
    {
        //clear previous selected colors
        SelectedWheelColors.Clear();
        SelectedWheelColors = new List<Color32>();

        //reset index for colors
        currentIndex = 0;
        colorsUsedIndex = 0;

        //clear previous color segments
        ColorSegmentsTransform.Clear();
        ColorSegmentsTransform = new List<Transform>();

    //delete previous color segments, while skipping the parent
    List<Transform> previousColorSegmentTransforms = colorSegmentsParent.GetComponentsInChildren<Transform>().Skip(1).ToList();
        /// Iterate from the end to avoid index shifting
        for(int i = previousColorSegmentTransforms.Count -1; i >=0; i--)
        {
            GameObject colorObject = previousColorSegmentTransforms[i].gameObject;
            Destroy(colorObject);
        }
    }

    public Vector3 GetQuadrantPosition(float currentAngle, float circleSize)
    {

        // Calculate the offset angle within the current quadrant
        float offsetAngle = currentAngle;// % anglePerSegment;

        // Calculate the x and y coordinates based on circle size and angle
        float x = circleSize * Mathf.Cos(Mathf.Deg2Rad * offsetAngle);
        float y = circleSize * Mathf.Sin(Mathf.Deg2Rad * offsetAngle);

        // Adjust for circle's center position and anchor point (assuming center anchor)
        Vector3 centerPosition = colorSegmentsParent.position;
        return centerPosition + new Vector3(x, y, 0);
    }

    private void GenerateColours()
    {
        ColoursList.Add(new Color32(144, 12, 63,255)); //maroon
        ColoursList.Add(new Color32(199, 0, 57,255)); //rede

        ColoursList.Add(new Color32(248, 222, 34,255)); //yellow
        ColoursList.Add(new Color32(182, 255, 250, 255)); //mint

        ColoursList.Add(new Color32(39, 0, 93,255)); //navy
        ColoursList.Add(new Color32(148, 0, 255, 255)); //purple
        ColoursList.Add(new Color32(166, 255, 150, 255)); //green
        ColoursList.Add(new Color32(67, 118, 108, 255)); //teal
        ColoursList.Add(new Color32(177, 148, 112, 255)); //Beige
        ColoursList.Add(new Color32(118, 69, 59, 255)); //brown
        ColoursList.Add(new Color32(255, 207, 157, 255)); //peach
        ColoursList.Add(new Color32(104, 126, 255, 255)); //cold
        ColoursList.Add(new Color32(35, 45, 63, 255)); //night
        ColoursList.Add(new Color32(255, 108, 34, 255)); //orange
        ColoursList.Add(new Color32(252, 233, 241, 255)); //retro
        ColoursList.Add(new Color32(169, 179, 136, 255)); //sage
    }

    private Color32 GetRandomColorFromList()
    {
        //gets a random color , then sort the list so that colors aren't repeated
        int colorCount = ColoursList.Count;
        int randomIndex = Random.Range(currentIndex, colorCount);


        Color32 tempHolder = ColoursList[colorsUsedIndex];
        Color32 colorToReturn = ColoursList[randomIndex];

        ColoursList[randomIndex] = tempHolder;
        ColoursList[colorsUsedIndex] = colorToReturn;

        currentIndex++;
        colorsUsedIndex++;

        return colorToReturn;

    }

    public Color32 GetRandomTileColor()
    {
        int colorCount = SelectedWheelColors.Count;
        int randomColorIndex = Random.Range(0, colorCount);
        return SelectedWheelColors[randomColorIndex];
    }

    /// <summary>
    /// Updates the colors that are displayed on the color wheel ,
    /// based on the List<Color> that are used by the available tiles
    /// </summary>
    public void UpdateColorsOnWheel(List<Color32> listOfColors)
    {
        foreach(Transform segmentTransform in ColorSegmentsTransform)
        {
            ColorSegmentController segmentController = segmentTransform.GetComponent<ColorSegmentController>();
            if(segmentController != null)
            {
                if (!listOfColors.Contains(segmentController.segmentColor))
                {
                    //this segment color wast used by any tile
                    //update it with any color that was used
                    segmentController.segmentColor = listOfColors[Random.Range(0, listOfColors.Count)];
                }
            }
            else
            {
                Debug.LogWarning("COULDNT GET THE SEGMENT CONTROLLER COMPONENT FROM THE TRANSFORM, Ensure it has one attached and is a valid color segment");
            }
            
        }
    }
}
