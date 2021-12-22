using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void LoadScene(string lvl)
    {
        SceneManager.LoadScene(int.Parse(lvl));
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Load menu");
        SceneManager.LoadScene("StartMenuNew");
    }
}
