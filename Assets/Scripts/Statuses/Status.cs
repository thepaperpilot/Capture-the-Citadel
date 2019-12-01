using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public enum STATUS_TYPE
    {
        BUFF,
        DEBUFF
    }

    public enum NAME
    {
        STRENGTH,
        DEFENSE,
        SWIFTNESS,
        FORTIFY,
        AEGIS,
        POISON,
        HAMSTRING,
        SPIKES,
        PRAYER_FATIGUE
    }

    protected STATUS_TYPE type;
    public NAME name;
    public int amount;
    public bool decreasing;
    protected int priority; //Lower values will go first
    public int displayOrder;

    public StatusController controller;

    public virtual void OnTurnStart() { }

    public virtual void OnTurnEnd()
    {
        if (decreasing)
        {
            amount--;
            CheckRemoval();
        }
    }

    public virtual void OnTakeDamage(CombatantController attacker) { } //Not yet hooked up

    public virtual void OnAttack(CombatantController target) { } //Not yet hooked up - future note: maybe change this to OnAttacked? Player attacks can be tracked via OnPlayCard and enemy attacks probably don't need to be tracked

    public virtual void OnPlayCard(AbstractCard card) { } //Not yet hooked up

    public virtual void OnMove() { } //Hooked up, but only for enemies

    public virtual int GetDamageDealt(int damage) { return damage; }

    public virtual int GetDamageTaken(int damage) { return damage; }

    public virtual int GetHealthLost(int amount) { return amount; }

    public virtual int GetMovement(int movement) { return movement; }

    public virtual void AddStacks(int stacks) { amount += stacks; CheckRemoval(); }

    public STATUS_TYPE GetStatusType()
    {
        return type;
    }

    public int GetPriority()
    {
        return priority;
    }

    protected void CheckRemoval()
    {
        if(amount == 0)
        {
            controller.RemoveStatus(this);
        }
    }

    public static Status FromName(NAME name)
    {
        switch (name)
        {
            case NAME.STRENGTH:
                return new StrengthStatus();
            case NAME.DEFENSE:
                return new DefenseStatus();
            case NAME.SWIFTNESS:
                return new SwiftnessStatus();
            case NAME.AEGIS:
                return new AegisStatus();
            case NAME.FORTIFY:
                return new FortifyStatus();
            case NAME.HAMSTRING:
                return new HamstringStatus();
            case NAME.POISON:
                return new PoisonStatus();
            case NAME.SPIKES:
                return new SpikesStatus();
            case NAME.PRAYER_FATIGUE:
                return new PrayerFatigueStatus();

        }
        return null;
    }
}

public class StatusPrioritySort : IComparer<Status>
{
    public int Compare(Status x, Status y)
    {
        return x.GetPriority().CompareTo(y.GetPriority());
    }
}

public class StatusDisplaySort : IComparer<Status>
{
    public int Compare(Status x, Status y)
    {
        return x.displayOrder.CompareTo(y.displayOrder);
    }
}
