using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : SingletonCreator<GameplayManager>
{
    [Header("Script References")]
    [SerializeField] private GridItemsSpawner gridManager;

    [Header("GamePlay Variables")]
    [HideInInspector] public bool isTimeZero;
    public bool canStillPlay;
    public bool gameSessionWon; //session simply refers to a single play time, when the player starts a level
    public bool gameSessionLost; //whether it was won or lost, a session has been played

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
    [SerializeField]
    private TimerManager timerManager;

    //EVENTS
    public event UnityAction OnTimeReachZero;
    public event UnityAction OnNewLevelLoad;
    public event UnityAction OnNewSessionDelayCountdownEvent;
    private bool hasInvokeOnTimeReachZeroEvent;
    private bool hasInvokedDelayResetCountDown;

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
        CacheScriptReferences();

        canStillPlay = true;
        hasInvokeOnTimeReachZeroEvent = false;
        hasInvokedOnWinOrLoseThisSessionEvent = false;


        //Get Current Level
        _currentLevel = PlayerPrefs.GetInt(GamePrefabsNames.CURRENT_LEVEL, 1);

        
    }

    private void CacheScriptReferences()
    {
        gridManager =GameObject.Find(GameObjectNames.GRID_MANAGER).GetComponent<GridItemsSpawner>();
        if (gridManager == null)
        {
            Debug.LogError("COULDNT GET THE GRID MANAGER SCRIPT FROM THIS OBJECT, TRY Attaching the script as a component");
        }
        else
        {
            Debug.Log("Got the componenet");
        }
    }
    private void OnEnable()
    {
        OnWinThisSession += IncreaseLevelNumber;
        OnNewSessionDelayCountdownEvent += ResetOnNewSessionLoaded;
    }
    private void OnDisable()
    {
        //OnWinThisSession -= IncreaseLevelNumber;
        //OnNewSessionDelayCountdownEvent -= ResetOnNewSessionLoaded;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateGameState();
        CheckDelayResetCountdownStatus();
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

    public void IncreaseLevelNumber()
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
        canStillPlay = !isTimeZero && !gridManager.IsAllTilesScored() ;

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

    private void CheckDelayResetCountdownStatus()
    {
        if (hasInvokedDelayResetCountDown)
            return;

        //once the game ends, a delay timer counts down , once the timer is up,
        //an event is fired to Reload or Progress to another level
            if(timerManager.hasDelayResetCountdown == true)
            {
                //reset the Grid for the new session

                //call function in grid manager, thats load new session
                GridItemsSpawner gridSpawner = GameObject.Find(GameObjectNames.GRID_MANAGER).GetComponent<GridItemsSpawner>();
                if (gridSpawner != null)
                {
                    gridSpawner.LoadNextSessionRespectively();
                }
                else
                {
                    Debug.LogError("COULDNT GET THE GRIDITEMSSPAWNER SCRIPT, ENSURE THERE IS A OBJECT NAMED CORRECTLY AS  REFERENCED IN SCRIPT");
                }

                //then let other listeners know that a new session has been loadede,
                OnNewSessionDelayCountdownEvent?.Invoke();
                hasInvokedDelayResetCountDown = true;
            }
    }

    private void ResetOnNewSessionLoaded()
    {
        _currentScore = 0;
        hasInvokeOnTimeReachZeroEvent = false;
        hasInvokedOnWinOrLoseThisSessionEvent = false;
        hasFinishedBeforeTimeUp = false;
        hasInvokedDelayResetCountDown = false;
    }
}
