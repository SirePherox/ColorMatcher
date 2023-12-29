using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ColorSegmentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform childColorVisual;

    //[Header("Variables")]
    
    public Color segmentColor
    {
        get
        {
            GetChildVisualColor();
            return _segmentColor;
        }
        set
        {
            SetChildVisualColor(value);
        }
    }
    private Color _segmentColor;
    // Start is called before the first frame update
    void Start()
    {
        //childColorVisual = transform.GetChild(0).GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetChildVisualColor()
    {
        Color color = childColorVisual.GetComponent<SpriteRenderer>().color;
        _segmentColor = color;
    }

    private void SetChildVisualColor(Color colorToSet)
    {
        childColorVisual.GetComponent<SpriteRenderer>().color = colorToSet;
        _segmentColor = colorToSet;
    }
    
}
