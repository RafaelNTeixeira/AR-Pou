using UnityEngine;
using System.Collections;

// ATTACH THIS SCRIPT TO '_PositionManager'
public class MazePositionManager : MonoBehaviour
{
    [Header("Object References")]
    [Tooltip("Drag your 'POU maze' object here (the one with the PositionTracker script).")]
    public PositionTracker mazeTracker;
    public Canvas mazeCanvas;

    public float displayTime = 10f;

    private bool goBack = false;

    void Start()
    {
        // Just check that the maze reference is set
        if (mazeTracker == null)
        {
            Debug.LogError("The 'Maze Tracker' (POU maze) is not assigned!", this);
        }
    }

    void Update()
    {
        if (mazeCanvas != null)
        {
            mazeCanvas.enabled = !goBack;
        }
    }

    /// <summary>
    /// This is the new, simple function.
    /// The script that creates the sunglasses will call this.
    /// </summary>
    public void TriggerMazeMove()
    {
        if (mazeTracker != null && !goBack)
        {
            Debug.Log("MANAGER: Trigger received! Telling maze to move.");

            // Tell the maze to move
            mazeTracker.MoveToPreviousSavedPosition();
            goBack = true;
            StartCoroutine(WaitToGoBack());
        }
    }

    // Wait for a few seconds before allowing another move
    private IEnumerator WaitToGoBack()
    {
        yield return new WaitForSeconds(displayTime);
        goBack = false;
    }
}