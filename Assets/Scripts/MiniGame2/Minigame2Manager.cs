using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

// Class to manage Minigame 2 (Memory Mini-game)
public class Minigame2Manager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject countdownTextObj;
    public GameObject roundTextObj;
    private TMPro.TextMeshProUGUI roundText => roundTextObj.GetComponent<TMPro.TextMeshProUGUI>();

    [Header("Game Settings")]
    public GameObject[] objectPrefabs;  // 0 = Pizza, 1 = Bed, 2 = Soap, 3 = Pill
    public float timeBetweenObjects = 1f;
    public float displayDuration = 1f; // How long each object stays visible

    [Header("Sequence Display")]
    private Vector3 sequenceDisplayPosition;
    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private int currentRound = 1;
    private bool playerTurn = false;
    private bool gameOver = false;
    public static bool IsMinigameActive = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip themeMusic;
    [SerializeField] private AudioClip countdownBeepSound;
    [SerializeField] private AudioClip goSound;

    void OnEnable()
    {
        IsMinigameActive = true;
        SetupMinigameMode(true);
        StartCoroutine(CountdownBeforeStart());
    }

    void OnDisable()
    {
        IsMinigameActive = false;
        SetupMinigameMode(false);

        // Stop theme when minigame ends
        if (audioSource != null)
            audioSource.Stop();
    }

    void Update()
    {
        // If Minigame is Minigame2 flag is set, stop the game
        if (!IsMinigameActive)
        {
            StopGame();
        }
    }


    // Method to handle the countdown before starting the game
    IEnumerator CountdownBeforeStart()
    {
        TMPro.TextMeshProUGUI countdownText = countdownTextObj?.GetComponent<TMPro.TextMeshProUGUI>();

        if (countdownTextObj != null)
            countdownTextObj.SetActive(true);

        // Colors for each step
        Color[] colors = {
            new Color(1f, 0.4f, 0.4f), // 3 = red
            new Color(1f, 0.7f, 0.3f), // 2 = orange
            new Color(1f, 1f, 0.3f),   // 1 = yellow
            new Color(0.3f, 1f, 0.3f)  // GO! = green
        };

        string[] numbers = { "3", "2", "1", "GO!" };


        // Show countdown text
        for (int i = 0; i < numbers.Length; i++)
        {
            string num = numbers[i];
            countdownText.text = num;
            countdownText.color = colors[i];
            countdownText.alpha = 1f;

            if (audioSource != null)
            {
                if (num == "GO!")
                {
                    if (goSound != null)
                        audioSource.PlayOneShot(goSound, 0.2f);

                    // Start theme after "GO!" finishes
                    if (themeMusic != null)
                    {
                        float delay = goSound != null ? goSound.length : 0.5f;
                        roundTextObj.SetActive(true);
                        roundText.text = $"Round {currentRound}";
                        StartCoroutine(PlayThemeAfterDelay(delay));
                    }
                }
                else if (countdownBeepSound != null)
                {
                    audioSource.PlayOneShot(countdownBeepSound, 1f);
                }
            }

            // Fade out over 0.5s
            float fadeDuration = 0.5f;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                countdownText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            countdownText.alpha = 0f;
            yield return new WaitForSeconds(0.5f); // small pause before next number
        }

        // Clear text and hide
        countdownText.text = "";
        countdownTextObj.SetActive(false);

        // Start the first round
        StartCoroutine(StartRound());
    }

    // Method to play theme music after a delay
    IEnumerator PlayThemeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioSource != null && themeMusic != null)
        {
            audioSource.clip = themeMusic;
            audioSource.loop = true;
            audioSource.volume = 0.05f;
            audioSource.Play();
        }
    }


    // Method to enable/disable minigame mode components
    public void SetupMinigameMode(bool isMinigame)
    {
        foreach (GameObject prefab in objectPrefabs)
        {
            if (prefab == null) continue;

            ItemTrigger itemTrigger = prefab.GetComponent<ItemTrigger>();
            if (itemTrigger != null)
                itemTrigger.enabled = !isMinigame;

            DeliverableObject deliverable = prefab.GetComponent<DeliverableObject>();
            if (deliverable != null)
                deliverable.enabled = isMinigame;
        }
    }

    // Coroutine to start a new round
    IEnumerator StartRound()
    {
        playerTurn = false;
        playerInput.Clear();

        sequence.Add(Random.Range(0, objectPrefabs.Length));
        Debug.Log($"Round {currentRound}: Sequence = {string.Join(", ", sequence)}");

        yield return new WaitForSeconds(0.8f);
        yield return ShowSequenceToPlayer();
        yield return new WaitForSeconds(0.2f);

        playerTurn = true;
    }

    // Coroutine to show the sequence to the player
    IEnumerator ShowSequenceToPlayer()
    {
        foreach (int objIndex in sequence)
        {
            sequenceDisplayPosition = MarkerObjectSpawner.MinigameMarkerPosition + new Vector3(0, 0.12f, 0);
            GameObject displayObj = Instantiate(
                objectPrefabs[objIndex],
                sequenceDisplayPosition,
                Quaternion.identity
            );

            displayObj.transform.SetParent(null, worldPositionStays: true);

            yield return new WaitForSeconds(displayDuration);
            Destroy(displayObj);
            yield return new WaitForSeconds(timeBetweenObjects);
        }
    }

    // Method called when an object is delivered to Pou
    public IEnumerator ObjectDelivered(int objectIndex)
    {
        if (!playerTurn || gameOver)
        {
            Debug.Log("Not your turn yet, or game is over!");
            yield break;
        }

        playerTurn = false;

        playerInput.Add(objectIndex);
        int step = playerInput.Count - 1;

        // Check if the delivered object matches the sequence
        if (objectIndex != sequence[step])
        {
            // Play wrong sound
            if (audioSource != null && wrongSound != null)
                audioSource.PlayOneShot(wrongSound, 0.8f);

            StartCoroutine(GameOverSequence());
            yield break;
        }

        // Play correct sound
        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound, 4f);

        // Check if the player has completed the sequence
        if (playerInput.Count < sequence.Count)
        {
            yield return new WaitForSeconds(2f);
            playerTurn = true;
        }
        else
        {
            currentRound++;
            roundText.text = $"Round {currentRound}";
            StartCoroutine(StartRound());
        }
    }

    // Method to handle Game Over sequence
    IEnumerator GameOverSequence()
    {
        gameOver = true;
        playerTurn = false;

        // Play the wrong sound
        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound, 1.0f);

        yield return new WaitForSeconds(1f);

        // Stop the theme music if it's playing
        if (audioSource != null && audioSource.clip == themeMusic)
            audioSource.Stop();

        // Play the Game Over sound
        if (audioSource != null && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound, 1.0f);

        roundText.text = $"Score: {currentRound - 1}";

        yield return new WaitForSeconds(4f);

        StopGame();

        Debug.Log($"Final Score: {currentRound - 1} rounds completed");
    }

    // Helper method to clear game state
    private void ClearGameState()
    {
        StopAllCoroutines();
        sequence.Clear();
        playerInput.Clear();
        currentRound = 1;
        playerTurn = false;
    }

    // Helper method to stop the game
    public void StopGame()
    {
        ClearGameState();
        SetupMinigameMode(false);
        roundTextObj.SetActive(false);
        gameOver = true;
        MarkerObjectSpawner.hasShownMinigame2 = false;
        IsMinigameActive = false;
        Debug.Log("Game stopped!");
    }

    // Method to reset the game
    public void ResetGame()
    {
        ClearGameState();
        SetupMinigameMode(true);
        gameOver = false;
        Debug.Log("Game restarted!");
        StartCoroutine(CountdownBeforeStart());
    }

    // Helper method to get object name by index
    string GetObjectName(int index)
    {
        string[] names = { "Pizza", "Bed", "Soap", "Pill" };
        return index >= 0 && index < names.Length ? names[index] : "Unknown";
    }

    // Public getters for testing or UI purposes
    public int GetCurrentRound() => currentRound;
    public int GetSequenceLength() => sequence.Count;
    public bool IsPlayerTurn() => playerTurn;
    public bool IsGameOver() => gameOver;
}
