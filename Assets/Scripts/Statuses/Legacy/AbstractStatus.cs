using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/Generic Status")]
public class AbstractStatus : ScriptableObject
{
    public enum Types {
        PASSIVE,
        ACTIVE
    }

    public enum Triggers {
        TURN_START,
        DAMAGE_DEALT,
        DAMAGE_TAKEN
    }

    public enum Effect{
        BUFF,
        DEBUFF
    }

    [HideInInlineEditors, Required, PropertySpace(0, 5)]
    new public string name;

    [BoxGroup("Status Type"), PropertySpace(0, 5)]
    [HideLabel, EnumToggleButtons]
    public Types type;

    [ShowIfGroup("Status Type/passive", MemberName="type", Value=Types.PASSIVE), InlineProperty, HideLabel]
    public Expression expression;

    [ShowIfGroup("Status Type/active", MemberName="type", Value=Types.ACTIVE), InlineProperty, HideLabel]
    public ActiveStatus activeStatus;

    public Sprite icon;

    [HideInInspector]
    public CombatantController combatant;
}
