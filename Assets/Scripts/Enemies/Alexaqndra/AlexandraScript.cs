using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // <--- TOTO je to nové a dùležité!

public class AlexandraScript : MonoBehaviour
{
    public string enemyName = "Alexandra";
    [Range(0, 100)] public int moveChance = 10;
    public float moveInterval = 10f;
    public int killProgress = 100;

    [Range(0, 100)] public float progress;
    private bool isProgressing;

    public GameObject windowUI;

    private Collider2D myCollider;

    private void Awake()
    {
        // Zajistíme, že máme Collider2D
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            // Tuhle chybu neignoruj! Collider2D je potøeba!
            Debug.LogError("Chyba! AlexandraScript potøebuje Collider2D komponentu pro detekci kliku. Pøidej ji ve scénì!");
        }
    }

    private void Start()
    {
        if (windowUI != null)
            windowUI.SetActive(false);

        StartCoroutine(MoveRoutine());
    }

    // OPRAVENÁ FUNKCE: Používá nový Input System k detekci kliku na Collider
    private void Update()
    {
        // 1. Zkontrolujeme, jestli se levé tlaèítko stisklo V TOMTO FRAMU
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 2. Získání pozice myši ze systému Input System (Screen souøadnice)
            Vector2 clickPosition = Mouse.current.position.ReadValue();

            // 3. Konverze Screen souøadnic na World souøadnice (pro 2D Collider)
            Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

            // 4. Kontrola, jestli klik spadá do Collideru 2D
            if (myCollider != null && myCollider.OverlapPoint(worldClickPosition))
            {
                HandlePlayerClick();
            }
        }
    }

    IEnumerator MoveRoutine()
    {
        // Zbytek tvého kódu pro pohyb Alexandry, beze zmìny
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            int roll = Random.Range(0, 100);

            if (roll < moveChance)
            {
                Debug.Log($"{enemyName}: Chance success! Starting progress.");
                if (!isProgressing)
                    StartCoroutine(ProgressRoutine());
            }
            else
            {
                Debug.Log($"{enemyName}: Chance failed.");
            }
        }
    }

    IEnumerator ProgressRoutine()
    {
        // Zbytek tvého kódu pro progres, beze zmìny
        isProgressing = true;
        progress = 0;

        // Zobrazíme sprite pøi startu progresu
        if (windowUI != null)
            windowUI.SetActive(true);

        float speed = moveChance / 10f;

        while (progress < killProgress && isProgressing)
        {
            progress += speed * Time.deltaTime;
            Debug.Log($"{enemyName}: Progress {progress}/{killProgress}");
            yield return null;
        }

        if (progress >= killProgress)
        {
            Debug.Log($"{enemyName}: Kill progress reached!");
        }

        isProgressing = false;

        // Schováme sprite, když progres skonèí
        if (windowUI != null)
            windowUI.SetActive(false);
    }

    // Logika, která se spustí po kliknutí
    private void HandlePlayerClick()
    {
        Debug.Log($"{enemyName}: Progress stopped by player. (KLIK ZAREGISTROVÁN)");
        isProgressing = false;
        progress = 0;

        // Schováme sprite pøi kliknutí
        if (windowUI != null)
            windowUI.SetActive(false);
    }
}