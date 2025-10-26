using UnityEngine;

public class PouReceiver : MonoBehaviour
{
    private Minigame2Manager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<Minigame2Manager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        int objectIndex = -1;
        
        // Identify object type by tag
        if (other.CompareTag("Food")) objectIndex = 0;
        else if (other.CompareTag("Bed")) objectIndex = 1;
        else if (other.CompareTag("Pill")) objectIndex = 2;
        
        // If a recognized object was detected
        if (objectIndex != -1)
        {
            // Notify game manager
            gameManager.ObjectDelivered(objectIndex);

            // Hide the object (don't destroy for AR compatibility)
            SetObjectVisible(other.gameObject, false);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Reset object visibility when it leaves Pou's area
        if (other.CompareTag("Food") || other.CompareTag("Bed") || other.CompareTag("Pill"))
        {
            SetObjectVisible(other.gameObject, true);
        }
    }
    
    private void SetObjectVisible(GameObject obj, bool visible)
    {
        if (obj == null) return;
        
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }
}