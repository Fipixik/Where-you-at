using UnityEngine;
using System.Collections;

// Důležité: Tento skript musí být na objektu Lan a musí mít přiřazený doorLock
public class LanScript : MonoBehaviour
{
    public string enemyName = "Lan"; // <--- Jméno nastaveno na Lan

    [Header("Movement Settings")]
    public float moveInterval = 6f;
    [Range(0, 100)] public int moveChance = 40;
    public int finalKillPosition = 6;

    [Header("Kill Settings")]
    public float killTimerDuration = 4f;

    [Header("Visuals")]
    public GameObject windowUI; // Vizuál upozornění před jumpscare (pokud se ukáže u dveří)

    // EXTERNAL REFERENCES
    [Header("External References")]
    public CameraManager cameraManager;
    public HoldDoorLock doorLock; // Dveřní mechanismus
    [Header("Game Manager")]
    public BaseNightManager nightManager;

    // Interné stavy
    public int currentPosition = 0;
    private bool isAwaitingKill = false;
    private bool isBlockedByPlayer = false;

    private Coroutine killCoroutine;
    private Coroutine moveCoroutine;

    private void Start()
    {
        if (windowUI != null)
            windowUI.SetActive(false);

        Debug.Log($"[{enemyName}] Start. Spúšťam Move Routine.");
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (currentPosition < finalKillPosition)
        {
            yield return new WaitForSeconds(moveInterval);

            if (isBlockedByPlayer)
            {
                continue;
            }

            if (Random.Range(0, 100) < moveChance)
            {
                int nextPos = currentPosition;
                int pathRoll = Random.Range(0, 100);

                // Používáme stejnou cestu jako Lin (0-5)
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
                    Debug.Log($"[{enemyName}] Move successful! Nová pozícia: {currentPosition}");

                    if (cameraManager != null)
                    {
                        cameraManager.UpdateCameraView();
                    }
                }
            }
        }

        Debug.Log($"[{enemyName}] Dosažená pozícia {finalKillPosition}. Spouštím kill timer.");
        moveCoroutine = null;
        killCoroutine = StartCoroutine(KillRoutine());
    }

    private IEnumerator KillRoutine()
    {
        isAwaitingKill = true;

        if (windowUI != null) windowUI.SetActive(true);

        Debug.Log($"[{enemyName}] Čeká {killTimerDuration}s. DVEŘE MUSÍ ZŮSTAT OTEVŘENÉ!");
        yield return new WaitForSeconds(killTimerDuration);

        // KONTROLA PO VYPRŠENÍ ČASU: Dveře musí být OTEVŘENÉ
        if (isAwaitingKill)
        {
            if (doorLock != null && !doorLock.isDoorClosed)
            {
                // Dveře jsou OTEVŘENÉ -> Lan odchází (DOBRÝ STAV)
                Debug.Log($"[{enemyName}] Čekání vypršelo. Dveře OTEVŘENÉ. Utíkám z P6.");

                isAwaitingKill = false;
                isBlockedByPlayer = true;
                currentPosition = 1;

                if (windowUI != null) windowUI.SetActive(false);

                if (moveCoroutine == null)
                {
                    moveCoroutine = StartCoroutine(MoveRoutine());
                }
            }
            else
            {
                // Dveře jsou ZAVŘENÉ po vypršení času -> Jumpscare
                Debug.Log($"[{enemyName}]: JUMPSCARE! Dveře ZAVŘENÉ po vypršení času.");

                if (windowUI != null) windowUI.SetActive(false);

                if (nightManager != null)
                {
                    nightManager.GameOver(enemyName);
                }
            }
        }

        isAwaitingKill = false;
        killCoroutine = null;
    }

    // Volá se z HoldDoorLock.cs, když hráč zavře dveře.
    public void DoorWasClosed()
    {
        if (isAwaitingKill)
        {
            // Kontrola: Pokud jsou dveře ZAVŘENÉ a Lan čeká, okamžitý Jumpscare
            if (doorLock != null && doorLock.isDoorClosed)
            {
                Debug.Log($"[{enemyName}]: 💥 JUMPSCARE OKAMŽITĚ! Dveře zavřeny, když čekala!");

                if (windowUI != null) windowUI.SetActive(false);

                if (killCoroutine != null)
                {
                    StopCoroutine(killCoroutine);
                    killCoroutine = null;
                }

                if (nightManager != null)
                {
                    nightManager.GameOver(enemyName);
                }
            }
        }
    }

    // Tato funkce se volá, když hráč odblokuje Lin (otevřením dveří), což by Lan nemělo blokovat
    public void Unblock()
    {
        if (isBlockedByPlayer)
        {
            Debug.Log($"[{enemyName}] ODBLOKOVÁNA. Pokračuje v pohybu.");
            isBlockedByPlayer = false;

            if (moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(MoveRoutine());
            }
        }
    }
}