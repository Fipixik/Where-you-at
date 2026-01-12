using UnityEngine;

public class Night1Manager : BaseNightManager
{
    // Reference jen na ty dva, co jsou v Noci 1
    public AlexandraScript alexandra;
    public LinScript lin;

    // Jumpscary jen pro tyhle dva
    public GameObject alexandraJumpscare;
    public GameObject linJumpscare;

    protected override void Start()
    {
        // 1. Spustíme počítání času (hodiny) z BaseManageru
        base.Start();

        // 2. ALEXANDRA (Agresivní)
        if (alexandra != null)
        {
            alexandra.moveChance = 40;     // Chodí často
            alexandra.moveInterval = 1.5f; // Rychlé intervaly
        }

        // 3. LIN (Pasivní / Méně aktivní)
        if (lin != null)
        {
            lin.moveChance = 15;      // Chodí málo
            lin.moveInterval = 3.0f;  // Pomalé intervaly
        }

        // 4. Vypnutí jumpscarů na startu (pro jistotu)
        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);
    }

    protected override void Update()
    {
        // Necháme běžet hodiny
        base.Update();
    }

    public override void GameOver(string killerName)
    {
        Debug.Log($"GAME OVER! Vrah: {killerName}");

        // Řešíme jen Alex a Lin. Nikoho jiného.
        if (killerName == "Alexandra" && alexandraJumpscare != null)
        {
            alexandraJumpscare.SetActive(true);
        }
        else if (killerName == "Lin" && linJumpscare != null)
        {
            linJumpscare.SetActive(true);
        }

        // Zbytek (tmu a tip) zařídí BaseManager
        base.GameOver(killerName);
    }
}