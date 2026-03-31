using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    [Header("Obrazy")]
    public SpriteRenderer[] storyImages;
    public SpriteRenderer blackOverlay;

    [Header("Nastavení Blackoutu")]
    public int changeAfterIndex = 3;
    public Vector3 newOverlayPosition;
    public Vector3 newOverlayScale = Vector3.one;

    [Header("Nastavení Scény")]
    public float fadeSpeed = 1.0f; // Snížil jsem rychlost pro lepší testování (1.0 = 1 vteøina)
    public string nextSceneName = "NightScene1";

    private int currentIndex = 0;
    private bool isTransitioning = false;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    void Start()
    {
        if (storyImages == null || storyImages.Length == 0 || blackOverlay == null) return;

        initialPosition = blackOverlay.transform.localPosition;
        initialScale = blackOverlay.transform.localScale;

        blackOverlay.sortingOrder = 999;

        // Všechny obrázky vypnout, jen první zapnout
        for (int i = 0; i < storyImages.Length; i++)
        {
            storyImages[i].gameObject.SetActive(i == 0);
        }

        // Zaèínáme z èerné
        SetOverlayAlpha(1f);
        StartCoroutine(FadeFromBlack());
    }

    void Update()
    {
        if (isTransitioning) return;

        // Lepší kontrola vstupu
        bool inputPressed = false;
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) inputPressed = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) inputPressed = true;

        if (inputPressed)
        {
            StartCoroutine(SwitchToNextStep());
        }
    }

    IEnumerator SwitchToNextStep()
    {
        isTransitioning = true;
        int nextIndex = currentIndex + 1;

        // --- 1. FÁZE: STMÍVÁNÍ (Do èerné) ---
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetOverlayAlpha(alpha);
            yield return null; // Èeká na další frame - TADY SE DÌJE TO POSTUPNÉ STMÍVÁNÍ
        }
        SetOverlayAlpha(1f);

        // --- MEZIKROK: Zmìna pozice/mìøítka panelu ---
        if (nextIndex >= storyImages.Length)
        {
            blackOverlay.transform.localPosition = initialPosition;
            blackOverlay.transform.localScale = initialScale;
        }
        else if (nextIndex == changeAfterIndex)
        {
            blackOverlay.transform.localPosition = newOverlayPosition;
            blackOverlay.transform.localScale = newOverlayScale;
        }

        // Pøepnutí obrázkù (když je èerná obrazovka)
        storyImages[currentIndex].gameObject.SetActive(false);
        currentIndex++;

        if (currentIndex < storyImages.Length)
        {
            storyImages[currentIndex].gameObject.SetActive(true);

            // --- 2. FÁZE: ROZJASÒOVÁNÍ (Z èerné do obrazu) ---
            alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                SetOverlayAlpha(alpha);
                yield return null;
            }
            SetOverlayAlpha(0f);

            // Malá pauza, aby hráè nemohl hned prokliknout další slide
            yield return new WaitForSeconds(0.2f);
            isTransitioning = false;
        }
        else
        {
            // KONEC CUTSCÉNY
            Debug.Log("Konec cutscény, naèítám: " + nextSceneName);
            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator FadeFromBlack()
    {
        isTransitioning = true; // Zablokuje klikání bìhem úvodního fade-inu
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetOverlayAlpha(alpha);
            yield return null;
        }
        SetOverlayAlpha(0f);
        isTransitioning = false;
    }

    void SetOverlayAlpha(float a)
    {
        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = a; // Clamp01 není nutný, pokud je fadeSpeed správnì
            blackOverlay.color = c;
        }
    }
}