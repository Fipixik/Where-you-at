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

            Debug.Log("Starting New Game -> Loading StoryScene1");

            // Když je to nová hra, jdeme nejdřív do příběhu
            SceneManager.LoadScene("Story1Scene");
        }
        else
        {
            Debug.Log("Continuing Game (Night " + savedNight + ")");

            // Pokud hráč pokračuje, story vynecháme a hodíme ho rovnou do scény té noci
            string nightScene = "Night" + savedNight + "Scene";
            SceneManager.LoadScene(nightScene);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited");
    }
}