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

    [Header("Managers")]
    public BaseNightManager nightManager;
    public CameraManager cameraManager;

    // --- INTERNÍ STAVY ---
    private bool isActive = false;
    private int activeCameraIndex = -1; // 0 až 4
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

            // 1. UPDATE VIZUÁLU (Oprava tvého problému)
            UpdateVisibility();

            // 2. KONTROLA ČASU (Smrt)
            if (timer >= killTimerDuration)
            {
                Debug.Log("[ThiefScript] ⏳ Čas vypršel!");
                Jumpscare();
            }

            // 3. KONTROLA KLIKNUTÍ
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                CheckClick();
            }
        }
    }

    // Tuhle funkci jsem přidal - stará se o to, aby byl vidět jen na správné kameře
    void UpdateVisibility()
    {
        if (cameraManager == null) return;

        // Zjistíme, jestli je monitor nahoře (podle toho panelu v CameraManageru)
        bool isMonitorOn = cameraManager.cameraDisplayPanel != null && cameraManager.cameraDisplayPanel.activeInHierarchy;

        // Zjistíme, na jakou kameru se koukáme (1-5)
        int currentCam = cameraManager.currentCameraID;

        // Santa má být vidět JENOM KDYŽ: (Monitor je ON) A (Kamera je ta správná)
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
        // Vizuál se zapne sám v UpdateVisibility()
    }

    void CheckClick()
    {
        Vector2 clickPosition = Mouse.current.position.ReadValue();
        Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldClickPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Klikl hráč na aktivní sprite Santy? (A je Santa zrovna vidět?)
            if (hit.collider.gameObject == santaPoses[activeCameraIndex] && santaPoses[activeCameraIndex].activeSelf)
            {
                SantaRepelled();
            }
        }
    }

    void SantaRepelled()
    {
        Debug.Log($"✅ [ThiefScript] {enemyName} ÚSPĚŠNĚ ZAHNÁN!");
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

        // Vypni všechny vizuály pro jistotu
        foreach (var pose in santaPoses)
        {
            if (pose != null) pose.SetActive(false);
        }
    }
}