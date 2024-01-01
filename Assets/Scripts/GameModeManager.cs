using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [Header("Time Variables")]
    [SerializeField] private TimerManager timeManager;

    [Header("GameMode Variables")]
    public GamePlayMode currentGameMode = GamePlayMode.QuickRush;
    public int finalScore;
    private int scoreMultiplier = 10;
    public enum GamePlayMode
    {
        QuickRush, //must score all times before time runs out, MUST BE THE FIRST MODE
        TimeLapse, //must take at least 2/3 of total number of tiles
    }

    #region -Singleton-
    private static GameModeManager instance;
    public static GameModeManager Instance
    {
        get
        {
            if (instance == null)
            {
                //attempt to search scene
                instance = GameObject.FindObjectOfType<GameModeManager>();
                if (instance == null)
                {
                    //no object with script attached
                    Debug.LogError("GameModeManager not found in the active scene. Creating a new instance.");
                    GameObject managerO = new GameObject("GameModeManager");
                    instance = managerO.AddComponent<GameModeManager>();
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

    }

    private void OnDestroy()
    {
        instance = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool CheckGameWonOrLostState(int scoredTilesCount, int totalTilesCount, out bool isScoreNewBestScore)
    {
        bool gameWon = false;

        if(currentGameMode == GamePlayMode.QuickRush)
        {
           gameWon =  CheckGameWon_QuickRush( scoredTilesCount, totalTilesCount);
          isScoreNewBestScore = CalculateTotalScoreWithTime(scoredTilesCount, totalTilesCount);
        }
        else if(currentGameMode == GamePlayMode.TimeLapse)
        {
            gameWon = CheckGameWon_TimeLapse(scoredTilesCount, totalTilesCount);
            isScoreNewBestScore = CalculateTotalScoreWithTime(scoredTilesCount, totalTilesCount);
        }
        else
        {
            isScoreNewBestScore = false;
            Debug.LogWarning("COULDN'T DETECT VALID GAMEPLAY MODE, TRY SETTING GAMEMODE.QUICKRUSH / TIMELAPSE");
        }
        return gameWon;
    }

    public void SwitchGameMode()
    {
        if (currentGameMode == GamePlayMode.QuickRush)
        {
            currentGameMode = GamePlayMode.TimeLapse;
            return;
        }
        if (currentGameMode == GamePlayMode.TimeLapse)
        {
            currentGameMode = GamePlayMode.QuickRush;
            return;
        }
    }
    #region -Game Mode Rules-
    /// <summary>
    /// In this mode, player must score all tiles to win
    /// </summary>
    /// <returns></returns>
    private bool CheckGameWon_QuickRush(int scoredTilesCount, int totalTilesCount)
    {
        return scoredTilesCount == totalTilesCount;

    }

    /// <summary>
    /// In this mode, returns True if player has scored two-third of the total instantiated tiles
    /// </summary>
    /// <param name="scoredTilesCount"></param>
    /// <param name="totalTilesCount"></param>
    /// <returns></returns>
    private bool CheckGameWon_TimeLapse(int scoredTilesCount, int totalTilesCount)
    {
        int minToBeWon = Mathf.RoundToInt(0.67f * totalTilesCount);
        return scoredTilesCount >= minToBeWon;
    }

    /// <summary>
    /// For QuickRush, the full time is used
    /// For TimeLapse, four-fifth of the time is used
    /// </summary>
    /// <returns></returns>
    public float SetTimeBasedOnMode(float defaultTime)
    {
        float timeToUSe = 0.0f;

        if (currentGameMode == GamePlayMode.QuickRush)
        {
            timeToUSe = defaultTime;
        }
        else if (currentGameMode == GamePlayMode.TimeLapse)
        {
            timeToUSe = (0.8f * defaultTime);
            Debug.Log("Mode is timelapse: " + timeToUSe);
        }
        else
        {
            Debug.LogWarning("COULDN'T DETECT VALID GAMEPLAY MODE TO SET TIME, TRY SETTING GAMEMODE.QUICKRUSH / TIMELAPSE");
        }
        return timeToUSe;
    }

    private bool CalculateTotalScoreWithTime(int scoredTilesCount, int totalTilesCount)
    {
        bool isScoreLatestHighscore = false;
        //attempt to get the Timer component from this object
        timeManager = GetComponent<TimerManager>();
        if(timeManager != null)
        {
            float defaultTime = timeManager.defaultTime;
            float remainingTime = timeManager.currentTime;

            // Calculate time-based score bonus
            float timeBonus = (remainingTime / defaultTime) * 100f; // Percentage of time remaining
            int timeScoreBonus = Mathf.RoundToInt(timeBonus * scoredTilesCount * 0.5f); // Scale bonus based on total tiles

            // Calculate final score with time bonus
             finalScore = scoredTilesCount * scoreMultiplier + timeScoreBonus; // Adjust base score as needed
            isScoreLatestHighscore = GameplayManager.Instance.SetHighScore(finalScore);
            Debug.Log("Final score: " + finalScore);

        }
        else
        {
            Debug.LogError("COULDNT GET THE TIMEMANAGER COMPONENT FROM THIS OBJECT, Try attaching the TimerManager script");
        }

        return isScoreLatestHighscore;
    }
    #endregion
}
