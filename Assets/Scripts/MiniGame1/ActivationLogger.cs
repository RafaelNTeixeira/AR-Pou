using UnityEngine;

// ATTACH THIS SCRIPT TO YOUR 'sunglasses' OBJECT
public class ActivationLogger : MonoBehaviour
{
    [Header("Confirmation Message")]
    [Tooltip("Type a unique message here to be sure this is the correct object.")]
    public string myMessage = "The MAIN sunglasses object was just ENABLED!";

    // This is the built-in Unity function that runs
    // ONLY when the GameObject is set from inactive to active.
    void OnEnable()
    {
        // This prints your message to the Unity Console.
        // The 'this' at the end makes the message clickable,
        // so it will highlight the 'sunglasses' object in the Hierarchy.
        Debug.Log(myMessage, this);
    }

    // You can also use this to confirm when it's hidden
    void OnDisable()
    {
        Debug.Log(gameObject.name + " was just DISABLED.", this);
    }
}