using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatus : Status
{
    public PoisonStatus(bool fromMonster)
    {
        name = Name.POISON;
        type = StatusType.DEBUFF;
        priority = 10; //After multiplication
        decreasing = true;
        gracePeriod = isGracePeriod(fromMonster);
    }

    public override int GetDamageTaken(int damage)
    {
        return damage + 1;
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
    }
}
