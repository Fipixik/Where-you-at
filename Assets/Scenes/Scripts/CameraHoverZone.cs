using UnityEngine;
using UnityEngine.EventSystems;

public class CameraHoverZone : MonoBehaviour, IPointerEnterHandler
{
    [Header("UI Panel s kamerou")]
    public GameObject cameraPanel;
    public CameraFollowCursor camFollow;

    public CameraManager cameraManager;      // <-- Zde je tvůj Manager

    private bool isShowing = false;

    // TATO FUNKCE ZAJIŠŤUJE, ŽE PO ZNOVUNAČTENÍ SCÉNY JE VŠE OK
    void Start()
    {
        // 1. Nastaví, že monitor je na 100% dole (isShowing = false)
        isShowing = false;

        // 2. Zajistí, že se kamera HÝBE
        if (camFollow != null)
        {
            camFollow.canMove = true;
        }

        // 3. Pošle signál Manageru, aby byl v klidu (vypnul vizuály)
        if (cameraManager != null)
        {
            cameraManager.DeactivateMonitor();
        }

        Debug.Log("HoverZone resetována. Kamera se hýbe.");
    }

    private void Awake()
    {
        // Původní kód. Už nemusí volat SetActive(false), to dělá DeactivateMonitor.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cameraPanel == null || camFollow == null || cameraManager == null) return;

        // Toggle panel ON/OFF
        isShowing = !isShowing;

        // Původní logika pro zamykání kamery
        camFollow.canMove = !isShowing;

        // Volání CameraManageru
        if (isShowing)
        {
            cameraManager.ActivateMonitor();
            Debug.Log("Jsi v kamerách!");
        }
        else
        {
            cameraManager.DeactivateMonitor();
            Debug.Log("Nejsi v kamerách!");
        }
    }
}