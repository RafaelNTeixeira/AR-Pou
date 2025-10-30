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
    public float energyDecay = 0.3f;
    public float cleanlinessDecay = 0.5f;

    [Tooltip("Multiplier for energy decay when it's night.")]
    public float nightDecayMultiplier = 2f; // e.g., 2x faster

    [Header("Illness parameters")]
    public float sickThreshold = 40f;
    public float sicknessRate = 0.3f;

    [Header("Sphere references")]
    public Transform hungerSphere; 
    public Transform energySphere; 
    public Transform healthSphere;
    public Transform cleanlinessSphere;

    [Header("Sphere settings")]
    public float sphereHeight = 0.35f;
    public float sphereSpacing = 0.08f;
    public bool faceCamera = true;

    [Header("Pou Face Materials")]
    public Material Dirty;
    public Material DirtyTired;
    public Material HappyFace;
    public Material OkayFace;
    public Material SadFace;
    public Material SickDirtyFace;
    public Material SickDirtyTiredFace;
    public Material SickFace;
    public Material SickTiredFace;
    public Material TiredFace;

    private Transform statusHolder;

    private Transform mainCameraTransform;
    public Renderer pouRenderer;

    // Last-state trackers to know when to update the material
    private string lastMood;
    private bool lastIsDirty;
    private bool lastIsSick;
    private bool lastIsTired;

    void Start()
{
    if (Camera.main != null)
        mainCameraTransform = Camera.main.transform;

    if (pouRenderer == null)
        Debug.LogWarning("Pou Renderer not assigned! Please assign it in the Inspector.");

        lastMood = pouMood;
        lastIsDirty = cleanliness < 50f;
        lastIsSick = health < 50f;
        lastIsTired = energy < 50f;

        UpdateTexture();
    }

    void Update()
    {
        UpdateNeeds(Time.deltaTime);
        UpdateSphereColors();
        UpdateMood();
        HandleCameraFacing();

        // Recompute current booleans
        bool isDirty = cleanliness < 50f;
        bool isSick = health < 50f;
        bool isTired = energy < 50f;

        // Update texture if any of the relevant state values changed (mood, dirty, sick, tired)
        if (pouMood != lastMood || isDirty != lastIsDirty || isSick != lastIsSick || isTired != lastIsTired)
        {
            UpdateTexture();
            lastMood = pouMood;
            lastIsDirty = isDirty;
            lastIsSick = isSick;
            lastIsTired = isTired;
        }
    }

    // Decrease needs over time
    void UpdateNeeds(float dt)
    {
        hunger = Mathf.Clamp(hunger - hungerDecay * dt, 0, 100);
        cleanliness = Mathf.Clamp(cleanliness - cleanlinessDecay * dt, 0, 100);

        float currentEnergyDecay = energyDecay;

        if (WeatherManager.Instance != null && WeatherManager.Instance.isNight)
        {
            // If it is night, apply the multiplier to consume more energy.
            currentEnergyDecay *= nightDecayMultiplier;
        }

        energy = Mathf.Clamp(energy - currentEnergyDecay * dt, 0, 100);

        // Health decreases if hunger or cleanliness are below the sick threshold
        if (hunger < sickThreshold || cleanliness < sickThreshold)
        {
            health = Mathf.Clamp(health - sicknessRate * dt, 0, 100);
        }
    }

    // Update the colors of the spheres based on current values
    void UpdateSphereColors()
    {
        if (hungerSphere) hungerSphere.GetComponent<Renderer>().material.color = ValueToColor(hunger);
        if (energySphere) energySphere.GetComponent<Renderer>().material.color = ValueToColor(energy);
        if (healthSphere) healthSphere.GetComponent<Renderer>().material.color = ValueToColor(health);
        if (cleanlinessSphere) cleanlinessSphere.GetComponent<Renderer>().material.color = ValueToColor(cleanliness);
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

        if (average >= 75f) pouMood = "Happy";
        else if (average >= 50f) pouMood = "Okay";
        else  pouMood = "Sad";
    }

    // Update the Pou's texture based on mood and cleanliness
    void UpdateTexture()
    {
        if (pouRenderer == null) return;

        Material newMaterial = null;

        // Choose texture based on cleanliness and mood
        bool isDirty = cleanliness < 50f;
        bool isSick = health < 50f;
        bool isTired = energy < 50f;

        // Choose material based on condition combinations
        if (isDirty && isSick && isTired) newMaterial = SickDirtyTiredFace;
        else if (isDirty && isSick) newMaterial = SickDirtyFace;
        else if (isSick && isTired) newMaterial = SickTiredFace;
        else if (isSick) newMaterial = SickFace;
        else if (isDirty && isTired) newMaterial = DirtyTired;
        else if (isDirty) newMaterial = Dirty;
        else if (isTired) newMaterial = TiredFace;
        else
        {
            // Mood based faces when none of the above special conditions apply
            switch (pouMood)
            {
                case "Happy": newMaterial = HappyFace; break;
                case "Okay": newMaterial = OkayFace; break;
                case "Sad": newMaterial = SadFace; break;
                default: newMaterial = OkayFace; break;
            }
        }

        // Apply material only if changed
        if (newMaterial != null && pouRenderer.material != newMaterial)
        {
            pouRenderer.material = newMaterial;
        }
    }
    
    // Rotates each sphere to look at the camera
    void HandleCameraFacing()
    {
        if (!faceCamera || mainCameraTransform == null)
        {
            return;
        }

        // Make the spheres tilt towards the camera
        if (hungerSphere) hungerSphere.LookAt(mainCameraTransform);
        if (energySphere) energySphere.LookAt(mainCameraTransform);
        if (healthSphere) healthSphere.LookAt(mainCameraTransform);
        if (cleanlinessSphere) cleanlinessSphere.LookAt(mainCameraTransform);


        /* 
        // ALTERNATIVE (Upright)
        // Get the camera's position
        Vector3 camPos = mainCameraTransform.position;

        // This version keeps the icons "upright" (no vertical tilt) by having them look at a point at their own height.
        if (hungerSphere)
        {
            Vector3 lookPos = new Vector3(camPos.x, hungerSphere.position.y, camPos.z);
            hungerSphere.LookAt(lookPos);
        }
        if (energySphere)
        {
            Vector3 lookPos = new Vector3(camPos.x, energySphere.position.y, camPos.z);
            energySphere.LookAt(lookPos);
        }
        if (healthSphere)
        {
            Vector3 lookPos = new Vector3(camPos.x, healthSphere.position.y, camPos.z);
            healthSphere.LookAt(lookPos);
        }
        if (cleanlinessSphere)
        {
            Vector3 lookPos = new Vector3(camPos.x, cleanlinessSphere.position.y, camPos.z);
            cleanlinessSphere.LookAt(lookPos);
        }
        */
    }
}
