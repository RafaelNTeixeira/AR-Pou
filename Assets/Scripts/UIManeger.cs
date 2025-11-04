using UnityEngine;
using UnityEngine.UI; // Required for UI components
using TMPro; // If you use TextMeshPro
using UnityEngine.SceneManagement; // For loading a new scene
#if UNITY_EDITOR
using UnityEditor; // Needed for stopping play mode in the editor
#endif

public class UIManeger : MonoBehaviour
{
    public GameObject mainMenuCanvas;  // The Canvas containing the buttons
    public GameObject tutorialText;    // The GameObject with the tutorial text (can be Text or TMP_Text)

     public GameObject mazeText;

    void Start()
    {
        // Make sure the tutorial text starts hidden
        if (tutorialText != null)
            tutorialText.SetActive(false);
        
        if (mazeText != null)
            mazeText.SetActive(false);
    }

    // Called when the Play Game button is clicked
    public void PlayGame()
    {
        // Hide the menu
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);

        // Here you can load the game scene, for example:
        // SceneManager.LoadScene("GameScene");
    }

    // Called when the Tutorial button is clicked
    public void ShowTutorial()
    {
        if (tutorialText != null)
        {
            mainMenuCanvas.SetActive(false);
            tutorialText.SetActive(true);
        }
            
    }

    public void GoBackTutorial()
    {
        if (tutorialText != null)
        {
            tutorialText.SetActive(false);
            mainMenuCanvas.SetActive(true);
        }

    }
    

    public void PlayGameMaze()
    {
        if (mazeText != null)
        {
            mazeText.SetActive(false);
        }
            
    }

    // Called when the Exit Game button is clicked
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Stop play mode in the editor
        EditorApplication.isPlaying = false;
#else
        // Quit the built game
        Application.Quit();
#endif
    }
}
