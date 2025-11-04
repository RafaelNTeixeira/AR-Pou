using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Collections;

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
    public float displayTime = 60f; // 1 minute
    public GameObject instructionsPanel;

    private bool hasShownMinigame1 = false;

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

    IEnumerator ShowInstructionsForTime()
    {
        instructionsPanel.SetActive(true); // show instructions
        //Debug.Log("Waiting...");
        yield return new WaitForSeconds(displayTime);
        instructionsPanel.SetActive(false); // hide after 1 minute
    }

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
            if (spawnedPrefabs.TryGetValue(trackedImage.referenceImage.name, out var spawned))
            {
                spawned.SetActive(false);
            }
            if (trackedImage.referenceImage.name == "Minigame2Marker")
            {
                ShowInstructionsMinigame2(false);
                Minigame2Manager.IsMinigameActive = false;
                hasShownMinigame2 = false;
                Debug.Log("Minigame2Marker lost - hiding instructions panel.");
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

        // Activate the instructions
        if (prefabEntry.markerName == "MazeMarker" && !hasShownMinigame1)
        {
            StartCoroutine(ShowInstructionsForTime());
            hasShownMinigame1 = true;
        }

        // When Minigame2Marker is detected, show instructions once
        if (prefabEntry.markerName == "Minigame2Marker" && !hasShownMinigame2)
        {
            Debug.Log("Minigame2Marker detected - showing instructions panel.");
            ShowInstructionsMinigame2(true);

            MinigameMarkerPosition = trackedImage.transform.position;
            Minigame2Manager.IsMinigameActive = true;
        }

        spawned.SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }
}
