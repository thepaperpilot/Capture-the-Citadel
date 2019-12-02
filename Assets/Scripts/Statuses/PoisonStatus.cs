using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatus : Status
{
    public PoisonStatus()
    {
        name = Name.POISON;
        type = StatusType.DEBUFF;
        priority = 10; //After multiplication
        decreasing = true;
    }

    public override int GetDamageTaken(int damage)
    {
        return damage + 1;
    }
}
