using UnityEngine;
using UnityEngine.EventSystems;

public class CameraHoverZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Panel s kamerou")]
    public GameObject cameraPanel;
    public CameraFollowCursor camFollow;

    public CameraManager cameraManager;

    private bool isShowing = false;

    void Start()
    {
        isShowing = false;

        if (camFollow != null)
        {
            camFollow.canMove = true;
        }

        if (cameraManager != null)
        {
            cameraManager.DeactivateMonitor();
        }
    }

    // TATO FUNKCE SE SPUSTÍ, KDYŽ NAJEDEŠ MYŠÍ NA OBRÁZEK
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cameraPanel == null || camFollow == null || cameraManager == null) return;

        // Pokud monitor NENÍ dole (je nahoře), tak ho chceme dát dolů? 
        // Nebo to funguje jako přepínač (Toggle)?

        // Zde záleží, jak to chceš. Původní kód dělal Toggle.
        // Pokud to má být tlačítko: "Najedu -> Otevře se", "Odjedu -> Zavře se"?
        // Nebo "Kliknu -> Otevře se"?

        // Předpokládám, že chceš Toggle (Najedu = Změna stavu).

        isShowing = !isShowing;
        camFollow.canMove = !isShowing;

        if (isShowing)
        {
            cameraManager.ActivateMonitor();
        }
        else
        {
            cameraManager.DeactivateMonitor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Pokud bys chtěl, aby se to zavřelo, když myš odjede, přidej kód sem.
    }
}