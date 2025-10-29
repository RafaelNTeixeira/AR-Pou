using UnityEngine;

public class PouMazeMovement : MonoBehaviour
{
    [Tooltip("How fast the object moves per second")]
    public float moveSpeed = 5.0f;

    // Counters for how many objects are telling us to move
    private int frontMovers = 0;
    private int backMovers = 0;
    private int leftMovers = 0;
    private int rightMovers = 0;

    // Subscribe to all 8 events when this object is enabled
    private void OnEnable()
    {
        MoverEvents.onStartMoveFront += StartMoveFront;
        MoverEvents.onStartMoveBack  += StartMoveBack;
        MoverEvents.onStartMoveLeft  += StartMoveLeft;
        MoverEvents.onStartMoveRight += StartMoveRight;

        MoverEvents.onStopMoveFront += StopMoveFront;
        MoverEvents.onStopMoveBack  += StopMoveBack;
        MoverEvents.onStopMoveLeft  += StopMoveLeft;
        MoverEvents.onStopMoveRight += StopMoveRight;
    }

    // ALWAYS unsubscribe when the object is disabled
    private void OnDisable()
    {
        MoverEvents.onStartMoveFront -= StartMoveFront;
        MoverEvents.onStartMoveBack  -= StartMoveBack;
        MoverEvents.onStartMoveLeft  -= StartMoveLeft;
        MoverEvents.onStartMoveRight -= StartMoveRight;

        MoverEvents.onStopMoveFront -= StopMoveFront;
        MoverEvents.onStopMoveBack  -= StopMoveBack;
        MoverEvents.onStopMoveLeft  -= StopMoveLeft;
        MoverEvents.onStopMoveRight -= StopMoveRight;
    }

    // Update is called once per frame
    void Update()
    {
        // Create a direction vector based on our active counters
        Vector3 moveDirection = Vector3.zero;

        if (frontMovers > 0) { moveDirection += Vector3.forward; }
        if (backMovers > 0)  { moveDirection += Vector3.back; }
        if (leftMovers > 0)  { moveDirection += Vector3.left; }
        if (rightMovers > 0) { moveDirection += Vector3.right; }

        // If the final direction is not zero, move the object
        // We normalize to prevent faster diagonal movement
        if (moveDirection != Vector3.zero)
        {
            transform.Translate(
                moveDirection.normalized * moveSpeed * Time.deltaTime, 
                Space.Self
            );
        }
    }

    // --- Event Handler Methods ---

    // Increment counters when START events are received
    private void StartMoveFront() { frontMovers++; }
    private void StartMoveBack()  { backMovers++; }
    private void StartMoveLeft()  { leftMovers++; }
    private void StartMoveRight() { rightMovers++; }

    // Decrement counters when STOP events are received
    // We add Mathf.Max to prevent the count from ever going below zero
    private void StopMoveFront() { frontMovers = Mathf.Max(0, frontMovers - 1); }
    private void StopMoveBack()  { backMovers  = Mathf.Max(0, backMovers - 1); }
    private void StopMoveLeft()  { leftMovers  = Mathf.Max(0, leftMovers - 1); }
    private void StopMoveRight() { rightMovers = Mathf.Max(0, rightMovers - 1); }
}