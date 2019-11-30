using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatus : System.IComparable<BaseStatus>
{
    public int amount;
    public bool decreasing;
    protected int priority; //Lower values will go first

    public virtual void OnTurnStart() { }

    public virtual void OnTurnEnd()
    {
        if (decreasing)
        {
            amount--;
        }
    }

    public virtual void OnTakeDamage(CombatantController attacker) { }

    public virtual void OnAttack(CombatantController target) { }

    public virtual void OnPlayCard(AbstractCard card) { }

    public virtual int DamageGiven(int damage) { return damage; }

    public virtual int DamageTaken(int damage) { return damage; }

    public virtual void AddStacks(int stacks) { amount += stacks; }


    public int GetPriority()
    {
        return priority;
    }


    public int CompareTo(BaseStatus other)
    {
        return priority.CompareTo(other.GetPriority());
    }
}
