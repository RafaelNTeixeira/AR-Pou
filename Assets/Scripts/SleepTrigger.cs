using UnityEngine;

public class SleepTrigger : MonoBehaviour
{
    [Header("Sleep Properties")]
    [SerializeField] private float sleepValue = 20f;
    
    private bool hasBeenSleept = false;
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
        if (hasBeenSleept)
        {
            // Just hide, don't destroy
            //gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pou") && !hasBeenSleept)
        {
            ShoweredPou(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            ResetSleep();
        }
    }

    private void ShoweredPou(GameObject pou)
    {
        if (hasBeenSleept) return;
        
        hasBeenSleept = true;
        
        // Clean the Pou
        PouStatus pouStatus = pou.GetComponent<PouStatus>();
        if (pouStatus != null)
        {
            pouStatus.Sleep(sleepValue);
            Debug.Log($"Pou sleept {gameObject.name}, Sleep increased by {sleepValue}");
        }
        else
        {
            Debug.LogWarning("PouStatus component not found on Pou!");
        }

        // Hide and disable the food immediately
        SetVisible(false);

        // Start timer for final cleanup
        hasBeenSleept = false;
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
    public void ResetSleep()
    {
        hasBeenSleept = false;
        SetVisible(true);
    }
}