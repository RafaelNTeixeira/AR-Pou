using UnityEngine;

public class ButtonMinigame2 : MonoBehaviour
{

    public GameObject instructionsPanelMinigame2;
    public void OnPlayButtonPressed()
    {
        instructionsPanelMinigame2.SetActive(false);

        Minigame2Manager minigameManager = FindObjectOfType<Minigame2Manager>();
        if (minigameManager != null)
        {
            if (minigameManager.enabled)
            {
                Debug.Log("🔄 Minigame already active — restarting...");
                minigameManager.ResetGame();
            }
            else
            {
                Debug.Log("▶️ Starting Minigame2...");
                minigameManager.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Minigame2Manager not found in the scene!");
        }
    }
}