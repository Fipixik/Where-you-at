using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // full-screen black UI Image
    public float fadeDuration = 1f; // doba fade

    private void Awake()
    {
        if (fadeImage == null)
        {
            Debug.LogError("ScreenFader: Fade Image není pøiøazeno!");
            return;
        }

        fadeImage.color = new Color(0, 0, 0, 0); // start transparent
    }

    // Fade do èerna
    public IEnumerator FadeToBlackAndWait()
    {
        float elapsed = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0, 0, 0, 1);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        fadeImage.color = targetColor;
    }

    // Reset alpha = 0
    public void ResetFade()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }
}
