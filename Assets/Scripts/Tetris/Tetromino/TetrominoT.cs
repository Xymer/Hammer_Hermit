using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoT : Tetromino
{
    int state = 0;

    public override void RotateC()
    {
        transform.Rotate(new Vector3(0, 0, -90));
        foreach (Transform mino in transform)
        {
            mino.Rotate(new Vector3(0, 0, 90));
        }
        if (state == 2)
        {
            Up();
        }
        state++;
        if (state > 3)
        {
            state = 0;
        }
        if (state == 2)
        {
            Down();
        }
        if (matrix.IsObstructed(this))
        {
            RotateCounterClockwise();
        }
    }

    public override void RotateCC()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        foreach (Transform mino in transform)
        {
            mino.Rotate(new Vector3(0, 0, -90));
        }
        if (state == 2)
        {
            Up();
        }
        state--;
        if (state < 0)
        {
            state = 3;
        }
        if (state == 2)
        {
            Down();
        }
        if (matrix.IsObstructed(this))
        {
            RotateClockwise();
        }
    }

    public override bool RotateClockwise()
    {
        transform.Rotate(new Vector3(0, 0, -90));
        foreach (Transform mino in transform)
        {
            mino.Rotate(new Vector3(0, 0, 90));
        }
        if (state == 2)
        {
            Up();
        }
        state++;
        if (state > 3)
        {
            state = 0;
        }
        if (state == 2)
        {
            Down();
        }
        if (matrix.IsObstructed(this, 0))
        {
            WallKick();
        }
        if (matrix.IsObstructed(this))
        {
            Up();
        }
        if (matrix.IsObstructed(this))
        {
            Down();
            RotateCounterClockwise();
            return false;
        }

        return true;
    }

    public override bool RotateCounterClockwise()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        foreach (Transform mino in transform)
        {
            mino.Rotate(new Vector3(0, 0, -90));
        }
        if (state == 2)
        {
            Up();
        }
        state--;
        if (state < 0)
        {
            state = 3;
        }
        if (state == 2)
        {
            Down();
        }
        if (matrix.IsObstructed(this, 0))
        {
            WallKick();
        }
        if (matrix.IsObstructed(this))
        {
            Up();
        }
        if (matrix.IsObstructed(this))
        {
            Down();
            RotateClockwise();
            return false;
        }

        return true;
    }
}
