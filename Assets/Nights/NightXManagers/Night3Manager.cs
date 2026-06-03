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
    // Přidáno: Ada a Sam
    public AdaScript ada;
    public SamScript sam;

    [Header("--- JUMPSCARE OBJEKTY (Ve scéně) ---")]
    public GameObject alexandraJumpscare;
    public GameObject linJumpscare;
    public GameObject lanJumpscare;
    public GameObject catJumpscare;
    public GameObject homuraJumpscare;
    public GameObject santaJumpscare;
    // Jumpscary pro Adu a Sama (pokud mají modely)
    public GameObject adaJumpscare;
    public GameObject samJumpscare;

    private void Awake()
    {
        // Všude přidány checky "!= null" - pokud skript nepřiřadíš, prostě se tahle část přeskočí
        if (alexandra != null) { alexandra.moveChance = 40; alexandra.moveInterval = 2f; }
        if (lin != null) { lin.moveChance = 80; lin.moveInterval = 4f; }
        if (lan != null) { lan.moveChance = 80; lan.moveInterval = 4f; }
        if (cat != null) { cat.moveChance = 50; cat.moveInterval = 5f; }
        if (homura != null) { homura.moveChance = 50; homura.moveInterval = 2f; }
        if (santa != null) { santa.spawnChance = 20; santa.spawnInterval = 5f; }

        // Nastavení pro Adu a Sama (jen pokud jsou ve scéně)
        if (ada != null) { ada.moveChance = 20; ada.moveInterval = 2f; }
        if (sam != null) { sam.moveChance = 45; sam.moveInterval = 5f; }
    }

    protected override void Start()
    {
        base.Start();

        // Hromadné vypnutí jumpscarů s checkem na null
        if (alexandraJumpscare != null) alexandraJumpscare.SetActive(false);
        if (linJumpscare != null) linJumpscare.SetActive(false);
        if (lanJumpscare != null) lanJumpscare.SetActive(false);
        if (catJumpscare != null) catJumpscare.SetActive(false);
        if (homuraJumpscare != null) homuraJumpscare.SetActive(false);
        if (santaJumpscare != null) santaJumpscare.SetActive(false);
        if (adaJumpscare != null) adaJumpscare.SetActive(false);
        if (samJumpscare != null) samJumpscare.SetActive(false);
    }

    public override void GameOver(string killerName)
    {
        if (gameEnded) return;

        Debug.Log($"GAME OVER! Vrah: {killerName}");

        // Aktivace jumpscaru podle jména - bezpečně přes null check
        if (killerName == "Alexandra" && alexandraJumpscare != null) alexandraJumpscare.SetActive(true);
        else if (killerName == "Lin" && linJumpscare != null) linJumpscare.SetActive(true);
        else if (killerName == "Lan" && lanJumpscare != null) lanJumpscare.SetActive(true);
        else if (killerName == "Cat" && catJumpscare != null) catJumpscare.SetActive(true);
        else if (killerName == "Homura" && homuraJumpscare != null) homuraJumpscare.SetActive(true);
        else if (killerName == "Evil Santa" && santaJumpscare != null) santaJumpscare.SetActive(true);
        else if (killerName == "Ada" && adaJumpscare != null) adaJumpscare.SetActive(true);
        else if (killerName == "Sam" && samJumpscare != null) samJumpscare.SetActive(true);

        base.GameOver(killerName);
    }
}