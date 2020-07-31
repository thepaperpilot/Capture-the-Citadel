using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageAction : AbstractAction
{
    public int amount;
    public CombatantController target;
    public CombatantController attacker;

    public TakeDamageAction(CombatantController attacker, CombatantController target, int amount) {
        this.amount = amount;
        this.attacker = attacker;
        this.target = target;
    }

    public IEnumerator Run()
    {
        target.TakeDamage(amount);
        attacker.statusController.OnAttack(target);
        target.statusController.OnAttacked(attacker);
        yield return null;
    }
}
