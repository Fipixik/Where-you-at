using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraTeleportZone : MonoBehaviour
{
    [Header("References")]
    public ScreenFader screenFader;
    public Transform mainCameraTransform;
    public CameraManager cameraManager;
    public GameObject objectToToggle;

    private bool isTransitioning = false;
    private bool mouseInside = false;

    private void Start()
    {
        // FORCE START: Vždycky tì to spawnne na 0,0,0
        if (mainCameraTransform != null)
        {
            mainCameraTransform.position = new Vector3(0f, 0f, mainCameraTransform.position.z);

            if (objectToToggle != null)
            {
                objectToToggle.SetActive(true);
            }
        }

        StartCoroutine(DisableTeleportTemporarily(0.5f));
    }

    private void Update()
    {
        if (cameraManager != null && cameraManager.cameraDisplayPanel != null)
        {
            if (cameraManager.cameraDisplayPanel.activeInHierarchy) return;
        }

        if (isTransitioning || screenFader == null || mainCameraTransform == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D col = GetComponent<Collider2D>();

        if (col != null && col.OverlapPoint(mousePos))
        {
            if (!mouseInside)
            {
                mouseInside = true;
                StartCoroutine(PerformTeleport());
            }
        }
        else
        {
            mouseInside = false;
        }
    }

    private IEnumerator PerformTeleport()
    {
        isTransitioning = true;

        float currentY = Mathf.Round(mainCameraTransform.position.y);

        // --- ZMÌNA TADY: Vypnutí buttonu PØED cernáním ---
        if (currentY == 0f && objectToToggle != null)
        {
            objectToToggle.SetActive(false); // Okamžitý zmizík
        }

        // Teï teprve zaène fade
        yield return StartCoroutine(screenFader.FadeToBlackAndWait());

        if (currentY == 0f)
        {
            // Jdeme do Pozice 2
            mainCameraTransform.position = new Vector3(0f, 12f, mainCameraTransform.position.z);
        }
        else
        {
            // Jdeme do Pozice 1
            mainCameraTransform.position = new Vector3(0f, 0f, mainCameraTransform.position.z);
            // Button zapneme až tady, aby se neobjevil uprostøed tmy
            if (objectToToggle != null) objectToToggle.SetActive(true);
        }

        yield return StartCoroutine(screenFader.FadeToClear());

        yield return new WaitForSeconds(0.4f);
        isTransitioning = false;
    }

    private IEnumerator DisableTeleportTemporarily(float duration)
    {
        isTransitioning = true;
        yield return new WaitForSeconds(duration);
        isTransitioning = false;
    }
}