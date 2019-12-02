using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public enum StatusType
    {
        BUFF,
        DEBUFF
    }

    public enum Name
    {
        STRENGTH,
        DEFENSE,
        SWIFTNESS,
        FORTIFY,
        AEGIS,
        POISON,
        HAMSTRING,
        SPIKES,
        PRAYER_FATIGUE,
        SABOTAGE
    }

    protected StatusType type;
    public Name name;
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

    public virtual void OnAttacked(CombatantController attacker) { }

    public virtual void OnAttack(CombatantController target) { }

    public virtual void OnPlayCard(AbstractCard card) { } //Not yet hooked up

    public virtual void OnMove() { } //Hooked up, but only for enemies

    public virtual int GetDamageDealt(int damage) { return damage; }

    public virtual int GetDamageTaken(int damage) { return damage; }

    public virtual int GetHealthLost(int amount) { return amount; }

    public virtual int GetMovement(int movement) { return movement; }

    public virtual void AddStacks(int stacks) { amount += stacks; CheckRemoval(); }

    public StatusType GetStatusType()
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

    public static Status FromName(Name name)
    {
        switch (name)
        {
            case Name.STRENGTH:
                return new StrengthStatus();
            case Name.DEFENSE:
                return new DefenseStatus();
            case Name.SWIFTNESS:
                return new SwiftnessStatus();
            case Name.AEGIS:
                return new AegisStatus();
            case Name.FORTIFY:
                return new FortifyStatus();
            case Name.HAMSTRING:
                return new HamstringStatus();
            case Name.POISON:
                return new PoisonStatus();
            case Name.SPIKES:
                return new SpikesStatus();
            case Name.PRAYER_FATIGUE:
                return new PrayerFatigueStatus();
            case Name.SABOTAGE:
                return new SabotageStatus();
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
