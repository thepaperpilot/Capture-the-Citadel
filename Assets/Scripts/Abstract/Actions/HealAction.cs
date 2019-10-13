using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : AbstractAction
{
    public int amount;

    public HealAction(int amount, AbstractAction[] chainedEvents) {
        this.amount = amount;
        this.chainedEvents = chainedEvents;
    }

    public override IEnumerator Run()
    {
        yield return null;
    }
}
