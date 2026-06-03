using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Night5Manager : BaseNightManager
{
    private void Awake()
    {
        // Tady si nastav moveChance atd. jako předtím (vynechal jsem to pro krátkost)
    }

    protected override IEnumerator WinSequence()
    {
        // 1. Zastavíme nepřátele přes Base metodu
        StopAllEnemies();

        if (backgroundMusic != null) backgroundMusic.Stop();
        if (cameraManager != null)
        {
            cameraManager.enabled = false;
            if (cameraManager.cameraDisplayPanel != null) cameraManager.cameraDisplayPanel.SetActive(false);
        }
        if (cameraUIButton != null) cameraUIButton.SetActive(false);

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            var movement = mainCam.GetComponent("CameraFollowCursor") as MonoBehaviour;
            if (movement != null) movement.enabled = false;
            mainCam.transform.position = winPos;
            mainCam.transform.rotation = Quaternion.identity;
        }

        if (jumpscareSource != null && winSound != null) jumpscareSource.PlayOneShot(winSound);
        if (winScreenObject != null) winScreenObject.SetActive(true);

        // Save progress na Noc 6
        int alreadySaved = PlayerPrefs.GetInt("SavedNight", 1);
        PlayerPrefs.SetInt("SavedNight", Mathf.Max(alreadySaved, 6));
        PlayerPrefs.Save();

        yield return new WaitForSeconds(5.0f);

        if (screenFader != null)
        {
            screenFader.enabled = true;
            yield return StartCoroutine(screenFader.FadeToBlackAndWait());
        }

        // Ending logika
        if (EndingObject.endingTriggered == 1)
        {
            SceneManager.LoadScene("Ending1Scene");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }
}