using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiController : MonoBehaviour
{


    public void QuitApplication()
    {
        Application.Quit();
    }

    public void StartFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

}
