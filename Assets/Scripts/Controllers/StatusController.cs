using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CombatantController))]
public class StatusController : MonoBehaviour
{
    private CombatantController combatant;
    private int order = 0;

    private List<Status> statuses = new List<Status>();
    private List<Status> toRemove = new List<Status>();

    private void Awake() {
        combatant = GetComponent<CombatantController>();
    }

    private void CheckRemoval()
    {
        bool changed = toRemove.Count > 0;
        while(toRemove.Count > 0)
        {
            statuses.Remove(toRemove[0]);
            toRemove.RemoveAt(0);
        }

        if (changed)
        {
            combatant.UpdateStatuses();
        }
    }

    public void ResetStatuses() {
        statuses.Clear();
        toRemove.Clear();
        order = 0;
        combatant.UpdateStatuses();
    }

    public void AddStatus(Status newStatus, int stacks = 1) {
        bool containsStatus = false;
        foreach(Status status in statuses)
        {
            if(status.GetType() == newStatus.GetType())
            {
                containsStatus = true;
                status.AddStacks(stacks);
                break;
            }
        }
        if (!containsStatus)
        {
            newStatus.amount = stacks;
            newStatus.displayOrder = order;
            order++;
            newStatus.controller = this;
            statuses.Add(newStatus);
            statuses.Sort(new StatusPrioritySort());
        }
        combatant.UpdateStatuses();
    }

    public void RemoveStatus(Status status)
    {
        toRemove.Add(status);
    }

    public void OnTurnStart() {
        foreach(Status status in statuses)
        {
            status.OnTurnStart();
        }
        CheckRemoval();
    }

    public void OnTurnEnd()
    {
        foreach (Status status in statuses)
        {
            status.OnTurnEnd();
        }
        CheckRemoval();
    }

    public void OnAttacked(CombatantController attacker)
    {
        foreach (Status status in statuses)
        {
            status.OnAttacked(attacker);
        }
        CheckRemoval();
    }

    public void OnAttack(CombatantController target)
    {
        foreach (Status status in statuses)
        {
            status.OnAttack(target);
        }
        CheckRemoval();
    }

    public void OnMove()
    {
        foreach (Status status in statuses)
        {
            status.OnMove();
        }
        CheckRemoval();
    }

    public void OnPlayCard(AbstractCard card)
    {
        foreach (Status status in statuses)
        {
            status.OnPlayCard(card);
        }
        CheckRemoval();
    }

    public int GetDamageDealt(int baseDamage)
    {
        int modifiedDamage = baseDamage;
        foreach (Status status in statuses)
        {
            modifiedDamage = status.GetDamageDealt(modifiedDamage);
        }
        CheckRemoval();
        return modifiedDamage;
    }

    public int GetDamageTaken(int baseDamage)
    {
        int modifiedDamage = baseDamage;
        foreach (Status status in statuses)
        {
            modifiedDamage = status.GetDamageTaken(modifiedDamage);
        }
        CheckRemoval();
        return modifiedDamage;
    }

    public int GetHealthLost(int baseAmount)
    {
        int modifiedAmount = baseAmount;
        foreach (Status status in statuses)
        {
            modifiedAmount = status.GetHealthLost(modifiedAmount);
        }
        CheckRemoval();
        return modifiedAmount;
    }

    public int GetMovement(int baseMovement)
    {
        int modifiedMovment = baseMovement;
        foreach (Status status in statuses)
        {
            modifiedMovment = status.GetMovement(modifiedMovment);
        }
        CheckRemoval();
        return modifiedMovment;
    }

    public int GetHandSize(int baseHand)
    {
        int modifiedHand = baseHand;
        foreach(Status status in statuses)
        {
            modifiedHand = status.GetHandSize(modifiedHand);
        }
        CheckRemoval();
        return modifiedHand;
    }

    public List<Status> GetStatuses()
    {
        return new List<Status>(statuses);
    }

    public CombatantController GetCombatant()
    {
        return combatant;
    }
}
