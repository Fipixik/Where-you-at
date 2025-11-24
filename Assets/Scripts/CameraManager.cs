using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Views (0 = Cam 1, 4 = Cam 5)")]
    public GameObject[] cameraViews;

    [Header("Display UI")]
    public GameObject cameraDisplayPanel;

    public int currentCameraID = 1;

    // LIN TRACKING FIELDS
    [Header("Enemy Tracking")]
    public LinScript linEnemy;
    public GameObject linSpriteOnCamera;

    // TOTO JE NOVÉ POLE: Pole pro tvých 5 Colliderù
    [Header("External Controls")]
    public GameObject[] cameraHotspots; // <-- POLE PRO 5 COLLIDER TRIGGER OBJEKTÙ

    private void Start()
    {
        if (cameraViews.Length < 5)
        {
            Debug.LogError("CHYBA: Nenastavil/a jsi dostatek kamer (potøebuješ alespoò 5 viewù)!");
        }

        if (linSpriteOnCamera != null)
            linSpriteOnCamera.SetActive(false);

        // NOVÉ: ZDE SE HOTSPOTY VYPNOU PØI STARTU!
        ToggleHotspots(false);
        UpdateCameraView();
    }

    public void SwitchCamera(int newCamID)
    {
        if (newCamID >= 1 && newCamID <= cameraViews.Length)
        {
            currentCameraID = newCamID;
            Debug.Log("Pøepínám na kameru " + currentCameraID);

            UpdateCameraView();
        }
        else
        {
            Debug.LogWarning("Neplatné ID kamery: " + newCamID);
        }
    }

    public void UpdateCameraView()
    {
        // 1. Pøepínání 5 vizuálù kamery
        for (int i = 0; i < cameraViews.Length; i++)
        {
            GameObject view = cameraViews[i];
            bool shouldBeActive = (i + 1 == currentCameraID);

            if (view != null)
            {
                view.SetActive(shouldBeActive);
            }
        }

        // 2. Logika: Zobrazení Lin
        if (linEnemy != null && linSpriteOnCamera != null)
        {
            bool isLinVisible = (currentCameraID == linEnemy.currentPosition);

            linSpriteOnCamera.SetActive(isLinVisible);

            if (isLinVisible)
            {
                Debug.Log("Lin JE vidìt na kameøe " + currentCameraID);
            }
        }
    }

    /// <summary>
    /// Zavolá se z CameraHoverZone (ZVEDÁ MONITOR).
    /// </summary>
    public void ActivateMonitor()
    {
        if (cameraDisplayPanel != null)
            cameraDisplayPanel.SetActive(true);

        // NOVÉ: ZAPNI COLLIDEROVÉ HOTSPOTY!
        ToggleHotspots(true);

        UpdateCameraView();
    }

    /// <summary>
    /// Zavolá se z CameraHoverZone (SKLÁPÍ MONITOR).
    /// </summary>
    public void DeactivateMonitor()
    {
        if (cameraDisplayPanel != null)
            cameraDisplayPanel.SetActive(false);

        // NOVÉ: VYPNOUT COLLIDEROVÉ HOTSPOTY!
        ToggleHotspots(false);

        if (linSpriteOnCamera != null)
            linSpriteOnCamera.SetActive(false);

        foreach (GameObject view in cameraViews)
        {
            if (view != null)
                view.SetActive(false);
        }
    }

    /// <summary>
    /// TATO NOVÁ FUNKCE ØEŠÍ ZAPÍNÁNÍ/VYPÍNÁNÍ TLAÈÍTEK
    /// </summary>
    private void ToggleHotspots(bool active)
    {
        if (cameraHotspots == null) return;

        foreach (GameObject hotspot in cameraHotspots)
        {
            if (hotspot != null)
            {
                // Vypínáme/zapínáme celý GameObject (Collider i SpriteRenderer pro hover)
                hotspot.SetActive(active);
            }
        }
    }
}