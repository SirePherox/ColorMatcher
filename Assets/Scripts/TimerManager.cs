using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("Time Variables")]
    public float defaultTime = 30.0f;
    [HideInInspector] public float currentTime;
    [SerializeField]
    private bool startCountDown;
    [SerializeField] private TextMeshProUGUI delayCountdownText;
    [SerializeField] private Image delayCountDown_img;
    [SerializeField] private GameObject winOrLoseParent; //object should only be seen at the end of each session
    [HideInInspector] public bool hasDelayResetCountdown;
    //EVENTS
    
    [Header("Level Variables")]
    private float defaultLoadLevelTime = 3.0f;
    [SerializeField]
    private float currentLoadLevelTime;
    private float defaultDelayTime = 5.0f; //time to wait for player to see the result of current level
    private float currentdelayTime;
    private bool canStartDelay;
    // Start is called before the first frame update
    void Start()
    {
        ResetDefaultTimer();
        canStartDelay = false;
        currentdelayTime = defaultDelayTime;
    }
    private void OnEnable()
    {
        GameplayManager.Instance.OnWinThisSession += StartDelayCountdown;
        GameplayManager.Instance.OnLoseThisSession += StartDelayCountdown;
        GameplayManager.Instance.OnNewSessionDelayCountdownEvent += ResetOnNewSessionLoaded;
    }
    private void OnDisable()
    {
        if(GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnWinThisSession -= StartDelayCountdown;
            GameplayManager.Instance.OnLoseThisSession -= StartDelayCountdown;
            GameplayManager.Instance.OnNewSessionDelayCountdownEvent -= ResetOnNewSessionLoaded;
        }

    }
    // Update is called once per frame
    void Update()
    {
        CountDown();
        UpdateTimeSlider();
        DelayCountDown_LoadSession();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            startCountDown = true;
            //can sync the button to go inactive based on timereachzero event in gameplaymanager
        }
    }

    private void CountDown()
    {
        if (startCountDown)
        {
            //stop countdown if player should score all tiles
            if (!GameplayManager.Instance.hasFinishedBeforeTimeUp)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    currentTime = 0.0f;
                    GameplayManager.Instance.isTimeZero = true;

                }
            }
          
        }
    }

    private void ResetDefaultTimer()
    {
        currentTime = GameModeManager.Instance.SetTimeBasedOnMode( defaultTime);
        GameplayManager.Instance.isTimeZero = false;
    }

    private void UpdateTimeSlider()
    {
        float value = Mathf.Clamp01(currentTime / defaultTime);
        GameplayManager.Instance.gameplayTimeFraction = value;
    }

  

    private void StartDelayCountdown()
    {
        canStartDelay = true;
    }

    /// <summary>
    /// After game session ends, starts a countdown to delay anything happening so player can see the result of current session
    /// after countdown hits zero, set "hasDelayResetCountdown" to True
    /// </summary>
    private void DelayCountDown_LoadSession()
    {
        if (canStartDelay)
        {

            //update image
            delayCountDown_img.gameObject.SetActive(true);
            float fillAmount = 1.0f - (currentdelayTime / defaultDelayTime);
            delayCountDown_img.fillAmount = fillAmount;

            winOrLoseParent.gameObject.SetActive(true);

            currentdelayTime -= Time.deltaTime;
            if(currentdelayTime <= 0.0f)
            {
                currentdelayTime = 0.0f;

                hasDelayResetCountdown = true;
                canStartDelay = false;
            }
        }
        else
        {
            delayCountDown_img.gameObject.SetActive(false);
            winOrLoseParent.gameObject.SetActive(false);
        }
    }

    private void ResetOnNewSessionLoaded()
    {
        currentdelayTime = defaultDelayTime;
        ResetDefaultTimer();
        startCountDown = false;
        hasDelayResetCountdown = false;
    }
}
