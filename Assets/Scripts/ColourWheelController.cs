using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class ColourWheelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject colorSegmentPrefab;
    [SerializeField] private Transform colorSegmentsParent;

    [Header("Variables")]
    [SerializeField] private int colourCount = 3;
    [SerializeField] private float colourWheelSize = 1.25f;
    [SerializeField]
    private List<Color> ColoursList = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateColours();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GenerateColoursToWheel(colourCount);
        }
    }

    private void GenerateColoursToWheel(int colorCount)
    {
        float anglePerQuadrant = 360f / colorCount;
        float currentAngle = 0f;


        for (int i = 0; i < colorCount; i++)
        {
            Vector2 itemPos = GetQuadrantPosition(currentAngle, colourWheelSize, colourCount);
            GameObject colorSampleNew = Instantiate(colorSegmentPrefab, itemPos, Quaternion.identity);
            //float angle = i * itemAngle;
            colorSampleNew.transform.parent = colorSegmentsParent;
            //colorSampleNew.transform.Rotate(Vector3.forward, angle);

            colorSampleNew.GetComponent<SpriteShapeRenderer>().color = ColoursList[i];

            currentAngle += anglePerQuadrant;
        }
    }

    public Vector3 GetQuadrantPosition(float currentAngle, float circleSize, int numSegments)
    {
        // Calculate the angle per segment
        // float anglePerSegment = 360f / numSegments;

        // Calculate the offset angle within the current quadrant
        float offsetAngle = currentAngle;// % anglePerSegment;

        // Calculate the x and y coordinates based on circle size and angle
        float x = circleSize * Mathf.Cos(Mathf.Deg2Rad * offsetAngle);
        float y = circleSize * Mathf.Sin(Mathf.Deg2Rad * offsetAngle);

        // Adjust for circle's center position and anchor point (assuming center anchor)
        Vector3 centerPosition = colorSegmentsParent.position;//+ new Vector3(0, circleSize / 2f, 0); // Adjust for anchor point if needed
        Debug.Log(centerPosition + new Vector3(x, y, 0));
        return centerPosition + new Vector3(x, y, 0);
    }

    private void GenerateColours()
    {
        ColoursList.Add(Color.black);
        ColoursList.Add(Color.blue);
        ColoursList.Add(Color.white);
        ColoursList.Add(Color.green);
    }
}
