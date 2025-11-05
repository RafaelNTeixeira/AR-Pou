using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    [Tooltip("The interval in seconds to check and save the position.")]
    public float saveInterval = 2.0f;

    [Tooltip("How close the object needs to be to the last position to be considered 'the same'.")]
    public float positionTolerance = 0.01f; // 1cm tolerance

    // This List will store our entire position history
    private List<Vector3> savedPositions = new List<Vector3>();

    // This index tracks which position we should move to next
    private int currentPositionIndex = -1;

    void Start()
    {
        // Start the repeating process of saving the position
        StartCoroutine(SavePositionRoutine());
    }

    /// <summary>
    /// A Coroutine that runs every 'saveInterval' seconds.
    /// </summary>
    private IEnumerator SavePositionRoutine()
    {
        // This loop will run forever as long as the object is active
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(saveInterval);

            Vector3 currentPosition = transform.position;

            // Check if we have any positions saved yet
            if (savedPositions.Count == 0)
            {
                // If this is the first one, just save it
                SaveNewPosition(currentPosition);
            }
            else
            {
                // Compare to the *last* saved position
                Vector3 lastSavedPosition = savedPositions[savedPositions.Count - 1];

                // Only save if the distance is greater than our tolerance
                // (We don't use == for positions due to floating-point inaccuracy)
                if (Vector3.Distance(currentPosition, lastSavedPosition) > positionTolerance)
                {
                    SaveNewPosition(currentPosition);
                }
            }
        }
    }

    /// <summary>
    /// Adds a new position to our history and logs it.
    /// </summary>
    private void SaveNewPosition(Vector3 position)
    {
        savedPositions.Add(position);
        
        // IMPORTANT: Every time we save a new position, 
        // we reset the index to point to this new, last-saved item.
        currentPositionIndex = savedPositions.Count - 1;
        
        Debug.Log($"New position saved: {position}. Total count: {savedPositions.Count}");
    }

    /// <summary>
    /// This is the public function that Object2 will call.
    /// </summary>
    public void MoveToPreviousSavedPosition()
    {
        // Do nothing if no positions have been saved
        if (savedPositions.Count == 0)
        {
            Debug.LogWarning("Object2 triggered a move, but no positions are saved yet.");
            return;
        }

        // Check if the index is valid (it might be -1 if we just started)
        if (currentPositionIndex < 0 || currentPositionIndex >= savedPositions.Count)
        {
            currentPositionIndex = savedPositions.Count - 1;
        }

        // Get the target position from our list
        Vector3 targetPosition = savedPositions[currentPositionIndex];

        // Move the object. This is an instant move.
        // See the "Improvements" section for a smooth move.
        transform.position = targetPosition;
        
        Debug.Log($"Moving to position index {currentPositionIndex}: {targetPosition}");

        // Decrement the index for the *next* time this is called
        currentPositionIndex--;

        // If the index has gone below 0, wrap it back around to the end of the list
        if (currentPositionIndex < 0)
        {
            currentPositionIndex = savedPositions.Count - 1;
        }
    }
}