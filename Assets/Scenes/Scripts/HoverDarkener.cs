using UnityEngine;
using UnityEngine.InputSystem;

public class HoverDarkener : MonoBehaviour
{
    [Header("Hover Settings")]
    public float darkenFactor = 0.7f; // O kolik ztmavit (0.7f = 70% pùvodní barvy)

    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private Color defaultColor;
    private bool isHovering = false;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (myCollider == null || spriteRenderer == null)
        {
            // Pro funkènost musí mít obì komponenty!
            Debug.LogError("CHYBA: HoverDarkener potøebuje SpriteRenderer a Collider2D!");
            enabled = false;
            return;
        }

        defaultColor = spriteRenderer.color;
    }

    void Update()
    {
        // Musíme manuálnì kontrolovat pozici kvùli New Input System a Collideru
        if (Mouse.current == null || Camera.main == null) return;

        // Získání aktuální pozice myši ve svìtových souøadnicích
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Kontrola pøekrytí
        bool isCurrentlyOver = myCollider.OverlapPoint(mouseWorldPos);

        // -- Stavový automat pro Hover --
        if (isCurrentlyOver && !isHovering)
        {
            // Myš najela
            OnHoverEnter();
        }
        else if (!isCurrentlyOver && isHovering)
        {
            // Myš odjela
            OnHoverExit();
        }
    }

    void OnHoverEnter()
    {
        isHovering = true;
        spriteRenderer.color = defaultColor * darkenFactor; // Ztmavíme
    }

    void OnHoverExit()
    {
        isHovering = false;
        spriteRenderer.color = defaultColor; // Reset
    }
}