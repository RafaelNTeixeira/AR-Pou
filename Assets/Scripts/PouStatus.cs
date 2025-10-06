using UnityEngine;
using System.Collections;

public class PouStatus : MonoBehaviour
{
    [Header("Pou Needs (0 = bad, 100 = full)")]
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float energy = 100f;
    [Range(0, 100)] public float health = 100f;
    [Range(0, 100)] public float cleanliness = 100f;

    [Header("Decay rates (points per second)")]
    public float hungerDecay = 1f;
    public float energyDecay = 0.5f;
    public float cleanlinessDecay = 0.3f;

    [Header("Illness parameters")]
    public float sickThreshold = 40f; // Below this, Pou may get sick
    public float sicknessRate = 0.2f; // Health decay per second when sick

    [Header("Status flags (read-only)")]
    public bool isHungry;
    public bool isSleepy;
    public bool isDirty;
    public bool isSick;

    void Update()
    {
        UpdateNeeds(Time.deltaTime);
        UpdateStatusFlags();
    }

    void UpdateNeeds(float deltaTime)
    {
        // Gradually decrease the stats
        hunger -= hungerDecay * deltaTime;
        energy -= energyDecay * deltaTime;
        cleanliness -= cleanlinessDecay * deltaTime;

        hunger = Mathf.Clamp(hunger, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);
        cleanliness = Mathf.Clamp(cleanliness, 0, 100);

        // If Pou is unhealthy (hunger or cleanliness too low), he gets sick. 
        // TEMP: May need updates later
        if (hunger < sickThreshold || cleanliness < sickThreshold)
        {
            health -= sicknessRate * deltaTime;
            isSick = true;
        }
        else
        {
            isSick = false;
        }

        health = Mathf.Clamp(health, 0, 100);
    }

    void UpdateStatusFlags()
    {
        isHungry = hunger < 50;
        isSleepy = energy < 50;
        isDirty = cleanliness < 50;
    }

    // Methods to interact with Pou and improve his stats
    public void Feed(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0, 100);
    }

    public void Sleep(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0, 100);
    }

    public void Clean(float amount)
    {
        cleanliness = Mathf.Clamp(cleanliness + amount, 0, 100);
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, 100);
    }

    // Debug
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Hunger: {hunger:F1}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Energy: {energy:F1}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Health: {health:F1}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Cleanliness: {cleanliness:F1}");
    }
}
