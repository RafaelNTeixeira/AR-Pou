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
        if (other.CompareTag("Pou") && !hasBeenDelivered)
        {
            DeliverToPou();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            // Reset visibility when leaving Pou
            ResetObject();
        }
    }

    private void DeliverToPou()
    {
        if (hasBeenDelivered) return;
        
        hasBeenDelivered = true;
        
        // Notify game manager
        if (gameManager != null)
        {
            gameManager.ObjectDelivered(objectIndex);
            Debug.Log($"Delivered object {objectIndex} ({gameObject.name}) to Pou");
        }
        else
        {
            //Debug.LogWarning("Minigame2Manager not found!");
            return;
        }

        // Hide the object
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        // Toggle renderers
        if (renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r != null)
                    r.enabled = visible;
            }
        }
    }

    // Reset object to fresh state
    public void ResetObject()
    {
        hasBeenDelivered = false;
        SetVisible(true);
    }
}