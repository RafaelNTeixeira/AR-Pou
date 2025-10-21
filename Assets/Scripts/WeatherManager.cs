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
    [Header("Visual Crossing Settings")]
    public string apiKey = "NMWLG82ZH5335NBB3KZXWFNDJ";
    public string city = "Porto"; // use GPS later

    [Header("Weather Status")]
    public bool isRaining;
    public bool isSunny;
    public bool isSnowing;
    public bool isCloudy;
    public bool isNight;

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        string url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}?unitGroup=metric&key={apiKey}&contentType=json";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
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
                }
                else
                {
                    Debug.LogWarning("Unable to parse weather data.");
                }
            }
        }
    }
}
