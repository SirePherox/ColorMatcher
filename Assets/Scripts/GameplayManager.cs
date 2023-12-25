using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : SingletonCreator<GameplayManager>
{
    [Header("GamePlay Variables")]
    [HideInInspector] public bool isTimeZero;
    public bool canStillPlay;
    public bool gameSessionWon; 
   

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

    [Header("Score Variables")]
    private int _currentScore = 0;
    public int currentScore
    {
        get
        {
            return _currentScore;
        }
    }
    //EVENTS
    public event UnityAction<int> OnScoreChange;

    [Header("Timer Variables")]
    [HideInInspector] public float gameplayTimeFraction; //this is the value used to update time slider on UI
    //EVENTS
    public event UnityAction OnTimeReachZero;
    private bool hasInvokeOnTimeReachZeroEvent;

    // Start is called before the first frame update
    void Start()
    {
        canStillPlay = true;
        hasInvokeOnTimeReachZeroEvent = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGameState();
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

    /// <summary>
    /// amountToUpdateWith, can be positive for adding score, or negative for subtraction 
    /// </summary>
    /// <param name="amountToUpdateWith"></param>
    public void UpdateCurrentScore(int amountToUpdateWith)
    {
        _currentScore += amountToUpdateWith;
        OnScoreChange?.Invoke(_currentScore);
    }

    private void UpdateGameState()
    {
        canStillPlay = !isTimeZero;

        //invoke event once time is zero
        if (!hasInvokeOnTimeReachZeroEvent)
        {
            if (isTimeZero)
            {
                OnTimeReachZero?.Invoke();
                hasInvokeOnTimeReachZeroEvent = true;
            }
        }
    }
}
