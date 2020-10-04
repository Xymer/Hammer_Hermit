using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] Spawner spawner = null;
    [SerializeField] GhostTetromino ghost = null;

    [NonSerialized] public Tetromino activeTetromino;
    [NonSerialized] public bool downHeld;
    GameDirector gameDirector;
    bool floored = false;
    bool updated = false;
    int dropFramesTicker;
    int lockFramesTicker;

    public Matrix matrix;

    private void Awake()
    {
        GameRules.instance.SetGameRules(0);
        gameDirector = FindObjectOfType<GameDirector>();
    }

    void Start()
    {
        InstantiateTimers();
        //StartCoroutine(StartNewGame());
    }

    /// <summary>
    /// Restarts the bag and updates the queue
    /// </summary>
    public void StartQueue()
    {
        spawner.gameObject.SetActive(true);
        spawner.InstantiateBag();
    }

    /// <summary>
    /// Play.
    /// </summary>
    public void StartNewGame()
    {
        GameRules.instance.gameStarted = true;
        activeTetromino = spawner.NextInQueue();
        matrix.UpdateMinoPositions(activeTetromino);
        spawner.UpdateQueue();
        InstantiateTimers();
        ghost.CreateGhost(activeTetromino);
    }

    /// <summary>
    /// F
    /// </summary>
    public void OnGameOver()
    {
        spawner.CleanQueue();
        if (spawner.gameObject.activeSelf)
            spawner.gameObject.SetActive(false);
        GameRules.instance.gameStarted = false;
        GameRules.instance.backgroundPosition = 0f;
    }

    /// <summary>
    /// Reset timers for tetromino.
    /// </summary>
    void InstantiateTimers()
    {
        floored = false;
        if (!downHeld)
        {
            dropFramesTicker = GameRules.instance.rules.dropFrames;
        }
        else
        {
            dropFramesTicker = 2;
        }
        lockFramesTicker = GameRules.instance.rules.lockFrames;
    }


    private void FixedUpdate()
    {

        if (!activeTetromino || !GameRules.instance.gameStarted)
        {
            return;
        }

        if (!floored)
        {
            dropFramesTicker--;
        }
        else
        {
            lockFramesTicker--;
            activeTetromino.UpdateBrightness(0.5f + 0.25f * lockFramesTicker / GameRules.instance.rules.lockFrames);
        }

        if (lockFramesTicker <= 0)
        {
            if (activeTetromino)
            {
                StartCoroutine(LockTetromino());
            }
            return;
        }

        if (dropFramesTicker <= 0)
        {
            Drop();
        }
    }

    private void LateUpdate()
    {
        if (!GameRules.instance.gameStarted)
        {
            return;
        }

        if (ghost.ghostEnabled)
        {
            ghost.UpdateGhost(activeTetromino);
        }
        updated = false;
    }

    /// <summary>
    /// Moves the tetromino down by 1 unit.
    /// </summary>
    public void Drop()
    {
        for (int i = 0; i < GameRules.instance.rules.gravity; i++)
        {
            try
            {
                activeTetromino.Down();
                if (matrix.IsBellowMatrix(activeTetromino))
                {
                    StartCoroutine(DestroyTetromino());
                }
                matrix.UpdateMinoPositions(activeTetromino);
            }
            catch (Exception)
            {
                return;
            }
        }
        if (downHeld)
        {
            dropFramesTicker = 2;
        }
        else
        {
            dropFramesTicker = GameRules.instance.rules.dropFrames;
        }
        lockFramesTicker = GameRules.instance.rules.lockFrames;
        activeTetromino.UpdateBrightness((lockFramesTicker + (float)GameRules.instance.rules.lockFrames / 2) / GameRules.instance.rules.lockFrames);
        if (matrix.IsObstructedNoMatrix(activeTetromino, new Vector2Int(0, -1)))
        {
            if (!floored && lockFramesTicker == GameRules.instance.rules.lockFrames)
            {
                SoundController.instance.PlaySound("land", 0.7f);
            }
            floored = true;
            if (downHeld)
            {
                if (activeTetromino)
                {
                    StartCoroutine(LockTetromino());
                }
                return;
            }
        }
    }

    /// <summary>
    /// Checks if the Tetromino is floored before it locks in place.
    /// </summary>
    public void DownPressed()
    {
        dropFramesTicker = 2;
        if (floored)
        {
            if (activeTetromino)
            {
                StartCoroutine(LockTetromino());
            }
        }
    }

    /// <summary>
    /// Ran whenever a movement of the tetromino occurs.
    /// </summary>
    public void TetrominoUpdated()
    {
        if (!activeTetromino)
            return;

        if (matrix.IsObstructedNoMatrix(activeTetromino, new Vector2Int(0, -1)))
        {
            if (!floored && lockFramesTicker == GameRules.instance.rules.lockFrames)
            {
                SoundController.instance.PlaySound("land", 0.7f);
            }
            floored = true;
        }
        else
        {
            floored = false;
            if (GameRules.instance.rules.dropFrames <= 2)
            {
                Drop();
            }
            activeTetromino.UpdateBrightness(1);
        }
        matrix.UpdateMinoPositions(activeTetromino);
    }

    /// <summary>
    /// Tetromino is locked in the matrix.
    /// </summary>
    IEnumerator LockTetromino()
    {
        //Before lineclear / ARS
        SoundController.instance.PlaySound("lock", .5f);
        activeTetromino.UpdateBrightness(1);
        ghost.DestroyGhost();
        matrix.LockMinosInMatrix(activeTetromino);
        activeTetromino = null;

        if (matrix.scrollAmount > 0)
        {
            ScoreController.instance.UpdateHeight(matrix.scrollAmount);
            for (int i = 0; i < GameRules.instance.rules.lineClearFrames; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            for (int i = 0; i < GameRules.instance.rules.areFrames; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        if (!GameRules.instance.gameStarted)
            yield break;
        //After lineclear / ARS
        activeTetromino = spawner.NextInQueue();
        ghost.CreateGhost(activeTetromino);
        matrix.UpdateBlocks();
        matrix.UpdateMinoPositions(activeTetromino);
        ScoreController.instance.UpdateHeight(0, matrix.GetHeighestGridPoint());
        if (GameRules.instance.rules.dropFrames <= 2)
        {
            activeTetromino.Fall();
        }
        if (!updated)
        {
            spawner.UpdateQueue();
            updated = true;
        }
        InstantiateTimers();
        if (GameRules.instance.rules.dropFrames <= 2)
        {
            Drop();
        }
    }

    /// <summary>
    /// Destroys the current falling tetromino and breaks the loop
    /// </summary>
    public void DestroyTetroAndGhost()
    {
        ghost.DestroyGhost();
        if (activeTetromino)
            Destroy(activeTetromino.gameObject);
        activeTetromino = null;
    }

    /// <summary>
    /// Destroys the current falling tetromino and pulls a new one from the queue
    /// </summary>
    IEnumerator DestroyTetromino()
    {
        //Before lineclear / ARS
        activeTetromino.UpdateBrightness(1);
        SoundController.instance.PlaySound("lock", .5f);
        DestroyTetroAndGhost();
        StartCoroutine(matrix.ScrollMatrix(1));
        ScoreController.instance.UpdateHeight(matrix.scrollAmount);
        for (int i = 0; i < GameRules.instance.rules.lineClearFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        //After lineclear / ARS
        activeTetromino = spawner.NextInQueue();
        ghost.CreateGhost(activeTetromino);
        matrix.UpdateBlocks();
        matrix.UpdateMinoPositions(activeTetromino);
        ScoreController.instance.UpdateHeight(0, matrix.GetHeighestGridPoint());
        if (GameRules.instance.rules.dropFrames <= 2)
        {
            activeTetromino.Fall();
        }
        if (!updated)
        {
            spawner.UpdateQueue();
            updated = true;
        }
        InstantiateTimers();
        if (GameRules.instance.rules.dropFrames <= 2)
        {
            Drop();
        }
    }
}
