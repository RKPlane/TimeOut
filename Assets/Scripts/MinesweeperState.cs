public class MinesweeperState //data class del estado del juego
{
    public int width;
    public int height;
    public int mineCount;

    public bool gameover;
    public bool generated;

    public CellState[] cells;
    public UnityEngine.Vector3 cameraPosition; //camara bugfix
}

