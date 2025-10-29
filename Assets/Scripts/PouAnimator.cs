using UnityEngine;
using System.Collections;

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

    private bool isAnimating = false;
    private Vector3 defaultScale;

    void Start()
    {
        if (pouBody == null) pouBody = transform;
        defaultScale = pouBody.localScale;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void PlayFeedAnimation()
    {
        if (!isAnimating) StartCoroutine(FeedRoutine());
        PlaySound(feedSound);
        ShowEmoji("<sprite name=feed>");
    }

    public void PlaySleepAnimation()
    {
        if (!isAnimating) StartCoroutine(SleepRoutine());
        PlaySound(sleepSound);
        ShowEmoji("<sprite name=sleep>");
    }

    public void PlayCleanAnimation()
    {
        if (!isAnimating) StartCoroutine(CleanRoutine());
        PlaySound(cleanSound);
        ShowEmoji("<sprite name=clean>");
    }

    public void PlayMedicineAnimation()
    {
        if (!isAnimating) StartCoroutine(MedicineRoutine());
        PlaySound(medicineSound);
        ShowEmoji("<sprite name=medicine>");
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

    private IEnumerator FeedRoutine()
    {
        isAnimating = true;

        // simple chew loop (scale squash/stretch)
        for (int i = 0; i < 4; i++)
        {
            pouBody.localScale = new Vector3(
                defaultScale.x * 1.1f,
                defaultScale.y * 0.9f,
                defaultScale.z * 1.1f
            );
            yield return new WaitForSeconds(0.1f);
            pouBody.localScale = defaultScale;
            yield return new WaitForSeconds(0.1f);
        }

        isAnimating = false;
    }

    private IEnumerator SleepRoutine()
    {
        isAnimating = true;

        float duration = 4f;
        float t = 0f;

        while (t < duration)
        {
            float scaleOffset = Mathf.Sin(Time.time * 2f) * 0.02f; // breathing motion
            pouBody.localScale = defaultScale * (1f + scaleOffset);
            t += Time.deltaTime;
            yield return null;
        }

        pouBody.localScale = defaultScale;
        isAnimating = false;
    }

    private IEnumerator MedicineRoutine()
    {
        isAnimating = true;
        SpawnParticle(healEffect);

        // Shake
        for (int i = 0; i < 10; i++)
        {
            pouBody.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
            yield return new WaitForSeconds(0.05f);
        }

        pouBody.localRotation = Quaternion.identity;
        isAnimating = false;
    }

    private IEnumerator CleanRoutine()
    {
        isAnimating = true;
        SpawnParticle(bubbleEffect);

        // Small wiggle while cleaning
        float timer = 1.5f;
        while (timer > 0f)
        {
            float angle = Mathf.Sin(Time.time * 10f) * 5f;
            pouBody.localRotation = Quaternion.Euler(0f, angle, 0f);
            timer -= Time.deltaTime;
            yield return null;
        }

        pouBody.localRotation = Quaternion.identity;
        isAnimating = false;
    }
}
