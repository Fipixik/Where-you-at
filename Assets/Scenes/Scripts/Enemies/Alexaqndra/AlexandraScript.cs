using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class AlexandraScript : MonoBehaviour
{
    public string enemyName = "Alexandra";
    [Range(0, 100)] public int moveChance = 10;
    public float moveInterval = 10f;
    public int killProgress = 100;

    [Range(0, 100)] public float progress;
    private bool isProgressing;

    public GameObject windowUI; // Pùvodní vizuál progresu

    [Header("External Manager")]
    public Night1Manager nightManager;

    // TOTO JE KLÍÈOVÝ VIZUÁL A STAV
    [Header("Kill State Vizuál")]
    public GameObject killStateUI; // <-- PØIØAÏ: Nový jumpscare vizuál (SetActive(false) na zaèátku)
    private bool isKillStateReached = false; // Flag pro finální stav

    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("Chyba! AlexandraScript potøebuje Collider2D komponentu pro detekci kliku. Pøidej ji ve scénì!");
        }
    }

    private void Start()
    {
        if (windowUI != null)
            windowUI.SetActive(false);

        progress = 0; // Fix: Inicializace progresu je jen zde na zaèátku noci!
        isKillStateReached = false;
        if (killStateUI != null) killStateUI.SetActive(false);

        StartCoroutine(MoveRoutine());
    }

    // VEØEJNÁ VLASTNOST: CameraManager kontroluje, zda se má Game Over spustit
    public bool IsInKillState()
    {
        return isKillStateReached;
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 clickPosition = Mouse.current.position.ReadValue();
            Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

            if (myCollider != null && myCollider.OverlapPoint(worldClickPosition))
            {
                HandlePlayerClick();
            }
        }
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            int roll = Random.Range(0, 100);

            // Bìžná logika: Nesmí progresovat, když už je v kill state
            if (roll < moveChance && !isKillStateReached)
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
        isProgressing = true;

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
            // FÁZE 2: VSTUP DO KILL STATE - NENÍ CESTY ZPÌT KLIKNUTÍM!
            isKillStateReached = true;

            // PÙVODNÍ UI se vypne, Nové UI (JUMPSCARE VIZUÁL) se zapne
            if (windowUI != null) windowUI.SetActive(false);
            if (killStateUI != null) killStateUI.SetActive(true);

            Debug.Log($"{enemyName}: Kill progress reached! Finální hrozba je aktivní a èeká na stáhnutí monitoru.");

            // Progres dosáhl cíle, ale NENÍ Game Over. Game Over je v CameraManageru.
        }

        isProgressing = false; // Tím se umožní MoveRoutine spustit to znovu

        // Vypneme pùvodní vizuál
        if (windowUI != null)
            windowUI.SetActive(false);
    }

    private void HandlePlayerClick()
    {
        // TOTO JE KLÍÈOVÝ FIX
        if (isKillStateReached)
        {
            // FÁZE 2: KILL STATE - KLIKNUTÍ NEDÌLÁ NIC!
            Debug.Log($"{enemyName}: Alexandra je v kill state. Kliknutí nefunguje.");
        }
        else // FÁZE 1: BÌŽNÝ PROGRES - Obrana funguje (Pøed 100%)
        {
            Debug.Log($"{enemyName}: Progress paused by player. (KLIK ZAREGISTROVÁN)");
            isProgressing = false;
        }

        // Vypneme pùvodní vizuál
        if (windowUI != null)
            windowUI.SetActive(false);
    }
}