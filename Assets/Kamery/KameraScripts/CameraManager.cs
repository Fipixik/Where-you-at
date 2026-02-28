using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;     // Přetáhni Audio Source z objektu
    public AudioClip cameraSwitchSound; // Zvuk přepnutí (click/beep)
    public AudioClip monitorOnSound;    // Zvuk zapnutí monitoru (static/on)

    [Header("Camera Views (0 = Cam 1, 4 = Cam 5)")]
    public GameObject[] cameraViews;

    [Header("Display UI")]
    public GameObject cameraDisplayPanel;

    public int currentCameraID = 1;

    // --- ENEMY REFERENCES ---
    [Header("Enemy Tracking - Lin")]
    public LinScript linEnemy;
    public GameObject[] linCameraViews;

    [Header("Enemy Tracking - Lan")]
    public LanScript lanEnemy;
    public GameObject[] lanCameraViews;

    [Header("Enemy Tracking - Santa")]
    public ThiefScript santaEnemy;

    [Header("External Controls")]
    public GameObject[] cameraHotspots;

    [Header("Game Over Check")]
    public AlexandraScript alexandra;

    private void Start()
    {
        if (cameraViews.Length < 5)
            Debug.LogError("CHYBA: Málo kamer v poli 'Camera Views'!");

        ResetVisuals(linCameraViews);
        ResetVisuals(lanCameraViews);

        ToggleHotspots(false);
        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(false);

        UpdateCameraView();
    }

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
            // PŘIDÁNO: Zvuk při každém přepnutí
            if (audioSource != null && cameraSwitchSound != null)
            {
                audioSource.PlayOneShot(cameraSwitchSound);
            }

            currentCameraID = newCamID;
            UpdateCameraView();
        }
    }

    public void UpdateCameraView()
    {
        bool isMonitorActive = (cameraDisplayPanel != null && cameraDisplayPanel.activeInHierarchy);

        for (int i = 0; i < cameraViews.Length; i++)
        {
            if (cameraViews[i] != null)
            {
                bool isCurrent = (i + 1 == currentCameraID);
                cameraViews[i].SetActive(isCurrent && isMonitorActive);
            }
        }

        UpdateEnemyVisibility(linEnemy, linCameraViews, isMonitorActive);
        UpdateEnemyVisibility(lanEnemy, lanCameraViews, isMonitorActive);
    }

    private void UpdateEnemyVisibility(dynamic enemyScript, GameObject[] enemyViews, bool isMonitorActive)
    {
        if (enemyScript == null || enemyViews == null) return;

        foreach (GameObject view in enemyViews)
        {
            if (view != null) view.SetActive(false);
        }

        int enemyPos = enemyScript.currentPosition;

        if (enemyPos == currentCameraID && isMonitorActive)
        {
            int viewIndex = enemyPos - 1;
            if (viewIndex < enemyViews.Length && enemyViews[viewIndex] != null)
            {
                enemyViews[viewIndex].SetActive(true);
            }
        }
    }

    public void ActivateMonitor()
    {
        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(true);

        // PŘIDÁNO: Zvuk při otevření monitoru
        if (audioSource != null && monitorOnSound != null)
        {
            audioSource.PlayOneShot(monitorOnSound);
        }

        ToggleHotspots(true);
        UpdateCameraView();
    }

    public void DeactivateMonitor()
    {
        if (alexandra != null && alexandra.IsInKillState())
        {
            if (alexandra.nightManager != null)
            {
                Debug.Log("💀 JUMPSCARE ALEXANDRA! Monitor stažen pozdě!");
                alexandra.nightManager.GameOver(alexandra.enemyName);
                return;
            }
        }

        if (cameraDisplayPanel != null) cameraDisplayPanel.SetActive(false);
        ToggleHotspots(false);

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