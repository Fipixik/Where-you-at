using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Night1Manager : MonoBehaviour
{
    [Header("Night Settings")]
    public float nightDuration = 10f;

    [Header("Enemy References")]
    public AlexandraScript alexandra; // <-- PØIØAÏ: Objekt Alexandry
    public LinScript lin; // <-- PØIØAÏ: Objekt Lin

    [Header("Screen Fader")]
    public ScreenFader screenFader;

    [Header("Menu Scene")]
    public string menuSceneName = "menu";

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    // ZDE BYL SLOT PRO VIDEO PLAYER

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
        // NASTAVENÍ OBTÍŽNOSTI PRO NOC 1
        if (alexandra != null)
        {
            // Pùvodní parametry pro Noc 1: Alexandra
            alexandra.moveChance = 50;
            alexandra.moveInterval = 1f;
            alexandra.killProgress = 100;
        }

        if (lin != null)
        {
            // Parametry pro Noc 1: Lin (PØEPISUJÍ HODNOTY V INSPECTORU!)
            lin.moveChance = 20;
            lin.moveInterval = 7f;
            lin.killTimerDuration = 4f;
        }

        Debug.Log("Night 1 started!");
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
        PlayerPrefs.SetInt("CurrentNight", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuSceneName && screenFader != null)
            screenFader.ResetFade();
    }

    // FINAL GAME OVER FUNKCE: Spustí se po jumpscare!
    public void GameOver(string killerName)
    {
        Debug.Log($"GAME OVER! Zabita: {killerName}. Spouštím návrat do menu.");
        // Jdeme rovnou na fade
        StartCoroutine(EndGameTransition());
    }

    private IEnumerator EndGameTransition()
    {
        // Rychlý fade (bez èekání na video)
        if (screenFader != null)
        {
            screenFader.fadeDuration = 0.5f;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        SceneManager.LoadScene(menuSceneName);
    }
}