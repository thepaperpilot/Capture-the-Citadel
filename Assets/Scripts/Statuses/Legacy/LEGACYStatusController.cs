﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CombatantController))]
public class LEGACYStatusController : MonoBehaviour
{
    private CombatantController combatant;

    readonly private Dictionary<AbstractStatus, int> statuses = new Dictionary<AbstractStatus, int>();

    private void Awake()
    {
        combatant = GetComponent<CombatantController>();
    }

    private void ResetStatuses(Scene scene, LoadSceneMode mode)
    {
        statuses.Clear();
        combatant.UpdateStatuses();
    }

    public void AddStatus(AbstractStatus status, int stacks = 1)
    {
        statuses[status] = (statuses.ContainsKey(status) ? statuses[status] : 0) + stacks;
        combatant.UpdateStatuses();
    }

    public void RemoveStatus(AbstractStatus status)
    {
        statuses.Remove(status);
        combatant.UpdateStatuses();
    }

    public int GetDamage(int baseDamage)
    {
        return GetEffectValue(baseDamage, Expression.Effects.DAMAGE);
    }

    public int GetMovement(int baseMovement)
    {
        return GetEffectValue(baseMovement, Expression.Effects.MOVEMENT);
    }

    public void OnTurnStart()
    {
        OnTrigger(ActiveStatus.Triggers.TURN_START, new CombatantController[] { combatant });
    }

    public void OnAttack(CombatantController[] targets)
    {
        OnTrigger(ActiveStatus.Triggers.DAMAGE_DEALT, targets);
    }

    public void OnAttacked(CombatantController attacker)
    {
        OnTrigger(ActiveStatus.Triggers.DAMAGE_TAKEN, new CombatantController[] { attacker });

        // Handle statuses that decay when damage taken
        IEnumerable<KeyValuePair<AbstractStatus, int>> pairs = statuses.
            Where(status => status.Key.type == AbstractStatus.Types.ACTIVE &&
                  status.Key.activeStatus.trigger != ActiveStatus.Triggers.DAMAGE_TAKEN);
        foreach (KeyValuePair<AbstractStatus, int> pair in pairs)
        {
            int newAmount = pair.Key.activeStatus.GetPostHitAmount(pair.Value);
            if (newAmount == 0)
                statuses.Remove(pair.Key);
            else
                statuses[pair.Key] = newAmount;
            combatant.UpdateStatuses();
        }
    }

    public void OnMove()
    {
        OnTrigger(ActiveStatus.Triggers.MOVEMENT, new CombatantController[] { });
    }

    private int GetEffectValue(int baseValue, Expression.Effects effect)
    {
        int value = baseValue;
        IEnumerable<KeyValuePair<AbstractStatus, int>> pairs = statuses.
            Where(status => status.Key.type == AbstractStatus.Types.PASSIVE &&
                  status.Key.expression.effect == effect);

        foreach (KeyValuePair<AbstractStatus, int> pair in
            pairs.Where(expression => expression.Key.expression.modifier == Expression.Modifiers.ADD))
        {
            value += pair.Key.expression.amount * pair.Value;
        }
        foreach (KeyValuePair<AbstractStatus, int> pair in
            pairs.Where(expression => expression.Key.expression.modifier == Expression.Modifiers.MULTIPLY))
        {
            // Set up so that stacks add to the multiplier additively
            // e.g. if the multiplier is 2, then 1 stack is 2x, 2 stacks is 3x, 5 stacks is 6x
            // e.g. if the multiplier is 1.1, then 1 stack is 1.1x, 2 stacks is 1.2x, 5 stacks is 1.5x
            // *Different* multiplying statuses will still apply multiplicatively, though
            value *= 1 + (pair.Key.expression.amount - 1) * pair.Value;
        }

        return value;
    }

    private void OnTrigger(ActiveStatus.Triggers trigger, CombatantController[] others)
    {
        IEnumerable<KeyValuePair<AbstractStatus, int>> pairs = statuses.
            Where(status => status.Key.type == AbstractStatus.Types.ACTIVE &&
                  status.Key.activeStatus.trigger == trigger);

        foreach (KeyValuePair<AbstractStatus, int> pair in pairs)
        {
            if (pair.Key.activeStatus.affectsSelf)
                pair.Key.activeStatus.targets = new CombatantController[] { combatant };
            else
                pair.Key.activeStatus.targets = others;
            pair.Key.activeStatus.stacks = pair.Value;

            int newAmount = pair.Key.activeStatus.GetPostUseAmount(pair.Value);
            if (newAmount == 0)
                statuses.Remove(pair.Key);
            else
                statuses[pair.Key] = newAmount;
            combatant.UpdateStatuses();
        }

        ActionsManager.Instance.AddToTop(pairs.Select(p => p.Key.activeStatus).ToArray());
    }

    public IEnumerable<KeyValuePair<AbstractStatus, int>> GetStatuses()
    {
        return statuses.Where(status => status.Value != 0);
    }
}
