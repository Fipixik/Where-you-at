using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Views (0 = Cam 1, 4 = Cam 5)")]
    public GameObject[] cameraViews; // Statické pozadí kamer

    [Header("Display UI")]
    public GameObject cameraDisplayPanel; // HLAVNÍ MONITOR PANEL

    public int currentCameraID = 1;

    // --- ENEMY REFERENCES ---
    [Header("Enemy Tracking - Lin")]
    public LinScript linEnemy;
    public GameObject[] linCameraViews; // Fotky pro Lin

    [Header("Enemy Tracking - Lan")]
    public LanScript lanEnemy;          // <-- ZDE MUSÍ BÝT LAN Z HIERARCHIE!
    public GameObject[] lanCameraViews; // Fotky pro Lana

    [Header("Enemy Tracking - Santa")]
    public ThiefScript santaEnemy;      // Odkaz na Santu
    // -------------------------

    [Header("External Controls")]
    public GameObject[] cameraHotspots;

    [Header("Game Over Check")]
    public AlexandraScript alexandra;

    private void Start()
    {
        // Kontrola
        if (cameraViews.Length < 5)
            Debug.LogError("CHYBA: Málo kamer v poli 'Camera Views'!");

        // 1. Vypneme všechny fotky monster na startu
        ResetVisuals(linCameraViews);
        ResetVisuals(lanCameraViews);

        // 2. Vypneme Hotspoty a Monitor
        ToggleHotspots(false);
        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(false);

        // 3. První update
        UpdateCameraView();
    }

    // Pomocná funkce pro vypnutí polí
    private void ResetVisuals(GameObject[] views)
    {
        if (views != null)
        {
            foreach (var v in views) if (v != null) v.SetActive(false);
        }
    }

    // Volá se tlačítky kamer
    public void SwitchCamera(int newCamID)
    {
        if (newCamID >= 1 && newCamID <= cameraViews.Length)
        {
            currentCameraID = newCamID;
            // Debug.Log("Přepínám na kameru " + currentCameraID);
            UpdateCameraView();
        }
    }

    // HLAVNÍ FUNKCE PRO AKTUALIZACI OBRAZU
    public void UpdateCameraView()
    {
        bool isMonitorActive = (cameraDisplayPanel != null && cameraDisplayPanel.activeInHierarchy);

        // A. Zobrazit statické pozadí (místnost)
        for (int i = 0; i < cameraViews.Length; i++)
        {
            if (cameraViews[i] != null)
            {
                bool isCurrent = (i + 1 == currentCameraID);
                cameraViews[i].SetActive(isCurrent && isMonitorActive);
            }
        }

        // B. Zobrazit LIN
        UpdateEnemyVisibility(linEnemy, linCameraViews, isMonitorActive);

        // C. Zobrazit LANA (Tady je ten problémový)
        UpdateEnemyVisibility(lanEnemy, lanCameraViews, isMonitorActive);

        // Santa se řeší sám ve svém skriptu, tady ho jen držíme v paměti
    }

    // UNIVERZÁLNÍ LOGIKA PRO ZOBRAZENÍ MONSTRA
    private void UpdateEnemyVisibility(dynamic enemyScript, GameObject[] enemyViews, bool isMonitorActive)
    {
        if (enemyScript == null || enemyViews == null) return;

        // 1. Nejdřív vše vypneme
        foreach (GameObject view in enemyViews)
        {
            if (view != null) view.SetActive(false);
        }

        // 2. Zjistíme data
        int enemyPos = enemyScript.currentPosition; // Kde je monstrum?

        // --- 🕵️‍♂️ ŠPIONÁŽNÍ VÝPIS PRO LANA ---
        if (enemyScript.enemyName == "Lan")
        {
            // Pokud tohle vypíše "Lan Pos: 0", ale LanScript říká 3 -> MÁŠ ŠPATNÝ ODKAZ V INSPECTORU!
            Debug.Log($"🕵️ [MANAGER VIDÍ]: Lan Pos: {enemyPos} | Kamera: {currentCameraID} | Monitor Active: {isMonitorActive}");
        }
        // -------------------------------------

        // 3. Pokud je monstrum na aktuální kameře a monitor svítí -> ZAPNOUT
        if (enemyPos == currentCameraID && isMonitorActive)
        {
            int viewIndex = enemyPos - 1; // Pole začíná od 0, kamery od 1

            if (viewIndex < enemyViews.Length && enemyViews[viewIndex] != null)
            {
                enemyViews[viewIndex].SetActive(true);
                // Debug.Log($"💡 Zapínám vizuál pro {enemyScript.enemyName} na kameře {currentCameraID}");
            }
        }
    }

    public void ActivateMonitor()
    {
        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(true);
        ToggleHotspots(true);
        UpdateCameraView();
    }

    public void DeactivateMonitor()
    {
        // 1. GAME OVER CHECK (Alexandra)
        if (alexandra != null && alexandra.IsInKillState())
        {
            if (alexandra.nightManager != null)
            {
                Debug.Log("💀 JUMPSCARE ALEXANDRA! Monitor stažen pozdě!");
                alexandra.nightManager.GameOver(alexandra.enemyName);
                return; // Konec hry, nic nevypínáme
            }
        }

        // 2. Vypnutí monitoru (pokud žijeme)
        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(false);
        ToggleHotspots(false);

        // 3. Vypnutí všech vizuálů (Lin, Lan, Pozadí)
        ResetVisuals(linCameraViews);
        ResetVisuals(lanCameraViews);
        ResetVisuals(cameraViews);
    }

    private void ToggleHotspots(bool active)
    {
        if (cameraHotspots != null)
        {
            foreach (var h in cameraHotspots) if (h != null) h.SetActive(active);
        }
    }
}