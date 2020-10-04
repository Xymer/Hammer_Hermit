using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tetromino : MonoBehaviour
{
    public Matrix matrix;
    public bool HoldSwapped;
    public enum Type
    {
        I,
        O,
        S,
        Z,
        T,
        L,
        J
    }

    public Type type;

    //Up down left right.
    public void Down()
    {
        transform.Translate(new Vector3(0, -1), Space.World);
    }
    public void Up()
    {
        transform.Translate(new Vector3(0, 1), Space.World);
    }
    public void Left()
    {
        transform.Translate(new Vector3(-1, 0), Space.World);
    }
    public void Right()
    {
        transform.Translate(new Vector3(1, 0), Space.World);
    }
    /// <summary>
    /// Down but it checks if it can go down.
    /// </summary>
    public void Fall()
    {
        transform.Translate(new Vector3(0, -1), Space.World);
        if (matrix.IsObstructed(this))
        {
            Up();
        }
    }

    /// <summary>
    /// Drop to the lowest possible point.
    /// </summary>
    public void Drop()
    {
        while (true)
        {
            transform.Translate(new Vector3(0, -1), Space.World);
            if (matrix.IsObstructedNoMatrix(this))
            {
                Up();
                break;
            }
            if (matrix.IsBellowMatrix(this))
            {
                break;
            }
        }
    }

    /// <summary>
    /// Move left with obstruction checks
    /// </summary>
    public bool MoveLeft()
    {
        transform.Translate(new Vector3(-1, 0), Space.World);
        if (matrix.IsObstructedNoMatrixFloor(this))
        {
            Right();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Move right with obstruction checks
    /// </summary>
    public bool MoveRight()
    {
        transform.Translate(new Vector3(1, 0), Space.World);
        if (matrix.IsObstructedNoMatrixFloor(this))
        {
            Left();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Rotate Clockwise with obstruction checks
    /// </summary>
    public abstract bool RotateClockwise();
    public abstract void RotateC();
    /// <summary>
    /// Rotate Counterclockwise with obstruction checks
    /// </summary>
    public abstract bool RotateCounterClockwise();
    public abstract void RotateCC();

    /// <summary>
    /// Kick the wall when rotating.
    /// </summary>
    public void WallKick()
    {
        MoveRight();
        if (!matrix.IsObstructed(this))
        {
            return;
        }
        MoveLeft();
    }
    /// <summary>
    /// Kick the floor when rotating.
    /// </summary>
    public void FloorKick()
    {
        if(matrix.IsObstructed(this, new Vector2Int(0, -1)))
        {
            Up();
            Up();
        }
    }

    /// <summary>
    /// Brightness of the block
    /// </summary>
    public void UpdateBrightness(float b)
    {
        foreach (Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(b, b, b, mino.GetComponent<SpriteRenderer>().color.a);
        }
    }
}
