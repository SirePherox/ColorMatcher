using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinOrLoseTextController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI winOrLoseText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOnWin()
    {
        winOrLoseText.text = "Yay!, You Won.";
        finalScoreText.text = GameModeManager.Instance.finalScore.ToString();
    }

    public void ShowOnLose()
    {
        winOrLoseText.text = "Oops!, You Lost.";
        finalScoreText.text = GameModeManager.Instance.finalScore.ToString();
    }

    public void ShowOnNewSessionLoaded()
    {
        winOrLoseText.text = "Loading...";
        finalScoreText.text = "00";
    }
}
