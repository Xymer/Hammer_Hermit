using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    GameController gameController;
    int dasTicker;
    int movementRestrictor;
    bool rightObstructed;
    bool leftObstructed;
    bool preventContinue;

    public IndividualInput input;

    void Start()
    {
        dasTicker = GameRules.instance.rules.dasFrames;
        movementRestrictor = 2;
        gameController = GetComponentInParent<GameController>();
        input = new IndividualInput();
        input.SetControllerOption(2);
    }

    private void FixedUpdate()
    {
        if (!gameController.activeTetromino)
        {
            if (input.IsButtonDown(PlayerButton.Left))
                dasTicker = GameRules.instance.rules.dasFrames;

            if (input.IsButtonHeld(PlayerButton.Left))
            {
                dasTicker--;
            }

            if (input.IsButtonDown(PlayerButton.Right))
                dasTicker = GameRules.instance.rules.dasFrames;

            if (input.IsButtonHeld(PlayerButton.Right))
            {
                dasTicker--;
            }
        }
        else
        {
            if (input.IsButtonDown(PlayerButton.Left))
                dasTicker = GameRules.instance.rules.dasFrames;

            if (input.IsButtonHeld(PlayerButton.Left))
            {
                movementRestrictor--;
                if (dasTicker <= 0 && movementRestrictor <= 0)
                {
                    if (gameController.activeTetromino.MoveLeft())
                    {
                        movementRestrictor = 2;
                        gameController.TetrominoUpdated();
                    }
                }
                else
                {
                    dasTicker--;
                }
            }

            if (input.IsButtonDown(PlayerButton.Right))
                dasTicker = GameRules.instance.rules.dasFrames;

            if (input.IsButtonHeld(PlayerButton.Right))
            {
                movementRestrictor--;
                if (dasTicker <= 0 && movementRestrictor <= 0)
                {
                    if (gameController.activeTetromino.MoveRight())
                    {
                        movementRestrictor = 2;
                        gameController.TetrominoUpdated();
                    }
                }
                else
                {
                    dasTicker--;
                }
            }
        }
    }

    private void Update()
    {
        input.UpdateAxisValues();
        if (preventContinue != input.IsButtonHeld(PlayerButton.A))
        {
            preventContinue = false;
        }
        if (!preventContinue)
        {
            gameController.downHeld = input.IsButtonHeld(PlayerButton.A);
        }
        else
        {
            gameController.downHeld = false;
        }

        if (!gameController.activeTetromino)
        {
            preventContinue = input.IsButtonHeld(PlayerButton.A);
        }
        else
        {
            if (input.IsButtonDown(PlayerButton.Left))
            {
                dasTicker = GameRules.instance.rules.dasFrames;
                if (gameController.activeTetromino.MoveLeft())
                {
                    gameController.TetrominoUpdated();
                }
            }
            if (input.IsButtonDown(PlayerButton.Right))
            {
                dasTicker = GameRules.instance.rules.dasFrames;
                if (gameController.activeTetromino.MoveRight())
                {
                    gameController.TetrominoUpdated();
                }
            }
            if (input.IsButtonDown(PlayerButton.Y))
            {
                CheckObstructed();
                if (gameController.activeTetromino.RotateClockwise())
                {
                    SoundController.instance.PlaySound("rotate", .5f);
                    if (input.IsButtonHeld(PlayerButton.Right) && rightObstructed)
                    {
                        gameController.activeTetromino.MoveRight();
                    }
                    if (input.IsButtonHeld(PlayerButton.Left) && leftObstructed)
                    {
                        gameController.activeTetromino.MoveLeft();
                    }
                    gameController.TetrominoUpdated();
                }
            }
            if (input.IsButtonDown(PlayerButton.X))
            {
                CheckObstructed();
                if (gameController.activeTetromino.RotateCounterClockwise())
                {
                    SoundController.instance.PlaySound("rotate",.5f);
                    if (input.IsButtonHeld(PlayerButton.Right) && rightObstructed)
                    {
                        gameController.activeTetromino.MoveRight();
                    }
                    if (input.IsButtonHeld(PlayerButton.Left) && leftObstructed)
                    {
                        gameController.activeTetromino.MoveLeft();
                    }
                    gameController.TetrominoUpdated();
                }
            }
            if (input.IsButtonDown(PlayerButton.A))
            {
                gameController.DownPressed();
            }
        }
    }

    /// <summary>
    /// Advanced movement, probably not gonna be used much.
    /// </summary>
    private void CheckObstructed()
    {
        if (gameController.matrix.IsObstructed(gameController.activeTetromino, new Vector2Int(1, 0)))
        {
            rightObstructed = true;
        }
        else
        {
            rightObstructed = false;
        }
        if (gameController.matrix.IsObstructed(gameController.activeTetromino, new Vector2Int(-1, 0)))
        {
            leftObstructed = true;
        }
        else
        {
            leftObstructed = false;
        }
    }
}
