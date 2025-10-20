using UnityEngine;

public class PouStatus : MonoBehaviour
{
    [Header("Pou Needs (0 = bad, 100 = full)")]
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float energy = 100f;
    [Range(0, 100)] public float health = 100f;
    [Range(0, 100)] public float cleanliness = 100f;
    public string pouMood = "Happy";

    [Header("Decay rates (points per second)")]
    public float hungerDecay = 1f;
    public float energyDecay = 0.5f;
    public float cleanlinessDecay = 0.3f;

    [Header("Illness parameters")]
    public float sickThreshold = 40f;
    public float sicknessRate = 0.2f;

    [Header("Sphere references")]
    public Renderer hungerSphere;
    public Renderer energySphere;
    public Renderer healthSphere;
    public Renderer cleanlinessSphere;

    [Header("Sphere settings")]
    public float sphereHeight = 0.35f;
    public float sphereSpacing = 0.08f;
    public bool faceCamera = true;

    private Transform statusHolder;

    void Update()
    {
        UpdateNeeds(Time.deltaTime);
        UpdateSphereColors();
        UpdateMood();

        // Make the marker always face the camera
        // if (faceCamera && Camera.main != null)
        //     statusHolder.LookAt(Camera.main.transform);
    }

    // Decrease needs over time
    void UpdateNeeds(float dt)
    {
        hunger = Mathf.Clamp(hunger - hungerDecay * dt, 0, 100);
        energy = Mathf.Clamp(energy - energyDecay * dt, 0, 100);
        cleanliness = Mathf.Clamp(cleanliness - cleanlinessDecay * dt, 0, 100);

        // Health decreases if hunger or cleanliness are below the sick threshold
        if (hunger < sickThreshold || cleanliness < sickThreshold)
        {
            health = Mathf.Clamp(health - sicknessRate * dt, 0, 100);
        }
    }

    // Update the colors of the spheres based on current values
    void UpdateSphereColors()
    {
        if (hungerSphere) hungerSphere.material.color = ValueToColor(hunger);
        if (energySphere) energySphere.material.color = ValueToColor(energy);
        if (healthSphere) healthSphere.material.color = ValueToColor(health);
        if (cleanlinessSphere) cleanlinessSphere.material.color = ValueToColor(cleanliness);
    }

    // Convert a value (0-100) to a color from red to green
    Color ValueToColor(float value)
    {
        // 100 = green, 0 = red
        return Color.Lerp(Color.red, Color.green, value / 100f);
    }

    // Public methods to modify Pou's needs
    public void Feed(float amount) => hunger = Mathf.Clamp(hunger + amount, 0, 100);
    public void Sleep(float amount) => energy = Mathf.Clamp(energy + amount, 0, 100);
    public void Clean(float amount) => cleanliness = Mathf.Clamp(cleanliness + amount, 0, 100);
    public void Heal(float amount) => health = Mathf.Clamp(health + amount, 0, 100);

    void UpdateMood()
    {
        float average = (hunger + energy + health + cleanliness) / 4f;
        
        if (average >= 85f) pouMood = "Happy";
        else if (average >= 65f) pouMood = "Okay";
        else if (average >= 40f) pouMood = "Sad";
        else if (average >= 20f) pouMood = "Sick";
        else pouMood = "Depressed";
    }
}
