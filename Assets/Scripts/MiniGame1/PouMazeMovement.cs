using UnityEngine;

public class PouMazeMovement : MonoBehaviour
{
    [Tooltip("How far to move in one step when triggered")]
    public float moveDistance = 1.0f;

    // Subscribe to the events when this object is enabled
    private void OnEnable()
    {
        MoverEvents.onMoveFront += MoveFront;
        MoverEvents.onMoveBack += MoveBack;
        MoverEvents.onMoveLeft += MoveLeft;
        MoverEvents.onMoveRight += MoveRight;
    }

    // ALWAYS unsubscribe when the object is disabled to prevent errors
    private void OnDisable()
    {
        MoverEvents.onMoveFront -= MoveFront;
        MoverEvents.onMoveBack -= MoveBack;
        MoverEvents.onMoveLeft -= MoveLeft;
        MoverEvents.onMoveRight -= MoveRight;
    }

    // --- Movement Methods ---

    private void MoveFront()
    {
        // Using Space.Self moves based on the object's local forward direction
        transform.Translate(Vector3.forward * moveDistance, Space.Self);
        Debug.Log("Moving Front!");
    }

    private void MoveBack()
    {
        transform.Translate(Vector3.back * moveDistance, Space.Self);
        Debug.Log("Moving Back!");
    }

    private void MoveLeft()
    {
        transform.Translate(Vector3.left * moveDistance, Space.Self);
        Debug.Log("Moving Left!");
    }

    private void MoveRight()
    {
        transform.Translate(Vector3.right * moveDistance, Space.Self);
        Debug.Log("Moving Right!");
    }
}
