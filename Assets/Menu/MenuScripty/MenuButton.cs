using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MenuButtonNew : MonoBehaviour
{
    public string action; // "new" "continue"  "exit"
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private ScreenFader screenFader;

    [Header("Hover Sprite Fade")]
    [SerializeField] private SpriteRenderer hoverSprite; //sprite pro hover
    [SerializeField] private float fadeSpeed = 120f; //rychlost fadu

    private Collider2D col;
    private float targetAlpha = 0f;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogWarning("mbutton potrebuje Collider2D");

        if (hoverSprite != null)
        {
            Color c = hoverSprite.color;
            c.a = 0f;
            hoverSprite.color = c; 
        }
    }

    private void Update()
    {
        if (col == null || hoverSprite == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // jakej sprite zobrazit
        targetAlpha = col.OverlapPoint(mousePos) ? 1f : 0f;

        // sprite fade
        Color color = hoverSprite.color;
        color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        hoverSprite.color = color;

        // Click check
        if (col.OverlapPoint(mousePos) && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(FadeAndPerformAction());
        }
    }

    private IEnumerator FadeAndPerformAction()
    {
        yield return screenFader.FadeToBlackAndWait();

        switch (action)
        {
            case "new":
                if (menuManager != null)
                    menuManager.PlayGame(true);
                break;
            case "continue":
                if (menuManager != null)
                    menuManager.PlayGame(false);
                break;
            case "exit":
                if (menuManager != null)
                    menuManager.ExitGame();
                break;
            default:
                Debug.LogWarning("Unknown action: " + action);
                break;
        }
    }
}
