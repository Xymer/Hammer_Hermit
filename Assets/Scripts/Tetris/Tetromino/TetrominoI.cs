using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoI : Tetromino
{
    bool rotated = false;

    public override void RotateC()
    {
        RotateNoKick();
    }

    public override void RotateCC()
    {
        RotateNoKick();
    }

    public override bool RotateClockwise()
    {
        bool final = Rotate();
        return final;
    }

    public override bool RotateCounterClockwise()
    {
        bool final = Rotate();
        return final;
    }

    private bool Rotate()
    {
        bool floorObstructed = false;
        if (matrix.IsObstructed(this, new Vector2Int(0, -1)) && !rotated)
        {
            floorObstructed = true;
        }

        if (rotated)
        {
            transform.Rotate(new Vector3(0, 0, 90));
            rotated = !rotated;
            Left();
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, -90));
            }
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -90));
            rotated = !rotated;
            Right();
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, 90));
            }
        }
        if (matrix.IsObstructed(this) && !rotated)
        {
            WallKick();
            if (matrix.IsObstructed(this))
            {
                transform.Translate(new Vector3(2, 0), Space.World);

                if (matrix.IsObstructed(this))
                {
                    transform.Translate(new Vector3(-2, 0), Space.World);
                }
            }
        }
        if (matrix.IsObstructed(this) && rotated && floorObstructed)
        {
            transform.Translate(new Vector3(0, 2), Space.World);

            if (matrix.IsObstructed(this))
            {
                transform.Translate(new Vector3(0, -2), Space.World);
            }
        }

        if (matrix.IsObstructed(this))
        {
            RotateNoKick();
            return false;
        }

        return true;
    }

    private void RotateNoKick()
    {
        if (rotated)
        {
            transform.Rotate(new Vector3(0, 0, 90));
            rotated = !rotated;
            Left();
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, -90));
            }
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -90));
            rotated = !rotated;
            Right();
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, 90));
            }
        }

        if (matrix.IsObstructed(this))
        {
            RotateNoKick();
        }
    }
}
