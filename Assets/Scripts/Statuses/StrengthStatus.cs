using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthStatus : Status
{
    public StrengthStatus()
    {
        name = Name.STRENGTH;
        type = StatusType.BUFF;
        decreasing = false;
        priority = 0;
    }

    public override int GetDamageDealt(int damage, bool preview)
    {
        return damage + amount;
    }

}
