using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float defaultSpinSpeed = 1.0f;
  
    private float currentSpinSpeed;
    [SerializeField] private float deceleration = 5f; // Adjust deceleration rate
    private float currentRotation = 0f;
    private bool isSpinning = false;
    [SerializeField]  private float timeToNextDirectionChange = 0f;
    [SerializeField] private bool isClockwise = true; // Initial spin direction
    private const float MAX_TIME_TO_CHANGE_DIRECTION = 5.0f;
    private const float MIN_TIME_TO_CHANGE_DIRECTION = 2.0f;
    private float minRotationSpeed = 0.5f;
    private float maxRotationSpeed = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        currentSpinSpeed = defaultSpinSpeed; // Reset spin speed
    }

    // Update is called once per frame
    void Update()
    {
        SpinColorWheel();

        if (Input.GetKeyDown(KeyCode.S))
        {
            SpinWheel();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StopSpin();
        }
    }

    public void SpinWheel()
    {
        isSpinning = true;
    }

    public void StopSpin()
    {
        isSpinning = false;

        //Gradually decelerate the spin,
        currentSpinSpeed -= deceleration * Time.deltaTime;
        if (currentSpinSpeed <= 0f)
        {
            currentSpinSpeed = 0.0f;
        }
    }

    private void SpinColorWheel()
    {
        Debug.Log("..........");
        if (isSpinning)
        {
            currentRotation += currentSpinSpeed * Time.deltaTime;
            currentRotation = Mathf.Clamp(currentRotation, minRotationSpeed, maxRotationSpeed);
            Debug.Log(currentRotation);
            transform.Rotate(0f, 0f, currentRotation);

            // Gradually decelerate the spin, decelerates to stop if isSpinning stops being true
            //spinSpeed -= deceleration * Time.deltaTime;
            //if (spinSpeed <= 0f)
            //{
            //    StopSpin();
            //}

         
            // Check for direction change with timer
            timeToNextDirectionChange -= Time.deltaTime;
            if (timeToNextDirectionChange <= 0f)
            {
                timeToNextDirectionChange = Random.Range(MIN_TIME_TO_CHANGE_DIRECTION, MAX_TIME_TO_CHANGE_DIRECTION);
                isClockwise = !isClockwise;
                currentSpinSpeed = defaultSpinSpeed;
            }

            // Apply direction to spin speed
            currentSpinSpeed *= isClockwise ? 1f : -1f;
        }
    }
}
