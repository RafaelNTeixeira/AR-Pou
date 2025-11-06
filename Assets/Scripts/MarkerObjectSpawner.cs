using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Collections;

// Class to spawn and manage marker-based objects in AR
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

    [Header("Minigame 1 Instructions")]
    private bool hasShownMinigame1 = false;

    //private MazePositionManager mazePositionManager;

    public GameObject instructionsPanel;

    [Header("Minigame 2 Instructions")]
    public GameObject instructionsPanelMinigame2;
    public static bool hasShownMinigame2 = false;
    public static Vector3 MinigameMarkerPosition;


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

    // Show or hide Minigame 2 instructions panel
    void ShowInstructionsMinigame2(bool show)
    {
        instructionsPanelMinigame2.SetActive(show);
        hasShownMinigame2 = show;
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
            Debug.Log($"Tracked image removed: {trackedImage.referenceImage.name}");
            string markerName = trackedImage.referenceImage.name;

            if (spawnedPrefabs.TryGetValue(markerName, out var spawned))
            {
                spawned.SetActive(false);
                Debug.Log($"Marker {markerName} lost - hiding spawned prefab.");
            }

            if (markerName == "Minigame2Marker")
            {
                Debug.Log("Minigame2Marker lost - hiding instructions panel.");
                ShowInstructionsMinigame2(false);
                Minigame2Manager.IsMinigameActive = false;
                hasShownMinigame2 = false;
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

        // When MazeMarker is detected, show instructions once
        if (prefabEntry.markerName == "MazeMarker" && !hasShownMinigame1)
        {
            instructionsPanel.SetActive(true);
            hasShownMinigame1 = true;
        }

        // When UndoMarker is detected, trigger maze move
        if (prefabEntry.markerName == "UndoMarker")
        {
            MazePositionManager mazePositionManager = FindObjectOfType<MazePositionManager>();
            mazePositionManager.TriggerMazeMove();
        }

        // When Minigame2Marker is detected, show instructions once
        if (prefabEntry.markerName == "Minigame2Marker" && !hasShownMinigame2)
        {
            Debug.Log("Minigame2Marker detected - showing instructions panel.");
            ShowInstructionsMinigame2(true);

            Minigame2Manager.IsMinigameActive = true;
        }

        // Store the position of the Minigame2Marker to update the sequence position
        if (prefabEntry.markerName == "Minigame2Marker")
        {
            MinigameMarkerPosition = trackedImage.transform.position;
        }

        spawned.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }
}
