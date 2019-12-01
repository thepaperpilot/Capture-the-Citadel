using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesStatus : Status
{
    public SpikesStatus()
    {
        name = NAME.SPIKES;
        type = STATUS_TYPE.BUFF;
        priority = 0;
    }

    public override int GetDamageTaken(int damage)
    {
        return damage - amount;
    }

    public override void OnTurnStart()
    {
        controller.RemoveStatus(this);
    }

    public override void OnTurnEnd() { /*Do Nothing*/ }
}
