using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageStatus : Status
{
    public SabotageStatus()
    {
        name = Name.SABOTAGE;
        type = StatusType.DEBUFF;
        priority = 10;
        decreasing = true;
    }

    public override int GetDamageDealt(int damage)
    {
        return Mathf.FloorToInt(0.5f * damage);
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
    }
}
