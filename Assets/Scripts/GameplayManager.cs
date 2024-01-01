using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[DefaultExecutionOrder(300)]
public class GameplayManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private GridItemsSpawner gridManager;

    [Header("GamePlay Variables")]
    [HideInInspector] public bool isTimeZero;
    public bool canStillPlay;
    public bool gameSessionWon; //session simply refers to a single play time, when the player starts a level
    public bool gameSessionLost; //whether it was won or lost, a session has been played
    public bool gamePaused;
    //EVENTS
    public event UnityAction OnWinThisSession;
    public event UnityAction OnLoseThisSession;
   

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

    #region -Singleton-
    private static GameplayManager instance;
    public static GameplayManager Instance
    {
        get
        {

            if (instance == null)
            {
                //attempt to search scene
                instance = GameObject.FindObjectOfType<GameplayManager>();
                if (instance == null )//&& SceneLoader.Instance.GetCurrentSceneIndex() == SceneIndex.GAME_SCENE)
                {
                    //no object with script attached
                    Debug.LogError("GameplayManager not found in the active scene. Try adding it as component  to a  gameobject"); //Creating a new instance.);
                }
                
            }
            return instance;
        }
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // If there's already an instance, destroy the duplicate
            Destroy(gameObject);
            return;
        }
            

        Debug.Log("Calling Awake function");
        CacheScriptReferences();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    
    // Start is called before the first frame update
    void Start()
    {
       

        canStillPlay = true;
        hasInvokeOnTimeReachZeroEvent = false;
        //hasInvokedOnWinOrLoseThisSessionEvent = false;


        //Get Current Level
        _currentLevel = PlayerPrefs.GetInt(GamePrefabsNames.CURRENT_LEVEL, 1);
        Debug.Log("Calling start function");
        
    }

    private void CacheScriptReferences()
    {
        //grid manager
        gridManager =  GameObject.Find(GameObjectNames.GRID_MANAGER).GetComponent<GridItemsSpawner>();
        if (gridManager == null)
        {
            Debug.LogError("COULDNT GET THE GRID MANAGER SCRIPT FROM THIS OBJECT, TRY Attaching the script as a component");
        }
        else
        {
            Debug.Log("Cache the GridManager componenet");
        }

        //timer manager
        timerManager  = GameObject.Find(GameObjectNames.TIMER_MANAGER).GetComponent<TimerManager>();
        if (timerManager == null)
        {
            Debug.LogError("COULDNT GET THE TIMER MANAGER SCRIPT FROM THIS OBJECT, TRY Attaching the script as a component");
        }
        else
        {
            Debug.Log("Got the TimerMAnager componenet");
        }
    }
    private void OnEnable()
    {
        OnWinThisSession += IncreaseLevelNumber;
        OnNewSessionDelayCountdownEvent += ResetOnNewSessionLoaded;
        Debug.Log("Calling enable ...");
    }
    private void OnDisable()
    {
        //OnWinThisSession -= IncreaseLevelNumber;
        //OnNewSessionDelayCountdownEvent -= ResetOnNewSessionLoaded;
        Debug.Log("Calling disnable ...");
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

    public int CalculateNewSessionColorSegmentCount()
    {
        int currentLvl = _currentLevel;
        int newSessionColorCount = 3;
        if(currentLvl <= 3)
        {
            return 3;
        }else if(currentLvl <= 5)
        {
            return 5;
        }
        else
        {
            return 6;
        }
    }

    public int CalculateNewSessionGridCount()
    {
        int currentLvl = _currentLevel;
        if (currentLvl <= 3)
        {
            return 3;
        }
        else if (currentLvl <= 5)
        {
            return 5;
        }
        else
        {
            return 6;
        }
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
        canStillPlay = !isTimeZero && !gridManager.IsAllTilesScored() && !gamePaused ;

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

            hasInvokedDelayResetCountDown = true;
            //then let other listeners know that a new session has been loadede,
            OnNewSessionDelayCountdownEvent?.Invoke();
               
            }
    }

    public bool SetHighScore(int score)
    {
        //returns true if score is the latest best score
        bool isNewHighScore = false;
        int prevHighScore = PlayerPrefs.GetInt(GamePrefabsNames.HIGHSCORE, 0);
        if(score > prevHighScore)
        {
            PlayerPrefs.SetInt(GamePrefabsNames.HIGHSCORE, score);
            isNewHighScore = true;
        }
        return isNewHighScore;
    }
    private void ResetOnNewSessionLoaded()
    {
        _currentScore = 0;
        hasInvokeOnTimeReachZeroEvent = false;
        hasFinishedBeforeTimeUp = false;
        hasInvokedDelayResetCountDown = false;
    }
}
