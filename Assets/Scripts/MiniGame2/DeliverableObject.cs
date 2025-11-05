using UnityEngine;

public class DeliverableObject : MonoBehaviour
{
    [Header("Object Properties")]
    [SerializeField] private int objectIndex; // 0=Pizza, 1=Bed, 2=Soap, 3=Pill
    
    private bool hasBeenDelivered = false;
    private Renderer[] renderers;
    private Minigame2Manager gameManager;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        gameManager = FindObjectOfType<Minigame2Manager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PouMinigame2") && !hasBeenDelivered)
        {
            DeliverToPou();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PouMinigame2"))
        {
            // Reset visibility when leaving Pou
            ResetObject();
        }
    }

    // Method to handle delivery to Pou
    private void DeliverToPou()
    {
        if (hasBeenDelivered) return;
        
        hasBeenDelivered = true;
        
        // Notify the game manager
        if (gameManager != null)
        {
            gameManager.StartCoroutine(gameManager.ObjectDelivered(objectIndex));
            Debug.Log($"Delivered object {objectIndex} ({gameObject.name}) to Pou");
        }
        else
        {
            Debug.LogWarning("Minigame2Manager not found!");
            return;
        }

        // Hide the object after delivery
        SetVisible(false);
    }

    // Helper method to set visibility
    private void SetVisible(bool visible)
    {
        if (renderers == null) return;

        foreach (var r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }

    // Method to reset the object for future deliveries
    public void ResetObject()
    {
        hasBeenDelivered = false;
        SetVisible(true);
    }
}
