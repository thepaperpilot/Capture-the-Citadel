using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseHealthAction : AbstractAction
{
    public int amount;
    public CombatantController target;

    public LoseHealthAction(CombatantController target, int amount) {
        this.amount = amount;
        this.target = target;
    }

    public IEnumerator Run()
    {
        target.LoseHealth(amount);
        yield return null;
    }
}
