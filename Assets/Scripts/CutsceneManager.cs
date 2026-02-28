using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
// PØIDÁNO: Nutné pro nový Input System
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    [Header("Obrazy")]
    public SpriteRenderer[] storyImages;
    public SpriteRenderer blackOverlay;

    [Header("Nastavení")]
    public float fadeSpeed = 1.5f;
    public string nextSceneName = "NightScene1";

    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        if (storyImages == null || storyImages.Length == 0 || blackOverlay == null)
        {
            Debug.LogError("Chybí pøiøazené objekty v Inspectoru!");
            return;
        }

        for (int i = 0; i < storyImages.Length; i++)
        {
            storyImages[i].gameObject.SetActive(i == 0);
        }
        StartCoroutine(FadeFromBlack());
    }

    void Update()
    {
        // ZMÌNÌNO: Nový zpùsob detekce kliknutí
        if (Mouse.current.leftButton.wasPressedThisFrame && !isTransitioning)
        {
            StartCoroutine(SwitchToNextStep());
        }
    }

    IEnumerator SwitchToNextStep()
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeToBlack());

        storyImages[currentIndex].gameObject.SetActive(false);
        currentIndex++;

        if (currentIndex < storyImages.Length)
        {
            storyImages[currentIndex].gameObject.SetActive(true);
            yield return StartCoroutine(FadeFromBlack());
            isTransitioning = false;
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator FadeToBlack()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetOverlayAlpha(alpha);
            yield return null;
        }
    }

    IEnumerator FadeFromBlack()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetOverlayAlpha(alpha);
            yield return null;
        }
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