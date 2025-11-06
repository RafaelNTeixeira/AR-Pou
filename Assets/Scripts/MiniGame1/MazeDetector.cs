using UnityEngine;

// Class to detect when Pou collides with a maze wall and push him back
public class MazeDetector : MonoBehaviour
{
    [Tooltip("How far the box gets pushed back when it hits the maze.")]
    public float knockbackDistance = 0.001f;
    private Collider myCollider;

    void Start()
    {
        // Get this object's collider and store it
        myCollider = GetComponent<Collider>();
        if (myCollider == null)
        {
            Debug.LogError("MazeDetector script needs a Collider component on this object!");
        }
    }

    // When we enter a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit the "Maze"
        if (other.gameObject.CompareTag("Maze"))
        {
            Debug.Log("The box has entered the maze! Pushing back...");

            // We need to calculate the overlap (penetration) between our collider (myCollider) and the maze's collider (other).
            Vector3 pushDirection;
            float penetrationDepth;

            // This function calculates the direction and distance to "un-stick" the two colliders. It returns true if they are overlapping.
            bool isOverlapping = Physics.ComputePenetration(
                myCollider,           // Our box's collider
                transform.position,   // Our box's position
                transform.rotation,   // Our box's rotation
                other,                // The maze's collider
                other.transform.position, // The maze's position
                other.transform.rotation, // The maze's rotation
                out pushDirection,    // The direction to push
                out penetrationDepth  // The distance they are overlapped
            );

            // If we are successfully overlapping...
            if (isOverlapping)
            {
                // pushDirection is the vector pointing away from the maze wall.
                // We multiply this direction by our desired knockback distance to move the box.
                transform.position += pushDirection * knockbackDistance * 0.5f;
            }
            else
            {
                // Fallback in case ComputePenetration fails (rare)
                Debug.LogWarning("Could not compute penetration. Using fallback.");
                Vector3 fallbackDirection = (transform.position - other.transform.position).normalized;
                transform.position += fallbackDirection * knockbackDistance * 0.5f;
            }
        }
    }

    // When we exit a trigger collider
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Maze"))
        {
            Debug.Log("The box has left the maze.");
        }
    }
}