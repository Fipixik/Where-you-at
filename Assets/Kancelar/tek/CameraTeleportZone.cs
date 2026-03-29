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
    public AdaScript adaScript;

    private bool isTransitioning = false;
    private bool mouseInside = false;

    private void Start()
    {
        // Kontrola hned po spuštìní hry
        if (adaScript == null)
        {
            Debug.LogError($"<color=red>Chyba na objektu {gameObject.name}: Nemáš pøiøazenou Adu v políèku Ada Script!</color>");
        }

        if (mainCameraTransform != null)
        {
            mainCameraTransform.position = new Vector3(0f, 0f, mainCameraTransform.position.z);
            if (objectToToggle != null) objectToToggle.SetActive(true);
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
                Debug.Log($"<color=white>TeleportTrigger: Detekován hover myši na {gameObject.name}. Spouštím teleport.</color>");
                StartCoroutine(PerformTeleport());
            }
        }
        else { mouseInside = false; }
    }

    private IEnumerator PerformTeleport()
    {
        isTransitioning = true;
        float currentY = Mathf.Round(mainCameraTransform.position.y);

        if (currentY == 0f && objectToToggle != null) objectToToggle.SetActive(false);

        yield return StartCoroutine(screenFader.FadeToBlackAndWait());

        if (currentY == 0f)
        {
            // Jdeme DOZADU (Pozice 2)
            mainCameraTransform.position = new Vector3(0f, 12f, mainCameraTransform.position.z);
            Debug.Log("<color=orange>ZONE: Kamera pøepnuta na Y=12 (DOZADU). Posílám info Adì.</color>");
            if (adaScript != null) adaScript.OnPlayerMoved(true);
        }
        else
        {
            // Jdeme DOPØEDU (Pozice 1)
            mainCameraTransform.position = new Vector3(0f, 0f, mainCameraTransform.position.z);
            if (objectToToggle != null) objectToToggle.SetActive(true);
            Debug.Log("<color=green>ZONE: Kamera pøepnuta na Y=0 (DOPØEDU). Posílám info Adì.</color>");
            if (adaScript != null) adaScript.OnPlayerMoved(false);
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