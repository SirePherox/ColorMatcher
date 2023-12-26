using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform childColorVisual;

    [Header("Variables")]
    public int scoreValue = 2;  //points for destroying this tile
    public int loseValue = 1; //points subtracted for clicking when it isnt correct
    public Color tileColor
    {
        get
        {
            GetChildVisualColor();
            return _tileColor;
        }
        set
        {
            SetChildVisualColor(value);
        }
    }
    private Color _tileColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GetChildVisualColor()
    {
        Color color = childColorVisual.GetComponent<SpriteRenderer>().color;
        _tileColor = color;
        Debug.Log("Tile Color gotten is : " + _tileColor);
    }

    private void SetChildVisualColor(Color colorToSet)
    {
        childColorVisual.GetComponent<SpriteRenderer>().color = colorToSet;
        _tileColor = colorToSet;
    }

    private void OnMouseDown()
    {
        if (!GameplayManager.Instance.canStillPlay)
            return;

        if (GameplayManager.Instance.CompareSelectedColor(_tileColor))
        {
            GameplayManager.Instance.UpdateCurrentScore(+scoreValue);
            gameObject.SetActive(false);
        }
        else
        {
            GameplayManager.Instance.UpdateCurrentScore(-loseValue);
            Debug.Log("Color isnt correct......");
        }
        
    }
}
