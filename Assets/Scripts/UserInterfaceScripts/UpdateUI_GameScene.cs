using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(300)] //delay UI updates, as it depends on some values from other scripts
public class UpdateUI_GameScene : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private GridItemsSpawner gridManager;

    [Header("UI References")]
    [SerializeField] private Button pauseGameBtn;
    [SerializeField] private Button resumeGameBtn;
    [SerializeField] private GameObject pauseGamePanel;
    [SerializeField] private Button goToMenuBtn;
    [SerializeField] private GameObject loadingScreen;

    [Header("Score Variables")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject highScore_visualFeedback;

    [Header("Timer Variables")]
    [SerializeField] private Slider timerSlider;

    [Header("Level Variables")]
    [SerializeField] private Transform winOrLoseObject;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelModeText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUIOnStart();
        AddButtonListeners();
    }
    private void OnEnable()
    {
        GameplayManager.Instance.OnScoreChange += UpdateUIOnScoreChange;
        GameplayManager.Instance.OnWinThisSession += UpdateUIOnSessionWon;
        GameplayManager.Instance.OnLoseThisSession += UpdateUIOnSessionLost;
        GameplayManager.Instance.OnNewSessionDelayCountdownEvent += UpdateUIOnNewSessionLoaded;
    }
    private void OnDisable()
    {
        if(GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnScoreChange -= UpdateUIOnScoreChange;
            GameplayManager.Instance.OnWinThisSession -= UpdateUIOnSessionWon;
            GameplayManager.Instance.OnLoseThisSession -= UpdateUIOnSessionLost;
            GameplayManager.Instance.OnNewSessionDelayCountdownEvent -= UpdateUIOnNewSessionLoaded;
        }

    }
    // Update is called once per frame
    void Update()
    {
        UpdateTimerSlider();
    }



    private void UpdateUIOnScoreChange(int scoreToShow)
    {
        scoreText.text = scoreToShow.ToString();
    }

    private void UpdateTimerSlider()
    {
        timerSlider.value = GameplayManager.Instance.gameplayTimeFraction;
    }

    private void UpdateUIOnSessionLost()
    {
        if(winOrLoseObject.GetComponent<WinOrLoseTextController>() != null)
        {
            winOrLoseObject.GetComponent<WinOrLoseTextController>().ShowOnLose();
            ShowOnHighScore();

        }
    }

    private void UpdateUIOnSessionWon()
    {
        if (winOrLoseObject.GetComponent<WinOrLoseTextController>() != null)
        {
            winOrLoseObject.GetComponent<WinOrLoseTextController>().ShowOnWin();
            ShowOnHighScore();
        }
    }

    private void UpdateUIOnNewSessionLoaded()
    {
        scoreText.text = "00";
        levelText.text = "LVL-" + GameplayManager.Instance.level.ToString();
        highScore_visualFeedback.SetActive(false);
        UpdateModeLevelText();

        if (winOrLoseObject.GetComponent<WinOrLoseTextController>() != null)
        {
            winOrLoseObject.GetComponent<WinOrLoseTextController>().ShowOnNewSessionLoaded();

        }
    }

    private void ShowOnHighScore()
    {
        if (gridManager.isCurrentWinBestScore)
        {
            highScore_visualFeedback.SetActive(true);
        }
    }
    private void UpdateUIOnStart()
    {
        UpdateModeLevelText();
        scoreText.text = "00";
        levelText.text = "LVL-" + GameplayManager.Instance.level.ToString();
        pauseGamePanel.SetActive(false);
        highScore_visualFeedback.SetActive(false);
      
    }

    private void UpdateModeLevelText()
    {
        if(GameModeManager.Instance.currentGameMode == GameModeManager.GamePlayMode.QuickRush)
        {
            levelModeText.text = "Q-R";
        }
        else if(GameModeManager.Instance.currentGameMode == GameModeManager.GamePlayMode.TimeLapse)
        {
            levelModeText.text = "T-L";
        }
        else
        {
            levelModeText.text = "SOS";
        }
    }
    #region -Button Onclick Events-

    private void AddButtonListeners()
    {
        goToMenuBtn.onClick.AddListener(GoToMainMenu);
        pauseGameBtn.onClick.AddListener(PauseGame);
        resumeGameBtn.onClick.AddListener(ResumeGame);
    }

    private void GoToMainMenu()
    {
        SoundController.Instance.PlayButtonClick();
        //show loading scene
        loadingScreen.SetActive(true);
        //load main menu
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(SceneIndex.MAIN_MENU));
    }

    private void PauseGame()
    {
        SoundController.Instance.PlayButtonClick();
        //show pause screen
        pauseGamePanel.SetActive(true);
        //pause game
        GameplayManager.Instance.gamePaused = true;
    }

    private void ResumeGame()
    {
        SoundController.Instance.PlayButtonClick();
        //disable loadingScreen, so it wont be active next time the game is paused
        loadingScreen.SetActive(false);
        //disable pause screen
        pauseGamePanel.SetActive(false);
        //unpause game
        GameplayManager.Instance.gamePaused = false;
    }
    #endregion
}
