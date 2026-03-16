using UnityEngine;
using System.Collections;

public class StoveLogic : MonoBehaviour
{
    [Header("UI a Sprity")]
    public GameObject woodInHand;
    public GameObject stoveOff;
    public GameObject stoveOn;

    [Header("Nastavení")]
    public float burnTime = 10f;

    public bool hasWood = false; // Teï je public, abys to vidìl v Inspectoru
    private bool isBurning = false;

    void Start()
    {
        woodInHand.SetActive(false);
        stoveOff.SetActive(true);
        stoveOn.SetActive(false);
    }

    // Tuhle funkci volá poleno - funguje jako vypínaè
    public void ToggleWood()
    {
        if (isBurning) return; // Pokud hoøí, se døevem nemanipuluj

        // OBRÁCENÍ STAVU: Pokud true -> false, pokud false -> true
        hasWood = !hasWood;

        // Podle toho zapneme/vypneme obrázek v ruce
        woodInHand.SetActive(hasWood);

        Debug.Log(hasWood ? "Vzal jsi døevo." : "Odložil jsi døevo.");
    }

    void OnMouseDown()
    {
        // Pøiložit mùžeme jen když máme døevo v ruce a nehoøíme
        if (hasWood && !isBurning)
        {
            StartCoroutine(BurnRoutine());
        }
    }

    IEnumerator BurnRoutine()
    {
        isBurning = true;
        hasWood = false; // Døevo zmizí z ruky, protože padlo do ohnì

        woodInHand.SetActive(false);
        stoveOff.SetActive(false);
        stoveOn.SetActive(true);

        yield return new WaitForSeconds(burnTime);

        stoveOff.SetActive(true);
        stoveOn.SetActive(false);
        isBurning = false;
    }
}