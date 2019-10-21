using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CardAction : CombatAction
{
    public enum Targets {
        PLAYER,
        ENEMY,
        ALL_ENEMIES
    }

    [EnumToggleButtons]
    public Targets target;
}
