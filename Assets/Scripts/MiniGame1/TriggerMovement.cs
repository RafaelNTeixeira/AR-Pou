using UnityEngine;

// Class to trigger movement events based on object enable/disable
public class TriggerMovement : MonoBehaviour
{
    // This enum lets we choose the direction in the Inspector
    public enum MoveDirection
    {
        Front,
        Back,
        Left,
        Right
    }

    [Tooltip("Which direction should this prefab trigger?")]
    public MoveDirection directionToTrigger;

    // OnEnable is called when the object is Instantiated or SetActive(true)
    private void OnEnable()
    {
        // Fire the START event
        switch (directionToTrigger)
        {
            case MoveDirection.Front: MoverEvents.TriggerStartMoveFront(); break;
            case MoveDirection.Back: MoverEvents.TriggerStartMoveBack(); break;
            case MoveDirection.Left: MoverEvents.TriggerStartMoveLeft(); break;
            case MoveDirection.Right: MoverEvents.TriggerStartMoveRight(); break;
        }
    }

    // Method called when the object is Destroyed or SetActive(false)
    private void OnDisable()
    {
        // Fire the STOP event
        switch (directionToTrigger)
        {
            case MoveDirection.Front: MoverEvents.TriggerStopMoveFront(); break;
            case MoveDirection.Back: MoverEvents.TriggerStopMoveBack(); break;
            case MoveDirection.Left: MoverEvents.TriggerStopMoveLeft(); break;
            case MoveDirection.Right: MoverEvents.TriggerStopMoveRight(); break;
        }
    }
}