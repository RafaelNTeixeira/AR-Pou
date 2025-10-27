using UnityEngine;

public class TriggerMovement : MonoBehaviour
{
    // This enum lets you choose the direction in the Inspector
    public enum MoveDirection
    {
        Front,
        Back,
        Left,
        Right
    }

    [Tooltip("Which direction should this prefab trigger?")]
    public MoveDirection directionToTrigger;

    // OnEnable is called whenever an object is Instantiated or SetActive(true)
    private void OnEnable()
    {
        // Fire the correct event based on the Inspector setting
        switch (directionToTrigger)
        {
            case MoveDirection.Front:
                MoverEvents.TriggerMoveFront();
                break;
            case MoveDirection.Back:
                MoverEvents.TriggerMoveBack();
                break;
            case MoveDirection.Left:
                MoverEvents.TriggerMoveLeft();
                break;
            case MoveDirection.Right:
                MoverEvents.TriggerMoveRight();
                break;
        }
    }
}