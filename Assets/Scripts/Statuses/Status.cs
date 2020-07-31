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
        SABOTAGE,
        HANDY,
        BURNING
    }

    protected StatusType type;
    public Name name;
    public int amount;
    public bool decreasing;
    protected bool gracePeriod = false;
    protected int priority; //Lower values will go first
    public int displayOrder;

    public StatusController controller;

    public virtual void OnTurnStart() { }

    public virtual void OnTurnEnd()
    {
        if (decreasing)
        {
            if (gracePeriod)
            {
                gracePeriod = false;
                return;
            }
            amount--;
            CheckRemoval();
        }
    }

    public virtual void OnAttacked(CombatantController attacker) { }

    public virtual void OnAttack(CombatantController target) { }

    public virtual void OnPlayCard(AbstractCard card) { }

    public virtual void OnMove() { }

    public virtual int GetDamageDealt(int damage, bool preview) { return damage; }

    public virtual int GetDamageTaken(int damage, bool preview) { return damage; }

    public virtual int GetHealthLost(int amount, bool preview) { return amount; }

    public virtual int GetMovement(int movement, bool preview) { return movement; }

    public virtual int GetHandSize(int hand, bool preview) { return hand; }

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

    protected bool isGracePeriod(bool fromMonster)
    {
        return fromMonster && !CombatManager.Instance.IsPlayerTurn();
    }

    protected void CauseStatus(Name status, int amt)
    {
        CauseStatus(new CombatantController[] { controller.GetCombatant()}, status, amt);
    }

    protected void CauseStatus(CombatantController[] recipients, Name status, int amt)
    {
        CombatAction statusAction = new CombatAction();
        statusAction.actor = controller.GetCombatant();
        statusAction.targets = recipients;
        statusAction.type = CombatAction.TYPE.STATUS;
        statusAction.status = status;
        statusAction.amount = amt;
        ActionsManager.Instance.AddToTop(statusAction);
    }

    public static Status FromName(Name name, bool fromMonster)
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
                return new PoisonStatus(fromMonster);
            case Name.SPIKES:
                return new SpikesStatus();
            case Name.PRAYER_FATIGUE:
                return new PrayerFatigueStatus();
            case Name.SABOTAGE:
                return new SabotageStatus(fromMonster);
            case Name.HANDY:
                return new HandyStatus();
            case Name.BURNING:
                return new BurningStatus();
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
