using UnityEngine;
using UnityEngine.InputSystem;

public class EndingObject : MonoBehaviour
{
    // Statická proměnná, kterou uvidí Night5Manager
    public static int endingTriggered = 0;

    void Start()
    {
        // Na začátku noci vždy vynulovat
        endingTriggered = 0;
    }

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
                Debug.Log("Ending 1 sebral jsi předmět!");
                endingTriggered = 1;
                // Objekt zmizí, aby hráč věděl, že na něj klikl
                gameObject.SetActive(false);
            }
        }
    }
}