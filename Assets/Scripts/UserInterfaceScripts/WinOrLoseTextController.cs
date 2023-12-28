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
        winOrLoseText.text = "Level Won";
        finalScoreText.text = GameModeManager.Instance.finalScore.ToString();
    }

    public void ShowOnLose()
    {
        winOrLoseText.text = "Level Lost";
        finalScoreText.text = GameModeManager.Instance.finalScore.ToString();
    }

    public void ShowOnNewSessionLoaded()
    {
        winOrLoseText.text = "Level Reload";
        finalScoreText.text = "00";
    }
}
