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
    public CameraManager cameraManager;

    [Header("--- WIN SETTINGS ---")]
    public AudioClip winSound;
    public GameObject winScreenObject;
    // Souøadnice natvrdo: -25, 0, -10
    private Vector3 winPos = new Vector3(-25f, 0f, -10f);

    [Header("--- JUMPSCARE AUDIO ---")]
    public AudioSource jumpscareSource;
    public AudioClip jumpscareSound;

    [Header("--- DEATH TIPS ---")]
    public GameObject alexandraTipObject;
    public GameObject linTipObject;
    public GameObject santaTipObject;
    public GameObject lanTipObject;
    public GameObject catTipObject;
    public GameObject homuraTipObject;

    protected float timer = 0f;
    protected bool gameEnded = false;
    protected bool canClickToMenu = false;

    protected virtual void Start()
    {
        Time.timeScale = 1f;
        timer = 0f;
        UpdateClockDisplay();
        if (jumpscareSource == null) jumpscareSource = GetComponent<AudioSource>();
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
            Time.timeScale = 1f;
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
        float hourDuration = lengthOfNight / 6.0f;
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
        Time.timeScale = 0f;

        if (backgroundMusic != null) backgroundMusic.Stop();
        if (jumpscareSource != null && jumpscareSound != null) jumpscareSource.PlayOneShot(jumpscareSound);

        if (cameraUIButton != null) cameraUIButton.SetActive(false);
        if (cameraManager != null && cameraManager.cameraDisplayPanel != null)
            cameraManager.cameraDisplayPanel.SetActive(false);

        StartCoroutine(DeathSequenceRealtime(killerName));
    }

    private IEnumerator DeathSequenceRealtime(string killerName)
    {
        yield return new WaitForSecondsRealtime(3.0f);
        if (screenFader != null)
        {
            screenFader.enabled = true;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }
        else if (blackoutSprite != null)
        {
            blackoutSprite.SetActive(true);
        }

        ShowTipObject(killerName);
        yield return new WaitForSecondsRealtime(0.8f);
        canClickToMenu = true;
    }

    protected virtual void ShowTipObject(string killer)
    {
        if (killer == "Alexandra" && alexandraTipObject != null) alexandraTipObject.SetActive(true);
        else if (killer == "Lin" && linTipObject != null) linTipObject.SetActive(true);
        else if (killer == "Lan" && lanTipObject != null) lanTipObject.SetActive(true);
        else if (killer == "Evil Santa" && santaTipObject != null) santaTipObject.SetActive(true);
        else if (killer == "Cat" && catTipObject != null) catTipObject.SetActive(true);
        else if (killer == "Homura" && homuraTipObject != null) homuraTipObject.SetActive(true);
    }

    public IEnumerator WinSequence()
    {
        Debug.Log("--- WIN! 6 AM Reached ---");
        if (backgroundMusic != null) backgroundMusic.Stop();

        // 1. Vypnutí ovládání
        if (cameraManager != null)
        {
            cameraManager.enabled = false;
            if (cameraManager.cameraDisplayPanel != null) cameraManager.cameraDisplayPanel.SetActive(false);
        }
        if (cameraUIButton != null) cameraUIButton.SetActive(false);

        // 2. Teleport na tvoje souøadnice
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = winPos;
            mainCam.transform.rotation = Quaternion.Euler(0, 0, 0); // Kouká rovnì
        }

        // 3. Efekty
        if (jumpscareSource != null && winSound != null) jumpscareSource.PlayOneShot(winSound);
        if (winScreenObject != null) winScreenObject.SetActive(true);

        // 4. Save progress
        int thisNightNumber = 3;
        int alreadySaved = PlayerPrefs.GetInt("SavedNight", 1);
        if (thisNightNumber >= alreadySaved)
        {
            PlayerPrefs.SetInt("SavedNight", thisNightNumber + 1);
            PlayerPrefs.Save();
        }

        yield return new WaitForSeconds(5.0f);

        if (screenFader != null)
        {
            screenFader.enabled = true;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        SceneManager.LoadScene("Menu");
    }
}