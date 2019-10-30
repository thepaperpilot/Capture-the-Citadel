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
        target.health += amount;
        if (target.health <= 0) {
            if (CombatManager.Instance.player == target) {
                // TODO player death
            } else {
                CombatManager.Instance.KillEnemy(target);
            }
        }
        yield return null;
    }
}
