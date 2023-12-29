using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPickerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private BoxCollider2D pickerCollider;
    [SerializeField] private LayerMask colorsLayerMask;
    [SerializeField] private WheelSpinner wheelSpinner;
    [SerializeField] private Transform pickerColor_VisualFeedback;

    [Header("Variables")]
    [SerializeField] private Color currentPickedColor;
    // Start is called before the first frame update
    void Start()
    {
        pickerCollider = GetComponent<BoxCollider2D>();
        
    }
    private void OnEnable()
    {
        wheelSpinner.OnPlayerStopSpin += PickColour;
    }
    private void OnDisable()
    {
        wheelSpinner.OnPlayerStopSpin -= PickColour;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void PickColour()
    {
        RaycastHit2D hitColor= Physics2D.BoxCast(pickerCollider.transform.position, pickerCollider.size, 0f,
                                                           Vector2.zero, 0f, colorsLayerMask);
        if(hitColor.collider == null)
        {
            Debug.Log("ROTATE WHEEL AGAIN TILL SOMETHING CONTACTS");
            StartCoroutine(wheelSpinner.SpinTemporarily());
        }
        else
        {
            if(hitColor.collider.transform.GetComponent<ColorSegmentController>() != null)
            {
                currentPickedColor = hitColor.collider.transform.GetComponent<ColorSegmentController>().segmentColor;
                GameplayManager.Instance.currentlyPickedColor = currentPickedColor;
                Color colorFeedback =new Color (currentPickedColor.r, currentPickedColor.g, currentPickedColor.b);
                pickerColor_VisualFeedback.GetComponent<SpriteRenderer>().color = colorFeedback;
            }
           
        }
    }
}
