using UnityEngine;

public class PizzaTrigger : MonoBehaviour
{
    public int test = 0;

    private void OnTriggerEnter(Collider other)
    {
        // print the object name to the console when an object enters the trigger
        Debug.Log("Objeto entrou no trigger: " + other.gameObject.name);

        if (other.CompareTag("Pou"))
        {
            Debug.Log("Pizza entrou em contacto com Pou — a fatia desaparece!");
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pou"))
        {
            Debug.Log("Pizza saiu do contacto com Pou — a fatia reaparece!");
            gameObject.SetActive(true);
        }
    }
}
