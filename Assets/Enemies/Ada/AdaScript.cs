using UnityEngine;
using System.Collections;

public class AdaScript : MonoBehaviour
{
    [Header("Nastavení šancí a času")]
    [Range(0, 100)] public float appearanceChance = 30f;
    public float timeToReact = 2.0f;

    [Header("Místa výskytu (Ada v dálce)")]
    public GameObject[] spawnLocations;

    [Header("Jumpscare Nastavení")]
    public GameObject jumpscareObject;
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
        Debug.Log("<color=cyan>ADA: Systém spuštěn a připraven.</color>");
    }

    private void FullReset()
    {
        isDead = false;
        isAdaPresent = false;
        isPlayerLookingAtBack = false;
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

        if (lookingAtBackOffice)
        {
            Debug.Log("<color=yellow>ADA: OnPlayerMoved(true) - Hráč se kouká dozadu!</color>");
            TrySpawnAda();
        }
        else
        {
            Debug.Log("<color=green>ADA: OnPlayerMoved(false) - Hráč se vrátil, vypínám nebezpečí.</color>");
            FullReset();
        }
    }

    void TrySpawnAda()
    {
        float roll = Random.Range(0f, 100f);
        Debug.Log($"ADA: Hod kostkou: {roll} (potřebuješ pod {appearanceChance})");

        if (roll <= appearanceChance)
        {
            if (spawnLocations.Length == 0)
            {
                Debug.LogError("ADA ERROR: Nemáš v poli spawnLocations žádné objekty!");
                return;
            }

            activeIndex = Random.Range(0, spawnLocations.Length);
            spawnLocations[activeIndex].SetActive(true);
            isAdaPresent = true;

            Debug.Log($"<color=red>ADA: !!! SPAWN !!! na pozici index {activeIndex}. Hráč má {timeToReact}s!</color>");

            if (staticSound != null) staticSound.Play();

            if (killCoroutine != null) StopCoroutine(killCoroutine);
            killCoroutine = StartCoroutine(KillTimer());
        }
        else
        {
            Debug.Log("ADA: Spawn se nepovedl (nízká šance).");
            isAdaPresent = false;
        }
    }

    IEnumerator KillTimer()
    {
        Debug.Log("ADA: KillTimer běží...");
        yield return new WaitForSeconds(timeToReact);

        if (isAdaPresent && isPlayerLookingAtBack && !isDead)
        {
            Debug.Log("<color=red>ADA: KONEC ČASU! Spouštím jumpscare!</color>");
            StartCoroutine(PerformJumpscare());
        }
        else
        {
            Debug.Log("ADA: Timer doběhl, ale podmínky pro smrt nejsou splněny (hráč utekl nebo Ada není).");
        }
    }

    IEnumerator PerformJumpscare()
    {
        isDead = true;
        if (jumpscareObject != null) jumpscareObject.SetActive(true);

        AudioSource source = GetComponent<AudioSource>();
        if (source != null && jumpscareSound != null) source.PlayOneShot(jumpscareSound);

        yield return new WaitForSecondsRealtime(jumpscareDuration);

        if (nightManager != null) nightManager.GameOver("Ada");
    }
}