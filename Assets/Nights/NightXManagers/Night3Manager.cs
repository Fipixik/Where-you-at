using UnityEngine;

public class Night3Manager : BaseNightManager
{
    [Header("--- ENEMY SCRIPTS ---")]
    public AlexandraScript alexandra;
    public LinScript lin;
    public LanScript lan;
    public CatScript cat;
    public HomuraScript homura;
    public ThiefScript santa;

    [Header("--- JUMPSCARE OBJEKTY (Ve scéně) ---")]
    public GameObject alexandraJumpscare;
    public GameObject linJumpscare;
    public GameObject lanJumpscare;
    public GameObject catJumpscare;
    public GameObject homuraJumpscare;
    public GameObject santaJumpscare;

    private void Awake()
    {
        // GOD MODE RYCHLOST - Všichni makají na max
        if (alexandra != null) { alexandra.moveChance = 10; alexandra.moveInterval = 3f; }
        if (lin != null) { lin.moveChance = 50; lin.moveInterval = 7f; }
        if (lan != null) { lan.moveChance = 50; lan.moveInterval = 7f; }
        if (cat != null) { cat.moveChance = 30; cat.moveInterval = 5f; }
        if (homura != null) { homura.moveChance = 60; homura.moveInterval = 5f; }
        if (santa != null) { santa.spawnChance = 20; santa.spawnInterval = 5f; }
    }

    protected override void Start()
    {
        base.Start();
        // Schováme jumpscare modely na začátku
        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);
        if (lanJumpscare != null) lanJumpscare.SetActive(false);
        if (catJumpscare != null) catJumpscare.SetActive(false);
        if (homuraJumpscare != null) homuraJumpscare.SetActive(false);
        if (santaJumpscare != null) santaJumpscare.SetActive(false);
    }

    public override void GameOver(string killerName)
    {
        if (gameEnded) return;

        Debug.Log($"GAME OVER! Vrah: {killerName}");

        // Aktivace modelu přímo do xichtu
        if (killerName == "Alexandra" && alexandraJumpscare != null) alexandraJumpscare.SetActive(true);
        else if (killerName == "Lin" && linJumpscare != null) linJumpscare.SetActive(true);
        else if (killerName == "Lan" && lanJumpscare != null) lanJumpscare.SetActive(true);
        else if (killerName == "Cat" && catJumpscare != null) catJumpscare.SetActive(true);
        else if (killerName == "Homura" && homuraJumpscare != null) homuraJumpscare.SetActive(true);
        else if (killerName == "Evil Santa" && santaJumpscare != null) santaJumpscare.SetActive(true);

        base.GameOver(killerName);
    }
}