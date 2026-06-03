using UnityEngine;
using UnityEngine.InputSystem;

public class UniversalSwitcher : MonoBehaviour
{
    [Header("Objekty k vypnutí")]
    public GameObject objectToDeactivate1;
    public GameObject objectToDeactivate2;

    [Header("Objekt k zapnutí")]
    public GameObject objectToActivate;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 mousePos2D = new Vector2(worldPos.x, worldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                PerformSwitch();
            }
        }
    }

    void PerformSwitch()
    {
        // Deaktivace prvního objektu
        if (objectToDeactivate1 != null)
            objectToDeactivate1.SetActive(false);

        // Deaktivace druhého objektu
        if (objectToDeactivate2 != null)
            objectToDeactivate2.SetActive(false);

        // Aktivace nového objektu
        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        Debug.Log("Switch proveden: 2 objekty vypnuty, 1 zapnut.");
    }
}