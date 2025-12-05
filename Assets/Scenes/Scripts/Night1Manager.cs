using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// DÌDÍ Z BASE NIGHT MANAGERU
public class Night1Manager : BaseNightManager
{
    [Header("Night Settings")]
    public float nightDuration = 10f; // <-- TATO PROMÌNNÁ ØÍDÍ DÉLKU NOCI!

    [Header("Enemy References")]
    public AlexandraScript alexandra;
    public LinScript lin;

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
        // NASTAVENÍ OBTÍŽNOSTI PRO NOC 1 (Zùstává stejné)
        if (alexandra != null)
        {
            alexandra.moveChance = 50;
            alexandra.moveInterval = 1f;
            alexandra.killProgress = 100;
        }

        if (lin != null)
        {
            lin.moveChance = 100;
            lin.moveInterval = 7f;
            lin.killTimerDuration = 5f;
        }

        Debug.Log("Night 1 started!");
        StartCoroutine(RunNight());
    }

    private IEnumerator RunNight()
    {
        // KLÍÈOVÝ ØÁDEK: ÈEKÁ, DOKUD NEUPLYNE nightDuration
        yield return new WaitForSeconds(nightDuration);

        // WIN SEQUENCE
        if (screenFader != null)
        {
            screenFader.fadeDuration = fadeDuration;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        Debug.Log("Night 1 ended! Loading Menu...");
        PlayerPrefs.SetInt("CurrentNight", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuSceneName && screenFader != null)
            screenFader.ResetFade();
    }

    // FINAL GAME OVER FUNKCE (OVERRIDE z BaseNightManager)
    public override void GameOver(string killerName)
    {
        Debug.Log($"GAME OVER! Zabita: {killerName}. Spouštím návrat do menu.");
        StartCoroutine(EndGameTransition());
    }

    private IEnumerator EndGameTransition()
    {
        if (screenFader != null)
        {
            screenFader.fadeDuration = 0.5f;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        SceneManager.LoadScene(menuSceneName);
    }
}