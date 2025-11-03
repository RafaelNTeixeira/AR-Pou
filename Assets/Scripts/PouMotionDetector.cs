using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PouAnimator))]
public class PouMotionDetector : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("How strong a motion spike must be to count as a shake (e.g., 1.5).")]
    public float shakeThreshold = 1.0f; 
    [Tooltip("How many spikes are needed to trigger the 'dizzy' animation.")]
    public int shakeCountRequired = 6;
    [Tooltip("How long we wait for the required spikes (in seconds).")]
    public float shakeWindow = 2.0f;

    private int shakeCount = 0;
    private float shakeTimer = 0f;
    private bool hasSpikedInThisShake = false; // Lock the spike detection so one flick doesn't count multiple times

    private PouAnimator pouAnimator;

    void Start()
    {
        pouAnimator = GetComponent<PouAnimator>();
        
        // Enable the sensor
        if (LinearAccelerationSensor.current != null)
        {
            InputSystem.EnableDevice(LinearAccelerationSensor.current);
            Debug.Log("Linear Acceleration Sensor Active! (Ignores gravity)");
        }
        else
        {
            Debug.LogError("No Linear Acceleration Sensor detected! Falling back to Accelerometer.");
            if (Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    void Update()
    {
        Vector3 accel;

        if (LinearAccelerationSensor.current != null)
        {
            accel = LinearAccelerationSensor.current.acceleration.ReadValue();
        }
        else if (Accelerometer.current != null)
        {
            accel = Accelerometer.current.acceleration.ReadValue();
        }
        else
        {
            return; // No sensors found
        }
        
        Debug.Log($"Pou Shake Debug | Mag = {accel.magnitude:F2} | Count = {shakeCount}");
        
        CheckForShake(accel);
    }

    // Method to check for shake patterns
    void CheckForShake(Vector3 accel)
    {
        // Check for a new spike. We only count if magnitude is high AND we haven't already counted this spike
        if (accel.magnitude > shakeThreshold && !hasSpikedInThisShake)
        {
            hasSpikedInThisShake = true; // Lock: We've counted this spike
            shakeCount++;
            Debug.Log($"Pou Shake spike! Count = {shakeCount}");

            // If this is the first shake, start the window
            if (shakeCount == 1)
            {
                shakeTimer = shakeWindow;
            }
        }

        // Check if the spike has ended. We can only count a new spike once the motion has stopped (gone below threshold)
        else if (accel.magnitude < shakeThreshold)
        {
            hasSpikedInThisShake = false; // Unlock: Ready for the next spike
        }

        // Tick down the window timer
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            
            // Check for completion
            if (shakeCount >= shakeCountRequired)
            {
                Debug.Log("--- POU SHAKE DETECTED! Pou is dizzy! ---");
                pouAnimator.PlayDizzyAnimation();
                ResetShake();
            }
        }
        else
        {
            // Window expired, reset
            ResetShake();
        }
    }

    void ResetShake()
    {
        shakeCount = 0;
        shakeTimer = 0f;
        hasSpikedInThisShake = false;
    }
}