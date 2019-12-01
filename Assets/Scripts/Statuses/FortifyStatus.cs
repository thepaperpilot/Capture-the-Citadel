using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortifyStatus : Status
{
    public FortifyStatus()
    {
        name = NAME.FORTIFY;
        priority = 0;
        type = STATUS_TYPE.BUFF;
        decreasing = false;
    }

    public override void OnTurnEnd()
    {
        controller.AddStatus(new DefenseStatus(), amount);
        base.OnTurnEnd();
    }
}
