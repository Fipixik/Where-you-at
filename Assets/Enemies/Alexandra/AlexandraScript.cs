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
    public BaseNightManager nightManager;

    [Header("Kill State Vizuál")]
    public GameObject killStateUI; // <-- PØIØAÏ: Nový jumpscare vizuál
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

        progress = 0;
        isKillStateReached = false;
        if (killStateUI != null) killStateUI.SetActive(false);

        StartCoroutine(MoveRoutine());
    }

    public bool IsInKillState()
    {
        return isKillStateReached;
    }

    // TATO FUNKCE ZAPNE VIZUÁL AŽ PØED GAME OVER

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

        // TOTO JE FINÁLNÍ ZMÌNA LOGIKY: Nastavení flagu PØED ukonèením rutiny
        if (progress >= killProgress)
        {
            isKillStateReached = true;
            killStateUI.SetActive(true);
            Debug.Log($"{enemyName}: Kill progress reached! Finální hrozba je aktivní a èeká na stáhnutí monitoru.");
        }

        isProgressing = false;

        if (windowUI != null)
            windowUI.SetActive(false);
    }

    private void HandlePlayerClick()
    {
        if (isKillStateReached)
        {
            Debug.Log($"{enemyName}: Alexandra je v kill state. Kliknutí nefunguje.");
        }
        else // FÁZE 1: BÌŽNÝ PROGRES - Obrana funguje (Pøed 100%)
        {
            Debug.Log($"{enemyName}: Progress paused by player. (KLIK ZAREGISTROVÁN)");
            isProgressing = false;
        }

        if (windowUI != null)
            windowUI.SetActive(false);
    }
}