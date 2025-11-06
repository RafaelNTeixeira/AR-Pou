using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to track and manage saved positions for an object
public class PositionTracker : MonoBehaviour
{
    // This List will store our entire position history
    private List<Vector3> savedPositions = new List<Vector3>();

    // Tolerance to prevent saving the same spot multiple times
    public float positionTolerance = 0.001f;

    // When the object enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider we entered has the "Check" tag
        if (other.CompareTag("Check"))
        {
            Vector3 currentPosition = transform.position;

            // If this is the first checkpoint, just save it.
            if (savedPositions.Count == 0)
            {
                SaveNewPosition(currentPosition);
            }
            // Otherwise, check distance from the last saved one
            else
            {
                Vector3 lastSavedPosition = savedPositions[savedPositions.Count - 1];
                
                // Only save if it's a new position
                if (Vector3.Distance(currentPosition, lastSavedPosition) > positionTolerance)
                {
                    SaveNewPosition(currentPosition);
                }
            }
        }
    }

    // Adds a new position to our history and logs it.
    private void SaveNewPosition(Vector3 position)
    {
        savedPositions.Add(position);

        Debug.Log($"New position saved: {position}. Total count: {savedPositions.Count}");
    }

    // Method to move the object to the previous saved position
    public void MoveToPreviousSavedPosition()
    {
        // We can't go back if there is only 1 (or 0) position saved.
        if (savedPositions.Count <= 1)
        {
            Debug.LogWarning("Not enough positions to go back to.");
            return;
        }

        // Remove the very last position from the list.
        savedPositions.RemoveAt(savedPositions.Count - 1);

        // Get the new last position from the list (which is our target)
        Vector3 targetPosition = savedPositions[savedPositions.Count - 1];

        // Move the object to that position
        transform.position = targetPosition;

        Debug.Log($"Moved to {targetPosition}. Checkpoints remaining: {savedPositions.Count}");
    }
}