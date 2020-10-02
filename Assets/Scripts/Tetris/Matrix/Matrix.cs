using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Matrix : MonoBehaviour
{
    //Public
    public int height;
    public int width;
    public int header;
    public int startFloorHeight;

    [NonSerialized] public int maxGridHeight;
    [NonSerialized] public int scrollAmount;
    [NonSerialized] public List<Vector2Int> currentPositions = new List<Vector2Int>();

    //Private
    int iterations;

    [SerializeField] bool scrollingEnabled = false;
    [SerializeField] GameObject gridTile = null;
    [SerializeField] Mino mino = null;
    [SerializeField] GameController gameController = null;
    [SerializeField] public ParticleSystem breakParticle = null;

    Mino[,] grid;
    [SerializeField] Tilemap colliderGrid = null;
    [SerializeField] Tilemap fallingColliderGrid = null;
    [SerializeField] TileBase colliderTile = null;
    [NonSerialized] public List<int> completedLines = new List<int>();


    public void Initialize(bool startPlaying)
    {
        gameController.enabled = startPlaying;
        grid = new Mino[width, height + header];
    }

    /// <summary>
    /// Update queue from matrix?
    /// </summary>
    internal void UpdateQueue()
    {
        fallingColliderGrid.ClearAllTiles();
        gameController.StartQueue();
    }

    /// <summary>
    /// Look there's a lot of things I don't understand in here either. Someone created a second game controller.
    /// -Sam
    /// No Comment
    /// -Antonio
    /// </summary>
    public void OnNewGame()
    {
        gameController.StartNewGame();
    }

    /// <summary>
    /// Restart the matrix.
    /// </summary>
    public void OnRestart()
    {
        DestroyAll();
        gameController.TetrominoUpdated();

        CreateFloor();
        iterations = 0;
    }

    /// <summary>
    /// Christ.
    /// </summary>
    public void ToggleGameController(bool state = true)
    {
        gameController.enabled = state;
    }

    /// <summary>
    /// Game is dead
    /// </summary>
    public void OnGameOver()
    {
        DestroyAll();
        gameController.OnGameOver();
        gameController.DestroyTetroAndGhost();
        if (gameController.enabled)
            gameController.enabled = false;

        foreach (LevelEntity entity in GetComponentsInChildren<LevelEntity>())
        {
            if (entity.gameObject.name != "Player")
                Destroy(entity.gameObject);
        }
    }

    /// <summary>
    /// Initialize the floor.
    /// </summary>
    public void CreateFloor()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < startFloorHeight; y++)
            {
                Mino spawn = Instantiate(mino, new Vector3(transform.position.x + x, transform.position.y + y, 0), Quaternion.identity, transform.GetChild(2)) as Mino;
                spawn.SetType(Mino.MinoType.solid);
                grid[x, y] = spawn;
                colliderGrid.SetTile(new Vector3Int(x, y, 0), colliderTile);
            }
        }
    }

    /// <summary>
    /// Check if the tetromino has any blocks colliding with the grid
    /// </summary>
    public bool IsObstructed(Tetromino tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2Int xy = GetPositionInMatrix(mino);

            if (!IsWithinMatrix(new Vector2Int(xy.x, xy.y)))
            {
                return true;
            }

            if (grid[xy.x, xy.y] != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the tetromino has any blocks colliding with the grid, however, ignores the matrix borders.
    /// </summary>
    public bool IsObstructedNoMatrix(Tetromino tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2Int xy = GetPositionInMatrix(mino);

            if (IsWithinMatrix(new Vector2Int(xy.x, xy.y)))
            {
                if (grid[xy.x, xy.y] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the tetromino has any blocks colliding with the grid, however, ignores the matrix borders.
    /// </summary>
    /// <param name="offset">Offset from tetromino center</param>
    public bool IsObstructedNoMatrix(Tetromino tetromino, Vector2Int offset)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2Int xy = GetPositionInMatrix(mino);

            if (IsWithinMatrix(new Vector2Int(xy.x + offset.x, xy.y + offset.y)))
            {
                if (grid[xy.x + offset.x, xy.y + offset.y] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the tetromino has any blocks colliding with the grid.
    /// </summary>
    /// <param name="offset">Offset from tetromino center</param>
    public bool IsObstructed(Tetromino tetromino, Vector2Int offset)
    {
        if (!tetromino)
        {
            return false;
        }

        foreach (Transform mino in tetromino.transform)
        {
            Vector2Int xy = GetPositionInMatrix(mino);

            if (!IsWithinMatrix(new Vector2Int(xy.x + offset.x, xy.y + offset.y)))
            {
                return true;
            }

            if (grid[xy.x + offset.x, xy.y + offset.y] != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the tetromino has any blocks colliding with the grid, however, ignores the matrix borders.
    /// </summary>
    /// <param name="exclude">Excludeds a row of the tetromino from the check</param>
    public bool IsObstructed(Tetromino tetromino, int exclude)
    {
        foreach (Transform mino in tetromino.transform)
        {
            if (mino.position.x - tetromino.transform.position.x != exclude)
            {
                Vector2Int xy = GetPositionInMatrix(mino);

                if (!IsWithinMatrix(new Vector2Int(xy.x, xy.y)))
                {
                    return true;
                }

                if (grid[xy.x, xy.y] != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Check if tetromino is bellow matrix.
    /// </summary>
    public bool IsBellowMatrix(Tetromino tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2Int xy = GetPositionInMatrix(mino);

            if (!(xy.y < 0))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Check if position is bellow matrix.
    /// </summary>
    public bool IsPositionBellowMatrix(Vector2Int position)
    {
        position = GetPositionInMatrix(position);

        if (!(position.y < 0))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if the position has any blocks colliding with the grid.
    /// </summary>
    public bool IsPositionObstructed(Vector2Int position)
    {
        position = GetPositionInMatrix(position);
        if (!IsWithinMatrix(position))
        {
            return true;
        }
        //Debug.Log(position);
        if (grid[position.x, position.y] != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the position has any blocks colliding with the grid, however, ignores the matrix borders.
    /// </summary>
    public bool IsPositionObstructedNoMatrix(Vector2Int position)
    {
        position = GetPositionInMatrix(position);
        if (IsWithinMatrix(position))
        {
            if (grid[position.x, position.y] != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if the grid position has any blocks colliding with the grid.
    /// </summary>
    public bool IsLocalPositionObstructed(Vector2Int position)
    {
        if (!IsWithinMatrix(position))
        {
            Debug.Log("Out of bounds");
            return true;
        }
        if (grid[position.x, position.y] != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get node from world position.
    /// </summary>
    public Transform GetNodeFromGrid(Vector2Int xy)
    {
        xy = GetPositionInMatrix(xy);
        if (IsWithinMatrix(xy) && grid[xy.x, xy.y])
            return grid[xy.x, xy.y].transform;
        else return null;
    }

    /// <summary>
    /// Update the falling tetromino colliders.
    /// </summary>
    public void UpdateMinoPositions(Tetromino tetromino)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        fallingColliderGrid.ClearAllTiles();

        foreach (Transform mino in tetromino.transform)
        {
            if (mino.GetComponent<Mino>().IsWalkable)
            {
                positions.Add(Vector2Int.RoundToInt(GetPositionInMatrix(mino)));
            }
            else
            {
                return;
            }
        }

        foreach (var pos in positions)
        {
            fallingColliderGrid.SetTile(new Vector3Int(pos.x, pos.y, 0), colliderTile);
        }

        currentPositions = positions;
    }

    /// <summary>
    /// World position to grid position.
    /// </summary>
    private Vector2Int GetPositionInMatrix(Vector2Int mino)
    {
        return mino - new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    /// <summary>
    /// Transform to grid position.
    /// </summary>
    private Vector2Int GetPositionInMatrix(Transform mino)
    {
        return new Vector2Int(Mathf.RoundToInt(mino.position.x), Mathf.RoundToInt(mino.position.y)) -
            new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    /// <summary>
    /// Check if world position is within the bonderies of the matrix.
    /// </summary>
    public bool IsPositionWithinMatrix(Vector2Int position)
    {
        position = GetPositionInMatrix(position);
        return IsWithinMatrix(position);
    }

    /// <summary>
    /// Check if grid position is within the bonderies of the matrix.
    /// </summary>
    private bool IsWithinMatrix(Vector2Int position)
    {
        if (position.x > width - 1 || position.x < 0)
        {
            return false;
        }

        if (position.y < 0)
        {
            return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Vector3 downLeft = new Vector3(-0.5f, -0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + downLeft, transform.position + new Vector3(width, 0) + downLeft);
        Gizmos.DrawLine(transform.position + downLeft + new Vector3(width, 0), transform.position + new Vector3(width, height) + downLeft);
        Gizmos.DrawLine(transform.position + downLeft, transform.position + new Vector3(0, height) + downLeft);
        Gizmos.DrawLine(transform.position + downLeft + new Vector3(0, height), transform.position + new Vector3(width, height) + downLeft);
    }

    /// <summary>
    /// Lock tetromino in matrix.
    /// </summary>
    public void LockMinosInMatrix(Tetromino tetromino)
    {
        List<int> yChecks = new List<int>();

        foreach (Transform mino in tetromino.transform)
        {
            Vector2Int xy = GetPositionInMatrix(mino);

            grid[xy.x, xy.y] = mino.GetComponent<Mino>();

            if (grid[xy.x, xy.y].IsWalkable)
            {
                colliderGrid.SetTile(new Vector3Int(xy.x, xy.y, 0), colliderTile);
            }

            if (!grid[xy.x, xy.y].IsFragile)
            {
                CheckBlockBellow(xy);
            }

            yChecks.Add(xy.y);
        }
        tetromino.transform.SetParent(transform.GetChild(2));

        fallingColliderGrid.ClearAllTiles();

        scrollAmount = 0;
        iterations++;
        maxGridHeight = GetHeighestGridPoint();

        if (iterations >= GameRules.instance.rules.scrollRequirement)
        {
            scrollAmount++;
            iterations = 0;
        }

        if (maxGridHeight > GameRules.instance.rules.maxHeight)
        {
            scrollAmount += maxGridHeight - GameRules.instance.rules.maxHeight;
        }

        if (!scrollingEnabled)
        {
            scrollAmount = 0;
        }

        if (scrollAmount > 0)
        {
            StartCoroutine(ScrollMatrix(scrollAmount));
        }
    }

    /// <summary>
    /// Highest Y position, select X position
    /// </summary>
    /// <param name="direction">-1 = Left, 1 = Right</param>
    public void SpawnObsticle(int Xposition, Mino.MinoType type, int direction)
    {
        int newPos = Mathf.Clamp(Xposition, 0, width - 1);
        Mino spawn = Instantiate(mino, new Vector3(transform.position.x + newPos, transform.position.y + height + 2), Quaternion.identity, transform.GetChild(2)) as Mino;
        spawn.SetType(direction);
        spawn.direction = direction;
        grid[Xposition, height + 2] = spawn;
        colliderGrid.SetTile(new Vector3Int(Xposition, height + 2, 0), colliderTile);
    }

    /// <summary>
    /// Place block from obsticle
    /// </summary>
    public void PlaceBlock(Vector2Int position, GameObject mino)
    {
        position = GetPositionInMatrix(position);
        grid[position.x, position.y] = mino.GetComponent<Mino>();
        colliderGrid.SetTile(new Vector3Int(position.x, position.y, 0), colliderTile);
    }

    /// <summary>
    /// Spawn obsticle
    /// </summary>
    public void SpawnObsticle(Vector2Int position, Mino.MinoType type, int direction)
    {
        Mino spawn = Instantiate(mino, new Vector3(transform.position.x + position.x, transform.position.y + position.y), Quaternion.identity, transform.GetChild(2)) as Mino;
        spawn.SetType(direction);
        spawn.direction = direction;
        grid[position.x, position.y] = spawn;
        colliderGrid.SetTile(new Vector3Int(position.x, position.y, 0), colliderTile);
        SoundController.instance.PlaySound("obstacleextend", .25f);
    }

    /// <summary>
    /// Update the blocks every lock update
    /// </summary>
    public void UpdateBlocks()
    {
        bool hasExtended = false;
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == null)
                {
                    continue;
                }

                var mino = grid[x, y];

                if (mino.IsObsticle)
                {
                    if (!IsLocalPositionObstructed(new Vector2Int(x + mino.direction, y)))
                    {
                        hasExtended = true;
                        SpawnObsticle(new Vector2Int(x + mino.direction, y), mino.type, mino.direction);
                        break;
                    }
                }
            }
        }
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == null)
                {
                    continue;
                }
                var mino = grid[x, y];

                if (mino.IsFalling)
                {
                    if (!IsLocalPositionObstructed(new Vector2Int(x, y - 1)))
                    {
                        mino.falling = true;
                        grid[x, y] = null;
                        colliderGrid.SetTile(new Vector3Int(x, y, 0), null);
                        StartCoroutine(UpdateFallingBlocksAbove(x, y));
                    }
                }
                if (mino.IsTrator)
                {
                    mino.ReverseTractorBlock();
                }
            }
        }
        if (hasExtended)
            SoundController.instance.PlaySound("obstacleextend", .25f);
    }

    /// <summary>
    /// Check if the blocks above are falling.
    /// </summary>
    IEnumerator UpdateFallingBlocksAbove(int x, int y)
    {
        int aboveY = 1;
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (grid[x, y + aboveY] == null)
            {
                break;
            }
            var aboveMino = grid[x, y + aboveY];

            if (aboveMino.IsFalling)
            {
                aboveMino.falling = true;
                grid[x, y + aboveY] = null;
                colliderGrid.SetTile(new Vector3Int(x, y + aboveY, 0), null);
                aboveY++;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Check if the blocks above are fragile
    /// </summary>
    private void CheckBlockBellow(Vector2Int pos)
    {
        if (!IsWithinMatrix(pos - new Vector2Int(0, 1)))
        {
            return;
        }

        if (grid[pos.x, pos.y - 1] != null)
        {
            if (grid[pos.x, pos.y - 1].IsFragile)
            {
                Destroy(grid[pos.x, pos.y - 1].gameObject);
                colliderGrid.SetTile(new Vector3Int(pos.x, pos.y - 1, 0), null);
                grid[pos.x, pos.y - 1] = null;
                StartCoroutine(FragileReaction(pos - new Vector2Int(0, 1)));
            }
        }
    }

    /// <summary>
    /// Scroll the matrix up 1 step, It's an illution to create the feel that you're traveling upwards. It's all static. I'm sorry.
    /// </summary>
    public IEnumerator ScrollMatrix(int scrollAmount)
    {
        Vector2 original = transform.position;
        Vector2 temp = new Vector2();
        Vector2 target = new Vector3(0, -scrollAmount) + transform.position;

        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector2.SmoothDamp(transform.position, target, ref temp, 0.1f);
            yield return new WaitForFixedUpdate();
        }

        for (int y = 0; y < scrollAmount; y++)
        {
            DestroyLine(y);
            completedLines.Add(y);
        }

        completedLines.Sort();

        foreach (LevelEntity entity in GetComponentsInChildren<LevelEntity>())
        {
            entity.MoveDownwards(scrollAmount);
        }

        MoveLines();

        transform.position = original;
    }

    /// <summary>
    /// Get the highest point in the grid.
    /// </summary>
    public int GetHeighestGridPoint()
    {
        int high = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].IsObsticle)
                    {
                        continue;
                    }
                    high = y + 1;
                }
            }
        }

        return high;
    }

    /// <summary>
    /// Move all lines down from "completed lines" points. Tetris function, not really useful in this game.
    /// </summary>
    public void MoveLines()
    {
        if (completedLines.Count > 0)
        {
            for (int i = completedLines.Count - 1; i >= 0; i--)
            {
                MoveLinesFrom(completedLines[i], 1);
                completedLines.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Destroy all lines.
    /// </summary>
    private void DestroyAll()
    {
        for (int y = 0; y < height; y++)
            DestroyLine(y);
        colliderGrid.gameObject.SetActive(false);
        colliderGrid.gameObject.SetActive(true);
    }

    /// <summary>
    /// Destroy a spesific block.
    /// </summary>
    public void DestroyBlock(Vector2Int position)
    {
        var pos = GetPositionInMatrix(position);

        if (!IsWithinMatrix(pos))
        {
            return;
        }

        if (grid[pos.x, pos.y] != null)
        {
            if (grid[pos.x, pos.y].IsBreakable)
            {
                grid[pos.x, pos.y].UponDestroy();
                colliderGrid.SetTile(new Vector3Int(pos.x, pos.y, 0), null);

                if (grid[pos.x, pos.y].IsFragile)
                {
                    StartCoroutine(FragileReaction(pos));
                }
            }
            else
                SoundController.instance.PlaySound("break", .75f);
        }
    }

    /// <summary>
    /// The reaction that causes all the fragile blocks to break in a chain.
    /// </summary>
    private IEnumerator FragileReaction(Vector2Int pos)
    {
        yield return new WaitForSeconds(0.01f);

        Vector2Int[] checks =
        {
            new Vector2Int (-1, 0),
            new Vector2Int (1, 0),
            new Vector2Int (0, -1),
            new Vector2Int (0, 1)
        };

        foreach (var item in checks)
        {
            if (!IsWithinMatrix(pos + item))
            {
                continue;
            }

            if (grid[pos.x + item.x, pos.y + item.y] == null)
            {
                continue;
            }

            if (grid[pos.x + item.x, pos.y + item.y].IsFragile)
            {
                grid[pos.x + item.x, pos.y + item.y].UponDestroy();
                colliderGrid.SetTile(new Vector3Int(pos.x + item.x, pos.y + item.y, 0), null);
                grid[pos.x + item.x, pos.y + item.y] = null;
                StartCoroutine(FragileReaction(pos + item));
            }
        }
    }

    /// <summary>
    /// Destroy a single line based on the y position.
    /// </summary>
    private void DestroyLine(int yPos)
    {
        for (int i = 0; i < width; i++)
        {
            if (grid[i, yPos] == null)
            {
                continue;
            }
            Destroy(grid[i, yPos].gameObject);
            colliderGrid.SetTile(new Vector3Int(i, yPos, 0), null);
            grid[i, yPos] = null;
        }
    }

    /// <summary>
    /// Move lines from y and up by amount.
    /// </summary>
    private void MoveLinesFrom(int yPos, int amount)
    {
        for (int y = yPos; y < height + header - amount; y++)
        {
            for (int x = 0; x < width; x++)
            {

                if (grid[x, y + amount] != null)
                {
                    grid[x, y + amount].transform.Translate(new Vector3(0, -amount), Space.World);
                    if (grid[x, y + amount].IsWalkable)
                    {
                        colliderGrid.SetTile(new Vector3Int(x, y + amount, 0), null);
                        colliderGrid.SetTile(new Vector3Int(x, y, 0), colliderTile);
                    }
                }
                grid[x, y] = grid[x, y + amount];

            }
        }
    }
}
