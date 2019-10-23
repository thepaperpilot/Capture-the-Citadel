using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : AbstractAction
{
    public int amount;

    public HealAction(CombatantController target, int amount) {
        this.amount = amount;
    }

    public IEnumerator Run()
    {
        yield return null;
    }
}
