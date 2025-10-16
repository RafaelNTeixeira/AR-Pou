using UnityEngine;

public class FoodTrigger : MonoBehaviour
{
    private bool isInsidePou = false;
    private Renderer[] renderers; // Array to hold all renderers of the food item

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Pou"))
        {
            if (!isInsidePou)
            {
                isInsidePou = true;
                SetVisible(false); // Since the object is a prefab, we disable renderers instead of the whole GameObject. Otherwise, it would cause the object from disappearing entirely from the AR scene.
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            if (isInsidePou)
            {
                isInsidePou = false;
                gameObject.SetActive(true);
                SetVisible(true);
            }
        }
    }

    private void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }
}
