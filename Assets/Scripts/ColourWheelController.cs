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
    [SerializeField] private int colourCount = 3;
    [SerializeField] private float colourWheelSize = 1.25f;

    [Header("Color Variables")]
    [SerializeField]
    private List<Color> ColoursList = new List<Color>();
    [SerializeField]
    private List<Color> SelectedWheelColors = new List<Color>();
    private int currentIndex = 0;
    private int colorsUsedIndex = 0;
    private List<Transform> ColorSegmentsTransform = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateColours();
        GenerateColoursToWheel(colourCount);
    }

    // Update is called once per frame
    void Update()
    {

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
        ClearPreviousColors();
        GenerateColoursToWheel(colourCount);
    }

    private void ClearPreviousColors()
    {
        //clear previous selected colors
        SelectedWheelColors.Clear();
        SelectedWheelColors = new List<Color>();

        //reset index for colors
        currentIndex = 0;
        colorsUsedIndex = 0;

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
        ColoursList.Add(Color.black);
        ColoursList.Add(Color.blue);
        ColoursList.Add(Color.white);
        ColoursList.Add(Color.green);
        ColoursList.Add(Color.yellow);
    }

    private Color GetRandomColorFromList()
    {
        //gets a random color , then sort the list so that colors aren't repeated
        int colorCount = ColoursList.Count;
        int randomIndex = Random.Range(currentIndex, colorCount);


        Color tempHolder = ColoursList[colorsUsedIndex];
        Color colorToReturn = ColoursList[randomIndex];

        ColoursList[randomIndex] = tempHolder;
        ColoursList[colorsUsedIndex] = colorToReturn;

        currentIndex++;
        colorsUsedIndex++;

        return colorToReturn;

    }

    public Color GetRandomTileColor()
    {
        int colorCount = SelectedWheelColors.Count;
        int randomColorIndex = Random.Range(0, colorCount);
        return SelectedWheelColors[randomColorIndex];
    }

    /// <summary>
    /// Updates the colors that are displayed on the color wheel ,
    /// based on the List<Color> that are used by the available tiles
    /// </summary>
    public void UpdateColorsOnWheel(List<Color> listOfColors)
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
