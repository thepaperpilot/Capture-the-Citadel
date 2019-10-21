using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CombatantController))]
public class StatusController : MonoBehaviour
{
    private CombatantController combatant;

    private void Awake() {
        combatant = GetComponent<CombatantController>();
    }

    readonly public List<AbstractStatus> statuses = new List<AbstractStatus>();

    private void ResetStatuses(Scene scene, LoadSceneMode mode) {
        statuses.RemoveRange(0, statuses.Count);
    }

    public void AddStatus(AbstractStatus status, int stacks = 1) {
        // TODO status effect stacks
        // (and make stacks of multipliers just increase the multiplier)
        statuses.Add(status);
    }

    public void RemoveStatus(AbstractStatus status) {
        statuses.Remove(status);
    }

    public int GetDamage(int baseDamage) {
        return GetEffectValue(baseDamage, Expression.Effects.DAMAGE);
    }

    public void OnTurnStart() {
        OnTrigger(ActiveStatus.Triggers.TURN_START, new CombatantController[] { combatant });
    }

    public void OnAttack(CombatantController[] targets) {
        OnTrigger(ActiveStatus.Triggers.DAMAGE_DEALT, targets);
    }

    public void OnAttacked(CombatantController attacker) {
        OnTrigger(ActiveStatus.Triggers.DAMAGE_TAKEN, new CombatantController[] { attacker });
    }

    private int GetEffectValue(int baseValue, Expression.Effects effect) {
        int value = baseValue;
        IEnumerable<Expression> expressions = statuses.
            Where(status => status.type == AbstractStatus.Types.PASSIVE).
            Select(status => status.expression).
            Where(expression => expression.effect == effect);
    
        foreach (Expression expression in expressions.Where(expression => expression.modifier == Expression.Modifiers.ADD)) {
            value += expression.amount;
        }
        foreach (Expression expression in expressions.Where(expression => expression.modifier == Expression.Modifiers.MULTIPLY)) {
            value *= expression.amount;
        }

        return value;
    }

    private void OnTrigger(ActiveStatus.Triggers trigger, CombatantController[] others) {
        IEnumerable<ActiveStatus> activeStatuses = statuses.
            Where(status => status.type == AbstractStatus.Types.ACTIVE).
            Select(status => status.activeStatus).
            Where(activeStatus => activeStatus.trigger == trigger);
        
        foreach (ActiveStatus activeStatus in activeStatuses) {
            if (activeStatus.affectsSelf)
                activeStatus.targets = new CombatantController[] { combatant };
            else
                activeStatus.targets = others;
        }
        ActionsManager.Instance.AddToTop(activeStatuses.ToArray());
    }
}
