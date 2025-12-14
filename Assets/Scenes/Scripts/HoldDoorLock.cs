using UnityEngine;
using UnityEngine.InputSystem;

public class HoldDoorLock : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdTimeRequired = 3f; // Potøebná doba držení (3 sekundy)
    private float holdTimer = 0f;

    // Hlavní stav dveøí, který budeme používat pro logiku
    [HideInInspector] public bool isDoorClosed = false;

    [Header("Enemy Interaction")]
    public LinScript linEnemy;
    public LanScript lanEnemy; // <-- NOVÁ REFERENCE PRO LAN!

    [Header("Door Visuals (Toggle Objects)")]
    // Objekt, který ukazuje ZAVØENÉ dveøe.
    public GameObject closedDoorVisual;
    // Objekt, který ukazuje OTEVØENÉ dveøe.
    public GameObject openDoorVisual;

    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();

        // Zajištìní, že zaèínáme vizuálnì otevøené
        if (openDoorVisual != null) openDoorVisual.SetActive(true);
        if (closedDoorVisual != null) closedDoorVisual.SetActive(false);

        isDoorClosed = false;
    }

    void Update()
    {
        // 1. Zjistíme, jestli je levé tlaèítko stisknuté V TÉTO CHVÍLI
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Zjistíme, jestli se kurzor nachází nad naším Colliderem
            if (myCollider != null && myCollider.OverlapPoint(clickPosition))
            {
                // Zvyšujeme èasovaè, jen pokud už nejsou dveøe zavøené
                if (!isDoorClosed)
                {
                    holdTimer += Time.deltaTime;

                    // 2. Kontrola, zda uplynuly 3 sekundy
                    if (holdTimer >= holdTimeRequired)
                    {
                        CloseDoor();
                    }
                }
            }
            else
            {
                // Pokud hráè myš drží, ale odjel z Collideru, resetujeme
                if (holdTimer > 0 && !isDoorClosed)
                {
                    holdTimer = 0f;
                    // Debug.Log("Kurzor opustil trigger. Reset èasu.");
                }
            }
        }

        // 3. Kontrola, jestli bylo tlaèítko právì PUSŠTÌNO
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isDoorClosed)
            {
                OpenDoor();
            }
            else if (holdTimer > 0)
            {
                // Pokud hráè pustil pøed zavøením (pøed 3s), resetujeme èasovaè
                holdTimer = 0f;
                // Debug.Log("Pusštìno pøed zavøením. Reset èasu.");
            }
        }
    }

    void CloseDoor()
    {
        if (isDoorClosed) return;

        isDoorClosed = true;
        holdTimer = holdTimeRequired;

        // VIZUÁL: Zapni Zavøené, Vypni Otevøené
        if (closedDoorVisual != null) closedDoorVisual.SetActive(true);
        if (openDoorVisual != null) openDoorVisual.SetActive(false);

        Debug.Log("DVEØE ZAVØENY!");

        // KLÍÈOVÝ KROK 1: Øíkáme Lin, že je zablokovaná (útìk)
        if (linEnemy != null)
        {
            linEnemy.StopKillRoutine();
        }

        // KLÍÈOVÝ KROK 2: Øíkáme Lan, že se dveøe zavøely!
        // Lan: Jumpscare, pokud èeká na pozici 6.
        if (lanEnemy != null)
        {
            lanEnemy.DoorWasClosed();
            // Debug se vypíše ze skriptu LanScript.cs
        }
    }

    void OpenDoor()
    {
        if (!isDoorClosed) return;

        isDoorClosed = false;
        holdTimer = 0f;

        // VIZUÁL: Vypni Zavøené, Zapni Otevøené
        if (closedDoorVisual != null) closedDoorVisual.SetActive(false);
        if (openDoorVisual != null) openDoorVisual.SetActive(true);

        Debug.Log("DVEØE OTEVØENY.");

        // KLÍÈOVÝ KROK 1: Lin smí pokraèovat v pohybu.
        if (linEnemy != null)
        {
            linEnemy.Unblock();
        }

        // KLÍÈOVÝ KROK 2: Lan smí pokraèovat v pohybu (pokud pøedtím ustoupila po èasovém limitu).
        if (lanEnemy != null)
        {
            lanEnemy.Unblock();
        }
    }
}