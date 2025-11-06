using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to track and manage saved positions for an object
public class PositionTracker : MonoBehaviour
{
    public Transform mazeAnchor;

    // This List will store our entire local position history
    private List<Vector3> savedPositions = new List<Vector3>();

    // Tolerance to prevent saving the same spot multiple times
    public float positionTolerance = 0.001f;

    // When the object enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider we entered has the "Check" tag
        if (other.CompareTag("Check"))
        {
            if (mazeAnchor == null)
            {
                Debug.LogError("Maze Anchor is not set on the PositionTracker! Cannot save relative position.", this.gameObject);
                return;
            }

            Vector3 currentLocalPosition = mazeAnchor.InverseTransformPoint(transform.position);

            // If this is the first checkpoint, just save it.
            if (savedPositions.Count == 0)
            {
                SaveNewPosition(currentLocalPosition);
            }
            // Otherwise, check distance from the last saved one
            else
            {
                Vector3 lastSavedPosition = savedPositions[savedPositions.Count - 1];
                
                // This distance check is still valid as we are comparing two local positions
                if (Vector3.Distance(currentLocalPosition, lastSavedPosition) > positionTolerance)
                {
                    SaveNewPosition(currentLocalPosition);
                }
            }
        }
    }

    // Adds a new local position to our history and logs it.
    private void SaveNewPosition(Vector3 localPosition)
    {
        savedPositions.Add(localPosition);

        Debug.Log($"New *local* position saved: {localPosition}. Total count: {savedPositions.Count}");
    }

    // Method to move the object to the previous saved position
    public void MoveToPreviousSavedPosition()
    {
        if (mazeAnchor == null)
        {
            Debug.LogError("Maze Anchor is not set on the PositionTracker! Cannot restore relative position.", this.gameObject);
            return;
        }

        // We can't go back if there is only 1 (or 0) position saved.
        if (savedPositions.Count <= 1)
        {
            Debug.LogWarning("Not enough positions to go back to.");
            return;
        }

        // Remove the very last position from the list.
        savedPositions.RemoveAt(savedPositions.Count - 1);

        // Get the new last local position from the list
        Vector3 targetLocalPosition = savedPositions[savedPositions.Count - 1];

        // Calculate the current world position based on the maze's current location
        Vector3 targetWorldPosition = mazeAnchor.TransformPoint(targetLocalPosition);

        // Move the object to that new world position
        transform.position = targetWorldPosition;

        Debug.Log($"Moved to {targetWorldPosition} (local: {targetLocalPosition}). Checkpoints remaining: {savedPositions.Count}");
    }
}