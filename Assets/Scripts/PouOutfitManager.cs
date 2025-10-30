using UnityEngine;

public class PouOutfitManager : MonoBehaviour
{
    [Header("Outfit GameObjects")]
    public GameObject rainHat;
    public GameObject snowBeanie;
    public GameObject sunglasses;

    /// Deactivates all outfit items. Called before equipping a new one to ensure no overlap.
    private void UnequipAll()
    {
        rainHat?.SetActive(false);
        snowBeanie?.SetActive(false);
        sunglasses?.SetActive(false);
    }

    /// Tries to equip the rain hat. Only succeeds if it is currently raining.
    public bool TryEquipRainHat()
    {
        if (WeatherManager.Instance == null) return false;

        if (WeatherManager.Instance.isRaining)
        {
            UnequipAll(); // Take off other items
            rainHat?.SetActive(true);
            Debug.Log("Pou put on the rain hat.");
            return true; 
        }
        
        Debug.Log("Pou can't wear the rain hat! It's not raining.");
        return false; 
    }

    /// Tries to equip the snow beanie. Only succeeds if it is currently snowing.
    public bool TryEquipSnowBeanie()
    {
        if (WeatherManager.Instance == null) return false;

        // CHECK: Is it snowing?
        if (WeatherManager.Instance.isSnowing)
        {
            UnequipAll();
            snowBeanie?.SetActive(true);
            Debug.Log("Pou put on the snow beanie.");
            return true;
        }

        Debug.Log("Pou can't wear the snow beanie! It's not snowing.");
        return false;
    }

    /// Tries to equip sunglasses. Only succeeds if it is sunny AND not night.
    public bool TryEquipSunglasses()
    {
        if (WeatherManager.Instance == null) return false;

        // CHECK: Is it sunny and daytime?
        if (WeatherManager.Instance.isSunny && !WeatherManager.Instance.isNight)
        {
            UnequipAll();
            sunglasses?.SetActive(true);
            Debug.Log("Pou put on the sunglasses.");
            return true;
        }

        Debug.Log("Pou can't wear sunglasses! It's not sunny or it's night.");
        return false;
    }
}