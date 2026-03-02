using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ThiefScript : MonoBehaviour
{
    [Header("Identity")]
    public string enemyName = "Evil Santa";

    [Header("Settings")]
    public float spawnInterval = 15f;
    [Range(0, 100)] public int spawnChance = 40;
    public float killTimerDuration = 6f;

    [Header("Visuals (5 pozic)")]
    public GameObject[] santaPoses;

    [Header("--- ZVUKY: ALARM ---")]
    public AudioSource alertAudioSource; // AudioSource pro pípání (nastav mu Loop!)
    public AudioClip alertSound;         // Ten zvuk notifikace

    [Header("--- ZVUKY: ZAHÁNĚNÍ (RANDOM) ---")]
    public AudioSource repelAudioSource; // AudioSource pro rány/křik
    public AudioClip[] repelSounds;      // Pole zvuků, co se náhodně protočí

    [Header("Managers")]
    public BaseNightManager nightManager;
    public CameraManager cameraManager;

    // --- INTERNÍ STAVY ---
    private bool isActive = false;
    private int activeCameraIndex = -1;
    private float timer = 0f;

    private void Start()
    {
        Debug.Log($"[ThiefScript] Start! Jméno: {enemyName}");
        ResetSanta();
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;

            // 1. UPDATE VIZUÁLU (Zobrazí se na správné kameře)
            UpdateVisibility();

            // 2. AKTIVACE ZVUKU ALARMU
            if (alertAudioSource != null && alertSound != null && !alertAudioSource.isPlaying)
            {
                alertAudioSource.clip = alertSound;
                alertAudioSource.loop = true;
                alertAudioSource.Play();
            }

            // 3. KONTROLA SMRTI
            if (timer >= killTimerDuration)
            {
                Debug.Log("[ThiefScript] ⏳ Čas vypršel!");
                Jumpscare();
            }

            // 4. KLIK CHECK
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                CheckClick();
            }
        }
    }

    void UpdateVisibility()
    {
        if (cameraManager == null || activeCameraIndex == -1) return;

        bool isMonitorOn = cameraManager.cameraDisplayPanel != null && cameraManager.cameraDisplayPanel.activeInHierarchy;
        int currentCam = cameraManager.currentCameraID;

        // Santa je vidět jen když je zapnutý monitor a hráč kouká na správnou kameru
        bool shouldBeVisible = isMonitorOn && (currentCam == activeCameraIndex + 1);

        if (santaPoses[activeCameraIndex] != null)
        {
            santaPoses[activeCameraIndex].SetActive(shouldBeVisible);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!isActive)
            {
                int roll = Random.Range(0, 100);
                if (roll < spawnChance)
                {
                    SpawnSanta();
                }
            }
        }
    }

    void SpawnSanta()
    {
        isActive = true;
        timer = 0f;
        activeCameraIndex = Random.Range(0, santaPoses.Length);

        Debug.Log($"🚨 [ThiefScript] SPAWN! {enemyName} je na kameře {activeCameraIndex + 1}");
    }

    void CheckClick()
    {
        if (!isActive) return;

        Vector2 clickPosition = Mouse.current.position.ReadValue();
        Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldClickPosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject == santaPoses[activeCameraIndex] && santaPoses[activeCameraIndex].activeSelf)
            {
                SantaRepelled();
            }
        }
    }

    void SantaRepelled()
    {
        Debug.Log($"✅ [ThiefScript] {enemyName} ÚSPĚŠNĚ ZAHNÁN!");

        // PŘEHRÁNÍ NÁHODNÉHO ZVUKU PŘI ZAHÁNĚNÍ
        if (repelAudioSource != null && repelSounds.Length > 0)
        {
            AudioClip randomClip = repelSounds[Random.Range(0, repelSounds.Length)];
            repelAudioSource.PlayOneShot(randomClip);
        }

        ResetSanta();
    }

    void Jumpscare()
    {
        if (nightManager != null)
        {
            nightManager.GameOver(enemyName);
        }

        ResetSanta();
    }

    void ResetSanta()
    {
        isActive = false;
        activeCameraIndex = -1;
        timer = 0f;

        // Vypnutí alarmu
        if (alertAudioSource != null) alertAudioSource.Stop();

        // Vypnutí všech póz
        foreach (var pose in santaPoses)
        {
            if (pose != null) pose.SetActive(false);
        }
    }
} //