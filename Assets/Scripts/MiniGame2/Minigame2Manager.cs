using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame2Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject[] objectPrefabs;  // 0 = Pizza, 1 = Bed, 2 = Soap, 3 = Pill
    public float timeBetweenObjects = 1f;
    public float displayDuration = 1.5f; // How long each object stays visible
    
    [Header("Sequence Display")]
    public Transform sequenceDisplayPosition; // Where to show the sequence (above Pou?)
    public float displayScale = 1.5f; // Size of objects when displayed
    
    [Header("Optional Effects")]
    public GameObject sequenceParticles; // Particles when showing object (optional)
    
    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private int currentRound = 1;
    private bool playerTurn = false;
    private bool gameOver = false;
    public static bool IsMinigameActive { get; private set; } = false;

    void Start()
    {
        IsMinigameActive = true;
        SetupMinigameMode(true);
        StartCoroutine(StartRound());
    }

    void OnDisable()
    {
        IsMinigameActive = false;
        SetupMinigameMode(false);
    }

    // Toggle between normal mode (ItemTrigger) and minigame mode (DeliverableObject)
    void SetupMinigameMode(bool isMinigame)
    {
        foreach (GameObject prefab in objectPrefabs)
        {
            if (prefab == null) continue;

            // Disable ItemTrigger for minigame, enable for normal mode
            ItemTrigger itemTrigger = prefab.GetComponent<ItemTrigger>();
            if (itemTrigger != null)
            {
                itemTrigger.enabled = !isMinigame;
            }

            // Enable DeliverableObject for minigame, disable for normal mode
            DeliverableObject deliverable = prefab.GetComponent<DeliverableObject>();
            if (deliverable != null)
            {
                deliverable.enabled = isMinigame;
            }
        }

        Debug.Log($"🎮 Minigame mode: {(isMinigame ? "ENABLED" : "DISABLED")}");
    }

    IEnumerator StartRound()
    {
        playerTurn = false;
        playerInput.Clear();
        
        // Add new random object to sequence
        sequence.Add(Random.Range(0, objectPrefabs.Length));
        
        Debug.Log($"🎯 Round {currentRound}: Sequence = {string.Join(", ", sequence)}");
        
        // Small delay before showing
        yield return new WaitForSeconds(0.8f);
        
        // SHOW SEQUENCE TO PLAYER
        yield return ShowSequenceToPlayer();
        
        // Delay before player can play
        yield return new WaitForSeconds(0.5f);
        
        playerTurn = true;
        Debug.Log("👉 Player's turn: Use markers to deliver objects in correct order!");
    }

    // Visually shows the sequence to the player
    IEnumerator ShowSequenceToPlayer()
    {
        Debug.Log("👀 Showing sequence...");

        foreach (int objIndex in sequence)
        {
            // Instantiate object (unparented to avoid inheriting scale)
            GameObject displayObj = Instantiate(
                objectPrefabs[objIndex],
                sequenceDisplayPosition.position,
                sequenceDisplayPosition.rotation
            );

            // Set exact world scale
            //displayObj.transform.localScale = Vector3.one * displayScale;

            // Optional: Parent it after scaling (keep world transform)
            displayObj.transform.SetParent(sequenceDisplayPosition, worldPositionStays: true);

            // Optional: Particles
            if (sequenceParticles != null)
            {
                GameObject particles = Instantiate(
                    sequenceParticles,
                    displayObj.transform.position,
                    Quaternion.identity
                );
                Destroy(particles, 2f);
            }

            // Keep object visible
            yield return new WaitForSeconds(displayDuration);

            // Destroy object
            Destroy(displayObj);

            // Pause between objects
            yield return new WaitForSeconds(timeBetweenObjects);
        }

        Debug.Log("✅ Sequence shown! Now it's player's turn.");
    }

    // Called when player delivers an object (via PouReceiver)
    public void ObjectDelivered(int objectIndex)
    {
        if (!playerTurn || gameOver) 
        {
            Debug.Log("⚠️ Not your turn yet, or game is over!");
            return;
        }

        playerInput.Add(objectIndex);
        int step = playerInput.Count - 1;

        // Check if correct
        if (objectIndex != sequence[step])
        {
            Debug.Log($"❌ Wrong object! Expected {GetObjectName(sequence[step])}, got {GetObjectName(objectIndex)}");
            StartCoroutine(GameOverSequence());
            return;
        }

        Debug.Log($"✅ Correct! {GetObjectName(objectIndex)} ({step + 1}/{sequence.Count})");

        // If full sequence completed
        if (playerInput.Count == sequence.Count)
        {
            Debug.Log($"🎉 Full sequence complete! Advancing to Round {currentRound + 1}");
            currentRound++;
            StartCoroutine(StartRound());
        }
    }

    IEnumerator GameOverSequence()
    {
        gameOver = true;
        playerTurn = false;
        
        yield return new WaitForSeconds(1f);
        
        Debug.Log($"💀 Game Over! You reached Round {currentRound}");
        Debug.Log($"📊 Final Score: {currentRound - 1} rounds completed");
        
        // Here you can add game over effects (UI, animations, etc)
    }

    public void ResetGame()
    {
        StopAllCoroutines();
        sequence.Clear();
        playerInput.Clear();
        currentRound = 1;
        gameOver = false;
        playerTurn = false;
        
        Debug.Log("🔄 Game restarted!");
        StartCoroutine(StartRound());
    }
    
    // Helper for debug
    string GetObjectName(int index)
    {
        string[] names = { "Pizza", "Bed", "Soap", "Pill" };
        return index >= 0 && index < names.Length ? names[index] : "Unknown";
    }

    // Useful getters for external UI
    public int GetCurrentRound() => currentRound;
    public int GetSequenceLength() => sequence.Count;
    public bool IsPlayerTurn() => playerTurn;
    public bool IsGameOver() => gameOver;
}