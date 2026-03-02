using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public abstract class BaseNightManager : MonoBehaviour
{
    [Header("--- NASTAVENÍ ÈASU (Base) ---")]
    public float lengthOfNight = 60f;
    public GameObject[] hourObjects;

    [Header("--- MUSICAL STRESS ---")]
    public AudioSource backgroundMusic;
    public float startPitch = 1.0f;
    public float maxPitch = 1.5f;

    [Header("--- SYSTÉM SMRTI A VÝHRY ---")]
    public GameObject blackoutSprite;
    public GameObject cameraUIButton;
    public ScreenFader screenFader;
    // PØIDÁNO: Reference na camera manager pro totální vypnutí
    public CameraManager cameraManager;

    [Header("--- JUMPSCARE AUDIO ---")]
    public AudioSource jumpscareSource;
    public AudioClip jumpscareSound;

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

        if (jumpscareSource == null) jumpscareSource = GetComponent<AudioSource>();

        // Zkusíme najít CameraManager, pokud jsi ho zapomnìla pøiøadit
        if (cameraManager == null) cameraManager = FindObjectOfType<CameraManager>();

        if (backgroundMusic != null)
        {
            backgroundMusic.pitch = startPitch;
            if (!backgroundMusic.isPlaying) backgroundMusic.Play();
        }
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

        if (backgroundMusic != null && timer <= lengthOfNight)
        {
            float progress = timer / lengthOfNight;
            backgroundMusic.pitch = Mathf.Lerp(startPitch, maxPitch, progress);
        }

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
            if (hourObjects[i] != null) hourObjects[i].SetActive(i == currentIndex);
        }
    }

    public virtual void GameOver(string killerName)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (backgroundMusic != null) backgroundMusic.Stop();

        if (jumpscareSource != null && jumpscareSound != null)
        {
            jumpscareSource.PlayOneShot(jumpscareSound);
        }

        // --- FIX PRO TLAÈÍTKO A KAMERY ---
        if (cameraUIButton != null) cameraUIButton.SetActive(false);

        // Vypneme celý displej kamer, aby tì to "vyhodilo" do kanclu k jumpscaru
        if (cameraManager != null && cameraManager.cameraDisplayPanel != null)
        {
            cameraManager.cameraDisplayPanel.SetActive(false);
        }

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
        if (backgroundMusic != null) backgroundMusic.Stop();
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