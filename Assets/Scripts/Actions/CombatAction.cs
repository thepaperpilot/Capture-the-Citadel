using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CombatAction : AbstractAction
{
    public enum TYPE {
        DAMAGE,
        DRAW,
        MOVE,
        STATUS
    }

    public enum TARGET {
        PLAYER,
        ENEMY,
        ALL_ENEMIES
    }

    [EnumToggleButtons]
    public TYPE type;
    [EnumToggleButtons]
    public TARGET target;
    [ShowIf("type", TYPE.STATUS)]
    public AbstractStatus status;
    public int amount;

    public IEnumerator Run() {
        yield return null;
    }
}
