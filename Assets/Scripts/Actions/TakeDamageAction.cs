using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageAction : AbstractAction
{
    public int amount;
    public CombatantController target;

    public TakeDamageAction(CombatantController target, int amount) {
        this.amount = amount;
        this.target = target;
    }

    public IEnumerator Run()
    {
        target.TakeDamage(amount);
        yield return null;
    }
}
