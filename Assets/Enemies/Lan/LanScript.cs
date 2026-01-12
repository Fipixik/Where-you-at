using UnityEngine;
using System.Collections;

public class LanScript : MonoBehaviour
{
    public string enemyName = "Lan";

    [Header("Movement Settings")]
    public float moveInterval = 6f;
    [Range(0, 100)] public int moveChance = 40;
    public int finalKillPosition = 6;

    [Header("Kill Settings")]
    public float killTimerDuration = 4f;

    [Header("Visuals")]
    public GameObject windowUI; // Upozornění u dveří

    // --- EXTERNAL REFERENCES ---
    [Header("External References")]
    public CameraManager cameraManager;
    public HoldDoorLock doorLock;

    [Header("Game Manager")]
    public BaseNightManager nightManager;

    // Interní stavy
    public int currentPosition = 0;
    private bool isAwaitingKill = false;
    private bool isBlockedByPlayer = false;

    private Coroutine killCoroutine;
    private Coroutine moveCoroutine;

    private void Start()
    {
        if (windowUI != null) windowUI.SetActive(false);

        Debug.Log($"[{enemyName}] Start. Spouštím Move Routine.");
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (currentPosition < finalKillPosition)
        {
            yield return new WaitForSeconds(moveInterval);

            if (isBlockedByPlayer) continue;

            if (Random.Range(0, 100) < moveChance)
            {
                int nextPos = currentPosition;
                int pathRoll = Random.Range(0, 100);

                // Logika pohybu (stejná jako u Lin)
                switch (currentPosition)
                {
                    case 0: nextPos = (pathRoll < 50) ? 1 : 3; break;
                    case 1: nextPos = (pathRoll < 50) ? 2 : 3; break;
                    case 2: nextPos = (pathRoll < 25) ? 1 : 4; break;
                    case 3: nextPos = (pathRoll < 75) ? 4 : 1; break;
                    case 4:
                        if (pathRoll < 50) nextPos = 5;
                        else if (pathRoll < 75) nextPos = 2;
                        else nextPos = 3; break;
                    case 5: nextPos = finalKillPosition; break;
                }

                if (nextPos != currentPosition)
                {
                    currentPosition = nextPos;
                    Debug.Log($"[{enemyName}] POHYB! Nová pozice: {currentPosition}");

                    // 🔥 DŮLEŽITÉ: Tady říkáme Manageru, ať aktualizuje fotky!
                    if (cameraManager != null)
                    {
                        cameraManager.UpdateCameraView();
                    }
                }
            }
        }

        Debug.Log($"[{enemyName}] Je u dveří (Pos {finalKillPosition}).");
        moveCoroutine = null;
        killCoroutine = StartCoroutine(KillRoutine());
    }

    private IEnumerator KillRoutine()
    {
        isAwaitingKill = true;

        if (windowUI != null) windowUI.SetActive(true);

        Debug.Log($"[{enemyName}] Čeká {killTimerDuration}s. Dveře musí být OTEVŘENÉ!");
        yield return new WaitForSeconds(killTimerDuration);

        // KONTROLA PO VYPRŠENÍ ČASU
        if (isAwaitingKill)
        {
            // Pokud jsou dveře OTEVŘENÉ -> Lan odchází
            if (doorLock != null && !doorLock.isDoorClosed)
            {
                Debug.Log($"[{enemyName}] ÚSPĚCH. Dveře otevřené, Lan odchází.");

                isAwaitingKill = false;
                isBlockedByPlayer = true;
                currentPosition = 1; // Reset na pozici 1

                // Vypneme UI okna
                if (windowUI != null) windowUI.SetActive(false);

                // 🔥 AKTUALIZACE KAMERY (Aby zmizel z okna i vizuálně)
                if (cameraManager != null) cameraManager.UpdateCameraView();

                // Restart pohybu
                if (moveCoroutine == null)
                {
                    moveCoroutine = StartCoroutine(MoveRoutine());
                }
            }
            else
            {
                // Dveře jsou ZAVŘENÉ -> Smrt
                Debug.Log($"[{enemyName}]: JUMPSCARE! Dveře byly zavřené.");
                Jumpscare();
            }
        }

        isAwaitingKill = false;
        killCoroutine = null;
    }

    // Volá se, když hráč ZAVŘE dveře (Lan to nesnáší)
    public void DoorWasClosed()
    {
        if (isAwaitingKill)
        {
            if (doorLock != null && doorLock.isDoorClosed)
            {
                Debug.Log($"[{enemyName}]: 💥 OKAMŽITÝ JUMPSCARE! Zavřel jsi jí před nosem!");
                Jumpscare();
            }
        }
    }

    public void Unblock()
    {
        if (isBlockedByPlayer)
        {
            Debug.Log($"[{enemyName}] Odblokována, pokračuje.");
            isBlockedByPlayer = false;

            if (moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(MoveRoutine());
            }
        }
    }

    void Jumpscare()
    {
        if (windowUI != null) windowUI.SetActive(false);
        if (killCoroutine != null) StopCoroutine(killCoroutine);

        if (nightManager != null)
        {
            nightManager.GameOver(enemyName);
        }
    }
}