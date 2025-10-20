using UnityEngine;

public class FoodTrigger : MonoBehaviour
{
    [Header("Food Properties")]
    [SerializeField] private float hungerValue = 20f;
    
    private bool hasBeenConsumed = false;
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
        if (hasBeenConsumed)
        {
            // Just hide, don't destroy
            //gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pou") && !hasBeenConsumed)
        {
            ConsumeFoodByPou(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            ResetFood();
        }
    }

    private void ConsumeFoodByPou(GameObject pou)
    {
        if (hasBeenConsumed) return;
        
        hasBeenConsumed = true;
        
        // Feed the Pou
        PouStatus pouStatus = pou.GetComponent<PouStatus>();
        if (pouStatus != null)
        {
            pouStatus.Feed(hungerValue);
            Debug.Log($"Pou ate {gameObject.name}, hunger increased by {hungerValue}");
        }
        else
        {
            Debug.LogWarning("PouStatus component not found on Pou!");
        }

        // Hide and disable the food immediately
        SetVisible(false);

        // Start timer for final cleanup
        hasBeenConsumed = false;
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
    public void ResetFood()
    {
        hasBeenConsumed = false;
        SetVisible(true);
    }
}