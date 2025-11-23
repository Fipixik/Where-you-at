using UnityEngine;
using UnityEngine.EventSystems;

public class CameraHoverZone : MonoBehaviour, IPointerEnterHandler
{
    [Header("UI Panel s kamerou")]
    public GameObject cameraPanel;          // UI panel / obrázek kamery
    public CameraFollowCursor camFollow;    // hlavní kamera s CameraFollowCursor

    private bool isShowing = false;

    private void Awake()
    {
        if (cameraPanel != null)
            cameraPanel.SetActive(false); // začíná skrytý
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cameraPanel == null || camFollow == null) return;

        // Toggle panel ON/OFF
        isShowing = !isShowing;
        cameraPanel.SetActive(isShowing);

        // Pokud je panel zobrazený → kamera se nepohybuje, jinak pohyb povolen
        camFollow.canMove = !isShowing;

        // Debug zpráva
        if (isShowing)
            Debug.Log("Jsi v kamerách!");
        else
            Debug.Log("Nejsi v kamerách!");
    }
}
