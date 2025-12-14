using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Views (0 = Cam 1, 4 = Cam 5)")]
    public GameObject[] cameraViews;

    [Header("Display UI")]
    public GameObject cameraDisplayPanel; // HLAVNÍ MONITOR PANEL

    public int currentCameraID = 1;

    // --- PŘIDANÉ REFERENCE PRO LAN A SANTU ---
    [Header("Enemy Tracking - Lin")]
    public LinScript linEnemy;
    [Header("Lin Camera Vizuály (0=P1, 4=P5)")]
    public GameObject[] linCameraViews;

    [Header("Enemy Tracking - Lan")]
    public LanScript lanEnemy; // <-- NOVÁ REFERENCE PRO LAN MANAGER
    [Header("Lan Camera Vizuály (0=P1, 4=P5)")]
    public GameObject[] lanCameraViews; // <-- NOVÉ POLE PRO 5 OBRÁZKŮ LAN

    [Header("Enemy Tracking - Santa")]
    public ThiefScript santaEnemy; // <-- NOVÁ REFERENCE PRO SANTA MANAGER (ThiefScript)
    // ------------------------------------------

    [Header("External Controls")]
    public GameObject[] cameraHotspots;

    // TOTO JE REFERENCE PRO GAME OVER CHECK (původní)
    [Header("Game Over Check")]
    public AlexandraScript alexandra; 

    private void Start()
    {
        if (cameraViews.Length < 5)
        {
            Debug.LogError("CHYBA: Nenastavil/a jsi dostatek kamer (potřebuješ alespoň 5 viewů)!");
        }

        // VYPÍNÁNÍ VIZUÁLŮ NA STARTU
        if (linCameraViews != null)
        {
            foreach (GameObject view in linCameraViews)
            {
                if (view != null) view.SetActive(false);
            }
        }
        if (lanCameraViews != null) // Vypnout i vizuály Lan!
        {
             foreach (GameObject view in lanCameraViews)
            {
                if (view != null) view.SetActive(false);
            }
        }
        // U Santy se o vypnutí stará jeho vlastní skript ResetSanta()

        ToggleHotspots(false);
        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(false);

        UpdateCameraView();
    }

    public void SwitchCamera(int newCamID)
    {
        if (newCamID >= 1 && newCamID <= cameraViews.Length)
        {
            currentCameraID = newCamID;
            Debug.Log("Přepínám na kameru " + currentCameraID);

            UpdateCameraView();
        }
        else
        {
            Debug.LogWarning("Neplatné ID kamery: " + newCamID);
        }
    }

    public void UpdateCameraView()
    {
        bool isMonitorActive = (cameraDisplayPanel != null && cameraDisplayPanel.activeInHierarchy);

        // 1. Přepínání 5 vizuálů kamery (Pozadí/Základ)
        for (int i = 0; i < cameraViews.Length; i++)
        {
            GameObject view = cameraViews[i];
            bool shouldBeActive = (i + 1 == currentCameraID);

            if (view != null)
            {
                view.SetActive(shouldBeActive && isMonitorActive);
            }
        }

        // --- 2. Logika: Zobrazení Lin ---
        UpdateEnemyVisibility(linEnemy, linCameraViews, isMonitorActive);
        
        // --- 3. Logika: Zobrazení Lan ---
        UpdateEnemyVisibility(lanEnemy, lanCameraViews, isMonitorActive);
        
        // --- 4. Logika: Aktualizace Santy ---
        // Santa nepotřebuje speciální logiku zde, protože jeho vizuál zapíná jeho ThiefScript.
        // My jen zajistíme, že jeho skript pracuje správně s aktivitou monitoru/kamery.
    }
    
    // Nová univerzální metoda pro Lin a Lan (úspora kódu a čitelnost)
    private void UpdateEnemyVisibility(dynamic enemyScript, GameObject[] enemyViews, bool isMonitorActive)
    {
        if (enemyScript == null || enemyViews == null) return;
        
        // Vypni všechny vizuály nepřítele (Lin/Lan)
        foreach (GameObject view in enemyViews)
        {
            if (view != null) view.SetActive(false);
        }

        // Zkontroluj, kde je nepřítel
        int enemyPos = enemyScript.currentPosition;
        int camIndex = enemyPos - 1;

        // Nepřítel je na pozici 1-5 A hráč se dívá na správnou kameru
        if (enemyPos >= 1 && enemyPos <= 5 && enemyPos == currentCameraID)
        {
            if (camIndex < enemyViews.Length && enemyViews[camIndex] != null)
            {
                enemyViews[camIndex].SetActive(isMonitorActive);
                // Debug.Log($"{enemyScript.enemyName} JE vidět na CAM {enemyPos}.");
            }
        }
    }


    public void ActivateMonitor()
    {
        if (cameraDisplayPanel != null)
            cameraDisplayPanel.SetActive(true);

        ToggleHotspots(true);
        UpdateCameraView();
    }

    /// <summary>
    /// Zavolá se z CameraHoverZone (SKLÁPÍ MONITOR) - ZDE JE GAME OVER CHECK
    /// </summary>
    public void DeactivateMonitor()
    {
        // TOTO JE KLÍČOVÝ GAME OVER CHECK PRO ALEXANDRU!
        if (alexandra != null && alexandra.IsInKillState())
        {
            if (alexandra.nightManager != null)
            {
                Debug.Log("JUMPSCARE ALEXANDRA! Monitor stažen v kill state!");
                alexandra.nightManager.GameOver(alexandra.enemyName);
                return; // Zastaví zbytek funkce, je Game Over
            }
        }

        // PŮVODNÍ LOGIKA PRO SKLOPENÍ MONITORU (pokud nenastane Game Over)
        if (cameraDisplayPanel != null)
            cameraDisplayPanel.SetActive(false);

        ToggleHotspots(false);

        // VYPÍNÁNÍ VIZUÁLŮ (Vše, co bylo na monitoru)
        if (linCameraViews != null)
        {
            foreach (GameObject view in linCameraViews)
            {
                if (view != null) view.SetActive(false);
            }
        }
        // Vypnout i Lan
        if (lanCameraViews != null)
        {
            foreach (GameObject view in lanCameraViews)
            {
                if (view != null) view.SetActive(false);
            }
        }

        foreach (GameObject view in cameraViews)
        {
            if (view != null)
                view.SetActive(false);
        }
        
        // Poznámka: Vizuály Santy se vypnou samy přes jeho ThiefScript.UpdateVisibility()
    }

    private void ToggleHotspots(bool active)
    {
        if (cameraHotspots == null) return;

        foreach (GameObject hotspot in cameraHotspots)
        {
            if (hotspot != null)
            {
                hotspot.SetActive(active);
            }
        }
    }
}