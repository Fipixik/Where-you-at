using UnityEngine;
using UnityEngine.InputSystem; // Pro kontrolu kliknutí v New Input System

public class CameraColliderTrigger : MonoBehaviour
{
    [Header("Camera Settings")]
    public int cameraID; // Kterou kameru tento Collider aktivuje (1 až 5)

    // Reference na Manager, který bude pøepínat vizuály
    public CameraManager cameraManager;

    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
            Debug.LogError("CHYBA! CameraColliderTrigger potøebuje Collider2D na GameObjectu!");
    }

    void Update()
    {
        // 1. Zkontrolujeme stisknutí levého tlaèítka myši (spolehlivá metoda)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Konverze Screen pozice na World pozici
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // 2. Zkontrolujeme, jestli klik zasáhl náš Collider
            if (myCollider != null && myCollider.OverlapPoint(clickPosition))
            {
                if (cameraManager != null)
                {
                    // 3. Spustíme pøepnutí kamery
                    cameraManager.SwitchCamera(cameraID);
                    Debug.Log($"Kliknuto na Collider pro kameru {cameraID}");
                }
                else
                {
                    Debug.LogError("Camera Manager není pøiøazen! Pøiøaï ho v Editoru.");
                }
            }
        }
    }
}