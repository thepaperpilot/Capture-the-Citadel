using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamstringStatus : Status
{
    public HamstringStatus()
    {
        name = Name.HAMSTRING;
        type = StatusType.DEBUFF;
        priority = -10;
        decreasing = false;
    }

    public override void OnMove()
    {
        ActionsManager.Instance.AddToTop(new LoseHealthAction(controller.GetCombatant(), amount));
    }
}
