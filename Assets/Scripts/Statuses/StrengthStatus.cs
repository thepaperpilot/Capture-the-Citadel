using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthStatus : Status
{
    public StrengthStatus()
    {
        name = NAME.STRENGTH;
        type = STATUS_TYPE.BUFF;
        decreasing = false;
        priority = 0;
    }

    public override int GetDamageDealt(int damage)
    {
        return damage + amount;
    }

}
