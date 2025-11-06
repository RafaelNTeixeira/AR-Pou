using UnityEngine;
using System.Collections;

// Class to handle Pou's animations and effects
public class PouAnimator : MonoBehaviour
{
    [Header("Pou Body")]
    public Transform pouBody;

    [Header("Emoji Popup")]
    public GameObject emojiPrefab;
    public float emojiHeight = 1.2f;

    [Header("Particles")]
    public ParticleSystem bubbleEffect;
    public ParticleSystem healEffect;

    [Header("Sounds (3D)")]
    public AudioSource audioSource;
    public AudioClip feedSound;
    public AudioClip sleepSound;
    public AudioClip cleanSound;
    public AudioClip medicineSound;
    public AudioClip dizzySound;

    [Header("Dizzy Settings")]
    [Tooltip("How long the 360-degree spin should take.")]
    public float spinDuration = 2.0f;
    [Tooltip("How much the body will wobble while spinning.")]
    public float wobbleStrength = 15f;

    private bool isAnimating = false;

    void Start()
    {
        if (pouBody == null) pouBody = transform;
    }

    // Helper function to play sound effects    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // Animation Routines
    public void PlayFeedAnimation()
    {
        if (!isAnimating) StartCoroutine(FeedRoutine());
        PlaySound(feedSound);
        ShowEmoji("<sprite name=feed>");
    }

    // Sleep Animation
    public void PlaySleepAnimation()
    {
        if (!isAnimating) StartCoroutine(SleepRoutine());
        PlaySound(sleepSound);
        ShowEmoji("<sprite name=sleep>");
    }

    // Clean Animation
    public void PlayCleanAnimation()
    {
        if (!isAnimating) StartCoroutine(CleanRoutine());
        PlaySound(cleanSound);
        ShowEmoji("<sprite name=clean>");
    }

    // Medicine Animation
    public void PlayMedicineAnimation()
    {
        if (!isAnimating) StartCoroutine(MedicineRoutine());
        PlaySound(medicineSound);
        ShowEmoji("<sprite name=medicine>");
    }

    // Dizzy Animation
    public void PlayDizzyAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(DizzyRoutine());
        }
    }

    // Helper function to show emoji popup above Pou
    void ShowEmoji(string spriteTag)
    {
        if (emojiPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * emojiHeight;
        GameObject popup = Instantiate(emojiPrefab, spawnPos, Quaternion.identity);

        EmojiPopup popupScript = popup.GetComponent<EmojiPopup>();
        if (popupScript != null)
            popupScript.Setup(spriteTag);
    }

    // Helper function to spawn particle effects
    private void SpawnParticle(ParticleSystem particlePrefab)
    {
        if (particlePrefab == null) return;

        // Instantiate at Pou's position
        ParticleSystem instance = Instantiate(particlePrefab, pouBody.position, Quaternion.identity);

        // Play the particle
        instance.Play();

        // Destroy after its duration
        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
    }

    // Feed Animation
    private IEnumerator FeedRoutine()
    {
        isAnimating = true;
        Vector3 currentInitialScale = pouBody.localScale;
        Vector3 chewScale = new Vector3(
            currentInitialScale.x * 1.1f,
            currentInitialScale.y * 0.9f,
            currentInitialScale.z * 1.1f
        );

        for (int i = 0; i < 4; i++)
        {
            pouBody.localScale = chewScale;
            yield return new WaitForSeconds(0.1f);
            pouBody.localScale = currentInitialScale;
            yield return new WaitForSeconds(0.1f);
        }
        isAnimating = false;
    }

    // Sleep Animation
    private IEnumerator SleepRoutine()
    {
        isAnimating = true;
        Vector3 currentInitialScale = pouBody.localScale;
        float duration = 4f;
        float t = 0f;

        while (t < duration)
        {
            float scaleOffset = Mathf.Sin(Time.time * 2f) * 0.02f;
            pouBody.localScale = currentInitialScale * (1f + scaleOffset);
            t += Time.deltaTime;
            yield return null;
        }
        pouBody.localScale = currentInitialScale;
        isAnimating = false;
    }

    // Medicine Animation
    private IEnumerator MedicineRoutine()
    {
        isAnimating = true;
        SpawnParticle(healEffect);
        Quaternion currentInitialRotation = pouBody.localRotation;

        for (int i = 0; i < 10; i++)
        {
            pouBody.localRotation = currentInitialRotation * Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
            yield return new WaitForSeconds(0.05f);
        }
        pouBody.localRotation = currentInitialRotation;
        isAnimating = false;
    }

    // Clean Animation
    private IEnumerator CleanRoutine()
    {
        isAnimating = true;
        SpawnParticle(bubbleEffect);
        Quaternion currentInitialRotation = pouBody.localRotation;
        float timer = 1.5f;

        while (timer > 0f)
        {
            float angle = Mathf.Sin(Time.time * 10f) * 5f;
            pouBody.localRotation = currentInitialRotation * Quaternion.Euler(0f, angle, 0f);
            timer -= Time.deltaTime;
            yield return null;
        }
        pouBody.localRotation = currentInitialRotation;
        isAnimating = false;
    }

    // Dizzy Animation
    private IEnumerator DizzyRoutine()
    {
        isAnimating = true;
        PlaySound(dizzySound);

        Quaternion originalRotation = pouBody.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;

            // Wobble angle over time
            float wobbleAngle = Mathf.Sin(elapsedTime * 12f) * wobbleStrength + Random.Range(-wobbleStrength, wobbleStrength) * 0.5f;

            pouBody.localRotation = originalRotation * Quaternion.Euler(0f, wobbleAngle, 0f);

            yield return null;
        }

        pouBody.localRotation = originalRotation;
        isAnimating = false;
    }
}
