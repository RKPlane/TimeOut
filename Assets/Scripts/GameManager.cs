using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Tiempo")]
    public float startTime = 30f;
    public float currentTime;
    public Vector3 startPos;

    public GameState state;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        startPos = transform.position;
        if (GlobalGameManager.Instance.platformerData != null)
            LoadPlatformerState(GlobalGameManager.Instance.platformerData);
        else
            StartGame();
    }


    void Update()
    {
        if (state != GameState.Playing) return;

        currentTime -= Time.deltaTime; //time

        if (currentTime <= 0)
        {
            LoseGame();
        }

        Debug.Log(GameManager.Instance);

    }

    void StartGame()
    {
        transform.position = startPos;
        currentTime = startTime;
        state = GameState.Playing;
    }

    public void AddTime(float amount) //checkpoints
    {
        currentTime += amount;
    }

    void LoseGame()
    {
        state = GameState.Lose;
        Debug.Log("Has perdido");
        SceneManager.LoadScene("LoseScene");
    }

    public void WinGame()
    {
        state = GameState.Win;
        Debug.Log("Victoria");
        SceneManager.LoadScene("WinScene");

    }

    public float getTime()
    {
        return currentTime;
    }

    public void SavePlatformerState() //guardar el estado del plataformero
    {
        PlatformerData state = new PlatformerData();

        state.currentTime = currentTime;
        state.playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        GlobalGameManager.Instance.platformerData = state; //guardar data en el objeto que no se destruye al cambiar de escenas
    }

    private void LoadPlatformerState(PlatformerData state) //cargar el estado del plataformero con los datos del globalgamemanager
    {
        currentTime = state.currentTime;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = state.playerPosition;
    }



}
