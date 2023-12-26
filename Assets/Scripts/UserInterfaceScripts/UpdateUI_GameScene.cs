using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(300)] //delay UI updates, as it depends on some values from other scripts
public class UpdateUI_GameScene : MonoBehaviour
{
    [Header("Score Variables")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Timer Variables")]
    [SerializeField] private Slider timerSlider;

    [Header("Level Variables")]
    [SerializeField] private Transform winOrLoseObject;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "00";
    }
    private void OnEnable()
    {
        GameplayManager.Instance.OnScoreChange += UpdateUIOnScoreChange;
        GameplayManager.Instance.OnWinThisSession += UpdateUIOnSessionWon;
        GameplayManager.Instance.OnLoseThisSession += UpdateUIOnSessionLost;
    }
    private void OnDisable()
    {
        GameplayManager.Instance.OnScoreChange -= UpdateUIOnScoreChange;
        GameplayManager.Instance.OnWinThisSession -= UpdateUIOnSessionWon;
        GameplayManager.Instance.OnLoseThisSession -= UpdateUIOnSessionLost;
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
  
        }
    }

    private void UpdateUIOnSessionWon()
    {
        if (winOrLoseObject.GetComponent<WinOrLoseTextController>() != null)
        {
            winOrLoseObject.GetComponent<WinOrLoseTextController>().ShowOnWin();
        }
    }
}
