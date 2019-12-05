using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesStatus : Status
{
    public SpikesStatus()
    {
        name = Name.SPIKES;
        type = StatusType.BUFF;
        priority = 0;
        decreasing = false;
    }

    public override void OnAttacked(CombatantController attacker)
    {
        if(attacker.tile.pathDistance == 1 || controller.GetCombatant().tile.pathDistance == 1)
        {
            int damage = attacker.statusController.GetHealthLost(amount, false);
            if (damage > 0)
            {
                ActionsManager.Instance.AddToTop(new LoseHealthAction(attacker, damage));
            }
        }
    }

    public override void OnTurnStart()
    {
        controller.RemoveStatus(this);
    }
}
