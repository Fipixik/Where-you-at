using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraTeleportBack : MonoBehaviour
{
    [Header("Souřadnice kamery")]
    public Vector3 targetCoordinates;

    [Header("Efekty")]
    public ScreenFader screenFader;
    public float blinkSpeed = 0.05f;

    private Collider2D myCollider;
    private bool isTransitioning = false;

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
        // Pokud už se teleportujeme nebo nemáme fader, nic nedělej
        if (isTransitioning || screenFader == null) return;

        // 1. Získáme pozici myši
        Vector2 mouseInput = Mouse.current.position.ReadValue();

        // 2. Přepočítáme ji na světové souřadnice (Z=10 aby to trefilo 2D rovinu)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));

        // 3. Kontrola, jestli je myš nad colliderem (Hover)
        if (myCollider != null && myCollider.OverlapPoint(worldPos))
        {
            StartCoroutine(QuickBlinkAndTeleport());
        }
    }

    private IEnumerator QuickBlinkAndTeleport()
    {
        isTransitioning = true; // Zámek, aby se to nespustilo 100x za sekundu

        // Fade do černé
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        // TELEPORT kamery na tvoje zadaná čísla
        Camera.main.transform.position = targetCoordinates;

        yield return new WaitForSeconds(blinkSpeed);

        // Fade zpět do obrazu
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeToClear());
        }

        // Malý cooldown, než dovolíme další teleport (aby tě to neházelo furt tam a zpět)
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }
}