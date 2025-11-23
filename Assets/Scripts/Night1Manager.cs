using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Night1Manager : MonoBehaviour
{
    [Header("Night Settings")]
    public float nightDuration = 10f; // délka noci v sekundách

    [Header("Enemy References")]
    public AlexandraScript alexandra; // nepøítel Alexandra

    [Header("Screen Fader")]
    public ScreenFader screenFader;

    [Header("Menu Scene")]
    public string menuSceneName = "menu";

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    void Awake()
    {
        // Registrace callbacku pro naètení scény
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Odregistrace callbacku
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (alexandra != null)
        {
            // Parametry pro Night 1
            alexandra.moveChance = 50;       // šance, že se pohne
            alexandra.moveInterval = 1f;   // interval mezi pohyby
            alexandra.killProgress = 100;    // kolik je potøeba progressu pro zabití
        }

        Debug.Log("Night 1 started!");
        StartCoroutine(RunNight());
    }

    private IEnumerator RunNight()
    {
        // èeká, dokud noc neskonèí
        yield return new WaitForSeconds(nightDuration);

        // fade do èerna
        if (screenFader != null)
        {
            screenFader.fadeDuration = fadeDuration;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        Debug.Log("Night 1 ended! Loading Menu...");

        // uloží, že noc skonèila
        PlayerPrefs.SetInt("CurrentNight", 1);
        PlayerPrefs.Save();

        // naète menu
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // reset fade po naètení menu
        if (scene.name == menuSceneName && screenFader != null)
            screenFader.ResetFade();
    }

    // Volitelná metoda pro test v editoru
    public void FinishNight()
    {
        StartCoroutine(GoToMenu());
    }

    private IEnumerator GoToMenu()
    {
        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());

        PlayerPrefs.SetInt("CurrentNight", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(menuSceneName);
    }
}
