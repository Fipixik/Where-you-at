using UnityEngine;
using UnityEngine.InputSystem;

public class SamDrawerClick : MonoBehaviour
{
    private SamScript samManager;

    void Start()
    {
        samManager = Object.FindFirstObjectByType<SamScript>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Collider2D col = GetComponent<Collider2D>();

            if (col != null && col.OverlapPoint(mousePos))
            {
                if (samManager != null)
                {
                    samManager.PlayerClickedDrawer(this.gameObject);
                }
            }
        }
    }
}