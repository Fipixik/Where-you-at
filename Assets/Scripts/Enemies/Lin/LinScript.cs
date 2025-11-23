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
    public float killTimerDuration = 3f;     // TOTO JE TEÏ PEVNÁ DOBA ÈEKÁNÍ

    [Header("Visuals")]
    public GameObject windowUI;

    // TOTO JE NOVÉ: Externí Reference
    [Header("External References")]
    public CameraManager cameraManager;
    public HoldDoorLock doorLock;           // <-- NOVÁ REFERENCE: Pro kontrolu stavu dveøí

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

            // 1. Roll: Šance, že se Lin pohne (zùstává beze zmìny)
            if (Random.Range(0, 100) < moveChance)
            {
                int nextPos = currentPosition;
                int pathRoll = Random.Range(0, 100);

                // 2. Roll: ROZHODOVÁNÍ O CESTÌ (Tvé pravidla)
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

        // Lin dosáhla finální pozice
        Debug.Log($"{enemyName}: Dosažená pozícia {finalKillPosition}. Spouštím kill timer.");
        moveCoroutine = null;
        killCoroutine = StartCoroutine(KillRoutine());
    }

    private IEnumerator KillRoutine()
    {
        isAwaitingKill = true;
        isBlockedByPlayer = false;

        if (windowUI != null) windowUI.SetActive(true);

        // ZMÌNA: Pevnì èekáme X sekund. Bìhem této doby má hráè èas zavøít dveøe.
        Debug.Log($"Lin èeká {killTimerDuration}s na reakci hráèe.");
        yield return new WaitForSeconds(killTimerDuration);

        // TOTO JE HLAVNÍ KONTROLA PO VYPRŠENÍ ÈASU
        if (isAwaitingKill) // Kontrolujeme, zda Lin nebyla zablokována v prùbìhu èekání (kliknutím na dveøe)
        {
            // ZMÌNA: KONTROLA STAVU DVEØÍ AŽ NA KONCI
            if (doorLock != null && doorLock.isDoorClosed)
            {
                // Dveøe jsou zavøené -> Lin utíká
                Debug.Log("Lin: Èekání vypršelo. Dveøe ZAVØENÉ. Utíkám z P6.");

                // Znovu spustíme logiku útìku
                isAwaitingKill = false;
                isBlockedByPlayer = true;
                currentPosition = 1;

                // Obnovíme pohybový cyklus
                if (moveCoroutine == null)
                {
                    moveCoroutine = StartCoroutine(MoveRoutine());
                }
            }
            else
            {
                // Dveøe jsou otevøené -> Jumpscare
                Debug.Log($"{enemyName}: JUMPSCARE! Dveøe OTEVØENÉ po vypršení èasu.");
                // Logika pro GAME OVER
            }
        }

        if (windowUI != null) windowUI.SetActive(false);

        isAwaitingKill = false;
        killCoroutine = null;
    }

    // ZAVOLÁNO Z HoldDoorLock.cs, když hráè ZAVØE dveøe (pøed vypršením èasu)
    public void StopKillRoutine()
    {
        if (isAwaitingKill)
        {
            // OKAMŽITÉ ZASTAVENÍ TIMERU!
            if (killCoroutine != null)
            {
                StopCoroutine(killCoroutine);
                killCoroutine = null;
            }

            Debug.Log($"Lin Zablokovaná! Dveøe zavøeny. Vracím ji na pozici 1.");
            isAwaitingKill = false;
            isBlockedByPlayer = true;
            currentPosition = 1;

            // Obnovíme pohybovou rutinu (aby Lin stála zamrzlá na P1)
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

    // ZAVOLÁNO Z HoldDoorLock.cs, když hráè OTEVØE dveøe
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