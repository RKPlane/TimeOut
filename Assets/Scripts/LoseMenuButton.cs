using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenuButton : MonoBehaviour
{
    public void RestartGame()
    {
        //Borrar guardados para que no puedas glichearte al cambiar al buscaminas antes de morir
        if (GlobalGameManager.Instance != null)
        {
            GlobalGameManager.Instance.platformerData = null;
            GlobalGameManager.Instance.minesweeperState = null;
        }

        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame() //boton de quit por si lo implemento
    {
        Application.Quit();
    }
}

