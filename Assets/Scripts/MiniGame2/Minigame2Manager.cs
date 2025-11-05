using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

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
        if (!IsMinigameActive)
        {
            StopGame();
        }
    }


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

        for (int i = 0; i < numbers.Length; i++)
        {
            string num = numbers[i];
            countdownText.text = num;
            countdownText.color = colors[i];
            countdownText.alpha = 1f; // fully visible

            // 🔊 Play countdown sounds
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

            // 💫 Fade out over 0.5s
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



    void SetupMinigameMode(bool isMinigame)
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

        Debug.Log($"🎮 Minigame mode: {(isMinigame ? "ENABLED" : "DISABLED")}");
    }

    IEnumerator StartRound()
    {
        playerTurn = false;
        playerInput.Clear();
        
        sequence.Add(Random.Range(0, objectPrefabs.Length));
        Debug.Log($"🎯 Round {currentRound}: Sequence = {string.Join(", ", sequence)}");

        yield return new WaitForSeconds(0.8f);
        yield return ShowSequenceToPlayer();
        yield return new WaitForSeconds(0.2f);

        playerTurn = true;
        Debug.Log("👉 Player's turn: Use markers to deliver objects in correct order!");
    }

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

        Debug.Log("✅ Sequence display complete — player's turn!");
    }

    public IEnumerator ObjectDelivered(int objectIndex)
    {
        if (!playerTurn || gameOver) 
        {
            Debug.Log("⚠️ Not your turn yet, or game is over!");
            yield break;
        }

        // Evita várias entregas rápidas (desativa turno durante a verificação)
        playerTurn = false;

        playerInput.Add(objectIndex);
        int step = playerInput.Count - 1;

        if (objectIndex != sequence[step])
        {
            Debug.Log($"❌ Wrong object! Expected {GetObjectName(sequence[step])}, got {GetObjectName(objectIndex)}");

            // ❌ Play wrong sound
            if (audioSource != null && wrongSound != null)
                audioSource.PlayOneShot(wrongSound, 0.8f);

            StartCoroutine(GameOverSequence());
            yield break;
        }

        Debug.Log($"✅ Correct! {GetObjectName(objectIndex)} ({step + 1}/{sequence.Count})");

        // ✅ Play correct sound
        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound, 4f);


        if (playerInput.Count < sequence.Count)
        {
            yield return new WaitForSeconds(2f);
            playerTurn = true;
            Debug.Log("⏱️ Ready for next object!");
        }
        else
        {
            Debug.Log($"🎉 Full sequence complete! Advancing to Round {currentRound + 1}");
            currentRound++;
            roundText.text = $"Round {currentRound}";
            StartCoroutine(StartRound());
        }
    }


    IEnumerator GameOverSequence()
    {
        gameOver = true;
        playerTurn = false;

        // 🔊 Play the "wrong" (mistake) sound first
        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound, 1.0f);

        // ⏳ Wait 1 second before switching to Game Over
        yield return new WaitForSeconds(1f);

        // ⏹️ Stop the theme music if it's playing
        if (audioSource != null && audioSource.clip == themeMusic)
            audioSource.Stop();

        // 🔊 Play the Game Over sound
        if (audioSource != null && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound, 1.0f);

        roundText.text = $"Score: {currentRound-1}";

        yield return new WaitForSeconds(4f);

        StopGame();

        Debug.Log($"💀 Game Over! You reached Round {currentRound}");
        Debug.Log($"📊 Final Score: {currentRound - 1} rounds completed");
    }


    // function to stop the game
    private void ClearGameState()
    {
        StopAllCoroutines();
        sequence.Clear();
        playerInput.Clear();
        currentRound = 1;
        playerTurn = false;
    }

    public void StopGame()
    {
        ClearGameState();
        SetupMinigameMode(false);
        roundTextObj.SetActive(false);
        gameOver = true;
        MarkerObjectSpawner.hasShownMinigame2 = false;
        Debug.Log("🛑 Game stopped!");
    }

    public void ResetGame()
    {
        ClearGameState();
        SetupMinigameMode(true);
        gameOver = false;
        Debug.Log("🔄 Game restarted!");
        StartCoroutine(CountdownBeforeStart());
    }

    
    string GetObjectName(int index)
    {
        string[] names = { "Pizza", "Bed", "Soap", "Pill" };
        return index >= 0 && index < names.Length ? names[index] : "Unknown";
    }

    public int GetCurrentRound() => currentRound;
    public int GetSequenceLength() => sequence.Count;
    public bool IsPlayerTurn() => playerTurn;
    public bool IsGameOver() => gameOver;
}
