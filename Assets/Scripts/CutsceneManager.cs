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
    public float fadeSpeed = 2.0f;
    public string nextSceneName = "NightScene1";

    private int currentIndex = 0;
    private bool isTransitioning = false;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    void Start()
    {
        if (storyImages == null || storyImages.Length == 0 || blackOverlay == null) return;

        // Uložíme si startovní pozici (mìla by být 0,0,0)
        initialPosition = blackOverlay.transform.localPosition;
        initialScale = blackOverlay.transform.localScale;

        // Blackout musí být nad vším
        blackOverlay.sortingOrder = 999;

        for (int i = 0; i < storyImages.Length; i++)
        {
            storyImages[i].gameObject.SetActive(i == 0);
        }

        SetOverlayAlpha(1);
        StartCoroutine(FadeFromBlack());
    }

    void Update()
    {
        // Reaguje na jakoukoliv klávesu nebo myš
        if ((Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && !isTransitioning)
        {
            StartCoroutine(SwitchToNextStep());
        }
    }

    IEnumerator SwitchToNextStep()
    {
        isTransitioning = true;

        int nextIndex = currentIndex + 1;

        // Pokud jdeme na poslední slide nebo za nìj, vrátíme panel na støed (0,0,0)
        if (nextIndex >= storyImages.Length)
        {
            blackOverlay.transform.localPosition = initialPosition;
            blackOverlay.transform.localScale = initialScale;
            Debug.Log("<color=yellow>RESET: Panel vrácen na støed pro finální blackout.</color>");
        }
        // Jinak pokud je to ten specifický index uprostøed, pohneme s ním
        else if (nextIndex == changeAfterIndex)
        {
            blackOverlay.transform.localPosition = newOverlayPosition;
            blackOverlay.transform.localScale = newOverlayScale;
        }

        // 1. FÁZE: Stmívání
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetOverlayAlpha(alpha);
            yield return null;
        }
        SetOverlayAlpha(1f);

        storyImages[currentIndex].gameObject.SetActive(false);
        currentIndex++;

        if (currentIndex < storyImages.Length)
        {
            storyImages[currentIndex].gameObject.SetActive(true);

            // 2. FÁZE: Rozjasòování
            alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                SetOverlayAlpha(alpha);
                yield return null;
            }
            SetOverlayAlpha(0f);
            isTransitioning = false;
        }
        else
        {
            // KONEC: Panel už je na 0,0,0 z kroku nahoøe, takže jen poèkáme a pøepneme scénu
            yield return new WaitForSeconds(0.8f);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator FadeFromBlack()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetOverlayAlpha(alpha);
            yield return null;
        }
        SetOverlayAlpha(0f);
    }

    void SetOverlayAlpha(float a)
    {
        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = Mathf.Clamp01(a);
            blackOverlay.color = c;
        }
    }
}