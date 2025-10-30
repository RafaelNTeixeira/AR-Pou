using UnityEngine;
using TMPro;

public class PouOutfitManager : MonoBehaviour
{
    [Header("Outfit GameObjects")]
    public GameObject rainHat;
    public GameObject snowBeanie;
    public GameObject sunglasses;

    [Header("UI Reference")]
    [Tooltip("The World Space Canvas that holds the speech bubble.")]
    public Canvas pouCanvas;

    [Tooltip("The TextMeshPro UI element that will show Pou's needs.")]
    public TextMeshProUGUI pouSpeechBubbleText;

    void OnEnable()
    {
        // Check if our canvas reference is set and if it's missing its camera
        if (pouCanvas != null && pouCanvas.worldCamera == null)
        {
            // Find the main camera in the scene and assign it
            pouCanvas.worldCamera = Camera.main;

            if (pouCanvas.worldCamera == null)
            {
                Debug.LogError("PouOutfitManager: Could not find a 'MainCamera' tagged camera to assign to the World Space Canvas!");
            }
        }

        // Subscribe our new check method to the weather event
        WeatherManager.OnWeatherUpdated += UpdateOutfitStatusMessage;

        // Check the weather immediately on spawn
        if (WeatherManager.Instance != null)
        {
            UpdateOutfitStatusMessage();
        }
        else
        {
            Debug.LogWarning("PouOutfitManager: WeatherManager not found on spawn. Can't update status message.");
        }
    }

    void OnDisable()
    {
        WeatherManager.OnWeatherUpdated -= UpdateOutfitStatusMessage;
    }

    /// <summary>
    /// Checks weather against current outfit and shows/hides the speech bubble.
    /// </summary>
    void UpdateOutfitStatusMessage()
    {
        if (WeatherManager.Instance == null || pouSpeechBubbleText == null)
        {
            return; // Not ready, do nothing
        }

        // Get current weather needs
        bool needsRainHat = WeatherManager.Instance.isRaining;
        bool needsSnowBeanie = WeatherManager.Instance.isSnowing;
        bool needsSunglasses = WeatherManager.Instance.isSunny && !WeatherManager.Instance.isNight;

        // Get current outfit status
        bool wearingRainHat = rainHat != null && rainHat.activeSelf;
        bool wearingSnowBeanie = snowBeanie != null && snowBeanie.activeSelf;
        bool wearingSunglasses = sunglasses != null && sunglasses.activeSelf;

        // Logic to decide what to say
        if (needsRainHat && !wearingRainHat)
        {
            ShowSpeechBubble("It's raining! I need my rain hat!");
        }
        else if (needsSnowBeanie && !wearingSnowBeanie)
        {
            ShowSpeechBubble("Brrr! It's snowing! I need my beanie!");
        }
        else if (needsSunglasses && !wearingSunglasses)
        {
            ShowSpeechBubble("It's so bright! I need my sunglasses!");
        }
        else
        {
            // No mismatch, so hide the bubble
            HideSpeechBubble();
        }
    }

    private void ShowSpeechBubble(string message)
    {
        if (pouSpeechBubbleText == null) return;
        
        pouSpeechBubbleText.text = message;
        pouSpeechBubbleText.gameObject.SetActive(true);
    }

    private void HideSpeechBubble()
    {
        if (pouSpeechBubbleText == null) return;

        pouSpeechBubbleText.text = "";
        pouSpeechBubbleText.gameObject.SetActive(false);
    }

    // Deactivates all outfit items. Called before equipping a new one to ensure no overlap.
    private void UnequipAll()
    {
        rainHat?.SetActive(false);
        snowBeanie?.SetActive(false);
        sunglasses?.SetActive(false);

        UpdateOutfitStatusMessage();
    }

    // Tries to equip the rain hat. Only succeeds if it is currently raining.
    public bool TryEquipRainHat()
    {
        if (WeatherManager.Instance == null) return false;

        if (WeatherManager.Instance.isRaining)
        {
            UnequipAll(); // Take off other items
            rainHat?.SetActive(true);
            Debug.Log("Pou put on the rain hat.");


            UpdateOutfitStatusMessage();
            return true; 
        }
        
        Debug.Log("Pou can't wear the rain hat! It's not raining.");

        UpdateOutfitStatusMessage();
        return false; 
    }

    // Tries to equip the snow beanie. Only succeeds if it is currently snowing.
    public bool TryEquipSnowBeanie()
    {
        if (WeatherManager.Instance == null) return false;

        if (WeatherManager.Instance.isSnowing)
        {
            UnequipAll();
            snowBeanie?.SetActive(true);
            Debug.Log("Pou put on the snow beanie.");


            UpdateOutfitStatusMessage();
            return true;
        }

        Debug.Log("Pou can't wear the snow beanie! It's not snowing.");

        UpdateOutfitStatusMessage();
        return false;
    }

    // Tries to equip sunglasses. Only succeeds if it is sunny AND not night.
    public bool TryEquipSunglasses()
    {
        if (WeatherManager.Instance == null) return false;

        if (WeatherManager.Instance.isSunny && !WeatherManager.Instance.isNight)
        {
            UnequipAll();
            sunglasses?.SetActive(true);
            Debug.Log("Pou put on the sunglasses.");

            UpdateOutfitStatusMessage();
            return true;
        }

        Debug.Log("Pou can't wear sunglasses! It's not sunny or it's night.");

        UpdateOutfitStatusMessage();
        return false;
    }
}