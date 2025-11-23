using UnityEngine;
using UnityEngine.EventSystems;

public class CameraHoverZone : MonoBehaviour, IPointerEnterHandler
{
    [Header("UI Panel s kamerou")]
    public GameObject cameraPanel;
    public CameraFollowCursor camFollow;

    // NOVE: Reference na tvůj Camera Manager
    public CameraManager cameraManager;      // <-- PŘIDEJ TOTO

    private bool isShowing = false;

    private void Awake()
    {
        // Tady už NECHÁME cameraPanel schovaný, aktivuje ho Manager
        // if (cameraPanel != null) cameraPanel.SetActive(false); 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Nová kontrola, včetně CameraManageru
        if (cameraPanel == null || camFollow == null || cameraManager == null) return;

        // Toggle panel ON/OFF
        isShowing = !isShowing;

        // Původní logika pro zamykání kamery
        camFollow.canMove = !isShowing;

        // NOVÁ LOGIKA: Volání CameraManageru
        if (isShowing)
        {
            // Zvedáme monitor
            cameraManager.ActivateMonitor();
            Debug.Log("Jsi v kamerách!");
        }
        else
        {
            // Sklápíme monitor
            cameraManager.DeactivateMonitor();
            Debug.Log("Nejsi v kamerách!");
        }

        // POZOR! Původní řádek 'cameraPanel.SetActive(isShowing);' zde teď vynecháme, 
        // protože aktivaci panelu řídí CameraManager!
    }
}