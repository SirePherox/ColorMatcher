using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : SingletonCreator<GameModeManager>
{
    [Header("Time Variables")]
    [SerializeField] private TimerManager timeManager;

    [Header("GameMode Variables")]
    public GamePlayMode gameMode = GamePlayMode.QuickRush;
    public int finalScore;
    private int scoreMultiplier = 10;
    public enum GamePlayMode
    {
        QuickRush, //must score all times before time runs out, MUST BE THE FIRST MODE
        TimeLapse, //must take at least 2/3 of total number of tiles
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool CheckGameWonOrLostState(int scoredTilesCount, int totalTilesCount)
    {
        bool gameWon = false;

        if(gameMode == GamePlayMode.QuickRush)
        {
           gameWon =  CheckGameWon_QuickRush( scoredTilesCount, totalTilesCount);
           CalculateTotalScoreWithTime(scoredTilesCount, totalTilesCount);
        }
        else if(gameMode == GamePlayMode.TimeLapse)
        {
            gameWon = CheckGameWon_TimeLapse(scoredTilesCount, totalTilesCount);
            CalculateTotalScoreWithTime(scoredTilesCount, totalTilesCount);
        }
        else
        {
            Debug.LogWarning("COULDN'T DETECT VALID GAMEPLAY MODE, TRY SETTING GAMEMODE.QUICKRUSH / TIMELAPSE");
        }
        return gameWon;
    }

    private void SwitchGameMode()
    {
        if (gameMode == GamePlayMode.QuickRush)
        {
            gameMode = GamePlayMode.TimeLapse;
            return;
        }
        if (gameMode == GamePlayMode.TimeLapse)
        {
            gameMode = GamePlayMode.QuickRush;
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

        if (gameMode == GamePlayMode.QuickRush)
        {
            timeToUSe = defaultTime;
        }
        else if (gameMode == GamePlayMode.TimeLapse)
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

    private void CalculateTotalScoreWithTime(int scoredTilesCount, int totalTilesCount)
    {
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

            Debug.Log("Final score: " + finalScore);

        }
        else
        {
            Debug.LogError("COULDNT GET THE TIMEMANAGER COMPONENT FROM THIS OBJECT, Try attaching the TimerManager script");
        }
    }
    #endregion
}
