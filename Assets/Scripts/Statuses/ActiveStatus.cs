using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ActiveStatus : CombatAction
{

    public enum Triggers {
        TURN_START,
        DAMAGE_DEALT,
        DAMAGE_TAKEN,
        MOVEMENT
    }

    public Triggers trigger;
    [ShowIf("ShowAffectsSelf")]
    public bool affectsSelf = true;
    [SerializeField]
    private Decay decaysAfterUse;
    [SerializeField, HideIf("trigger", Triggers.DAMAGE_TAKEN)]
    private Decay decaysWhenHit;

    // Variable used for temporarily setting the amount of stacks before adding the
    // action to the queue
    [HideInInspector]
    public int stacks;

    public int GetPostUseAmount(int currentAmount) {
        return decaysAfterUse.isDecaying ? currentAmount - decaysAfterUse.amount : currentAmount;
    }

    public int GetPostHitAmount(int currentAmount) {
        return decaysWhenHit.isDecaying ? currentAmount - decaysWhenHit.amount : currentAmount;
    }

    protected override int GetAmount() {
        return amount * stacks;
    }

    [Serializable, Toggle("isDecaying")]
    class Decay
    {
        public bool isDecaying;
        public int amount = 1;
    }

#if UNITY_EDITOR
    private bool ShowAffectsSelf() {
        return trigger != Triggers.TURN_START;
    }
#endif
}
