using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PouAutoPlacer : MonoBehaviour
{
    public GameObject pouPrefab;

    private ARPlaneManager planeManager;
    private bool pouSpawned = false;

    void Start()
    {
        planeManager = Object.FindFirstObjectByType<ARPlaneManager>();
    }

    // TEMP: Use markers later
    void Update()
    {
        if (pouSpawned || planeManager == null || planeManager.trackables.count == 0)
            return;

        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                SpawnPou(plane.center);
                pouSpawned = true;

                // Stop detecting planes after Pou is placed
                planeManager.enabled = false;
                foreach (var p in planeManager.trackables)
                    p.gameObject.SetActive(false);

                break;
            }
        }
    }

    void SpawnPou(Vector3 position)
    {
        Instantiate(pouPrefab, position, Quaternion.identity);
        Debug.Log("Pou spawned automatically on detected surface!");
    }
}
