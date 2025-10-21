using UnityEngine;

public class ShowerTrigger : MonoBehaviour
{
    [Header("Shower Properties")]
    [SerializeField] private float showerValue = 20f;
    
    private bool hasBeenShowered = false;
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
        if (hasBeenShowered)
        {
            // Just hide, don't destroy
            //gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pou") && !hasBeenShowered)
        {
            ShoweredPou(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            ResetShower();
        }
    }

    private void ShoweredPou(GameObject pou)
    {
        if (hasBeenShowered) return;
        
        hasBeenShowered = true;
        
        // Clean the Pou
        PouStatus pouStatus = pou.GetComponent<PouStatus>();
        if (pouStatus != null)
        {
            pouStatus.Clean(showerValue);
            Debug.Log($"Pou showered {gameObject.name}, Cleaned increased by {showerValue}");
        }
        else
        {
            Debug.LogWarning("PouStatus component not found on Pou!");
        }

        // Hide and disable the food immediately
        SetVisible(false);

        // Start timer for final cleanup
        hasBeenShowered = false;
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
    public void ResetShower()
    {
        hasBeenShowered = false;
        SetVisible(true);
    }
}