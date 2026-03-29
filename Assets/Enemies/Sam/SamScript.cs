using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamScript : MonoBehaviour
{
    [System.Serializable]
    public class Drawer
    {
        public string name;
        public GameObject clickCollider;    // Na co hráč kliká
        public GameObject emptyOpenedVisual; // Otevřený prázdný šuplík
        public GameObject itemOpenedVisual;  // Otevřený šuplík s věcmi
    }

    [Header("Nastavení šancí")]
    public float checkInterval = 10f; // Y vteřin
    [Range(0, 100)] public float appearanceChance = 20f; // X šance
    public float timeToFind = 15f; // Z vteřin na nalezení

    [Header("Šuplíky (přiřaď všech 12)")]
    public List<Drawer> drawers = new List<Drawer>();

    [Header("Jumpscare")]
    public GameObject jumpscareObject;
    public AudioClip jumpscareSound;

    [Header("Reference")]
    public BaseNightManager nightManager;

    private int targetDrawerIndex = -1;
    private bool isSamActive = false;
    private bool isDead = false;

    void Start()
    {
        if (nightManager == null) nightManager = Object.FindFirstObjectByType<BaseNightManager>();

        // Vše schovat na startu
        foreach (var d in drawers)
        {
            if (d.emptyOpenedVisual != null) d.emptyOpenedVisual.SetActive(false);
            if (d.itemOpenedVisual != null) d.itemOpenedVisual.SetActive(false);
        }
        if (jumpscareObject != null) jumpscareObject.SetActive(false);

        InvokeRepeating("TryActivateSam", checkInterval, checkInterval);
    }

    void TryActivateSam()
    {
        if (isSamActive || isDead || nightManager == null) return;

        if (Random.Range(0f, 100f) <= appearanceChance)
        {
            ActivateSam();
        }
    }

    void ActivateSam()
    {
        isSamActive = true;
        targetDrawerIndex = Random.Range(0, drawers.Count);
        Debug.Log("<color=purple>SAM: Aktivní! Hledá věci v šuplíku: " + drawers[targetDrawerIndex].name + "</color>");

        StartCoroutine(SamTimer());
    }

    IEnumerator SamTimer()
    {
        yield return new WaitForSeconds(timeToFind);

        if (isSamActive && !isDead)
        {
            StartCoroutine(PerformJumpscare());
        }
    }

    // Tuto funkci zavoláme skrze pomocný skript na každém šuplíku
    public void PlayerClickedDrawer(GameObject clickedObject)
    {
        if (!isSamActive || isDead) return;

        for (int i = 0; i < drawers.Count; i++)
        {
            if (drawers[i].clickCollider == clickedObject)
            {
                if (i == targetDrawerIndex)
                {
                    // TREFIL TO!
                    Debug.Log("<color=cyan>SAM: Hráč našel správný šuplík!</color>");
                    StartCoroutine(SuccessSequence(i));
                }
                else
                {
                    // ŠPATNÝ ŠUPLÍK
                    Debug.Log("SAM: Špatný šuplík (" + drawers[i].name + "), hledej dál.");
                    if (drawers[i].emptyOpenedVisual != null) drawers[i].emptyOpenedVisual.SetActive(true);
                }
                break;
            }
        }
    }

    IEnumerator SuccessSequence(int index)
    {
        isSamActive = false; // Sam je pryč

        // Ukážeme šuplík s věcmi
        if (drawers[index].itemOpenedVisual != null) drawers[index].itemOpenedVisual.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        // Po 3s vše smazat (schovat)
        foreach (var d in drawers)
        {
            if (d.emptyOpenedVisual != null) d.emptyOpenedVisual.SetActive(false);
            if (d.itemOpenedVisual != null) d.itemOpenedVisual.SetActive(false);
        }

        targetDrawerIndex = -1;
        Debug.Log("SAM: Všechny šuplíky zavřeny, Sam resetován.");
    }

    IEnumerator PerformJumpscare()
    {
        isDead = true;
        if (jumpscareObject != null) jumpscareObject.SetActive(true);

        AudioSource source = GetComponent<AudioSource>();
        if (source != null && jumpscareSound != null) source.PlayOneShot(jumpscareSound);

        yield return new WaitForSecondsRealtime(1.5f);
        nightManager.GameOver("Sam");
    }
}