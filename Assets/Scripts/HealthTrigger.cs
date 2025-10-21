using UnityEngine;

public class HealthTrigger : MonoBehaviour
{
    [Header("Health Properties")]
    [SerializeField] private float healthValue = 20f;
    
    private bool hasBeenCured = false;
    private Renderer[] renderers;
    //private Collider[] colliders;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        //colliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        // Reset food state whenever it becomes active
        //ResetFood();
    }

    private void Update()
    {
        // Handle delayed hiding
        if (hasBeenCured)
        {
            // Just hide, don't destroy
            //gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pou") && !hasBeenCured)
        {
            ShoweredPou(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            ResetHealth();
        }
    }

    private void ShoweredPou(GameObject pou)
    {
        if (hasBeenCured) return;
        
        hasBeenCured = true;
        
        // Clean the Pou
        PouStatus pouStatus = pou.GetComponent<PouStatus>();
        if (pouStatus != null)
        {
            pouStatus.Heal(healthValue);
            Debug.Log($"Pou cured {gameObject.name}, Health increased by {healthValue}");
        }
        else
        {
            Debug.LogWarning("PouStatus component not found on Pou!");
        }

        // Hide and disable the food immediately
        SetVisible(false);

        // Start timer for final cleanup
        hasBeenCured = false;
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null) return;
        
        foreach (var r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }

    // Reset food to fresh state
    public void ResetHealth()
    {
        hasBeenCured = false;
        SetVisible(true);
    }
}