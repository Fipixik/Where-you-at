using UnityEngine;
using System.Collections;

public class AdaScript : MonoBehaviour
{
    [Header("Nastavení šancí a času")]
    [Range(0, 100)] public float appearanceChance = 30f;
    public float timeToReact = 2.0f;
    public float moveChance;
    public float moveInterval;
    [Header("Místa výskytu")]
    public GameObject[] spawnLocations;

    [Header("Jumpscare Nastavení")]
    public GameObject jumpscareObject;
    public GameObject darkenerObject; // TVŮJ DARKENER/BLACKOUT ČTVEREC
    public AudioClip jumpscareSound;
    public float jumpscareDuration = 1.5f;

    [Header("Reference")]
    public BaseNightManager nightManager;
    public AudioSource staticSound;

    private bool isPlayerLookingAtBack = false;
    private int activeIndex = -1;
    private bool isAdaPresent = false;
    private Coroutine killCoroutine;
    private bool isDead = false;

    void Start()
    {
        if (nightManager == null) nightManager = Object.FindFirstObjectByType<BaseNightManager>();
        FullReset();
    }

    private void FullReset()
    {
        isDead = false;
        isAdaPresent = false;
        if (killCoroutine != null) StopCoroutine(killCoroutine);
        foreach (GameObject loc in spawnLocations) if (loc != null) loc.SetActive(false);
        if (jumpscareObject != null) jumpscareObject.SetActive(false);
        if (staticSound != null) staticSound.Stop();
        activeIndex = -1;
    }

    public void OnPlayerMoved(bool lookingAtBackOffice)
    {
        if (isDead) return;
        isPlayerLookingAtBack = lookingAtBackOffice;
        if (lookingAtBackOffice) TrySpawnAda();
        else FullReset();
    }

    void TrySpawnAda()
    {
        float roll = Random.Range(0f, 100f);
        if (roll <= appearanceChance)
        {
            activeIndex = Random.Range(0, spawnLocations.Length);
            spawnLocations[activeIndex].SetActive(true);
            isAdaPresent = true;
            if (staticSound != null) staticSound.Play();
            if (killCoroutine != null) StopCoroutine(killCoroutine);
            killCoroutine = StartCoroutine(KillTimer());
        }
    }

    IEnumerator KillTimer()
    {
        yield return new WaitForSeconds(timeToReact);
        if (isAdaPresent && isPlayerLookingAtBack && !isDead) StartCoroutine(PerformJumpscare());
    }

    IEnumerator PerformJumpscare()
    {
        isDead = true;
        if (jumpscareObject != null) jumpscareObject.SetActive(true);

        AudioSource source = GetComponent<AudioSource>();
        if (source != null && jumpscareSound != null) source.PlayOneShot(jumpscareSound);

        yield return new WaitForSecondsRealtime(jumpscareDuration);

        // KLÍČOVÝ FIX: Vypneme darkener před zobrazením tipu
        if (darkenerObject != null) darkenerObject.SetActive(false);

        if (nightManager != null) nightManager.GameOver("Ada");
    }
}