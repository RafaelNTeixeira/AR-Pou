using UnityEngine;
using TMPro;

// Class to display rising and fading emoji popups above Pou
public class EmojiPopup : MonoBehaviour
{
    public float riseHeight = 0.3f;
    public float lifetime = 1f;

    private TextMeshPro textMesh;
    private Transform cam;
    private Vector3 startPos;
    private float elapsedTime = 0f;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        cam = Camera.main.transform;
        startPos = transform.position - new Vector3(0f, riseHeight, 0f);
    }

    // Initialize the popup with the desired text
    public void Setup(string text)
    {
        textMesh.text = text;
    }

    void Update()
    {
        // Face the camera
        transform.LookAt(transform.position + cam.forward);

        // Increment elapsed time
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / lifetime);

        // Smooth upward movement
        transform.position = startPos + Vector3.up * riseHeight * t;

        // Fade out
        Color c = textMesh.color;
        c.a = 1f - t;  // fade proportional to time
        textMesh.color = c;

        // Destroy when lifetime ends
        if (elapsedTime >= lifetime)
            Destroy(gameObject);
    }
}
