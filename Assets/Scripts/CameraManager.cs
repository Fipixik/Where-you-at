using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Views (0 = Cam 1, 4 = Cam 5)")]
    public GameObject[] cameraViews;

    [Header("Display UI")]
    public GameObject cameraDisplayPanel;

    public int currentCameraID = 1;

    // TOTO JE NOVÉ: Reference pro Lin
    [Header("Enemy Tracking")]
    public LinScript linEnemy;             // Sem pøijde Lin GameObject
    public GameObject linSpriteOnCamera;     // Sem pøijde VIZUÁL Lin na kameru (schovaný!)

    private void Start()
    {
        if (cameraViews.Length < 5)
        {
            Debug.LogError("CHYBA: Nenastavil/a jsi dostatek kamer (potøebuješ alespoò 5 viewù)!");
        }

        if (linSpriteOnCamera != null)
            linSpriteOnCamera.SetActive(false);

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

    /// <summary>
    /// Zobrazí pouze aktuálnì vybranou kameru a Lin, pokud je v dané pozici.
    /// Zmìnìno na PUBLIC, aby ji mohl volat LinScript po pohybu!
    /// </summary>
    public void UpdateCameraView() // <-- ZMÌNÌNO NA PUBLIC
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

    public void ActivateMonitor()
    {
        if (cameraDisplayPanel != null)
            cameraDisplayPanel.SetActive(true);

        UpdateCameraView();
    }

    public void DeactivateMonitor()
    {
        if (cameraDisplayPanel != null)
            cameraDisplayPanel.SetActive(false);

        if (linSpriteOnCamera != null)
            linSpriteOnCamera.SetActive(false);

        foreach (GameObject view in cameraViews)
        {
            if (view != null)
                view.SetActive(false);
        }
    }
}