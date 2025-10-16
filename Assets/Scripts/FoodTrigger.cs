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
        // print the object name to the console when an object enters the trigger
        Debug.Log("Objeto entrou no trigger: " + other.gameObject.name);

        if (other.CompareTag("Pou"))
        {
            if (!isInsidePou)
            {
                isInsidePou = true;
                Debug.Log("Pizza entrou em contacto com Pou — a fatia desaparece!");
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
                Debug.Log("Pizza saiu do contacto com Pou — a fatia reaparece!");
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
