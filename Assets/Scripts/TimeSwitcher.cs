using UnityEngine;
using System.Collections;

public class TimeSwitcher : MonoBehaviour
{
    [Header("NASTAVENÍ ČASU")]
    public float timeToWait = 3.0f; // Kolik vteřin bude objekt vidět

    [Header("OBJEKTY K INTERAKCI")]
    public GameObject objectToActivate;   // Ten, co se zapne potom
    public GameObject objectToDeactivate; // Ten, co má zmizet (pokud to není přímo tenhle)

    void Start()
    {
        // Jakmile se objekt aktivuje/spawnne, spustí se odpočet
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        // 1. Čekáme zadaný čas
        yield return new WaitForSeconds(timeToWait);

        // 2. Zapneme ten další objekt
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        // 3. Vypneme určený objekt
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
        }
        else
        {
            // Pokud jsi do kolonky nic nedal, vypne to automaticky samo sebe
            gameObject.SetActive(false);
        }

        Debug.Log("Čas vypršel, objekty prohozeny.");
    }
}