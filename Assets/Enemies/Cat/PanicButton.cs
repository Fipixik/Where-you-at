using UnityEngine;
using UnityEngine.InputSystem;

public class PanicButton : MonoBehaviour
{
    public bool isRedButton;
    private static bool panicModeActive = false;

    private Collider2D col;

    // Statická funkce pro ostatní skripty
    public static bool IsPanicMode() { return panicModeActive; }

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Tvůj styl OverlapPoint + Input System
        if (col != null && col.OverlapPoint(mousePos) && Mouse.current.leftButton.wasPressedThisFrame)
        {
            panicModeActive = isRedButton; // Červené zapne (true), Zelené vypne (false)
            Debug.Log(panicModeActive ? "🚨 PANIC MODE ON!" : "✅ PANIC MODE OFF.");
        }
    }
}