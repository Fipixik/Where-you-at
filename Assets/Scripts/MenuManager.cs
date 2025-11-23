using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("New Game Settings")]
    public bool clearOldDataOnNewGame = true; // smaže staré uložené noci při novém startu

    public void PlayGame(bool startNewGame = false)
    {
        int savedNight = PlayerPrefs.GetInt("CurrentNight", 0);

        if (startNewGame || savedNight == 0)
        {
            if (clearOldDataOnNewGame)
            {
                PlayerPrefs.DeleteAll();
            }

            PlayerPrefs.SetInt("CurrentNight", 1);
            PlayerPrefs.Save();
            savedNight = 1;
            Debug.Log("Starting New Game (Night 1)");
        }
        else
        {
            Debug.Log("Continuing Game (Night " + savedNight + ")");
        }

        // načte správnou scénu podle noci
        string nightScene = "Night" + savedNight + "Scene";
        SceneManager.LoadScene(nightScene);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited");
    }
}
