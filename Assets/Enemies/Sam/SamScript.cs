using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamScript : MonoBehaviour
{
    [System.Serializable]
    public class Drawer
    {
        public string name;
        public GameObject clickCollider;
        public GameObject emptyOpenedVisual;
        public GameObject itemOpenedVisual;
    }

    [Header("Nastavení šancí")]
    public float checkInterval = 10f;
    [Range(0, 100)] public float appearanceChance = 20f;
    public float timeToFind = 15f;

    [Header("Vizuály Sama")]
    public GameObject samSpriteObject;
    public GameObject jumpscareObject;
    public GameObject darkenerObject; // TVŮJ DARKENER/BLACKOUT ČTVEREC

    [Header("Šuplíky (přiřaď všech 12)")]
    public List<Drawer> drawers = new List<Drawer>();

    [Header("Audio")]
    public AudioClip jumpscareSound;

    [Header("Reference")]
    public BaseNightManager nightManager;

    private int targetDrawerIndex = -1;
    private bool isSamActive = false;
    private bool isDead = false;
    private bool isInSuccessSequence = false;

    void Start()
    {
        if (nightManager == null) nightManager = Object.FindFirstObjectByType<BaseNightManager>();
        FullReset();
        InvokeRepeating("TryActivateSam", checkInterval, checkInterval);
    }

    void TryActivateSam()
    {
        if (isSamActive || isDead || isInSuccessSequence) return;
        if (Random.Range(0f, 100f) <= appearanceChance) ActivateSam();
    }

    void ActivateSam()
    {
        isSamActive = true;
        targetDrawerIndex = Random.Range(0, drawers.Count);
        if (samSpriteObject != null) samSpriteObject.SetActive(true);
        StartCoroutine(SamTimer());
    }

    IEnumerator SamTimer()
    {
        yield return new WaitForSeconds(timeToFind);
        if (isSamActive && !isDead && !isInSuccessSequence) StartCoroutine(PerformJumpscare());
    }

    public void PlayerClickedDrawer(GameObject clickedObject)
    {
        if (isDead) return;

        if (isSamActive && !isInSuccessSequence)
        {
            for (int i = 0; i < drawers.Count; i++)
            {
                if (drawers[i].clickCollider == clickedObject)
                {
                    if (i == targetDrawerIndex) StartCoroutine(SuccessSequence(i));
                    else if (drawers[i].emptyOpenedVisual != null) drawers[i].emptyOpenedVisual.SetActive(true);
                    return;
                }
            }
        }
        else if (isInSuccessSequence)
        {
            StartCoroutine(PerformJumpscare());
        }
    }

    IEnumerator SuccessSequence(int index)
    {
        isSamActive = false;
        isInSuccessSequence = true;
        if (samSpriteObject != null) samSpriteObject.SetActive(false);
        if (drawers[index].itemOpenedVisual != null) drawers[index].itemOpenedVisual.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        if (!isDead) { FullReset(); isInSuccessSequence = false; }
    }

    void FullReset()
    {
        foreach (var d in drawers)
        {
            if (d.emptyOpenedVisual != null) d.emptyOpenedVisual.SetActive(false);
            if (d.itemOpenedVisual != null) d.itemOpenedVisual.SetActive(false);
        }
        if (samSpriteObject != null) samSpriteObject.SetActive(false);
        if (jumpscareObject != null) jumpscareObject.SetActive(false);
    }

    IEnumerator PerformJumpscare()
    {
        isDead = true;
        if (jumpscareObject != null) jumpscareObject.SetActive(true);

        AudioSource source = GetComponent<AudioSource>();
        if (source != null && jumpscareSound != null) source.PlayOneShot(jumpscareSound);

        yield return new WaitForSecondsRealtime(1.5f);

        // KLÍČOVÝ FIX: Vypneme darkener před zobrazením tipu
        if (darkenerObject != null) darkenerObject.SetActive(false);

        nightManager.GameOver("Sam");
    }
}