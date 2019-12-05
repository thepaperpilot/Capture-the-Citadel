using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseStatus : Status
{
    public DefenseStatus()
    {
        name = Name.DEFENSE;
        type = StatusType.BUFF;
        priority = 0;
    }

    public override int GetDamageTaken(int damage, bool preview)
    {
        return damage - amount;
    }

    public override void OnTurnStart()
    {
        controller.RemoveStatus(this);
    }

    public override void OnTurnEnd() { /*Do Nothing*/ }
}
