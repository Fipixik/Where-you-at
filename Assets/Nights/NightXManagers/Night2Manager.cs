using UnityEngine;

public class Night2Manager : BaseNightManager
{
    [Header("--- ENEMY SCRIPTS ---")]
    public AlexandraScript alexandra;
    public LinScript lin;
    public LanScript lan;

    // Tady používáme tvůj původní ThiefScript pro Santu
    public ThiefScript santa;

    [Header("--- JUMPSCARE OBJEKTY (Ve scéně) ---")]
    public GameObject alexandraJumpscare;
    public GameObject linJumpscare;
    public GameObject lanJumpscare;
    public GameObject santaJumpscare;

    protected override void Start()
    {
        // 1. Spustíme systém hodin (7-12) z BaseManageru
        base.Start();

        // 2. NASTAVENÍ OBTÍŽNOSTI PRO NOC 2

        // Alexandra (Klasika)
        if (alexandra != null)
        {
            alexandra.moveChance = 30;
            alexandra.moveInterval = 1.5f;
        }

        // Lin (Začíná být aktivnější)
        if (lin != null)
        {
            lin.moveChance = 25;
            lin.moveInterval = 2.0f;
        }

        // Lan (Nováček - střední obtížnost)
        if (lan != null)
        {
            lan.moveChance = 100;
            lan.moveInterval = 5.0f;
        }

        // Evil Santa (ThiefScript - používáme jeho proměnné)
        if (santa != null)
        {
            // ThiefScript používá 'spawnChance' a 'spawnInterval'
            santa.spawnChance = 15;
            santa.spawnInterval = 3.0f;
        }

        // 3. Pro jistotu vypneme všechny jumpscary na startu
        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);
        if (lanJumpscare != null) lanJumpscare.SetActive(false);
        if (santaJumpscare != null) santaJumpscare.SetActive(false);
    }

    protected override void Update()
    {
        // Tohle musí běžet, aby fungoval čas a výhra
        base.Update();
    }

    public override void GameOver(string killerName)
    {
        Debug.Log($"GAME OVER! Vrah: {killerName}");

        // --- 1. NUCLEAR FIX PRO TLAČÍTKO ---
        // Místo vypnutí ho zničíme, aby ho CameraManager nemohl znova zapnout
        if (cameraUIButton != null)
        {
            Destroy(cameraUIButton);
        }

        // --- 2. ZAPNUTÍ JUMPSCARU ---
        if (killerName == "Alexandra" && alexandraJumpscare != null)
        {
            alexandraJumpscare.SetActive(true);
        }
        else if (killerName == "Lin" && linJumpscare != null)
        {
            linJumpscare.SetActive(true);
        }
        else if (killerName == "Lan" && lanJumpscare != null)
        {
            lanJumpscare.SetActive(true);
        }
        // Jméno v ThiefScriptu musí být "Evil Santa"
        else if (killerName == "Evil Santa" && santaJumpscare != null)
        {
            santaJumpscare.SetActive(true);
        }

        // --- 3. SPUŠTĚNÍ ZBYTKU SEKVENCE (Blackout, Tip) ---
        base.GameOver(killerName);
    }
}