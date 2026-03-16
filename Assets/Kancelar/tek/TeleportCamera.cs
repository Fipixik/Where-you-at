using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TeleportCamera : MonoBehaviour
{
    [Header("Souřadnice kamery")]
    public Vector3 targetCoordinates;

    [Header("Efekty")]
    public ScreenFader screenFader;
    public float blinkSpeed = 0.1f;

    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError($"Bote, na objektu {gameObject.name} ti chybí Collider2D!");
        }
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));

            if (myCollider != null && myCollider.OverlapPoint(worldPos))
            {
                StartCoroutine(QuickBlinkTeleport());
            }
        }
    }

    private IEnumerator QuickBlinkTeleport()
    {
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        // Teleport bez vypínání objektů
        Camera.main.transform.position = targetCoordinates;

        yield return new WaitForSeconds(blinkSpeed);

        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeToClear());
        }
    }
}