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
    public bool gameSessionLost;

    //EVENTS
    public event UnityAction OnWinThisSession;
    public event UnityAction OnLoseThisSession;
    private bool hasInvokedOnWinOrLoseThisSessionEvent;

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
    [HideInInspector] public bool hasFinishedBeforeTimeUp; //has the player scored all tiles before time run out
    //EVENTS
    public event UnityAction OnTimeReachZero;
    private bool hasInvokeOnTimeReachZeroEvent;


    [Header("Level Variables")]
    private int _currentLevel;
    public int level
    {
        get
        {
            return _currentLevel;
        }
    }
    public int finalSessionScore;
    
    // Start is called before the first frame update
    void Start()
    {
        canStillPlay = true;
        hasInvokeOnTimeReachZeroEvent = false;
        hasInvokedOnWinOrLoseThisSessionEvent = false;


        //Get Current Level
        _currentLevel = PlayerPrefs.GetInt(GamePrefabsNames.CURRENT_LEVEL, 1);
    }

    private void OnEnable()
    {
        OnWinThisSession += IncreaseLevelNumber;
    }
    private void OnDisable()
    {
        //OnWinThisSession -= IncreaseLevelNumber;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateGameState();
    }
    #region -Color Codes-
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
    #endregion

    #region -Level Codes-
    public void InvokeLevelWonOrLostEvents()
    {
         //invoke event once game win or lost
            if (gameSessionWon)
            {
                OnWinThisSession?.Invoke();
            }
            if (gameSessionLost)
            {
                OnLoseThisSession?.Invoke();
            }
    }

    private void IncreaseLevelNumber()
    {
        int currentLvl = PlayerPrefs.GetInt(GamePrefabsNames.CURRENT_LEVEL, 1);
        _currentLevel = currentLvl + 1;
        PlayerPrefs.SetInt(GamePrefabsNames.CURRENT_LEVEL, _currentLevel);
    }
    #endregion

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

    private void ResetOnAnotherLevelLoad()
    {
        hasInvokeOnTimeReachZeroEvent = false;
        hasInvokedOnWinOrLoseThisSessionEvent = false;
        hasFinishedBeforeTimeUp = false;
    }
}
