using UnityEngine;
using System.Collections;

public class AlexandraScript : MonoBehaviour
{
    public string enemyName = "Alexandra";
    [Range(0, 100)] public int moveChance = 10;
    public float moveInterval = 10f;
    public int killProgress = 100;

    [Range(0, 100)] public float progress;
    private bool isProgressing;

    public GameObject windowUI;

    private void Start()
    {
        if (windowUI != null)
            windowUI.SetActive(false);

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            int roll = Random.Range(0, 100);

            if (roll < moveChance)
            {
                Debug.Log($"{enemyName}: Chance success! Starting progress.");
                if (!isProgressing)
                    StartCoroutine(ProgressRoutine());
            }
            else
            {
                Debug.Log($"{enemyName}: Chance failed.");
            }
        }
    }

    IEnumerator ProgressRoutine()
    {
        isProgressing = true;
        progress = 0;

        // Zobrazíme sprite pøi startu progresu
        if (windowUI != null)
            windowUI.SetActive(true);

        float speed = moveChance / 10f;

        while (progress < killProgress && isProgressing)
        {
            progress += speed * Time.deltaTime;
            Debug.Log($"{enemyName}: Progress {progress}/{killProgress}");
            yield return null;
        }

        if (progress >= killProgress)
        {
            Debug.Log($"{enemyName}: Kill progress reached!");
        }

        isProgressing = false;

        // Schováme sprite, když progres skonèí
        if (windowUI != null)
            windowUI.SetActive(false);
    }

    private void OnMouseDown()
    {
        Debug.Log($"{enemyName}: Progress stopped by player.");
        isProgressing = false;
        progress = 0;

        // Schováme sprite pøi kliknutí
        if (windowUI != null)
            windowUI.SetActive(false);
    }
}
