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
            spawned = Instantiate(prefabEntry.prefab, trackedImage.transform);
            spawnedPrefabs.Add(markerName, spawned);

            spawned.transform.localPosition = prefabEntry.translationOffset;
            spawned.transform.localRotation = Quaternion.Euler(prefabEntry.rotationOffset);
        }

        // Update position + visibility
        spawned.transform.SetPositionAndRotation(
            trackedImage.transform.position + trackedImage.transform.rotation * prefabEntry.translationOffset,
            trackedImage.transform.rotation * Quaternion.Euler(prefabEntry.rotationOffset)
        );

        spawned.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }
}
