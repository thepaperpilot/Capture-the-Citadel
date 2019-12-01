﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesStatus : Status
{
    public SpikesStatus()
    {
        name = NAME.SPIKES;
        type = STATUS_TYPE.BUFF;
        priority = 0;
        decreasing = false;
    }

    public override void OnAttacked(CombatantController attacker)
    {
        int damage = attacker.statusController.GetHealthLost(amount);
        if(damage > 0)
        {
            ActionsManager.Instance.AddToTop(new LoseHealthAction(attacker, damage));
        }
    }

    public override void OnTurnStart()
    {
        controller.RemoveStatus(this);
    }
}
