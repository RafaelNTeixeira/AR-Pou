using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class CurrentConditions
{
    public string conditions;
    public string datetime; // "HH:M:SS"
}

[System.Serializable]
public class WeatherResponse
{
    public CurrentConditions currentConditions;
}

public class WeatherManager : MonoBehaviour
{
    // Singleton Pattern
    public static WeatherManager Instance { get; private set; }

    // Any script can subscribe to this without needing a reference to the specific WeatherManager object.
    public static event Action OnWeatherUpdated;

    [Header("Visual Crossing Settings")]
    public string apiKey = "NMWLG82ZH5335NBB3KZXWFNDJ";
    public string city = "Porto";

    [Header("Update Frequency")]
    public float updateIntervalSeconds = 300f; // Every 5 minutes

    [Header("Weather Status")]
    public bool isRaining;
    public bool isSunny;
    public bool isSnowing;
    public bool isCloudy;
    public bool isNight;

    [Header("Scene Lighting")]
    [Tooltip("The main directional light in the scene (e.g., the 'Sun').")]
    public Light mainDirectionalLight;
    
    [Header("Day Lighting")]
    [ColorUsage(false, false)]
    public Color dayAmbientColor = new Color(0.8f, 0.8f, 0.8f);
    [Range(0, 8)]
    public float dayAmbientIntensity = 1f;
    [Range(0, 8)]
    public float dayLightIntensity = 1f;

    [Header("Night Lighting")]
    [ColorUsage(false, false)]
    public Color nightAmbientColor = new Color(0.2f, 0.2f, 0.3f);
    [Range(0, 8)]
    public float nightAmbientIntensity = 0.3f;
    [Range(0, 8)]
    public float nightLightIntensity = 0.1f;

    [Header("Debug Mode")]
    [Tooltip("If true, the live API will not be called. Instead, you can force weather states below.")]
    public bool useDebugMode = false;
    
    [Header("Debug Weather States")]
    public bool forceRaining;
    public bool forceSunny;
    public bool forceSnowing;
    public bool forceCloudy;
    public bool forceNight;

    void Awake()
    {
        // Standard singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateSceneLighting(); 
        
        // Start a loop that will repeatedly call GetWeather.
        StartCoroutine(WeatherUpdateLoop());
    }
    
    IEnumerator WeatherUpdateLoop()
    {
        while (true)
        {
            // If in debug mode, run the debug logic. Otherwise, fetch the real weather.
            if (useDebugMode)
            {
                // This function will apply your forced states
                ApplyDebugWeather();
                // Wait 1 second before checking again
                yield return new WaitForSeconds(1f); 
            }
            else
            {
                // This is the normal, original behavior
                Debug.Log("WeatherManager: Fetching new weather data...");
                yield return StartCoroutine(GetWeather());
                
                Debug.Log($"WeatherManager: Waiting {updateIntervalSeconds} seconds for next update.");
                yield return new WaitForSeconds(updateIntervalSeconds);
            }
        }
    }

    // Function to apply debug weather states
    void ApplyDebugWeather()
    {
        // Check if any debug value has changed since the last check
        bool stateChanged =
            isRaining != forceRaining ||
            isSnowing != forceSnowing ||
            isSunny != forceSunny ||
            isCloudy != forceCloudy ||
            isNight != forceNight;

        if (stateChanged)
        {
            Debug.LogWarning("--- DEBUG MODE: Forcing new weather state ---");
            
            // Apply the forced values to the actual state variables
            isRaining = forceRaining;
            isSnowing = forceSnowing;
            isSunny = forceSunny;
            isCloudy = forceCloudy;
            isNight = forceNight;

            Debug.Log($"Weather flags — Rain: {isRaining}, Snow: {isSnowing}, Sunny: {isSunny}, Cloudy: {isCloudy}, Night: {isNight}");

            UpdateSceneLighting();

            // Fire the event to make Pou change his outfit!
            OnWeatherUpdated?.Invoke();
        }
    }

    IEnumerator GetWeather()
    {
        if (useDebugMode) yield break;

        string url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}?unitGroup=metric&key={apiKey}&contentType=json";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // ... (The rest of your GetWeather function is identical) ...
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Weather API error: " + www.error);
            }
            else
            {
                string jsonText = www.downloadHandler.text;
                WeatherResponse weather = JsonUtility.FromJson<WeatherResponse>(jsonText);

                if (weather != null && weather.currentConditions != null)
                {
                    string conditions = weather.currentConditions.conditions;
                    string datetime = weather.currentConditions.datetime;
                    Debug.Log("Current conditions: " + conditions);

                    // Reset all
                    isRaining = isSunny = isSnowing = isCloudy = isNight = false;

                    // Detect night based on hour
                    if (!string.IsNullOrEmpty(datetime))
                    {
                        try
                        {
                            DateTime parsedTime = DateTime.Parse(datetime);
                            int hour = parsedTime.Hour;

                            if (hour >= 20 || hour < 6)
                                isNight = true;
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Could not parse datetime: " + e.Message);
                        }
                    }

                    // Set weather flags
                    string c = conditions.ToLower();
                    if (c.Contains("rain"))
                        isRaining = true;
                    else if (c.Contains("snow"))
                        isSnowing = true;
                    else if (c.Contains("clear"))
                        isSunny = true;
                    else if (c.Contains("cloud"))
                        isCloudy = true;

                    Debug.Log($"Weather flags — Rain: {isRaining}, Snow: {isSnowing}, Sunny: {isSunny}, Cloudy: {isCloudy}, Night: {isNight}");

                    UpdateSceneLighting();

                    // Notify all subscribers that the weather has been updated
                    OnWeatherUpdated?.Invoke();
                }
                else
                {
                    Debug.LogWarning("Unable to parse weather data.");
                }
            }
        }
    }
    
    // Updates the scene's ambient and directional light based on the isNight flag
    void UpdateSceneLighting()
    {
        if (isNight)
        {
            // Set scene lighting to night values
            RenderSettings.ambientLight = nightAmbientColor;
            RenderSettings.ambientIntensity = nightAmbientIntensity;
            if (mainDirectionalLight != null)
            {
                mainDirectionalLight.intensity = nightLightIntensity;
            }
        }
        else
        {
            // Set scene lighting to day values
            RenderSettings.ambientLight = dayAmbientColor;
            RenderSettings.ambientIntensity = dayAmbientIntensity;
            if (mainDirectionalLight != null)
            {
                mainDirectionalLight.intensity = dayLightIntensity;
            }
        }
    }
}