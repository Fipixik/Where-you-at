using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Night1Manager : BaseNightManager
{
    [Header("Night Settings")]
    public float nightDuration = 100f; // Délka noci

    [Header("Enemy References")]
    public AlexandraScript alexandra;
    public LinScript lin;

    [Header("Jumpscare Images (UI Panels)")]
    public GameObject alexandraJumpscare;
    public GameObject linJumpscare;

    [Header("Jumpscare Settings")]
    public float jumpscareDuration = 2f;

    [Header("Screen Fader")]
    public ScreenFader screenFader;

    [Header("Menu Scene")]
    public string menuSceneName = "menu";

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 1. ALEXANDRA
        if (alexandra != null)
        {
            alexandra.moveChance = 10;
            alexandra.moveInterval = 1f;
            alexandra.killProgress = 100;
        }

        // 2. LIN
        if (lin != null)
        {
            lin.moveChance = 25;
            lin.moveInterval = 5f;
            lin.killTimerDuration = 10f;
        }

        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);


        StartCoroutine(RunNight());
    }

    private IEnumerator RunNight()
    {
        yield return new WaitForSeconds(nightDuration);

        // WIN SEQUENCE
        if (screenFader != null)
        {
            screenFader.fadeDuration = fadeDuration;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        Debug.Log("Night 1 ended! Loading Menu...");

        // Odemkneme Noc 2
        PlayerPrefs.SetInt("CurrentNight", 2);
        PlayerPrefs.Save();

        SceneManager.LoadScene(menuSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuSceneName && screenFader != null)
            screenFader.ResetFade();
    }

    // --- GAME OVER
    public override void GameOver(string killerName)
    {
        Debug.Log($"GAME OVER! Zabita: {killerName}.");
        StopAllCoroutines(); // Zastaví čas
        StartCoroutine(JumpscareSequence(killerName));
    }

    private IEnumerator JumpscareSequence(string killerName)
    {
        // Zapneme správný obrázek
        if (killerName == "Alexandra" && alexandraJumpscare != null)
        {
            alexandraJumpscare.SetActive(true);
        }
        else if (killerName == "Lin" && linJumpscare != null)
        {
            linJumpscare.SetActive(true);
        }

        // Čekáme a děsíme hráče
        yield return new WaitForSeconds(jumpscareDuration);

        // Návrat do menu
        if (screenFader != null)
        {
            screenFader.fadeDuration = 0.5f;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }
        SceneManager.LoadScene(menuSceneName);
    }
}