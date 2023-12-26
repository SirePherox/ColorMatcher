using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    [Header("Time Variables")]
    public float defaultTime = 30.0f;
    [HideInInspector] public float currentTime;
    [SerializeField]
    private bool startCountDown;

    //EVENTS
 

    // Start is called before the first frame update
    void Start()
    {
        ResetDefaultTimer();
    }

    // Update is called once per frame
    void Update()
    {
        CountDown();
        UpdateTimeSlider();

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
}
