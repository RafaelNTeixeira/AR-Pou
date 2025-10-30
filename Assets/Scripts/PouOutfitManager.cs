using UnityEngine;

public class PouOutfitManager : MonoBehaviour
{
    [Header("Outfit GameObjects")]
    public GameObject rainHat;
    public GameObject snowBeanie;
    public GameObject sunglasses;


    // OnEnable runs every time the prefab is instantiated or set to active.
    void OnEnable()
    {
        // Subscribe our 'UpdateOutfit' method to the static event.
        WeatherManager.OnWeatherUpdated += UpdateOutfit;

        // We must also check the weather *immediately* when Pou spawns, in case we missed the last event.
        // We check if 'Instance' is not null, which it should be if WeatherManager is in the scene.
        if (WeatherManager.Instance != null)
        {
            UpdateOutfit();
        }
        else
        {
            Debug.LogWarning("PouOutfitManager: Could not find WeatherManager.Instance on spawn. Outfits may not be correct.");
        }
    }

    // OnDisable runs when the object is disabled (e.g., marker lost) or destroyed.
    void OnDisable()
    {
        // Always unsubscribe from static events to prevent errors when the object is destroyed.
        WeatherManager.OnWeatherUpdated -= UpdateOutfit;
    }

    void UpdateOutfit()
    {
        // Check if the WeatherManager singleton instance exists
        if (WeatherManager.Instance == null)
        {
            Debug.LogWarning("PouOutfitManager: WeatherManager.Instance is not ready. Skipping outfit update.");
            return;
        }

        // Deactivate all outfits by default
        rainHat?.SetActive(false);
        snowBeanie?.SetActive(false);
        sunglasses?.SetActive(false);

        // Activate outfits based on current weather conditions
        if (WeatherManager.Instance.isRaining)
        {
            rainHat?.SetActive(true);
        }
        else if (WeatherManager.Instance.isSnowing)
        {
            snowBeanie?.SetActive(true);
        }
        else if (WeatherManager.Instance.isSunny && !WeatherManager.Instance.isNight)
        {
            // Only wear sunglasses if it's sunny and it's not night
            sunglasses?.SetActive(true);
        }
    }
}