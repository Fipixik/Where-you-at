using UnityEngine;
using System.Collections;

public class CatScript : MonoBehaviour
{
    public string enemyName = "Cat";

    [Header("Movement Settings")]
    public float moveInterval = 1f;
    [Range(0, 100)] public int moveChance = 8;

    [Header("Visual Objects (L/P)")]
    public GameObject[] leftSprites;
    public GameObject[] rightSprites;

    [Header("External")]
    public BaseNightManager nightManager;

    public int currentPosition = 0;
    public bool isOnLeft = true;
    private Coroutine moveCoroutine;

    private void Start()
    {
        ResetAllVisuals();
        UpdateVisuals();
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        int finalPosition = leftSprites.Length;

        while (currentPosition < finalPosition)
        {
            // --- ZMĚNA TADY: PANIC MODE ZRYCHLENÍ ---
            // Pokud PanicButton hlásí true, dělíme interval dvěma (zrychlení 2x)
            float actualInterval = PanicButton.IsPanicMode() ? moveInterval / 2f : moveInterval;
            yield return new WaitForSeconds(actualInterval);

            if (Random.Range(0, 100) < moveChance)
            {
                int nextPos = currentPosition + 1;
                isOnLeft = Random.value > 0.5f;

                // --- DYNAMICKÁ LOGIKA OHNĚ ---
                if (WoodAndStoveController.IsStoveBurning())
                {
                    if (nextPos >= finalPosition - 1)
                    {
                        Debug.Log($"{enemyName} narazila na žár u tvého prahu a utíká na start!");
                        currentPosition = 0;
                    }
                    else
                    {
                        currentPosition = nextPos;
                    }
                }
                else
                {
                    currentPosition = nextPos;
                }

                UpdateVisuals();
                Debug.Log($"{enemyName} je na pozici: {currentPosition}/{finalPosition} (Panic: {PanicButton.IsPanicMode()})");
            }
        }

        if (nightManager != null) nightManager.GameOver(enemyName);
    }

    private void UpdateVisuals()
    {
        ResetAllVisuals();
        if (currentPosition < leftSprites.Length && currentPosition < rightSprites.Length)
        {
            if (isOnLeft)
            {
                if (leftSprites[currentPosition] != null) leftSprites[currentPosition].SetActive(true);
            }
            else
            {
                if (rightSprites[currentPosition] != null) rightSprites[currentPosition].SetActive(true);
            }
        }
    }

    private void ResetAllVisuals()
    {
        foreach (GameObject go in leftSprites) if (go != null) go.SetActive(false);
        foreach (GameObject go in rightSprites) if (go != null) go.SetActive(false);
    }
}