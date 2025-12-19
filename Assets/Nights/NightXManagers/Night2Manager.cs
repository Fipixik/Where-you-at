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
    public LanScript lan;          // Lan (AssistantScript)

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
    public ScreenFader screenFader; // <-- KRITICKÁ REFERENCE PRO FADE

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
        // --- NASTAVENÍ OBTÍŽNOSTI PRO NOC 2 ---

        // 1. ALEXANDRA (Harder)
        if (alexandra != null)
        {
            alexandra.moveChance = 100;
            alexandra.moveInterval = 10f;
            alexandra.killProgress = 5;
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
            evilSanta.spawnChance = 100;
            evilSanta.spawnInterval = 5f;
            evilSanta.killTimerDuration = 5f;
        }

        // 4. LAN (ASISTENTKA)
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
        // Zde byla hlášena chyba (řádek 118). 
        // LogError se vypíše, ale nebrání spuštění další sekvence (JumpscareSequence).
        Debug.LogError($"GAME OVER! Zabita: {killerName}. Spouštím Jumpscare.");

        // Zastavíme pohyb všech nepřátel (aby to nevypadalo divně)
        StopAllCoroutines();

        // Spustíme sekvenci s obrázkem
        StartCoroutine(JumpscareSequence(killerName));
    }

    private IEnumerator JumpscareSequence(string killerName)
    {
        // 1. Zjistíme, kdo zabil, a zapneme jeho obrázek

        // Vypneme všechny, pokud náhodou něco zůstalo aktivní (Double Check)
        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);
        if (santaJumpscare != null) santaJumpscare.SetActive(false);
        if (lanJumpscare != null) lanJumpscare.SetActive(false);

        // Zapneme správný obrázek
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
        else if (killerName == "Lan" && lanJumpscare != null)
        {
            lanJumpscare.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Neznámý vrah ('{killerName}') nebo chybí reference na Jumpscare obrázek!");
        }

        // 2. Čekáme (aby hráč ten obrázek viděl)
        yield return new WaitForSeconds(jumpscareDuration);

        // 3. POMALÝ FADE OUT (Tato část zajistí plynulý přechod)
        if (screenFader != null)
        {
            screenFader.fadeDuration = gameOverFadeDuration; // Použijeme delší fade (2.0f)
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }
        else
        {
            Debug.LogError("Screen Fader není přiřazen! Přecházím rovnou do menu.");
        }

        SceneManager.LoadScene(menuSceneName);
    }
}