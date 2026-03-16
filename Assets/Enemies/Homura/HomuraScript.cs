using UnityEngine;
using System.Collections;

public class HomuraScript : MonoBehaviour
{
    public string enemyName = "Homura";

    [Header("Movement Settings")]
    public float moveInterval = 1f;
    [Range(0, 100)] public int moveChance = 5;

    [Header("Visual Positions")]
    public GameObject[] positions;

    [Header("External")]
    public BaseNightManager nightManager;

    public int currentPosition = 0;
    private bool lastBurningState = false;

    private void Start()
    {
        ResetAllVisuals();
        UpdateVisuals();
        // Náhodný plíživý pohyb běží na pozadí
        StartCoroutine(RandomMoveRoutine());
    }

    private void Update()
    {
        // --- NOVINKA: RESET PŘI PANIC MODU ---
        // Pokud hráč zapnul Panic Mode (červené tlačítko)
        if (PanicButton.IsPanicMode())
        {
            // Pokud je Homura na úplně poslední nebo předposlední pozici (v tvém případě 8 nebo 9)
            if (currentPosition >= positions.Length - 2)
            {
                Debug.Log($"💥 {enemyName} byla vyhnána Panic Módem zpět na start!");
                currentPosition = 0;
                UpdateVisuals();
            }
        }

        // --- INSTANTNÍ REAKCE NA KRB (zůstává) ---
        bool isStoveBurning = WoodAndStoveController.IsStoveBurning();

        if (isStoveBurning && !lastBurningState)
        {
            Debug.Log($"🔥 {enemyName} slyšela krb a INSTANTNĚ skočila blíž!");
            MoveHomuraForward();
        }

        lastBurningState = isStoveBurning;
    }

    private IEnumerator RandomMoveRoutine()
    {
        while (currentPosition < positions.Length)
        {
            yield return new WaitForSeconds(moveInterval);

            if (Random.Range(0, 100) < moveChance)
            {
                MoveHomuraForward();
            }
        }
    }

    private void MoveHomuraForward()
    {
        currentPosition++;
        UpdateVisuals();

        if (currentPosition >= positions.Length)
        {
            Debug.Log($"{enemyName} tě dostala!");
            if (nightManager != null) nightManager.GameOver(enemyName);

            StopAllCoroutines();
            this.enabled = false;
        }
    }

    private void UpdateVisuals()
    {
        ResetAllVisuals();
        if (currentPosition < positions.Length && positions[currentPosition] != null)
        {
            positions[currentPosition].SetActive(true);
        }
    }

    private void ResetAllVisuals()
    {
        foreach (GameObject go in positions) if (go != null) go.SetActive(false);
    }
}