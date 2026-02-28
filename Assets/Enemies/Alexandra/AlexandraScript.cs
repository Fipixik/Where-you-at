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

    public GameObject windowUI;

    [Header("External Manager")]
    public BaseNightManager nightManager;

    [Header("Kill State Vizuál")]
    public GameObject killStateUI;
    private bool isKillStateReached = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip killStateSound;   // Zvuk, když Alexandra vyhraje (kill state)

    [Space]
    public AudioClip[] defenseSounds;  // POLE PRO TVOJE 2 ZVUKY OBRANY (když na ni klikneš)

    private bool hasPlayedKillSound = false;

    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("Chyba! AlexandraScript potřebuje Collider2D!");
        }
    }

    private void Start()
    {
        if (windowUI != null) windowUI.SetActive(false);
        progress = 0;
        isKillStateReached = false;
        hasPlayedKillSound = false;
        if (killStateUI != null) killStateUI.SetActive(false);
        StartCoroutine(MoveRoutine());
    }

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

            if (roll < moveChance && !isKillStateReached)
            {
                if (!isProgressing)
                    StartCoroutine(ProgressRoutine());
            }
        }
    }

    IEnumerator ProgressRoutine()
    {
        isProgressing = true;
        if (windowUI != null) windowUI.SetActive(true);
        float speed = moveChance / 10f;

        while (progress < killProgress && isProgressing)
        {
            progress += speed * Time.deltaTime;
            yield return null;
        }

        if (progress >= killProgress)
        {
            isKillStateReached = true;
            if (killStateUI != null) killStateUI.SetActive(true);

            if (!hasPlayedKillSound && audioSource != null && killStateSound != null)
            {
                audioSource.PlayOneShot(killStateSound);
                hasPlayedKillSound = true;
            }
        }

        isProgressing = false;
        if (windowUI != null) windowUI.SetActive(false);
    }

    private void HandlePlayerClick()
    {
        if (isKillStateReached)
        {
            Debug.Log($"{enemyName}: Alexandra je v kill state. Kliknutí už nepomůže.");
        }
        else
        {
            // --- LOGIKA PRO NÁHODNÝ ZVUK OBRANY ---
            if (audioSource != null && defenseSounds != null && defenseSounds.Length > 0)
            {
                // Vybere náhodný index z pole (0 nebo 1, pokud tam dáš dva zvuky)
                int randomIndex = Random.Range(0, defenseSounds.Length);
                audioSource.PlayOneShot(defenseSounds[randomIndex]);
            }
            // --------------------------------------

            Debug.Log($"{enemyName}: Zaháníme Alexandru náhodným zvukem!");
            isProgressing = false;
            progress = 0;
            hasPlayedKillSound = false;
        }

        if (windowUI != null) windowUI.SetActive(false);
    }
}