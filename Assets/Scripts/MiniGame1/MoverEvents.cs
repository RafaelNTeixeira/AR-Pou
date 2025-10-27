using System; // Required for using 'Action'

public static class MoverEvents
{
    // Define an event 'Action' for each direction
    public static event Action onMoveFront;
    public static event Action onMoveBack;
    public static event Action onMoveLeft;
    public static event Action onMoveRight;

    // Public methods to 'fire' or 'trigger' the events safely
    // The '?' checks if any script is currently listening before firing.
    
    public static void TriggerMoveFront() => onMoveFront?.Invoke();
    public static void TriggerMoveBack() => onMoveBack?.Invoke();
    public static void TriggerMoveLeft() => onMoveLeft?.Invoke();
    public static void TriggerMoveRight() => onMoveRight?.Invoke();
}