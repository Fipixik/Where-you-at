using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneSwitcher : MonoBehaviour
{
    [Header("NASTAVENÍ")]
    public string sceneToLoad; // Název scény, kam chceme letět

    // Volá se automaticky, když se objekt zapne (SetActive(true))
    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Objekt aktivován, okamžitě načítám: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Zapomněl jsi vyplnit název scény u objektu " + gameObject.name);
        }
    }
}