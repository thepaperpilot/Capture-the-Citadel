using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyStatus : Status
{
    public HandyStatus()
    {
        name = Name.HANDY;
        priority = 0;
        type = StatusType.BUFF;
        decreasing = false;
    }

    public override int GetHandSize(int hand, bool preview)
    {
        return hand + amount;
    }
}
