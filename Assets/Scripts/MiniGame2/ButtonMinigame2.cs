using UnityEngine;

public class ButtonMinigame2 : MonoBehaviour
{

    public GameObject instructionsPanelMinigame2;
    public void OnPlayButtonPressed()
    {
        instructionsPanelMinigame2.SetActive(false);
        
        // Start the minigame by enabling the Minigame2Manager
        Minigame2Manager minigameManager = FindObjectOfType<Minigame2Manager>();
        if (minigameManager != null)
        {
            minigameManager.enabled = true;
        }
        else
        {
            Debug.LogWarning("Minigame2Manager not found in the scene!");
        }
    }
}