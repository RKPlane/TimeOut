using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class Game : MonoBehaviour //GRAN PARTE DEL CODIGO DEL BUSCAMINAS ES DE INTERNET, LOS METODOS PARA GUARDAR EL ESTADO Y CO-ROUTINAS SON HECHOS POR MI
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;

    private Board board;
    private CellGrid grid;
    private bool gameover;
    private bool generated;


    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        if (GlobalGameManager.Instance.minesweeperState != null)
            LoadState(GlobalGameManager.Instance.minesweeperState);
        else
            NewGame();
    }


    private void NewGame()
    {
        StopAllCoroutines();

        if (GlobalGameManager.Instance.minesweeperState == null)
            Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f); //Camara bug fix

        gameover = false;
        generated = false;

        grid = new CellGrid(width, height);
        board.Draw(grid);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            NewGame();
            return;
        }

        if (!gameover)
        {
            if (Input.GetMouseButtonDown(0)) {
                Reveal();
            } else if (Input.GetMouseButtonDown(1)) {
                Flag();
            } else if (Input.GetMouseButton(2)) {
                Chord();
            } else if (Input.GetMouseButtonUp(2)) {
                Unchord();
            }
        }
    }

    private void Reveal()
    {
        if (TryGetCellAtMousePosition(out Cell cell))
        {
            if (!generated)
            {
                grid.GenerateMines(cell, mineCount);
                grid.GenerateNumbers();
                generated = true;
            }

            Reveal(cell);
        }
    }

    private void Reveal(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.flagged) return;

        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;

            case Cell.Type.Empty:
                StartCoroutine(Flood(cell));
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                CheckWinCondition();
                break;
        }

        board.Draw(grid);
    }

    private IEnumerator Flood(Cell cell)
    {
        if (gameover) yield break;
        if (cell.revealed) yield break;
        if (cell.type == Cell.Type.Mine) yield break;

        cell.revealed = true;
        board.Draw(grid);

        yield return null;

        if (cell.type == Cell.Type.Empty)
        {
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y, out Cell left)) {
                StartCoroutine(Flood(left));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y, out Cell right)) {
                StartCoroutine(Flood(right));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y - 1, out Cell down)) {
                StartCoroutine(Flood(down));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y + 1, out Cell up)) {
                StartCoroutine(Flood(up));
            }
        }
    }

    private void Flag()
    {
        if (!TryGetCellAtMousePosition(out Cell cell)) return;
        if (cell.revealed) return;

        cell.flagged = !cell.flagged;
        board.Draw(grid);
    }

    private void Chord()
    {
        //unchord
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].chorded = false;
            }
        }

        // chord
        if (TryGetCellAtMousePosition(out Cell chord))
        {
            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    int x = chord.position.x + adjacentX;
                    int y = chord.position.y + adjacentY;

                    if (grid.TryGetCell(x, y, out Cell cell)) {
                        cell.chorded = !cell.revealed && !cell.flagged;
                    }
                }
            }
        }

        board.Draw(grid);
    }

    private void Unchord()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.chorded) {
                    Unchord(cell);
                }
            }
        }

        board.Draw(grid);
    }

    private void Unchord(Cell chord)
    {
        chord.chorded = false;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }

                int x = chord.position.x + adjacentX;
                int y = chord.position.y + adjacentY;

                if (grid.TryGetCell(x, y, out Cell cell))
                {
                    if (cell.revealed && cell.type == Cell.Type.Number)
                    {
                        if (grid.CountAdjacentFlags(cell) >= cell.number)
                        {
                            Reveal(chord);
                            return;
                        }
                    }
                }
            }
        }
    }

    private void Explode(Cell cell)
    {
        gameover = true;

        //explota
        cell.exploded = true;
        cell.revealed = true;

        //revela las otras minas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.revealed = true;
                }
            }
        }
        StartCoroutine(RestartAfterWin()); //se reinicia cuando pierdes
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                // todas las cells tipo minas tienen que ser reveladas para ganar
                if (cell.type != Cell.Type.Mine && !cell.revealed) {
                    return; // no win
                }
            }
        }

        gameover = true;

        // Flag a las minas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.flagged = true;
                }
            }
        }

        StartCoroutine(RestartAfterWin()); //es imposible ganar como pidió Hernan siempre se resetea
    }

    private IEnumerator RestartAfterWin()
    {
        yield return new WaitForSeconds(2);
        NewGame();
    }
    private bool TryGetCellAtMousePosition(out Cell cell)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        return grid.TryGetCell(cellPosition.x, cellPosition.y, out cell);
    }

    public void SaveState() //Guardar estado del juego
    {
        MinesweeperState state = new MinesweeperState(); //data class save

        state.width = width;
        state.height = height;
        state.mineCount = mineCount;
        state.gameover = gameover;
        state.generated = generated;

        state.cells = new CellState[width * height];

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                state.cells[i++] = new CellState
                {
                    x = x,
                    y = y,
                    type = cell.type,
                    number = cell.number,
                    revealed = cell.revealed,
                    flagged = cell.flagged,
                    exploded = cell.exploded
                };
            }
        }
        state.cameraPosition = Camera.main.transform.position; //camara bugfix
        GlobalGameManager.Instance.minesweeperState = state; //guardar datos en objeto que no se destruye
    }

    public void LoadState(MinesweeperState state) //Cargar estado del juego
    {
        Camera.main.transform.position = state.cameraPosition; //camara bugfix
        width = state.width;
        height = state.height;
        mineCount = state.mineCount;

        gameover = state.gameover;
        generated = state.generated;

        grid = new CellGrid(width, height);

        foreach (CellState cs in state.cells)
        {
            Cell cell = grid[cs.x, cs.y];
            cell.type = cs.type;
            cell.number = cs.number;
            cell.revealed = cs.revealed;
            cell.flagged = cs.flagged;
            cell.exploded = cs.exploded;
        }

        board.Draw(grid); //dibujar el grid de nuevo
    }
}
