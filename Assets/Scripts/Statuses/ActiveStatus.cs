using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ActiveStatus : CombatAction
{

    public enum Trigger {
        TURN_START,
        DAMAGE_DEALT,
        DAMAGE_TAKEN
    }

    public Trigger trigger;
    [ShowIf("ShowAffectsSelf")]
    public bool affectsSelf = true;
    [SerializeField]
    private Decay decaysAfterUse;

    public int GetDecayedAmount(int currentAmount) {
        return decaysAfterUse.isDecaying ? currentAmount - decaysAfterUse.amount : currentAmount;
    }

    [Serializable, Toggle("isDecaying")]
    class Decay
    {
        public bool isDecaying;
        public int amount = 1;
    }

#if UNITY_EDITOR
    private bool ShowAffectsSelf() {
        return trigger != Trigger.TURN_START;
    }
#endif
}
