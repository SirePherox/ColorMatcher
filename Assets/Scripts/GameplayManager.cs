using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonCreator<GameplayManager>
{
    [Header("Color Picker Variables")]
    private Color _currentlyPickedColor;
    public Color currentlyPickedColor
    {
        get
        {
            
            return _currentlyPickedColor;
        }
        set
        {
            SetCurrentlyPickedColor(value);
        }
    }
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetCurrentlyPickedColor(Color color)
    {
        _currentlyPickedColor = color;
    }

    /// <summary>
    /// Compares colorToCompare with the current color that has been picked by color picker
    /// returns True, if colorToCompare is the same as the currently picked color
    /// </summary>
    /// <param name="colorToCompare"></param>
    public bool CompareSelectedColor(Color colorToCompare)
    {
        return colorToCompare == _currentlyPickedColor;
    }
}
