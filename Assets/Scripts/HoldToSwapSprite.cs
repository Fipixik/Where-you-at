using UnityEngine;
using UnityEngine.InputSystem; // for the new Input System

public class HoldToSwapSprite : MonoBehaviour
{
    [Header("Sprites")]
    public GameObject openSpriteObject;   // Drag the GameObject showing the open sprite
    public GameObject closedSpriteObject; // Drag the GameObject showing the closed sprite

    [Header("Settings")]
    public float holdTime = 0.5f;         // How long to hold before swapping

    private float timer = 0f;
    private bool isClosed = false;

    void Start()
    {
        // Ensure the correct default visibility
        if (openSpriteObject != null) openSpriteObject.SetActive(true);
        if (closedSpriteObject != null) closedSpriteObject.SetActive(false);
    }

    void Update()
    {
        if (openSpriteObject == null || closedSpriteObject == null)
            return; // safety check

        // Check if left mouse button is held
        if (Mouse.current.leftButton.isPressed)
        {
            timer += Time.deltaTime;

            if (!isClosed && timer >= holdTime)
            {
                openSpriteObject.SetActive(false);
                closedSpriteObject.SetActive(true);
                isClosed = true;
            }
        }
        else // button released
        {
            timer = 0f;

            if (isClosed)
            {
                openSpriteObject.SetActive(true);
                closedSpriteObject.SetActive(false);
                isClosed = false;
            }
        }
    }
}
