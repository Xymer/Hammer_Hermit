using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoZ : Tetromino
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
        if (rotated)
        {
            transform.Rotate(new Vector3(0, 0, -90));
            rotated = !rotated;
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, 90));
            }
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, 90));
            rotated = !rotated;
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, -90));
            }
        }

        if (matrix.IsObstructed(this, 0))
        {
            WallKick();
        }
        if (matrix.IsObstructed(this))
        {
            Rotate();
            return false;
        }

        return true;
    }

    private void RotateNoKick()
    {
        if (rotated)
        {
            transform.Rotate(new Vector3(0, 0, -90));
            rotated = !rotated;
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, 90));
            }
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, 90));
            rotated = !rotated;
            foreach (Transform mino in transform)
            {
                mino.Rotate(new Vector3(0, 0, -90));
            }
        }
        if (matrix.IsObstructed(this))
        {
            RotateNoKick();
        }
    }
}
