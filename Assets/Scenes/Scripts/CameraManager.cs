using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Views (0 = Cam 1, 4 = Cam 5)")]
    public GameObject[] cameraViews;

    [Header("Display UI")]
    public GameObject cameraDisplayPanel; // HLAVNÍ MONITOR PANEL

    public int currentCameraID = 1;

    // LIN TRACKING FIELDS
    [Header("Enemy Tracking")]
    public LinScript linEnemy;
    [Header("Lin Camera Vizuály (0=P1, 4=P5)")]
    public GameObject[] linCameraViews;

    // HOTSPOT TOGGLE FIELD
    [Header("External Controls")]
    public GameObject[] cameraHotspots;

    // TOTO JE NOVÉ: REFERENCE PRO GAME OVER CHECK
    [Header("Game Over Check")]
    public AlexandraScript alexandra; // <-- ZDE MUSÍ BÝT PŘIŘAZENA ALEXANDRA

    private void Start()
    {
        if (cameraViews.Length < 5)
        {
            Debug.LogError("CHYBA: Nenastavil/a jsi dostatek kamer (potřebuješ alespoň 5 viewů)!");
        }

        if (linCameraViews != null)
        {
            foreach (GameObject view in linCameraViews)
            {
                if (view != null) view.SetActive(false);
            }
        }

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

        // 1. Přepínání 5 vizuálů kamery
        for (int i = 0; i < cameraViews.Length; i++)
        {
            GameObject view = cameraViews[i];
            bool shouldBeActive = (i + 1 == currentCameraID);

            if (view != null)
            {
                view.SetActive(shouldBeActive && isMonitorActive);
            }
        }

        // 2. Logika: Zobrazení Lin
        if (linEnemy != null && linCameraViews != null)
        {
            foreach (GameObject view in linCameraViews)
            {
                if (view != null) view.SetActive(false);
            }

            int linPos = linEnemy.currentPosition;
            int camIndex = linPos - 1;

            // Kontrola: Shoda pozice Lin s AKTUALNĚ ZAPNUTOU kamerou
            if (linPos >= 1 && linPos <= 5 && linPos == currentCameraID)
            {
                if (camIndex < linCameraViews.Length && linCameraViews[camIndex] != null)
                {
                    linCameraViews[camIndex].SetActive(isMonitorActive);
                    Debug.Log($"Lin JE vidět na CAM {linPos}.");
                }
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
            // Hráč stáhl monitor, když byla Alexandra v kill state (100%)!
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

        // VYPÍNÁNÍ VIZUÁLŮ
        if (linCameraViews != null)
        {
            foreach (GameObject view in linCameraViews)
            {
                if (view != null) view.SetActive(false);
            }
        }

        foreach (GameObject view in cameraViews)
        {
            if (view != null)
                view.SetActive(false);
        }
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