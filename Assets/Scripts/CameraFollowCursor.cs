using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // jen pokud máš nový Input System
#endif

public class CameraFollowCursor : MonoBehaviour
{
    public float maxSpeed = 10f; // maximální rychlost
    public float edgeSize = 50f; // oblast od okraje obrazovky
    public float minX = -10f; // levá hranice kamery
    public float maxX = 10f;  // pravá hranice kamery

    [Header("Enable/Disable movement")]
    public bool canMove = true; // <-- nový flag

    void Update()
    {
        if (!canMove) return; // pokud false, kamera se nepohybuje

        Vector3 pos = transform.position;
        float speed = 0f;

        // Získat pozici myši
        Vector2 mousePos;
#if ENABLE_INPUT_SYSTEM
        mousePos = Mouse.current.position.ReadValue();
#else
        mousePos = Input.mousePosition;
#endif

        if (mousePos.x > Screen.width - edgeSize)
        {
            float distance = mousePos.x - (Screen.width - edgeSize);
            speed = maxSpeed * (distance / edgeSize);
        }
        else if (mousePos.x < edgeSize)
        {
            float distance = edgeSize - mousePos.x;
            speed = -maxSpeed * (distance / edgeSize);
        }

        pos.x += speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }
}
