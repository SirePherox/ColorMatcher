using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : SingletonCreator<GameModeManager>
{
    [Header("GameMode Variables")]
    public GamePlayMode gameMode = GamePlayMode.QuickRush;

    public enum GamePlayMode
    {
        QuickRush, //must finish everything before time 
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


    public bool CheckGameWonState(int scoredTilesCount, int totalTilesCount)
    {
        bool gameWon = false;

        if(gameMode == GamePlayMode.QuickRush)
        {
           gameWon =  CheckGameWon_QuickRush( scoredTilesCount, totalTilesCount);
        }
        else if(gameMode == GamePlayMode.TimeLapse)
        {
            gameWon = CheckGameWon_TimeLapse(scoredTilesCount, totalTilesCount);
        }
        else
        {
            Debug.LogWarning("COULDN'T DETECT VALID GAMEPLAY MODE, TRY SETTING GAMEMODE.QUICKRUSH / TIMELAPSE");
        }
        return gameWon;
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
        int minToBeWon = 2 / 3 * (totalTilesCount);
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
            timeToUSe = 4 / 5 * (defaultTime);
            Debug.Log("Mode is timelapse: " + timeToUSe);
        }
        else
        {
            Debug.LogWarning("COULDN'T DETECT VALID GAMEPLAY MODE TO SET TIME, TRY SETTING GAMEMODE.QUICKRUSH / TIMELAPSE");
        }
        return timeToUSe;
    }
    #endregion
}
