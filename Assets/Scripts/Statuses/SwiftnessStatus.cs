using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiftnessStatus : Status
{
    public SwiftnessStatus()
    {
        name = NAME.SWIFTNESS;
        type = STATUS_TYPE.BUFF;
        priority = 0;
        decreasing = false;
    }

    public override int GetMovement(int movement)
    {
        return movement + amount;
    }
}
