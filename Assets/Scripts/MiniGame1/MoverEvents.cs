using System;

// Class to define and trigger movement events for the Mover
public static class MoverEvents
{
    // Define START moving events
    public static event Action onStartMoveFront;
    public static event Action onStartMoveBack;
    public static event Action onStartMoveLeft;
    public static event Action onStartMoveRight;

    // Define STOP moving events
    public static event Action onStopMoveFront;
    public static event Action onStopMoveBack;
    public static event Action onStopMoveLeft;
    public static event Action onStopMoveRight;

    // Public methods to fire START events
    public static void TriggerStartMoveFront() => onStartMoveFront?.Invoke();
    public static void TriggerStartMoveBack() => onStartMoveBack?.Invoke();
    public static void TriggerStartMoveLeft() => onStartMoveLeft?.Invoke();
    public static void TriggerStartMoveRight() => onStartMoveRight?.Invoke();

    // Public methods to fire STOP events
    public static void TriggerStopMoveFront() => onStopMoveFront?.Invoke();
    public static void TriggerStopMoveBack() => onStopMoveBack?.Invoke();
    public static void TriggerStopMoveLeft() => onStopMoveLeft?.Invoke();
    public static void TriggerStopMoveRight() => onStopMoveRight?.Invoke();
}