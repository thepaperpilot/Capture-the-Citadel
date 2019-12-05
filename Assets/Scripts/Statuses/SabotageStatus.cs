using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageStatus : Status
{
    public SabotageStatus(bool fromMonster)
    {
        name = Name.SABOTAGE;
        type = StatusType.DEBUFF;
        priority = 10;
        decreasing = true;
        gracePeriod = isGracePeriod(fromMonster);
    }

    public override int GetDamageDealt(int damage, bool preview)
    {
        return Mathf.FloorToInt(0.5f * damage);
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
    }
}
