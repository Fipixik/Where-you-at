using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// DĚDÍ Z BASE NIGHT MANAGERU
public class Night2Manager : BaseNightManager
{
    [Header("Night Settings")]
    public float nightDuration = 120f; // Např. 2 minuty na Noc 2

    [Header("Enemy References")]
    public AlexandraScript alexandra;
    public LinScript lin;
    public ThiefScript evilSanta; // Evil Santa (ThiefScript)
    public LanScript lan;         // Lan (AssistantScript)

    [Header("Jumpscare Images (UI Panels)")]
    // Sem přetáhneš GameObjekty s obrázky přes celou obrazovku
    public GameObject alexandraJumpscare;
    public GameObject linJumpscare;
    public GameObject santaJumpscare;
    public GameObject lanJumpscare; // NOVÝ OBRÁZEK PRO LAN

    [Header("Jumpscare Settings")]
    public float jumpscareDuration = 2.5f; // Jak dlouho obrázek visí

    [Header("Fade Settings")]
    public float fadeDuration = 2f; // Fade pro WIN (pomalý)
    private float gameOverFadeDuration = 2.0f; // Pomalý fade pro Game Over, jak jsi chtěl/a

    [Header("Screen Fader")]
    public ScreenFader screenFader;

    [Header("Menu Scene")]
    public string menuSceneName = "menu";

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
        // 1. ALEXANDRA (Harder)
        if (alexandra != null)
        {
            alexandra.moveChance = 60;
            alexandra.moveInterval = 1.5f;
            alexandra.killProgress = 100;
        }

        // 2. LIN (Harder)
        if (lin != null)
        {
            lin.moveChance = 90;
            lin.moveInterval = 6f;
            lin.killTimerDuration = 5f;
        }

        // 3. EVIL SANTA (THIEF)
        if (evilSanta != null)
        {
            evilSanta.spawnChance = 50;
            evilSanta.spawnInterval = 12f;
            evilSanta.killTimerDuration = 8f;
        }

        // 4. LAN (ASISTENTKA) - Obrácená Logika
        if (lan != null)
        {
            lan.moveChance = 50;
            lan.moveInterval = 10f;
            lan.killTimerDuration = 5f;
            Debug.Log("👩‍💼 Lan je aktivní.");
        }

        // Vypneme všechny jumpscare obrázky na startu
        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);
        if (santaJumpscare != null) santaJumpscare.SetActive(false);
        if (lanJumpscare != null) lanJumpscare.SetActive(false);

        Debug.Log("Night 2 started! (All Enemies Active)");
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

        Debug.Log("Night 2 ended! Good job.");
        PlayerPrefs.SetInt("CurrentNight", 3);
        PlayerPrefs.Save();
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuSceneName && screenFader != null)
            screenFader.ResetFade();
    }

    // --- FINAL GAME OVER FUNKCE ---
    public override void GameOver(string killerName)
    {
        Debug.LogError($"GAME OVER! Zabita: {killerName}. Spouštím Jumpscare.");

        // Zastavíme pohyb všech nepřátel (aby to nevypadalo divně)
        StopAllCoroutines();

        // Spustíme sekvenci s obrázkem
        StartCoroutine(JumpscareSequence(killerName));
    }

    private IEnumerator JumpscareSequence(string killerName)
    {
        // 1. Zjistíme, kdo zabil, a zapneme jeho obrázek
        if (killerName == "Alexandra" && alexandraJumpscare != null)
        {
            alexandraJumpscare.SetActive(true);
        }
        else if (killerName == "Lin" && linJumpscare != null)
        {
            linJumpscare.SetActive(true);
        }
        else if (killerName == "Evil Santa" && santaJumpscare != null)
        {
            santaJumpscare.SetActive(true);
        }
        else if (killerName == "Lan" && lanJumpscare != null) // NOVÁ KONTROLA PRO LAN
        {
            lanJumpscare.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Neznámý vrah ('{killerName}') nebo chybí reference na Jumpscare obrázek!");
        }

        // 2. Čekáme (aby hráč ten obrázek viděl)
        yield return new WaitForSeconds(jumpscareDuration);

        // 3. POMALÝ FADE OUT (jak jsi chtěl/a) a návrat do menu
        if (screenFader != null)
        {
            screenFader.fadeDuration = gameOverFadeDuration; // Použijeme delší fade (2.0f)
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        SceneManager.LoadScene(menuSceneName);
    }
}