using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningStatus : Status
{
    public BurningStatus()
    {
        name = Name.BURNING;
        type = StatusType.DEBUFF;
        priority = 10;
        decreasing = false;
    }

    public override void OnTurnEnd()
    {
        ActionsManager.Instance.AddToTop(new LoseHealthAction(controller.GetCombatant(), amount));
    }
}
