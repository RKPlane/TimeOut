using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour //Conservar datos despues de cambiar escenas
{
    public static GlobalGameManager Instance;

    public PlatformerData platformerData;
    public MinesweeperState minesweeperState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
