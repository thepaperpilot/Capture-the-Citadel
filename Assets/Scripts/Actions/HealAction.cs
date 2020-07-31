using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : AbstractAction
{
    public int amount;
    public CombatantController target;

    public HealAction(CombatantController target, int amount) {
        this.amount = amount;
        this.target = target;
    }

    public IEnumerator Run()
    {
        target.Heal(amount);
        yield return null;
    }
}
