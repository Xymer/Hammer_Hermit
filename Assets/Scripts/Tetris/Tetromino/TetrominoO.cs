using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoO : Tetromino
{
    public override void RotateC()
    {
    }

    public override void RotateCC()
    {
    }

    public override bool RotateClockwise()
    {
        return false;
    }

    public override bool RotateCounterClockwise()
    {
        return false;
    }
}
