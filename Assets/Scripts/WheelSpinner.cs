using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WheelSpinner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button wheelControl_btn;

    [Header("Variables")]
    [SerializeField] private float defaultSpinSpeed = 1.0f; 
    private float currentSpinSpeed;
    [SerializeField] private float deceleration = 5f; // Adjust deceleration rate
    private float currentRotation = 0f;
    [SerializeField]
    private bool isSpinning = false;
    [SerializeField]  private float timeToNextDirectionChange = 0f;
    [SerializeField] private bool isClockwise = true; // Initial spin direction
    private const float MAX_TIME_TO_CHANGE_DIRECTION = 5.0f;
    private const float MIN_TIME_TO_CHANGE_DIRECTION = 2.0f;

    //EVENTS
    public event UnityAction OnPlayerStopSpin;

    //TEMPORARY SPIN
    private float defaultTempTimer = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        currentSpinSpeed = defaultSpinSpeed; // Reset spin speed
        wheelControl_btn.onClick.AddListener(ControlWheelWithButton);
    }

    // Update is called once per frame
    void Update()
    {
        SpinColorWheel();

        if (!GameplayManager.Instance.canStillPlay)
        {
            //stop spin once player cant play 
            StopSpin();
        }
            
    }

    private void ControlWheelWithButton()
    {
      
            if (!GameplayManager.Instance.canStillPlay)
                return;

            if (isSpinning)
            {
                StopSpin();
            }
            else
            {
                SpinWheel();
            }
        
    }
    public void SpinWheel()
    {
        isSpinning = true;
    }

    public void StopSpin()
    {
        isSpinning = false;
        OnPlayerStopSpin?.Invoke();
        //Gradually decelerate the spin,
        currentSpinSpeed -= deceleration * Time.deltaTime;
        if (currentSpinSpeed <= 0f)
        {
            currentSpinSpeed = 0.0f;
        }
    }

    private void SpinColorWheel()
    {
        if (isSpinning)
        {
            currentRotation += currentSpinSpeed * Time.deltaTime;
          //  currentRotation = Mathf.Clamp(currentRotation, minRotationSpeed, maxRotationSpeed);
          
            transform.Rotate(0f, 0f, currentRotation);

         
            // Check for direction change with timer
            timeToNextDirectionChange -= Time.deltaTime;
            if (timeToNextDirectionChange <= 0f)
            {
                timeToNextDirectionChange = Random.Range(MIN_TIME_TO_CHANGE_DIRECTION, MAX_TIME_TO_CHANGE_DIRECTION);
                isClockwise = !isClockwise;
                currentSpinSpeed = defaultSpinSpeed;

                // Apply direction to spin speed
                currentSpinSpeed *= isClockwise ? 1f : -1f;
            }


        }
    }

    public IEnumerator SpinTemporarily()
    {
        //this is called when the player stops the spin but it doesnt stop on any valid color
        SpinWheel();
        yield return new WaitForSeconds(defaultTempTimer);
        StopSpin();
    }
}
