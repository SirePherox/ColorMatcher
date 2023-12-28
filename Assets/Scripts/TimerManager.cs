using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Time Variables")]
    public float defaultTime = 30.0f;
    [HideInInspector] public float currentTime;
    [SerializeField]
    private bool startCountDown;
    [SerializeField] private TextMeshProUGUI delayCountdownText;
    [HideInInspector] public bool hasDelayResetCountdown;
    //EVENTS
    
    [Header("Level Variables")]
    private float defaultLoadLevelTime = 3.0f;
    [SerializeField]
    private float currentLoadLevelTime;
    private float delayTime = 2.0f; //time to wait for player to see the result of current level
    private float currentdelayTime;
    private bool canStartDelay;
    // Start is called before the first frame update
    void Start()
    {
        ResetDefaultTimer();
        canStartDelay = false;
        currentdelayTime = delayTime;
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
            
            delayCountdownText.gameObject.SetActive(true);
            delayCountdownText.text = currentdelayTime.ToString("F1");
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
            delayCountdownText.gameObject.SetActive(false);
        }
    }

    private void ResetOnNewSessionLoaded()
    {
        currentdelayTime = delayTime;
        ResetDefaultTimer();
        startCountDown = false;
        hasDelayResetCountdown = false;
    }
}
