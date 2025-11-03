using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class MarkerObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct MarkerPrefab
    {
        public string markerName;
        public GameObject prefab;
        public Vector3 rotationOffset;
        public Vector3 translationOffset;
    }

    public MarkerPrefab[] markerPrefabs;

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    [Header("Optional Effects")]
    public GameObject instructionsPanelMinigame2;
    private bool hasShownMinigame2 = false;

    void Awake()
    {
        trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackablesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackablesChanged;
    }

    void ShowInstructionsMinigame2()
    {
        instructionsPanelMinigame2.SetActive(true);
        hasShownMinigame2 = true;
    }

    // Called when tracked images are added, updated, or removed
    void OnTrackablesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // When a new marker is detected
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnOrUpdatePrefab(trackedImage);
        }

        // When existing markers move or update
        foreach (var trackedImage in eventArgs.updated)
        {
            SpawnOrUpdatePrefab(trackedImage);
        }

        // When markers are lost
        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedPrefabs.TryGetValue(trackedImage.referenceImage.name, out var spawned))
            {
                spawned.SetActive(false);
            }
        }
    }

    // Spawn or update the prefab corresponding to the tracked image
    void SpawnOrUpdatePrefab(ARTrackedImage trackedImage)
    {
        string markerName = trackedImage.referenceImage.name;

        // Find corresponding prefab
        var prefabEntry = System.Array.Find(markerPrefabs, p => p.markerName == markerName);
        if (prefabEntry.prefab == null) return;

        GameObject spawned;
        if (!spawnedPrefabs.TryGetValue(markerName, out spawned))
        {
            spawned = Instantiate(prefabEntry.prefab, trackedImage.transform.position, trackedImage.transform.rotation);
            spawnedPrefabs[markerName] = spawned;
        }

        // Update position + visibility
        spawned.transform.SetPositionAndRotation(
            trackedImage.transform.position + trackedImage.transform.rotation * prefabEntry.translationOffset,
            trackedImage.transform.rotation * Quaternion.Euler(prefabEntry.rotationOffset)
        );

        // Activate only while tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            spawned.SetActive(true);
        }
        else
        {
            spawned.SetActive(false);
        }

        // When Minigame2Marker is detected, show instructions once
        if (prefabEntry.markerName == "Minigame2Marker" && !hasShownMinigame2)
        {
            Debug.Log("Minigame2Marker detected - showing instructions panel.");
            ShowInstructionsMinigame2();
        }
    }
}
