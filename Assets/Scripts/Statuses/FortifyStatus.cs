using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortifyStatus : Status
{
    public FortifyStatus()
    {
        name = Name.FORTIFY;
        priority = 0;
        type = StatusType.BUFF;
        decreasing = false;
    }

    public override void OnTurnEnd()
    {
        CauseStatus(Name.DEFENSE, amount);
        base.OnTurnEnd();
    }
}
