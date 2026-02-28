using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public abstract class BaseNightManager : MonoBehaviour
{
    [Header("--- NASTAVENÍ ÈASU (Base) ---")]
    public float lengthOfNight = 60f;

    [Tooltip("Sem naházej objekty hodin: Index 0 = 7am, Index 1 = 8am...")]
    public GameObject[] hourObjects;

    [Header("--- SYSTÉM SMRTI A VÝHRY ---")]
    public GameObject blackoutSprite;
    public GameObject cameraUIButton;
    public ScreenFader screenFader;

    [Header("--- JUMPSCARE AUDIO ---")]
    public AudioSource audioSource;      // Komponenta AudioSource
    public AudioClip jumpscareSound;     // Ten hnusnej zvuk jumpscaru

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
        UpdateClockDisplay();

        // Automaticky zkusíme najít AudioSource, pokud není pøiøazen
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if (canClickToMenu && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Menu");
        }

        if (gameEnded) return;

        timer += Time.deltaTime;
        UpdateClockDisplay();

        if (timer >= lengthOfNight)
        {
            gameEnded = true;
            StartCoroutine(WinSequence());
        }
    }

    private void UpdateClockDisplay()
    {
        if (hourObjects == null || hourObjects.Length == 0) return;

        float hourDuration = lengthOfNight / 5.0f;
        int currentIndex = Mathf.FloorToInt(timer / hourDuration);
        currentIndex = Mathf.Clamp(currentIndex, 0, hourObjects.Length - 1);

        for (int i = 0; i < hourObjects.Length; i++)
        {
            if (hourObjects[i] != null)
            {
                hourObjects[i].SetActive(i == currentIndex);
            }
        }
    }

    public virtual void GameOver(string killerName)
    {
        if (gameEnded) return; // Aby se jumpscare nespustil víckrát
        gameEnded = true;

        // --- ZDE SE SPOUŠTÍ JUMPSCARE ZVUK ---
        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }
        // ------------------------------------

        if (cameraUIButton != null) cameraUIButton.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(DeathSequence(killerName));
    }

    private IEnumerator DeathSequence(string killerName)
    {
        // Poèkáme chvíli, než se ukáže jumpscare vizuál (pokud ho máš ve skriptech monster)
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
        Debug.Log("VÝHRA! 6:00 AM");
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