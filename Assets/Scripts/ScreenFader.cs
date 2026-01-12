using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 2.0f;

    [Header("Reference")]
    public SpriteRenderer fadeSprite;

    void Start()
    {
        if (fadeSprite == null) fadeSprite = GetComponent<SpriteRenderer>();
        ResetFade();
    }

    public IEnumerator FadeToBlackAndWait()
    {
        // 1. Zapneme objekt
        this.gameObject.SetActive(true);

        // 2. Startovní stav
        if (fadeSprite != null)
        {
            Color c = Color.black;
            c.a = 0f;
            fadeSprite.color = c;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            // Používáme unscaledDeltaTime (ignoruje pauzu hry)
            timer += Time.unscaledDeltaTime;

            // Tady vypoèítáme alphu
            float alpha = Mathf.Clamp01(timer / fadeDuration);

            // --- TADY JE TEN VÝPIS DO KONZOLE ---
            Debug.Log("Fading... Alpha: " + alpha);
            // ------------------------------------

            if (fadeSprite != null)
            {
                Color newColor = fadeSprite.color;
                newColor.a = alpha;
                fadeSprite.color = newColor;
            }

            yield return null;
        }

        // 3. Finální stav (èerná)
        if (fadeSprite != null)
        {
            Color finalColor = fadeSprite.color;
            finalColor.a = 1f;
            fadeSprite.color = finalColor;
        }

        Debug.Log("Fading DOKONÈEN! Jsme na èerné.");
    }

    public void ResetFade()
    {
        if (fadeSprite != null)
        {
            Color c = fadeSprite.color;
            c.a = 0f;
            fadeSprite.color = c;
        }
        this.gameObject.SetActive(false);
    }
}