using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/Generic Status")]
public class AbstractStatus : ScriptableObject
{
    public enum Type {
        PASSIVE,
        ACTIVE
    }

    public enum Trigger {
        TURN_START,
        DAMAGE_DEALT,
        DAMAGE_TAKEN
    }

    [HideInInlineEditors, Required, PropertySpace(0, 5)]
    new public string name;

    [BoxGroup("Status Type"), PropertySpace(0, 5)]
    [HideLabel, EnumToggleButtons]
    public Type type;

    [ShowIfGroup("Status Type/passive", MemberName="type", Value=Type.PASSIVE), InlineProperty, HideLabel]
    public Expression expression;

    [ShowIfGroup("Status Type/active", MemberName="type", Value=Type.ACTIVE), InlineProperty, HideLabel]
    public ActiveStatus activeStatus;

    [HideInInspector]
    public CombatantController combatant;
}
