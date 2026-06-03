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

    [Header("--- NEPØÁTELÉ K VYPNUTÍ PØI VÝHØE ---")]
    public MonoBehaviour[] enemyScripts;

    [Header("--- SYSTÉM SMRTI A VÝHRY ---")]
    public GameObject blackoutSprite;
    public GameObject cameraUIButton;
    public ScreenFader screenFader;
    public CameraManager cameraManager;

    [Header("--- WIN SETTINGS ---")]
    public AudioClip winSound;
    public GameObject winScreenObject;
    protected Vector3 winPos = new Vector3(-25f, 0f, -10f);

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
    public GameObject adaTipObject;
    public GameObject samTipObject;

    protected float timer = 0f;
    protected bool gameEnded = false;
    protected bool canClickToMenu = false;

    protected virtual void Start()
    {
        Time.timeScale = 1f;
        timer = 0f;
        UpdateClockDisplay();

        if (jumpscareSource == null) jumpscareSource = GetComponent<AudioSource>();
        if (cameraManager == null) cameraManager = Object.FindFirstObjectByType<CameraManager>();

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

        if (timer >= lengthOfNight && !gameEnded)
        {
            gameEnded = true;
            StartCoroutine(WinSequence());
        }
    }

    protected void StopAllEnemies()
    {
        if (enemyScripts == null) return;
        foreach (var script in enemyScripts)
        {
            if (script != null) script.enabled = false;
        }
        Debug.Log("Všichni nepøátelé zastaveni.");
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

        if (killerName != "Ada" && killerName != "Sam")
        {
            if (jumpscareSource != null && jumpscareSound != null)
                jumpscareSource.PlayOneShot(jumpscareSound);
        }

        if (cameraUIButton != null) cameraUIButton.SetActive(false);
        if (cameraManager != null && cameraManager.cameraDisplayPanel != null)
            cameraManager.cameraDisplayPanel.SetActive(false);

        StartCoroutine(DeathSequenceRealtime(killerName));
    }

    private IEnumerator DeathSequenceRealtime(string killerName)
    {
        yield return new WaitForSecondsRealtime(2.0f);

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

    private void HideAllTips()
    {
        if (alexandraTipObject != null) alexandraTipObject.SetActive(false);
        if (linTipObject != null) linTipObject.SetActive(false);
        if (lanTipObject != null) lanTipObject.SetActive(false);
        if (santaTipObject != null) santaTipObject.SetActive(false);
        if (catTipObject != null) catTipObject.SetActive(false);
        if (homuraTipObject != null) homuraTipObject.SetActive(false);
        if (adaTipObject != null) adaTipObject.SetActive(false);
        if (samTipObject != null) samTipObject.SetActive(false);
    }

    protected virtual void ShowTipObject(string killer)
    {
        HideAllTips();
        switch (killer)
        {
            case "Alexandra": alexandraTipObject?.SetActive(true); break;
            case "Lin": linTipObject?.SetActive(true); break;
            case "Lan": lanTipObject?.SetActive(true); break;
            case "Evil Santa": santaTipObject?.SetActive(true); break;
            case "Cat": catTipObject?.SetActive(true); break;
            case "Homura": homuraTipObject?.SetActive(true); break;
            case "Ada": adaTipObject?.SetActive(true); break;
            case "Sam": samTipObject?.SetActive(true); break;
        }
    }

    protected virtual IEnumerator WinSequence()
    {
        StopAllEnemies();
        if (backgroundMusic != null) backgroundMusic.Stop();

        if (cameraManager != null)
        {
            cameraManager.enabled = false;
            if (cameraManager.cameraDisplayPanel != null) cameraManager.cameraDisplayPanel.SetActive(false);
        }
        if (cameraUIButton != null) cameraUIButton.SetActive(false);

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            var movement = mainCam.GetComponent("CameraFollowCursor") as MonoBehaviour;
            if (movement != null) movement.enabled = false;

            mainCam.transform.position = winPos;
            mainCam.transform.rotation = Quaternion.identity;
        }

        if (jumpscareSource != null && winSound != null) jumpscareSource.PlayOneShot(winSound);
        if (winScreenObject != null) winScreenObject.SetActive(true);

        // --- OPRAVENÁ LOGIKA UKLÁDÁNÍ ---
        // Naèteme aktuální noc (default 1)
        int currentNight = PlayerPrefs.GetInt("CurrentNight", 1);

        // Uložíme další noc (napø. po dohrání Night1 se uloží 2)
        PlayerPrefs.SetInt("CurrentNight", currentNight + 1);
        PlayerPrefs.Save();

        Debug.Log("Noc úspìšnì uložena. Pøíští noc bude: " + (currentNight + 1));
        // --------------------------------

        yield return new WaitForSeconds(5.0f);

        if (screenFader != null)
        {
            screenFader.enabled = true;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        SceneManager.LoadScene("Menu");
    }
}