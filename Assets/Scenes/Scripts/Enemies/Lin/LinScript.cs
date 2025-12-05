using UnityEngine;
using System.Collections;

public class LinScript : MonoBehaviour
{
    public string enemyName = "Lin";

    [Header("Lin Movement Settings")]
    public float moveInterval = 5f;
    [Range(0, 100)] public int moveChance = 30;
    public int finalKillPosition = 6;

    [Header("Kill Settings")]
    public float killTimerDuration = 3f;

    [Header("Visuals")]
    public GameObject windowUI;

    // EXTERNAL REFERENCES
    [Header("External References")]
    public CameraManager cameraManager;
    public HoldDoorLock doorLock;
    [Header("Game Manager")]
    // ZMÌNA: Používá generický BaseNightManager
    public BaseNightManager nightManager; // <-- PØIØAÏ: Night1Manager

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

        Debug.Log("Lin je pripravená v pozícii 0. Spúšam Move Routine.");
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

            // 1. Roll a 2. Roll logika (zùstává nezmìnìna)
            if (Random.Range(0, 100) < moveChance)
            {
                int nextPos = currentPosition;
                int pathRoll = Random.Range(0, 100);

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
                    Debug.Log($"Lin: Move chance successful! Nová pozícia: {currentPosition}");

                    if (cameraManager != null)
                    {
                        cameraManager.UpdateCameraView();
                    }
                }
            }
            else
            {
                Debug.Log($"{enemyName}: Move chance failed. Zostáva v pozícii {currentPosition}");
            }
        }

        Debug.Log($"{enemyName}: Dosažená pozícia {finalKillPosition}. Spouštím kill timer.");
        moveCoroutine = null;
        killCoroutine = StartCoroutine(KillRoutine());
    }

    private IEnumerator KillRoutine()
    {
        isAwaitingKill = true;
        isBlockedByPlayer = false;

        if (windowUI != null) windowUI.SetActive(true);

        Debug.Log($"Lin èeká {killTimerDuration}s na reakci hráèe.");
        yield return new WaitForSeconds(killTimerDuration);

        if (isAwaitingKill)
        {
            // Kontrola dveøí po èekání
            if (doorLock != null && doorLock.isDoorClosed)
            {
                // Dveøe jsou zavøené -> Lin utíká
                Debug.Log("Lin: Èekání vypršelo. Dveøe ZAVØENÉ. Utíkám z P6.");

                isAwaitingKill = false;
                isBlockedByPlayer = true;
                currentPosition = 1;

                if (moveCoroutine == null)
                {
                    moveCoroutine = StartCoroutine(MoveRoutine());
                }
            }
            else
            {
                // Dveøe jsou otevøené -> Jumpscare
                Debug.Log($"{enemyName}: JUMPSCARE! Dveøe OTEVØENÉ po vypršení èasu.");

                // VOLÁNÍ GAME OVER
                if (nightManager != null)
                {
                    nightManager.GameOver(enemyName);
                }
            }
        }

        if (windowUI != null) windowUI.SetActive(false);

        isAwaitingKill = false;
        killCoroutine = null;
    }

    public void StopKillRoutine()
    {
        if (isAwaitingKill)
        {
            if (killCoroutine != null)
            {
                StopCoroutine(killCoroutine);
                killCoroutine = null;
            }

            Debug.Log($"Lin Zablokovaná! Dveøe zavøeny. Vracím ji na pozici 1.");
            isAwaitingKill = false;
            isBlockedByPlayer = true;
            currentPosition = 1;

            if (moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(MoveRoutine());
            }

            if (cameraManager != null)
            {
                cameraManager.UpdateCameraView();
            }
        }
    }

    public void Unblock()
    {
        if (isBlockedByPlayer)
        {
            Debug.Log("Lin ODBLOKOVÁNA. Pokraèuje v pohybu.");
            isBlockedByPlayer = false;

            if (moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(MoveRoutine());
            }
        }
    }
}