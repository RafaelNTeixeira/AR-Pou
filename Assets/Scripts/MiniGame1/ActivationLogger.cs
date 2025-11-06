using UnityEngine;

// Class to log activation and deactivation of the GameObject
public class ActivationLogger : MonoBehaviour
{
    [Header("Confirmation Message")]
    [Tooltip("Type a unique message here to be sure this is the correct object.")]
    public string myMessage = "The MAIN sunglasses object was just ENABLED!";

    // Log message when the GameObject is enabled
    void OnEnable()
    {
        Debug.Log(myMessage, this);
    }

    // Log message when the GameObject is disabled
    void OnDisable()
    {
        Debug.Log(gameObject.name + " was just DISABLED.", this);
    }
}