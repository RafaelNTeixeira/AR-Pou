using UnityEngine;

public class ButtonMinigame2 : MonoBehaviour
{

    public GameObject instructionsPanelMinigame2;

    // Called when the Play button is pressed in the Minigame 2 instructions panel
    public void OnPlayButtonPressed()
    {
        instructionsPanelMinigame2.SetActive(false);

        Minigame2Manager minigameManager = FindObjectOfType<Minigame2Manager>();
        if (minigameManager != null)
        {
            // If the minigame is already active, reset it
            if (minigameManager.enabled)
            {
                Debug.Log("Minigame already active — restarting...");
                minigameManager.ResetGame();
            }
            // If the minigame is not active, start it
            else
            {
                Debug.Log("Starting Minigame2...");
                minigameManager.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("Minigame2Manager not found in the scene!");
        }
    }

    // Called when the Dismiss button is pressed in the Minigame 2 instructions panel
    public void OnDismissButtonPressed()
    {
        instructionsPanelMinigame2.SetActive(false);
        MarkerObjectSpawner.hasShownMinigame2 = false;
    }
}