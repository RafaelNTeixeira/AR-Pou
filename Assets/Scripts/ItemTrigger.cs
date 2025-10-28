using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public enum ItemType
    {
        Food,
        Health,
        Shower,
        Sleep
    }

    [Header("Item Properties")]
    [SerializeField] private ItemType itemType = ItemType.Food;
    [SerializeField] private float itemValue = 20f;
    
    private bool hasBeenUsed = false;
    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pou") && !hasBeenUsed)
        {
            UsedByPou(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            ResetItem();
        }
    }

    private void UsedByPou(GameObject pou)
    {
        if (hasBeenUsed) return;
        
        hasBeenUsed = true;

        PouStatus pouStatus = pou.GetComponent<PouStatus>();
        PouAnimator pouAnimator = pou.GetComponent<PouAnimator>();
        if (pouStatus != null)
        {
            // Call appropriate method based on item type
            switch (itemType)
            {
                case ItemType.Food:
                    pouStatus.Feed(itemValue);
                    pouAnimator.PlayFeedAnimation();
                    Debug.Log($"Pou ate {gameObject.name}, hunger increased by {itemValue}");
                    break;
                    
                case ItemType.Health:
                    pouStatus.Heal(itemValue);
                    pouAnimator.PlayMedicineAnimation();
                    Debug.Log($"Pou used {gameObject.name}, health increased by {itemValue}");
                    break;
                    
                case ItemType.Shower:
                    pouStatus.Clean(itemValue);
                    pouAnimator.PlayCleanAnimation();
                    Debug.Log($"Pou showered with {gameObject.name}, cleanliness increased by {itemValue}");
                    break;
                    
                case ItemType.Sleep:
                    pouStatus.Sleep(itemValue);
                    pouAnimator.PlaySleepAnimation();
                    Debug.Log($"Pou slept on {gameObject.name}, energy increased by {itemValue}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("PouStatus component not found on Pou!");
        }

        // Hide the item immediately
        SetVisible(false);
        
        // Reset for next use
        hasBeenUsed = false;
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

    // Reset item to fresh state
    public void ResetItem()
    {
        hasBeenUsed = false;
        SetVisible(true);
    }
}