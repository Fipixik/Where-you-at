using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public abstract class BaseNightManager : MonoBehaviour
{
    [Header("--- NASTAVENÍ ÈASU (Base) ---")]
    public float lengthOfNight = 60f;   // Délka noci ve vteøinách

    // TADY JE ZMÌNA: Pole objektù místo Spritù
    [Tooltip("Sem naházej objekty hodin: Index 0 = 7am, Index 1 = 8am...")]
    public GameObject[] hourObjects;    // 5 objektù ve scénì (7, 8, 9, 10, 11)

    [Header("--- SYSTÉM SMRTI A VÝHRY ---")]
    public GameObject blackoutSprite;
    public GameObject cameraUIButton;
    public ScreenFader screenFader;

    [Header("--- DEATH TIPS ---")]
    public GameObject alexandraTipObject;
    public GameObject linTipObject;
    public GameObject santaTipObject;
    public GameObject lanTipObject;

    protected float timer = 0f;
    protected bool gameEnded = false;
    protected bool canClickToMenu = false;

    protected virtual void Start()
    {
        timer = 0f;
        // Na zaèátku ruènì zavoláme update, aby se zapla sedmièka a zbytek vypnul
        UpdateClockDisplay();
    }

    protected virtual void Update()
    {
        if (canClickToMenu && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Menu");
        }

        if (gameEnded) return;

        timer += Time.deltaTime;

        // Aktualizujeme hodiny (pøepínání objektù)
        UpdateClockDisplay();

        // Kontrola výhry (12 PM)
        if (timer >= lengthOfNight)
        {
            gameEnded = true;
            StartCoroutine(WinSequence());
        }
    }

    private void UpdateClockDisplay()
    {
        // Pokud nemáš nastavené objekty, nic nedìlej
        if (hourObjects == null || hourObjects.Length == 0) return;

        // 1. Spoèítáme, která "hodina" právì bìží (index 0 až 4)
        float hourDuration = lengthOfNight / 5.0f;
        int currentIndex = Mathf.FloorToInt(timer / hourDuration);

        // Pojistka, abychom nepøetekli (zùstaneme na poslední hodinì tìsnì pøed výhrou)
        currentIndex = Mathf.Clamp(currentIndex, 0, hourObjects.Length - 1);

        // 2. Projdeme všechny objekty hodin
        for (int i = 0; i < hourObjects.Length; i++)
        {
            if (hourObjects[i] != null)
            {
                // Pokud se index rovná aktuální hodinì -> ZAPNOUT. Jinak -> VYPNOUT.
                if (i == currentIndex)
                {
                    hourObjects[i].SetActive(true);
                }
                else
                {
                    hourObjects[i].SetActive(false);
                }
            }
        }
    }

    // --- ZBYTEK KÓDU (SMRT, VÝHRA) ZÙSTÁVÁ STEJNÝ ---

    public virtual void GameOver(string killerName)
    {
        gameEnded = true;
        if (cameraUIButton != null) cameraUIButton.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(DeathSequence(killerName));
    }

    private IEnumerator DeathSequence(string killerName)
    {
        yield return new WaitForSeconds(2.0f);

        if (screenFader != null)
        {
            screenFader.enabled = true;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }
        else if (blackoutSprite != null)
        {
            blackoutSprite.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);
        ShowTipObject(killerName);
        yield return new WaitForSeconds(0.5f);
        canClickToMenu = true;
    }

    private void ShowTipObject(string killer)
    {
        if (killer == "Alexandra" && alexandraTipObject != null) alexandraTipObject.SetActive(true);
        else if (killer == "Lin" && linTipObject != null) linTipObject.SetActive(true);
        else if (killer == "Lan" && lanTipObject != null) lanTipObject.SetActive(true);
        else if (killer == "Evil Santa" && santaTipObject != null) santaTipObject.SetActive(true);
    }

    public IEnumerator WinSequence()
    {
        Debug.Log("?? VÝHRA! 6:00 AM");
        if (cameraUIButton != null) cameraUIButton.SetActive(false);

        if (screenFader != null)
        {
            screenFader.enabled = true;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Menu");
    }
}