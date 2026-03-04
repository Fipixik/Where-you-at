using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 2f;

    [Header("Reference")]
    public SpriteRenderer fadeSprite;

    void Start()
    {
        if (fadeSprite == null) fadeSprite = GetComponent<SpriteRenderer>();
        ResetFade();
    }

    public IEnumerator FadeToBlackAndWait()
    {
        this.gameObject.SetActive(true);

        if (fadeSprite != null)
        {
            Color c = Color.black;
            c.a = 0f;
            fadeSprite.color = c;
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);

            if (fadeSprite != null)
            {
                Color newColor = fadeSprite.color;
                newColor.a = alpha;
                fadeSprite.color = newColor;
            }
            yield return null;
        }

        if (fadeSprite != null)
        {
            Color finalColor = fadeSprite.color;
            finalColor.a = 1f;
            fadeSprite.color = finalColor;
        }
    }

    // NOVÁ FUNKCE: Odfádování z èerné do prùhledné
    public IEnumerator FadeToClear()
    {
        float timer = fadeDuration;

        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);

            if (fadeSprite != null)
            {
                Color newColor = fadeSprite.color;
                newColor.a = alpha;
                fadeSprite.color = newColor;
            }
            yield return null;
        }

        ResetFade();
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