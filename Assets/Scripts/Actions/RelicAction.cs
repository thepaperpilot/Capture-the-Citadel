using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RelicAction : AbstractAction
{
    public enum Triggers
    {
        COLLECT,
        COMBAT_START,
        COMBAT_END,
        TURN_START,
        TURN_END,
        DAMAGE_TAKEN,
        DAMAGE_GIVEN,
        PLAY_CARD,
        SHUFFLE,
        DROP_ZONE,
        MOVEMENT,
        MONSTER_DEATH
    }

    public enum Effects
    {
        ADD_STATUS,
        AFFECT_CARD,
        DAMAGE,
        DRAW,
        ENERGY,
        HEAL,
        MAX_HEALTH,
        GOLD
    }

    public enum Targets
    {
        PLAYER,
        ALL_ENEMIES,
        RANDOM_ENEMY
    }

    public enum TargetedTriggerTargets
    {
        PLAYER,
        ALL_ENEMIES,
        RANDOM_ENEMY,
        ENEMY
    }

    [EnumToggleButtons]
    [BoxGroup("Trigger")]
    public Triggers trigger;
    [BoxGroup("Trigger")]
    [ShowIf("trigger", Triggers.TURN_START)]
    public bool everyTurn;
    [BoxGroup("Trigger"), MinMaxSlider(0, 11)]
    [InfoBox("Turn it up to 11 to indicate their is no upper limit")]
    [ShowIf("@(trigger == Triggers.TURN_START && !everyTurn) || trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
    public Vector2Int range;
    [BoxGroup("Trigger")]
    [ShowIf("trigger", Triggers.DROP_ZONE), PreviewField(50)]
    public Sprite sprite;
    [BoxGroup("Trigger")]
    [ShowIf("trigger", Triggers.DROP_ZONE)]
    public Color color;

    [BoxGroup("Effect")]
    public Effects effect;
    [BoxGroup("Effect")]
    [HideIf("@effect == Effects.AFFECT_CARD || effect == Effects.DRAW")]
    [ShowIf("@trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
    public TargetedTriggerTargets targetedEffect;
    [BoxGroup("Effect")]
    [HideIf("@effect == Effects.AFFECT_CARD || effect == Effects.DRAW")]
    [HideIf("@trigger == Triggers.DAMAGE_GIVEN || trigger == Triggers.DAMAGE_TAKEN")]
    public Targets target;
    [BoxGroup("Effect")]
    [ShowIf("effect", Effects.ADD_STATUS)]
    public Status.Name status;
    [BoxGroup("Effect")]
    [HideIf("effect", Effects.AFFECT_CARD)]
    public int amount;


    [HideInInspector]
    public int data;
    [HideInInspector]
    public AbstractCard discard;
    [HideInInspector]
    // Targets must be set before adding this action to the ActionsManager
    public CombatantController[] targets;

    public IEnumerator Run()
    {
        // Make sure all conditions are met
        if ((trigger == Triggers.TURN_START && !everyTurn) ||
            trigger == Triggers.DAMAGE_GIVEN ||
            trigger == Triggers.DAMAGE_TAKEN)
        {
            if (data < range.x || data > range.y)
                yield break;
        }

        // Handle effects that only apply to the player
        if (effect == Effects.AFFECT_CARD)
        {
            // TODO
            yield break;
        }
        else if (effect == Effects.DRAW)
        {
            ActionsManager.Instance.AddToTop(new DrawAction(amount));
            yield break;
        }
        else if (effect == Effects.ENERGY)
        {
            CombatManager.Instance.player.AddEnergy(amount);
        }
        else if (effect == Effects.HEAL)
        {
            CombatManager.Instance.player.Heal(amount);
        }
        else if (effect == Effects.MAX_HEALTH)
        {
            PlayerManager.Instance.ChangeMaxHealth(amount);
        }
        else if (effect == Effects.GOLD)
        {
            PlayerManager.Instance.Gold = Mathf.Max(PlayerManager.Instance.Gold + amount, 0);
        }

        // Handle effects that can target others
        foreach (CombatantController controller in targets)
        {
            switch (effect)
            {
                case Effects.ADD_STATUS:
                    Debug.Log("status add");
                    controller.GetComponent<StatusController>().AddStatus(Status.FromName(status, trigger == Triggers.DAMAGE_TAKEN), amount);
                    break;
                case Effects.DAMAGE:
                    if (controller != CombatManager.Instance.player)
                        RelicsManager.Instance.OnDamageGiven(amount, controller);
                    ActionsManager.Instance.AddToTop(new HealAction(controller, -amount));
                    break;
            }
        }

        yield return null;
    }
}
