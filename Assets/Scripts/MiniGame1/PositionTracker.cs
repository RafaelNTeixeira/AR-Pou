using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    // This List will store our entire position history
    private List<Vector3> savedPositions = new List<Vector3>();

    // This index tracks which position we should move to next
    private int currentPositionIndex = -1;

    /// <summary>
    /// This is a built-in Unity function that is called when this object's
    /// collider enters another collider marked as 'Is Trigger'.
    /// </summary>
    /// <param name="other">The collider this object entered.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider we entered has the "check" tag
        if (other.CompareTag("Check"))
        {
            // We entered a checkpoint.
            // Save our current position. This function is only called
            // ONCE per entry, achieving your goal.
            SaveNewPosition(transform.position);
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