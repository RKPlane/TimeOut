using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToPlatformer()
    {
        Object.FindFirstObjectByType<Game>().SaveState(); //Guarda estado buscaminas
        SceneManager.LoadScene("MainScene");
    }

    public void GoToMinesweeper()
    {
        Object.FindFirstObjectByType<GameManager>().SavePlatformerState(); //Guarda estado platformer
        SceneManager.LoadScene("Minesweeper");
    }
}


